using com.absence.timersystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.timersystem.editor
{
    [CustomEditor(typeof(TimerManager))]
    public class TimerManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TimerManager manager = (TimerManager)target;

            serializedObject.Update();

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 18;
            headerStyle.margin.bottom = 10;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontStyle = FontStyle.Bold;

            bool dontDestroyOnLoad = manager.m_dontDestroyOnLoad;
            bool useSingleton = manager.m_useSingleton;

            GUILayout.Label("Timer Manager", headerStyle);

            EditorGUI.BeginChangeCheck();

            if (Application.isPlaying) GUI.enabled = false;

            DrawSingletonToggle();
            DrawDontDestroyOnLoadToggle();

            if (Application.isPlaying) DrawActiveTimerList();

            if (Application.isPlaying) GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(manager, "Timer Manager (Editor)");

                manager.m_dontDestroyOnLoad = dontDestroyOnLoad;
                manager.m_useSingleton = useSingleton;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(manager);
            }

            return;

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
                if (!useSingleton)
                {
                    dontDestroyOnLoad = false;
                    GUI.enabled = false;
                }

                GUIContent content = new GUIContent()
                {
                    text = "Don't Destroy On Load",
                    tooltip = "If true, this object will try to move itself to DontDestroyOnLoad scene on awake.",
                };

                dontDestroyOnLoad = EditorGUILayout.Toggle(content, dontDestroyOnLoad);

                if (!useSingleton) GUI.enabled = true; 
            }

            void DrawActiveTimerList()
            {
                SerializedProperty timerListProp = serializedObject.FindProperty("m_activeTimers");

                GUI.enabled = false;

                EditorGUILayout.PropertyField(timerListProp, new("Active Timers"), true);

                GUI.enabled = true;
            }

        }
    }
}
