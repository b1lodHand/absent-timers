//using com.absence.timersystem.internals;
//using UnityEditor;
//using UnityEngine;

//namespace com.absence.timersystem.editor
//{
//    [CustomPropertyDrawer(typeof(Timer))]
//    public class TimerPropertyDrawer : PropertyDrawer
//    {
//        const int k_lineCount = 3;
//        const float k_verticalPadding = 0f;

//        const float k_horizontalPadding = 4f;
//        const float k_horizontalSpacing = 8f;

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            Timer timer = property.boxedValue as Timer;

//            bool isExpanded = property.isExpanded;
//            //bool isCollapsed = boxedValue.TimerProvider == null 
//            //    || boxedValue.TimerProvider?.Invoke() == null;

//            //bool isCollapsed = boxedValue.Timer == null;

//            //bool isCollapsed = 
//            //    (reference == null) || ((!reference.IsValid) && (!reference.HasRecentData));

//            bool isCollapsed = false;

//            if (!isExpanded) return EditorGUIUtility.singleLineHeight
//                    + EditorGUIUtility.standardVerticalSpacing;

//            if (isCollapsed)
//                return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;

//            return (k_lineCount * EditorGUIUtility.singleLineHeight)
//                + (k_lineCount * EditorGUIUtility.standardVerticalSpacing)
//                + k_verticalPadding;
//        }

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            Timer timer = property.boxedValue as Timer;

//            //ITimer timer = null;

//            bool isExpanded = property.isExpanded;

//            //if (!providerNull) timer = boxedValue.TimerProvider?.Invoke();
//            //timer = boxedValue.Timer;

//            GUIContent realLabel = EditorGUI.BeginProperty(position, label, property);

//            GUIStyle style = new GUIStyle(GUI.skin.label);
//            style.richText = true;

//            float shift = k_verticalPadding / 2f;
//            position.y += shift;

//            position.height = EditorGUIUtility.singleLineHeight;

//            isExpanded = EditorGUI.Foldout(position, isExpanded, label, true);
//            property.isExpanded = isExpanded;

//            if (!isExpanded) return;

//            position.y += EditorGUIUtility.singleLineHeight;
//            position.y += EditorGUIUtility.standardVerticalSpacing;

//            GUI.enabled = false;

//            Draw();

//            GUI.enabled = true;

//            return;

//            void Draw()
//            {
//                //bool isNull = reference == null;
//                bool isNull = false;
//                //bool isValid = isNull ? false : reference.IsValid;
//                bool isValid = true;
//                //bool hasRecentData = isNull ? false : reference.HasRecentData;
//                bool hasRecentData = true;

//                //if (providerNull)
//                //{
//                //    EditorGUI.LabelField(position, "<i>Provider is null.</i>", style);
//                //    return;
//                //}

//                if (isNull || ((!isValid) && (!hasRecentData)))
//                {
//                    EditorGUI.LabelField(position, "<i>Timer is null.</i>", style);
//                    return;
//                }

//                float current = timer.CurrentTime;
//                float duration = timer.Duration;

//                Rect[] rects = Helpers.SliceRectHorizontally(position, 3, k_horizontalSpacing, 
//                    k_horizontalPadding, 1f, 6f, 1f);

//                EditorGUI.LabelField(rects[0], "0");

//                GUI.Slider(rects[1], current, 0f, 0f, duration, 
//                    GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true,
//                    0, null);

//                EditorGUI.LabelField(rects[2], duration.ToString());

//                position.y += EditorGUIUtility.singleLineHeight;
//                position.y += EditorGUIUtility.standardVerticalSpacing;

//                //string state = isValid ? timer.State.ToString() : reference.LastState;
//                string state = timer.State.ToString();

//                EditorGUI.LabelField(position, $"<i>State: {state} ({current})</i>", style);
//            }
//        }
//    }
//}
