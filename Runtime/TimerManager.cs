using System;
using UnityEngine;
using UnityEngine.Pool;

namespace com.absence.timersystem
{
    internal class TimerManager : MonoBehaviour
    {
        public const int DefaultPoolCapacity = 16;
        public static readonly bool InstantiateManagerAutomatically = true;

        #region Singleton
        private static TimerManager m_instance;
        public static TimerManager Instance => m_instance;

        private void SetupSingleton()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
                return;
            }

            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Pooling
        private IObjectPool<Timer> m_pool;

        private void SetupPool()
        {
            m_pool = new ObjectPool<Timer>(OnTimerCreate, OnTimerGet, OnTimerRelease, OnTimerDestroy, true, DefaultPoolCapacity, 10000);
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
            t.m_behaviour.SetActive(true);
            t.m_behaviour.Restart();
        }
        private void OnTimerRelease(Timer t)
        {
            t.m_hasSucceeded = false;
            t.m_hasFailed = false;
            t.m_isPaused = false;

            t.m_hasStarted = false;
            t.m_hasEnded = false;

            t.m_behaviour.SetActive(false);
        }
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InstantiateTimerManager()
        {
            if (!InstantiateManagerAutomatically) return;

            new GameObject("Timer Manager [absent-timers]").AddComponent<TimerManager>();
        }

        private void Awake()
        {
            SetupSingleton();
            SetupPool();
        }

        public Timer Get()
        {
            return m_pool.Get();
        }
        public void Release(Timer timerToRelease)
        {
            m_pool.Release(timerToRelease);
        }
    }
}
