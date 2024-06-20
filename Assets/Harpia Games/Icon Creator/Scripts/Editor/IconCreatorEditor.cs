using UnityEditor;
using UnityEngine;

namespace ItemIconCreator
{
    public class IconCreatorEditor : Editor
    {
        private SerializedProperty useDafaultNameProp;
        private SerializedProperty includeResolutionInFileNameProp;
        private SerializedProperty iconFileNameProp;
        private SerializedProperty useTransparencyProp;
        private SerializedProperty lookAtObjectCenterProp;
        private SerializedProperty dynamicFovProp;
        private SerializedProperty folderNameProp;
        private SerializedProperty modeProp;
        private SerializedProperty nextKeyProp;
        private SerializedProperty saveLocationProp;
        private SerializedProperty previewProp;
        private SerializedProperty fovOffsetProp;

        private bool showLocation = true;
        private string status = "Select a GameObject";

        protected void DrawDefaultIconCreatorInspector(IconCreator script)
        {
            GUILayout.BeginVertical("GroupBox");

            GUILayout.Label("1 - Select mode", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(modeProp, new GUIContent("Mode", "Manual mode you can set the angle of the object"));
            if (script.mode == IconCreator.Mode.Manual)
            {
                EditorGUILayout.PropertyField(nextKeyProp, new GUIContent("Next icon key", "Goes to next icon in manual mode"));
            }
            EditorGUILayout.PropertyField(previewProp, new GUIContent("Preview", "(recommended) Preview the icon in game view. Icon Camera Creator must be active in game view. 'Dynamic fov' or 'Look at mesh center' must be active"));

            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("2 - Select files location", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(saveLocationProp, (new GUIContent("Path", "Root folder of the icons")));
            EditorGUILayout.PropertyField(folderNameProp, new GUIContent("Root folder name", "The parent folder of your icons"));

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("3 - Personalize the files names", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useDafaultNameProp, new GUIContent("Use material / prefab name", "Use same name as material / prefab"));

            EditorGUILayout.PropertyField(includeResolutionInFileNameProp, (new GUIContent("Include resolution", "Include resolution into the file name?")));

            if (!script.useDafaultName)
            {
                EditorGUILayout.PropertyField(iconFileNameProp, new GUIContent("Icon file name", "The base name of the final png File"));

                if (string.IsNullOrWhiteSpace(script.iconFileName))
                {
                    EditorGUILayout.HelpBox("File name cannot be null or white spaces", MessageType.Warning);
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Final location: ");
            EditorGUILayout.LabelField(script.GetFinalFolder().Replace(@"\", @"/") + "/" + script.GetFileName("exempleName", 1));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("4 - Transparency settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useTransparencyProp, new GUIContent("Use transparency", "Do you want transparent background in your icons?"));
            //if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().Name == "UniversalRenderPipelineAsset" && script.useTransparency)
            //{
            //    EditorGUILayout.HelpBox("Transparency and post processing is not yet supported", MessageType.Warning);
            //}
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("5 - Camera settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(dynamicFovProp, new GUIContent("Dynamic fov ", "Try to find the best FOV value to fit the object whitin the image file."));

            if (script.dynamicFov)
            {
                EditorGUILayout.PropertyField(fovOffsetProp, new GUIContent("Icon padding", "Icon padding"));
            }

            EditorGUILayout.PropertyField(lookAtObjectCenterProp, new GUIContent("Look at mesh center", "Look at the object's mesh center"), script.lookAtObjectCenter);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("6 - Check your resolution", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current resolution " + script.mainCam.pixelWidth + " x " + script.mainCam.pixelHeight);
            EditorGUILayout.LabelField("Choose your desired resolution in Unity's Game window");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("7 - Position your camera", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Place the camera in the desired position");
            EditorGUILayout.LabelField("Tip: Use Ctrl + Shift + F");
            GUILayout.EndVertical();
        }


        protected static void DrawButtons(IconCreator script)
        {
            if (GUILayout.Button("Build icons!", GUILayout.Height(40)))
            {
                if (!script.CheckConditions()) return;

                if (!EditorApplication.isPlaying)
                {
                    Debug.LogError("You need to enter the play mode!");
                    return;
                }

                script.BuildIcons();
            }

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.Space(15);
                EditorGUILayout.HelpBox("You need to enter the play mode!", MessageType.Warning);
            }
        }

        protected void OnEnable()
        {
            // Fetch the objects from the GameObject script to display in the inspector

            useDafaultNameProp = serializedObject.FindProperty("useDafaultName");
            includeResolutionInFileNameProp = serializedObject.FindProperty("includeResolutionInFileName");
            iconFileNameProp = serializedObject.FindProperty("iconFileName");
            useTransparencyProp = serializedObject.FindProperty("useTransparency");
            lookAtObjectCenterProp = serializedObject.FindProperty("lookAtObjectCenter");
            dynamicFovProp = serializedObject.FindProperty("dynamicFov");
            folderNameProp = serializedObject.FindProperty("folderName");
            modeProp = serializedObject.FindProperty("mode");
            nextKeyProp = serializedObject.FindProperty("nextIconKey");
            saveLocationProp = serializedObject.FindProperty("pathLocation");
            previewProp = serializedObject.FindProperty("preview");
            fovOffsetProp = serializedObject.FindProperty("fovOffset");
        }
    }
}