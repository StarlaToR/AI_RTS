using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace RTS
{

    public class InfluenceMap : Graph
    {
        // Singleton access
        static InfluenceMap _Instance = null;
        static public InfluenceMap Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<InfluenceMap>();
                return _Instance;
            }
        }

        public float UpdateFrequency = 0.5f;
        private float m_lastUpdateTime = float.MinValue;

        private bool m_isGraphCreated = false;
        private bool m_isInitialized = false;

        public GameObject baseObject;
        [SerializeField] private ETeam m_teamPOV;
        [SerializeField] private ETeam m_adversaryTeam;

        [Header("Influencal Parameters")]
        public float baseValuePowerUnit = 0.8f;
        public float maxValuePerNode = 3.0f;
        public float minThresholdPower = 0.2f;
        public AttunationFunction attunationFunction;

        private FogOfWarSystem m_fogOfWarSystem;

        private void Awake()
        {
            m_fogOfWarSystem = GetComponent<FogOfWarSystem>();
            InitGridParameters();
            CreateTiledGrid();
            OnGraphCreated += () => { m_isGraphCreated = true; };
        }

        protected void InitGridParameters()
        {          
            GridSizeV = (int)baseObject.transform.localScale.x;
            GridSizeH = (int)baseObject.transform.localScale.y;

            GridStartPos = baseObject.transform.position + new Vector3(-GridSizeH / 2f, 0f, -GridSizeV / 2f);
        }

        private void Update()
        {
            if (!m_isGraphCreated)
                return;

            if (!m_isInitialized)
            {
                m_isInitialized = true;
            }

            // TODO : don't update influence map if no Unit has moved
            if (Time.time - m_lastUpdateTime > UpdateFrequency)
            {
                ComputeInfluence();
            }
        }

        protected override Node CreateNode()
        {
            return new InfluenceNode();
        }


        public void ComputeInfluence()
        {
           
            List<BaseEntity> units = new List<BaseEntity>(GameServices.GetControllerByTeam(m_teamPOV).GetFactoryList);
            units.AddRange(GameServices.GetControllerByTeam(m_teamPOV).UnitList);
            units.AddRange(GameServices.GetControllerByTeam(m_adversaryTeam).UnitList);
            units.AddRange(GameServices.GetControllerByTeam(m_adversaryTeam).GetFactoryList);
            List<InfluenceNode> influenceNodes = new List<InfluenceNode>();

            foreach (InfluenceNode node in NodeList.ToArray())
            {
                 
                // Reset all influence nodes
                node.SetValue(ETeam.Neutral, 0f);

                // Test of node visibility 
                Vector2 pos2D = new Vector2(node.Position.x, node.Position.z);
                bool isVisible = m_fogOfWarSystem.IsVisible(1 << (int)(m_teamPOV), pos2D); 
                bool wasVisible = m_fogOfWarSystem.WasVisible(1 << (int)(m_teamPOV), pos2D); 
               
                if (!isVisible && !wasVisible)
                    continue;

                // Find and setup influence cell with units 
                for (int i = 0; i < units.Count; i++)
                {
                    Vector3 unit2DPos = units[i].transform.position - node.Position;
                    bool isInSquare = Mathf.Abs(unit2DPos.x) <= (SquareSize * 0.5f) && Mathf.Abs(unit2DPos.z) <= (SquareSize * 0.5f);

                    if (isInSquare)
                    {
                        node.SetValue(units[i].GetTeam(), baseValuePowerUnit);

                        if (influenceNodes.Contains(node)) continue;

                        influenceNodes.Add(node);
                        units.Remove(units[i]);
                        i--;
                    }
                }
            }


            InfluenceNode[] influenceNodesArray = influenceNodes.ToArray();
            float invSquareSize = 1.0f / SquareSize;

            for (int i = 0; i < influenceNodesArray.Length; i++)
            {
                List<InfluenceNode> visitedNode = new List<InfluenceNode>();
                Queue<InfluenceNode> queue = new Queue<InfluenceNode>();
                queue.Enqueue(influenceNodesArray[i]);
                bool firstIteration = true;
                while (queue.Count != 0)
                {
                    InfluenceNode influenceNode = queue.Dequeue();
                    visitedNode.Add(influenceNode);
                    float distance = Vector3.Distance(influenceNodesArray[i].Position, influenceNode.Position);
                    if (!firstIteration)
                    {
                        float power = attunationFunction.ApplyAttenuationFunction(baseValuePowerUnit, distance * invSquareSize);
                        influenceNode.SetValue(influenceNodesArray[i].faction, power);
                        if (power <= minThresholdPower)
                            continue;
                    }
                    else
                    {
                        firstIteration = false;
                    }

                    for (int j = 0; j < influenceNode.Neighbours.Count; j++)
                    {
                        InfluenceNode neighbourNode = influenceNode.Neighbours[j] as InfluenceNode;
                        if (visitedNode.Contains(neighbourNode) || queue.Contains(neighbourNode)) continue;

                        queue.Enqueue(neighbourNode);
                    }


                }


            }

        }




        #region Gizmos

        // Draw influence map result as colored cubes using Gizmos
        protected override void DrawNodesGizmo()
        {
           
            for (int i = 0; i < NodeList.Count; i++)
            {
                InfluenceNode node = NodeList[i] as InfluenceNode;
                if (node != null)
                {
                    Color nodeColor = node.faction switch
                    {
                        ETeam.Blue => Color.blue,
                        ETeam.Red => Color.red,
                        ETeam.Neutral => Color.black,
                        _ => throw new System.NotImplementedException()
                    };
                    node.value = Mathf.Clamp(node.value, 0, maxValuePerNode);
                    node.value = node.value / maxValuePerNode;
                    nodeColor.a = Mathf.Max(node.value, 0.1f);
                    Gizmos.color = nodeColor;
                    Gizmos.DrawCube(node.Position, Vector3.one * SquareSize * 0.95f);
                }
            }
        }
        #endregion
    }
}