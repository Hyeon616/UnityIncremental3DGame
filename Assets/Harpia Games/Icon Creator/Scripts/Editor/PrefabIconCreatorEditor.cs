using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ItemIconCreator
{

    [CustomEditor(typeof(PrefabIconCreator))]

    public class PrefabIconCreatorEditor : IconCreatorEditor
    {

        SerializedProperty itemPositionProp;
        SerializedProperty itensToShotProp;



        public override void OnInspectorGUI()
        {
            PrefabIconCreator script = (PrefabIconCreator) target;

            DrawDefaultIconCreatorInspector(script);

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("8 - Select prefab spawn position", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(itemPositionProp, new GUIContent("Item position"));
            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("9 - Select the prefabs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(itensToShotProp, new GUIContent("Prefabs", "Prefab list to take pictures"));
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();


            DrawButtons(script);
        }


        void OnEnable()
        {
            base.OnEnable();
            // Fetch the objects from the GameObject script to display in the inspector
            itemPositionProp = serializedObject.FindProperty("itemPosition");
            itensToShotProp = serializedObject.FindProperty("itensToShot");

        }

    }
}