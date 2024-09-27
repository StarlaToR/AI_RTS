using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public class BehaviorTimer
    {
        public bool isRunning = false;
        public bool isStarted = false;

        float m_startingTime;
        float m_totalTime;
        float m_timer;

        public void Start(float time)
        {
            m_timer = 0f;
            m_totalTime = time;
            m_startingTime = Time.time;
            isRunning = true;
            isStarted = true;
        }

        // Update is called once per frame
        public void Pause()
        {
            m_timer += Time.time - m_startingTime;
            isRunning = false;
        }

        public void Unpause()
        {
            m_startingTime = Time.time;
            isRunning = true;
        }

        public bool IsFinished()
        {
            return isStarted && (m_timer + (Time.time - m_startingTime) * (isRunning ? 1 : 0) >= m_totalTime);
        }
    }
}
