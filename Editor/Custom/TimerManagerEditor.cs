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

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 18;
            headerStyle.margin.bottom = 10;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label("Timer Manager", headerStyle);

            Undo.RecordObject(manager, "Timer Manager (Editor)");

            EditorGUI.BeginChangeCheck();

            if (Application.isPlaying) GUI.enabled = false;

            bool dontDestroyOnLoad = manager.m_dontDestroyOnLoad;

            GUIContent ddolContent = new GUIContent()
            {
                text = "Don't Destroy On Load",
                tooltip = "If true, this object will try to move itself to DontDestroyOnLoad scene on awake.",
            };

            dontDestroyOnLoad = EditorGUILayout.Toggle(ddolContent, dontDestroyOnLoad);

            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                manager.m_dontDestroyOnLoad = dontDestroyOnLoad;
                EditorUtility.SetDirty(manager);
            }
        }
    }
}
