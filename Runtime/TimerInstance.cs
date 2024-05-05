using UnityEngine;
using UnityEngine.Events;

namespace com.absence.timersystem
{
    [AddComponentMenu("absencee_/absent-timers/Timer Instance")]
    public class TimerInstance : MonoBehaviour
    {
        [SerializeField] private float m_duration = 0f;
        [SerializeField] private bool m_startOnAwake = true;
        [SerializeField] private UnityEvent m_onSuccess;
        [SerializeField] private UnityEvent m_onFail;

        private void Start()
        {
            var t = Timer.Create(m_duration,
                () => { },
                () => m_onSuccess?.Invoke(),
                () => m_onFail?.Invoke());

            if (m_startOnAwake) t.Restart();
        }
    }
}
