using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Magio
{
    [CustomEditor(typeof(MagioEngine))]
    public class MagioEngineEditor : Editor
    {
        private MagioEngine eng;

        private void OnEnable()
        {
            eng = (MagioEngine)target;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space(20);


            if (!eng.pause)
            {

                if (GUILayout.Button("Pause effects"))
                {
                    eng.PauseEffects();
                }
            }
            else
            {
                if (GUILayout.Button("Resume effects"))
                {
                    eng.ResumeEffects();
                }
            }
        }
    }
}
