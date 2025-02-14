using com.absence.timersystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.timersystem.editor
{
    [CustomEditor(typeof(TimerManager))]
    public class TimerManagerEditor : Editor
    {
        const float k_timerFieldWidth = 20f;

        public override void OnInspectorGUI()
        {
            TimerManager manager = (TimerManager)target;

            serializedObject.Update();

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 18;
            headerStyle.margin.bottom = 10;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontStyle = FontStyle.Bold;

            int poolCapacity = manager.m_initialPoolCapacity;
            bool dontDestroyOnLoad = manager.m_dontDestroyOnLoad;
            bool useSingleton = manager.m_useSingleton;

            GUILayout.Label("Timer Manager", headerStyle);

            EditorGUI.BeginChangeCheck();

            if (Application.isPlaying) GUI.enabled = false;

            DrawPoolCapacityField();
            DrawSingletonToggle();
            DrawDontDestroyOnLoadToggle();

            EditorGUILayout.Space();

            if (Application.isPlaying) DrawActiveTimerList();
            else
            {
                EditorGUILayout.HelpBox("You need to start the game to see any runtime data.",
                    MessageType.Info);
            }

            if (Application.isPlaying) GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(manager, "Timer Manager (Editor)");

                manager.m_dontDestroyOnLoad = dontDestroyOnLoad;
                manager.m_useSingleton = useSingleton;
                manager.m_initialPoolCapacity = poolCapacity;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(manager);
            }

            return;

            void DrawPoolCapacityField()
            {
                GUIContent content = new GUIContent()
                {
                    text = "Initial Pool Capacity",
                    tooltip = "This is the capacity that will be used when creating the timer pool.",
                };

                poolCapacity = EditorGUILayout.IntField(content, poolCapacity);
            }

            void DrawSingletonToggle()
            {
                GUIContent content = new GUIContent()
                {
                    text = "Use Singleton",
                    tooltip = "If true, this TimerManager will try to be the singleton.",
                };

                useSingleton = EditorGUILayout.Toggle(content, useSingleton);
            }

            void DrawDontDestroyOnLoadToggle()
            {
                GUIContent content = new GUIContent()
                {
                    text = "Don't Destroy On Load",
                    tooltip = "If true, this object will try to move itself to DontDestroyOnLoad scene on awake.",
                };

                dontDestroyOnLoad = EditorGUILayout.Toggle(content, dontDestroyOnLoad);
            }

            void DrawActiveTimerList()
            {
                GUI.enabled = false;

                EditorGUILayout.LabelField("Active Timers:");

                manager.m_activeTimers.ForEach(timer =>
                {
                    EditorGUILayout.BeginHorizontal();

                    float duration = timer.Duration;
                    float current = timer.CurrentTime;

                    EditorGUILayout.LabelField("0", GUILayout.Width(k_timerFieldWidth));

                    GUILayout.HorizontalSlider(current, 0f, duration);

                    EditorGUILayout.LabelField(duration.ToString(), GUILayout.Width(k_timerFieldWidth));

                    EditorGUILayout.EndHorizontal();
                });

                GUI.enabled = true;
            }

        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}
