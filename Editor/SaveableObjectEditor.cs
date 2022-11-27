using UnityEngine;
using UnityEditor;

namespace Saveable.Editor
{
    [CustomEditor(typeof(SaveableObject))]
    public class SaveableObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var label = new GUIStyle(EditorStyles.whiteLabel)
            {
                fontStyle = FontStyle.Bold,
                wordWrap = true,
            };

            var ctx = (SaveableObject)target;
            EditorGUI.BeginDisabledGroup(disabled: true);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (string.IsNullOrEmpty(ctx.ID))
                EditorGUILayout.LabelField(label: "ID is null", label);
            else
                EditorGUILayout.LabelField(label: ctx.ID, label);

            EditorGUILayout.EndHorizontal();

            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(label: $"Components [{ctx.Saveables.Count}]", label);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    } 
}