using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using static Saveable.SaveableExtensions;

namespace Saveable.Editor
{
    [CustomEditor(typeof(SaveableManager))]
    public class SaveableManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty pathProperty = default;

        private SerializedProperty FindPropertyByAutoPropertyName(string propName) => serializedObject.FindProperty(GetAutoPropertyName(propName));

        private void OnEnable() => pathProperty = FindPropertyByAutoPropertyName("FilePath");

        public override void OnInspectorGUI()
        {
            var button = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 12,
                richText = true,
                wordWrap = true,
            };

            var pathLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
            };

            var manager = (SaveableManager)target;
            DrawPropertiesExcluding(serializedObject, GetAutoPropertyName("FilePath"));

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.LabelField(pathProperty.stringValue, pathLabel);

            if (GUILayout.Button(text: "Find", GUILayout.MaxWidth(maxWidth: 45)))
            {
                SearchWindow.Open(new SearchWindowContext
                (
                    GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    new SavefilesSearchProvider(manager)
                );
            }

            if (!string.IsNullOrEmpty(pathProperty.stringValue))
            {
                if (GUILayout.Button(text: "Edit", GUILayout.MaxWidth(45)))
                    OpenFile(manager.GetFullPath());

                if (GUILayout.Button(text: "Show", GUILayout.MaxWidth(45)))
                    ShowFile($"/open, {System.IO.Path.GetDirectoryName(manager.GetFullPath())}");

                if (GUILayout.Button(text: "Delete", GUILayout.MaxWidth(60)))
                {
                    if (!EditorUtility.DisplayDialog(title: "Deleting save file.",
                        message: $"Are you sure you want to delete '{manager.FilePath}' file? You cannot recover the file beyond this point!",
                        ok: "Yes", cancel: "No"))
                        return;

                    manager.DeleteSave();

                    pathProperty.stringValue = string.Empty;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();

                    UnityEditor.AssetDatabase.Refresh();
                    return;
                }
            }
            else
            {
                if (GUILayout.Button(text: "Show In Explorer", GUILayout.MaxWidth(110)))
                    OpenFile(System.IO.Path.GetDirectoryName(manager.GetDirectoryPath()));
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<b>Reload</b> Snapshot", button))
                manager.ReloadSnapshot();

            if (GUILayout.Button("<b>Save</b> Snapshot", button))
                manager.SaveSnapshot();

            if (GUILayout.Button("<b>Delete</b> Snapshot", button))
                manager.DeleteSnapshot();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<b>Fetch</b> Snapshot", button))
                manager.FetchSnapshot();

            if (GUILayout.Button("<b>Push</b> Snapshot", button))
                manager.PushSnapshot();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<b>Fetch&Reload</b> Snapshot", button))
                manager.FetchAndReloadSnapshot();

            if (GUILayout.Button("<b>Save&Push</b> Snapshot", button))
                manager.SaveAndPushSnapshot();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("<b>Create Save</b>", button))
                manager.CreateSave();

            if (GUILayout.Button("<b>Clear Save</b>", button))
                manager.ClearSave();

            //if (GUILayout.Button("<b>Print Save [JSON]</b>", button) &&
            //    manager.GetJsonFromSave(out var json))
            //{
            //    if (string.IsNullOrEmpty(json))
            //        return;

            //    foreach (SceneView scene in SceneView.sceneViews)
            //        scene.ShowNotification(new GUIContent(text: "Copy to clipboard"));

            //    GUIUtility.systemCopyBuffer = json;
            //    Debug.Log(json);
            //}

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    } 
}