using com.absence.timersystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.timersystem.editor
{
    public static class EditorJobsHelper
    {
        [MenuItem("GameObject/absencee_/absent-timers/Timer Manager", priority = 0)]
        static void CreateTimerManager_MenuItem()
        {
            GameObject obj = new GameObject("Timer Manager");
            obj.AddComponent<TimerManager>();

            Undo.RegisterCreatedObjectUndo(obj, "Timer Manager Created via MenuItem");

            Selection.activeGameObject = obj; 
        }
    }
}
