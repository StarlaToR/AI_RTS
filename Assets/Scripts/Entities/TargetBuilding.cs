using UnityEngine;
using UnityEngine.UI;

public class TargetBuilding : MonoBehaviour
{
    [SerializeField]
    float CaptureGaugeStart = 100f;
    [SerializeField]
    float CaptureGaugeSpeed = 1f;
    [SerializeField]
    int BuildPoints = 5;
    [SerializeField]
    Material BlueTeamMaterial = null;
    [SerializeField]
    Material RedTeamMaterial = null;
    [SerializeField]
    private float m_timeBetweenGain = 5.0f;
    private float m_timerRessourceGain = 0.0f;

    Material NeutralMaterial = null;
    MeshRenderer BuildingMeshRenderer = null;
    Image GaugeImage;
    Image MinimapImage;

    int[] TeamScore;
    float CaptureGaugeValue;
    public ETeam OwningTeam = ETeam.Neutral;
    ETeam CapturingTeam = ETeam.Neutral;

    public bool isCaptured;
    public System.Action<TargetBuilding> onStartCapture;
    public System.Action<TargetBuilding> OnStopCapture;
    public ETeam GetTeam() { return OwningTeam; }

    private EntityVisibility _Visibility;
    public EntityVisibility Visibility
    {
        get
        {
            if (_Visibility == null)
            {
                _Visibility = GetComponent<EntityVisibility>();
            }
            return _Visibility;
        }
    }


    #region MonoBehaviour methods
    void Start()
    {

        BuildingMeshRenderer = GetComponentInChildren<MeshRenderer>();
        NeutralMaterial = BuildingMeshRenderer.material;

        GaugeImage = GetComponentInChildren<Image>();
        if (GaugeImage)
            GaugeImage.fillAmount = 0f;
        CaptureGaugeValue = CaptureGaugeStart;
        TeamScore = new int[2];
        TeamScore[0] = 0;
        TeamScore[1] = 0;

        Transform minimapTransform = transform.Find("MinimapCanvas");
        if (minimapTransform != null)
            MinimapImage = minimapTransform.GetComponentInChildren<Image>();
    }
    void Update()
    {
        if (OwningTeam == ETeam.Neutral && CapturingTeam == ETeam.Neutral)
            return;

        if (OwningTeam != ETeam.Neutral)
        {
            if (m_timerRessourceGain > m_timeBetweenGain)
            {
                m_timerRessourceGain = 0.0f;
                UnitController teamController = GameServices.GetControllerByTeam(OwningTeam);
                if (teamController != null)
                    teamController.CaptureTarget(BuildPoints, false);
            }
            else
            {
                m_timerRessourceGain += Time.deltaTime;
            }
        }

        if (CapturingTeam == OwningTeam || CapturingTeam == ETeam.Neutral)
            return;
        CaptureGaugeValue -= TeamScore[(int)CapturingTeam] * CaptureGaugeSpeed * Time.deltaTime;

        GaugeImage.fillAmount = 1f - CaptureGaugeValue / CaptureGaugeStart;

        if (CaptureGaugeValue <= 0f)
        {
            CaptureGaugeValue = 0f;
            OnCaptured(CapturingTeam);
        }


    }
    #endregion

    #region Capture methods
    public void StartCapture(Unit unit)
    {
        if (unit == null)
            return;

        TeamScore[(int)unit.GetTeam()] += unit.Cost;

        if (CapturingTeam == ETeam.Neutral)
        {
            if (TeamScore[(int)GameServices.GetOpponent(unit.GetTeam())] == 0)
            {
                CapturingTeam = unit.GetTeam();

                if (OwningTeam != ETeam.Neutral)
                {

                    UnitController controller = GameServices.GetControllerByTeam((OwningTeam));
                    AIController aiController = controller as AIController;
                    if (aiController)
                    {
                        isCaptured = true;
                        onStartCapture?.Invoke(this);
                    }
                    
                    GaugeImage.color = GameServices.GetTeamColor(CapturingTeam);
                }
            }
        }
        else
        {

            if (TeamScore[(int)GameServices.GetOpponent(unit.GetTeam())] > 0)
                ResetCapture();
        }
    }
    public void StopCapture(Unit unit)
    {
        if (unit == null)
            return;
        isCaptured = false;
        TeamScore[(int)unit.GetTeam()] -= unit.Cost;
        if (TeamScore[(int)unit.GetTeam()] == 0)
        {
            ETeam opponentTeam = GameServices.GetOpponent(unit.GetTeam());
            if (TeamScore[(int)opponentTeam] == 0)
            {
                ResetCapture();
            }
            else
            {
                CapturingTeam = opponentTeam;
                UnitController controller = GameServices.GetControllerByTeam((OwningTeam));
                AIController aiController = controller as AIController;
                if (aiController)
                    OnStopCapture?.Invoke(this);
                GaugeImage.color = GameServices.GetTeamColor(CapturingTeam);
            }
        }
    }
    void ResetCapture()
    {
        CaptureGaugeValue = CaptureGaugeStart;
        CapturingTeam = ETeam.Neutral;
        GaugeImage.fillAmount = 0f;
    }
    void OnCaptured(ETeam newTeam)
    {
        isCaptured = false;
        Debug.Log("target captured by " + newTeam.ToString());
        if (OwningTeam != newTeam)
        {
            UnitController teamController = GameServices.GetControllerByTeam(newTeam);
            if (teamController != null)
                teamController.CaptureTarget(BuildPoints);

            if (OwningTeam != ETeam.Neutral)
            {
                // remove points to previously owning team
                teamController = GameServices.GetControllerByTeam(OwningTeam);
                // if (teamController != null)
                // teamController.LoseTarget(BuildPoints);
            }
        }

        ResetCapture();
        OwningTeam = newTeam;
        if (Visibility) { Visibility.Team = OwningTeam; }
        if (MinimapImage) { MinimapImage.color = GameServices.GetTeamColor(OwningTeam); }
        BuildingMeshRenderer.material = newTeam == ETeam.Blue ? BlueTeamMaterial : RedTeamMaterial;

        UnitController controller = GameServices.GetControllerByTeam((OwningTeam));
        AIController aiController = controller as AIController;
        if (aiController)
        {
            aiController.manager.AddTargetBuilding(this);
        }

    }
    #endregion
}
