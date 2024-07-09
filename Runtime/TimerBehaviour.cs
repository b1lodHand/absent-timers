using UnityEngine;

namespace com.absence.timersystem
{
    /// <summary>
    /// A gameobject gets created with this component attached when you create and
    /// initialize a new timer. <b>IT IS NOT INTENDED</b> to be used as a component.
    /// </summary>
    internal class TimerBehaviour : MonoBehaviour
    {
        [SerializeField] private Timer m_timer;
        private float m_timeSpent = 0f;

        internal void Initialize(Timer timer)
        {
            if (m_timer != null && m_timer.IsActive) m_timer.Fail();

            m_timer = timer;
        }

        private void Update()
        {
            if (m_timer == null)
            {
                this.Destroy();
                return;
            }

            if (!m_timer.IsActive) return;
            if (m_timer.IsPaused) return;

            if (m_timeSpent >= m_timer.Duration)
            {
                m_timer.Succeed();
                return;
            }

            m_timeSpent += Time.deltaTime;
            m_timer.Tick();
        }

        internal void Restart()
        {
            m_timeSpent = 0f;
        }

        internal void SetActive(bool activeValue) => gameObject.SetActive(activeValue);

        internal void Destroy()
        {
            m_timer = null;
            Destroy(gameObject);
        }
    }

}