using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public sealed class AIController : UnitController
{
   [HideInInspector] public RTS.IA_Manager manager;
   [HideInInspector] public RTS.IA_Commander commander;
   [HideInInspector] public RTS.IA_Perception perception;
    #region MonoBehaviour methods

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        manager = GetComponent<RTS.IA_Manager>();
        commander = GetComponent<RTS.IA_Commander>();
        perception = GetComponent<RTS.IA_Perception>();
        perception.Init();
        manager.Init();
        commander.Init();
    }

    protected override void Update()
    {
        base.Update();
    }

    #endregion
    
    public bool TryBuildFactory(int factoryIndex, Vector3 pos)
    {
        SelectedFactory = manager.perception.mainFactory;

        if (RequestFactoryBuild(factoryIndex, pos, true))
        {
            manager.AddFactoriesList(FactoryList[FactoryList.Count - 1]);
            return true;
        }

        return false;
    }
}
