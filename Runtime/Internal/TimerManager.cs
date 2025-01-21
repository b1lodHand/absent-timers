using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace com.absence.timersystem.internals
{
    /// <summary>
    /// The singleton class responsible for handling anything based on absent-timers.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("absencee_/absent-timers/Timer Manager")]
    public class TimerManager : MonoBehaviour
    {
        internal const int DEFAULT_POOL_CAPACITY = 16;
        internal const bool INSTANTIATE_AUTOMATICALLY = false;

        [SerializeField] internal bool m_dontDestroyOnLoad = true;
        [SerializeField] internal List<Timer> m_activeTimers;

        #region Singleton
        private static TimerManager m_instance;
        public static TimerManager Instance => m_instance;
        #endregion

        private void Awake()
        {
            SetupSingleton();
            SetupPool();
        }

        private void Update()
        {
            for (int i = 0; i < m_activeTimers.Count; i++)
            {
                Timer timer = m_activeTimers[i];

                if (timer.IsActive && !timer.IsPaused) timer.Tick();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InstantiateTimerManager()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (!INSTANTIATE_AUTOMATICALLY) return;

            new GameObject("Timer Manager [absent-timers]").AddComponent<TimerManager>();
#pragma warning restore CS0162 // Unreachable code detected
        }

        #region Pooling
        private IObjectPool<Timer> m_pool;

        private void SetupPool()
        {
            m_activeTimers = new();
            m_pool = new ObjectPool<Timer>(OnTimerCreate, OnTimerGet, OnTimerRelease, OnTimerDestroy, true, DEFAULT_POOL_CAPACITY, 10000);
        }

        private Timer OnTimerCreate()
        {
            Timer t = new Timer();
            return t;
        }
        private void OnTimerDestroy(Timer t)
        {
            t.Dispose();
        }
        private void OnTimerGet(Timer t)
        {
            
        }
        private void OnTimerRelease(Timer t)
        {
            t.ResetProperties();
        }
        #endregion

        private void SetupSingleton()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            m_instance = this;
            if (m_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        internal Timer Get()
        {
            Timer target = m_pool.Get();
            m_activeTimers.Add(target);
            return target;
        }

        internal void Release(Timer timerToRelease)
        {
            m_activeTimers.Remove(timerToRelease);
            m_pool.Release(timerToRelease);
        }
    }
}
