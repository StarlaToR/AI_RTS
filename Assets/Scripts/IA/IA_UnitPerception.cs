using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_UnitPerception : MonoBehaviour
{
    public List<Unit> ennemiesInSight = new List<Unit>();
    public List<Factory> ennemyBuildingsInSight = new List<Factory>();
    public List<Factory> allyBuildingsInSight = new List<Factory>();
    public List<Factory> neutralBuildingsInSight = new List<Factory>();

    ETeam currTeam = ETeam.Red;

    // Start is called before the first frame update
    void Start()
    {
        currTeam = GetComponentInParent<Unit>().GetTeam();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject objectSeen = other.gameObject;

        Unit unitSeen;
        Factory factorySeen;

        if (objectSeen.TryGetComponent(out unitSeen) && unitSeen.GetTeam() != currTeam)
        {
            ennemiesInSight.Add(unitSeen);
        }
        else if (objectSeen.TryGetComponent(out factorySeen))
        {
            if (factorySeen.GetTeam() == currTeam)
            {
                allyBuildingsInSight.Add(factorySeen);
            }
            else if (factorySeen.GetTeam() == ETeam.Neutral)
            {
                neutralBuildingsInSight.Add(factorySeen);
            }
            else
            {
                ennemyBuildingsInSight.Add(factorySeen);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject objectSeen = other.gameObject;

        Unit unitSeen;
        Factory factorySeen;

        if (objectSeen.TryGetComponent(out unitSeen) && unitSeen.GetTeam() != currTeam)
        {
            ennemiesInSight.Remove(unitSeen);
        }
        else if (objectSeen.TryGetComponent(out factorySeen))
        {
            if (factorySeen.GetTeam() == currTeam)
            {
                allyBuildingsInSight.Remove(factorySeen);
            }
            else if (factorySeen.GetTeam() == ETeam.Neutral)
            {
                neutralBuildingsInSight.Remove(factorySeen);
            }
            else
            {
                ennemyBuildingsInSight.Remove(factorySeen);
            }
        }
    }
}
