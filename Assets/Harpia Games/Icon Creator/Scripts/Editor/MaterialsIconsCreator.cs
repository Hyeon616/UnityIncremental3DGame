using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ItemIconCreator
{

    [CustomEditor(typeof(MaterialIconCreator))]
    public class MaterialsIconsCreator : IconCreatorEditor
    {
        SerializedProperty renderTargetProp;
        SerializedProperty materialListtProp;

        public override void OnInspectorGUI()
        {
            MaterialIconCreator script = (MaterialIconCreator)target;

            DrawDefaultIconCreatorInspector(script);

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("8 - Select the target renderer", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(renderTargetProp, new GUIContent("Target renderer"));
            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("9 - Select the materials", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(materialListtProp, new GUIContent("Material list", "Materials list to take pictures"));
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            DrawButtons(script);

        }



        void OnEnable()
        {
            base.OnEnable();

            // Fetch the objects from the GameObject script to display in the inspector
            renderTargetProp = serializedObject.FindProperty("targetRenderer");
            materialListtProp = serializedObject.FindProperty("materials");
        }




    }
}
