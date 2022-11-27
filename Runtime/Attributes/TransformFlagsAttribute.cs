using UnityEngine;
using static Saveable.SaveableTransform;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Saveable
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TransformFlagsAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TransformFlagsAttribute))]
    public class TransformFlagsDrawer : PropertyDrawer
    {
        private bool foldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            foldout = EditorGUI.Foldout(position, foldout, content: "Transform Constraints");

            if (!foldout)
                return;

            EditorGUI.indentLevel++;

            var flags = (TransformConstraints)property.enumValueFlag;
            property.serializedObject.Update();

            var labelWidth = GUILayout.Width(80);

            var toggleWidth = GUILayout.Width(40);
            var toggleHeight = GUILayout.Height(20);

            var toggleOptions = new GUILayoutOption[2] { toggleWidth, toggleHeight };

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label: "Position", labelWidth, 
                GUILayout.ExpandWidth(expand: false));

            var posX = EditorGUILayout.ToggleLeft(label: "X", value: 
                flags.HasFlag(TransformConstraints.PositionX), toggleOptions);

            var posY = EditorGUILayout.ToggleLeft(label: "Y", value: 
                flags.HasFlag(TransformConstraints.PositionY), toggleOptions);

            var posZ = EditorGUILayout.ToggleLeft(label: "Z", value: 
                flags.HasFlag(TransformConstraints.PositionZ), toggleOptions);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label: "Rotation", labelWidth,
                GUILayout.ExpandWidth(expand: false));

            var rotX = EditorGUILayout.ToggleLeft(label: "X", value: flags.HasFlag(TransformConstraints.RotationX), toggleOptions);

            var rotY = EditorGUILayout.ToggleLeft(label: "Y", value: flags.HasFlag(TransformConstraints.RotationY), toggleOptions);

            var rotZ = EditorGUILayout.ToggleLeft(label: "Z", value: flags.HasFlag(TransformConstraints.RotationZ), toggleOptions);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label: "Scale", labelWidth,
                GUILayout.ExpandWidth(expand: false));

            var scaleX = EditorGUILayout.ToggleLeft(label: "X", value: flags.HasFlag(TransformConstraints.ScaleX), toggleOptions);

            var scaleY = EditorGUILayout.ToggleLeft(label: "Y", value: flags.HasFlag(TransformConstraints.ScaleY), toggleOptions);

            var scaleZ = EditorGUILayout.ToggleLeft(label: "Z", value: flags.HasFlag(TransformConstraints.ScaleZ), toggleOptions);

            EditorGUILayout.EndHorizontal();

            if (posX)
                AddFlag(TransformConstraints.PositionX);
            else
                RemoveFlag(TransformConstraints.PositionX);

            if (posY)
                AddFlag(TransformConstraints.PositionY);
            else
                RemoveFlag(TransformConstraints.PositionY);

            if (posZ)
                AddFlag(TransformConstraints.PositionZ);
            else
                RemoveFlag(TransformConstraints.PositionZ);

            if (rotX)
                AddFlag(TransformConstraints.RotationX);
            else
                RemoveFlag(TransformConstraints.RotationX);

            if (rotY)
                AddFlag(TransformConstraints.RotationY);
            else
                RemoveFlag(TransformConstraints.RotationY);

            if (rotZ)
                AddFlag(TransformConstraints.RotationZ);
            else
                RemoveFlag(TransformConstraints.RotationZ);

            if (scaleX)
                AddFlag(TransformConstraints.ScaleX);
            else
                RemoveFlag(TransformConstraints.ScaleX);

            if (scaleY)
                AddFlag(TransformConstraints.ScaleY);
            else
                RemoveFlag(TransformConstraints.ScaleY);

            if (scaleZ)
                AddFlag(TransformConstraints.ScaleZ);
            else
                RemoveFlag(TransformConstraints.ScaleZ);

            EditorGUI.indentLevel = indent;

            property.enumValueFlag = (int)flags;
            property.serializedObject.ApplyModifiedProperties();

            void AddFlag(TransformConstraints flag) => flags |= flag;

            void RemoveFlag(TransformConstraints flag) => flags &= ~flag;
        }
    }
#endif
}