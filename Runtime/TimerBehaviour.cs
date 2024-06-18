
using UnityEngine;
using UnityEngine.Pool;

namespace com.absence.timersystem
{
    /// <summary>
    /// A gameobject gets created with this component attached when you create and
    /// initialize a new timer. <b>IT IS NOT INTENDED</b> to be used as a component.
    /// </summary>
    internal class TimerBehaviour : MonoBehaviour
    {
        //static IObjectPool<TimerBehaviour> m_pool;
        //public static IObjectPool<TimerBehaviour> Pool => m_pool;

        //[RuntimeInitializeOnLoadMethod]
        //static void SetupPool()
        //{
        //    m_pool = new ObjectPool<TimerBehaviour>(OnTimerCreate, OnTimerGet, OnTimerRelease, OnTimerDestroy);
        //}

        [SerializeField] private Timer m_timer;
        private float m_timeSpent = 0f;
        private bool m_oneTimeOnly = false;

        internal void Initialize(Timer timer, bool oneTimeOnly)
        {
            if (m_timer != null && m_timer.IsActive) m_timer.Fail();

            m_timer = timer;
            m_oneTimeOnly = oneTimeOnly;
        }

        private void Update()
        {
            if (m_timer == null)
            {
                Destroy(gameObject);
                return;
            }

            if (!m_timer.IsActive) return;
            if (m_timer.IsPaused) return;

            if (m_timeSpent >= m_timer.Duration)
            {
                m_timer.Succeed();
                if (m_oneTimeOnly) Destroy(gameObject);
                return;
            }

            m_timeSpent += Time.deltaTime;
            m_timer.Tick();
        }

        internal void Restart()
        {
            m_timeSpent = 0f;
        }
    }

}