using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public struct SquadData
    {
        public ActionParameters parameters;
        public IA_Manager ai_manager;
        public IA_Perception ai_perception;
    }


    public class IA_Squad : MonoBehaviour
    {
        public IA_Behavior currentBehavior;
        public IA_SquadPerception squadPerception;
        public SquadData squadData;

        public IA_Squad(SquadData data)
        {
            squadData = data;
         
        }

        public virtual void ComputeBestAction()
        {

        }

        public virtual void SendDirectionData(SquadData squadData) { }

        public void CallBehaviorFinish(ActionState state)
        {
            if (!currentBehavior) return;
            squadData.ai_manager.FinishBehavior(currentBehavior, state);
            currentBehavior = null;
        }

    }
}
