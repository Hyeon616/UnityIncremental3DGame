using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Magio
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MagioObjectMaster))]
    public class MagioEffectMasterEditor : Editor
    {
        public struct EffectInfo
        {
            public bool differentMaster;
            public bool enableOnStart;
            public MagioObjectEffect magioObj;
            public MagioObjectMaster master;
        }


        private SerializedProperty effectParent;
        private void OnEnable()
        {
            effectParent = serializedObject.FindProperty("effectParent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(effectParent);

            EditorGUILayout.Space(20);

            Dictionary<string, List<EffectInfo>> effectsToGameObjects = new Dictionary<string, List<EffectInfo>>();

            foreach(UnityEngine.Object masterO in targets)
            {
                MagioObjectMaster master = masterO as MagioObjectMaster;

                if (!master.effectParent && !Application.isPlaying)
                {
                    GUIStyle s = new GUIStyle(EditorStyles.textField);
                    s.normal.textColor = Color.red;
                    EditorGUILayout.LabelField("Error: Please assign effect parent.", s);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                    return;
                }

                MagioObjectEffect[] effects;
                if (!Application.isPlaying)
                {
                    if (master.effectParent)
                        effects = master.effectParent.GetComponentsInChildren<MagioObjectEffect>(true);
                    else
                    {
                        effects = new MagioObjectEffect[0];
                    }
                }
                else
                {
                    effects = master.magioObjects.ToArray();
                }

                foreach (MagioObjectEffect obj in effects)
                {
                    GameObject eff = obj.EffectPrefab;
                    if (MagioEngine.instance.effectPacks.Count > obj.effectPackNumber && obj.effectPackNumber > -1)
                    {
                        if (obj.vfxSpawnerType == VFXSpawnerType.Mesh)
                        {
                            if (MagioEngine.instance.effectPacks[obj.effectPackNumber].meshEffects.Count > obj.effectNumber)
                            {
                                eff = MagioEngine.instance.effectPacks[obj.effectPackNumber].meshEffects[obj.effectNumber];
                            }
                        }
                        else
                        {
                            if (MagioEngine.instance.effectPacks[obj.effectPackNumber].skinnedMeshEffects.Count > obj.effectNumber)
                            {
                                eff = MagioEngine.instance.effectPacks[obj.effectPackNumber].skinnedMeshEffects[obj.effectNumber];
                            }
                        }
                    }

                    string effName;
                    if (!eff)
                    {
                        effName = "INVALID";
                    }
                    else
                    {
                        effName = string.Join("", eff.name.Split('_').Skip(1));
                    }



                    bool differentMaster = false;
                    if (obj.targetGameObject != master.gameObject || (obj.useOnThisGameObject && obj.gameObject != master.gameObject))
                    {
                        differentMaster = true;
                    }

                    effName = SeparateCamelCase(effName);

                    if (!effectsToGameObjects.ContainsKey(effName))
                    {
                        effectsToGameObjects.Add(effName, new List<EffectInfo>());
                    }

                    EffectInfo info = new EffectInfo
                    {
                        differentMaster = differentMaster,
                        magioObj = obj,
                        master = master,
                        enableOnStart = obj.beginEffectOnStart
                    };

                    effectsToGameObjects[effName].Add(info);
                }
            }
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
                

            foreach(string key in effectsToGameObjects.Keys)
            {
                GUILayout.BeginHorizontal("box");
                GUIStyle s = new GUIStyle(EditorStyles.boldLabel);
                s.wordWrap = true;
                bool differentMaster = false;
                int beginOnStart = 0;

                foreach(EffectInfo info in effectsToGameObjects[key])
                {
                    MagioObjectEffect obj = info.magioObj;
                    s.normal.textColor = obj.shaderEmissionColor;

                    if (info.differentMaster)
                    {
                        differentMaster = true;
                    }
                    if (info.enableOnStart)
                    {
                        beginOnStart++;
                    }
                }

                string additional = "";

                if(beginOnStart > 0)
                {
                    additional = " (" + beginOnStart + " Play)";
                }

                EditorGUILayout.LabelField(effectsToGameObjects[key].Count + " - " + key + additional, s);

                if (differentMaster)
                {
                    if (GUILayout.Button("Link to parent", GUILayout.Width(100)))
                    {
                        foreach (EffectInfo info in effectsToGameObjects[key])
                        {
                            MagioObjectEffect obj = info.magioObj;
                            Undo.RecordObject(obj, "Changed Target GameObject");
                            obj.targetGameObject = info.master.gameObject;
                            obj.useOnThisGameObject = false;
                            if (info.master.GetComponentInChildren<SkinnedMeshRenderer>())
                            {
                                obj.vfxSpawnerType = VFXSpawnerType.SkinnedMesh;
                            }
                            else
                            {
                                obj.vfxSpawnerType = VFXSpawnerType.Mesh;
                            }
                        }
                        
                    }
                }

                if (GUILayout.Button("Edit", GUILayout.Width(100)))
                {
                    List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
                    foreach(EffectInfo info in effectsToGameObjects[key])
                    {
                        objs.Add(info.magioObj.gameObject);
                    }

                    Selection.objects = objs.ToArray();
                }

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    foreach (EffectInfo info in effectsToGameObjects[key])
                    {
                        Undo.DestroyObjectImmediate(info.magioObj.gameObject);
                    }
                    
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Add Effect.."))
            {
                GenericMenu menu = new GenericMenu();

                List<string> effectPacks = new List<string>();
                foreach (MagioEffectPack pack in MagioEngine.instance.effectPacks)
                {
                    effectPacks.Add(SeparateCamelCase(pack.name));
                }


                bool skinnedMesh = false;
                if (((MagioObjectMaster)target).GetComponentInChildren<SkinnedMeshRenderer>())
                {
                    skinnedMesh = true;
                }
                else
                {
                    skinnedMesh = false;
                }
                List<string> effects = new List<string>();

                if (!skinnedMesh)
                {
                    for(int i = 0; i < effectPacks.Count; i++)
                    {
                        int a = 0;
                        foreach (GameObject eff in MagioEngine.instance.effectPacks[i].meshEffects)
                        {
                            string effName = string.Join("", eff.name.Split('_').Skip(1));
                            menu.AddItem(new GUIContent(effectPacks[i] + "/" + SeparateCamelCase(effName)), false, AddEffect, new List<int> { i, a });
                            a++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < effectPacks.Count; i++)
                    {
                        int a = 0;
                        foreach (GameObject eff in MagioEngine.instance.effectPacks[i].skinnedMeshEffects)
                        {
                            string effName = string.Join("", eff.name.Split('_').Skip(1));
                            menu.AddItem(new GUIContent(effectPacks[i] + "/" + SeparateCamelCase(effName)), false, AddEffect, new List<int> { i, a });
                            a++;
                        }
                    }
                }

                menu.ShowAsContext();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void AddEffect(object effectPackAndNumber)
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            List<int> effect = effectPackAndNumber as List<int>;

            foreach (UnityEngine.Object masterO in targets)
            {
                MagioObjectMaster master = masterO as MagioObjectMaster;
                GameObject newEffect = new GameObject();

                Undo.RegisterCreatedObjectUndo(newEffect, "Created magio element");

                newEffect.name = "New Magio Effect";

                if (master.effectParent)
                {
                    newEffect.transform.parent = master.effectParent.transform;
                }

                newEffect.transform.localPosition = Vector3.zero;
                newEffect.transform.localRotation = Quaternion.identity;

                MagioObjectEffect obj = Undo.AddComponent<MagioObjectEffect>(newEffect);

                if (master.GetComponentInChildren<SkinnedMeshRenderer>())
                {
                    obj.vfxSpawnerType = VFXSpawnerType.SkinnedMesh;
                }
                else
                {
                    obj.vfxSpawnerType = VFXSpawnerType.Mesh;
                }

                obj.useOnThisGameObject = false;
                obj.targetGameObject = master.gameObject;
                obj.effectPackNumber = effect[0];
                obj.effectNumber = effect[1];


                if (MagioEngine.instance.effectPacks.Count > obj.effectPackNumber)
                {
                    if (obj.vfxSpawnerType == VFXSpawnerType.Mesh)
                    {
                        if (MagioEngine.instance.effectPacks[obj.effectPackNumber].meshEffects.Count > obj.effectNumber)
                        {
                            obj.EffectPrefab = MagioEngine.instance.effectPacks[obj.effectPackNumber].meshEffects[obj.effectNumber];
                        }
                        else
                        {
                            obj.effectNumber = 0;
                            obj.EffectPrefab = MagioEngine.instance.effectPacks[obj.effectPackNumber].meshEffects[obj.effectNumber];
                        }
                    }
                    else
                    {
                        if (MagioEngine.instance.effectPacks[obj.effectPackNumber].skinnedMeshEffects.Count > obj.effectNumber)
                        {
                            obj.EffectPrefab = MagioEngine.instance.effectPacks[obj.effectPackNumber].skinnedMeshEffects[obj.effectNumber];
                        }
                        else
                        {
                            obj.effectNumber = 0;
                            obj.EffectPrefab = MagioEngine.instance.effectPacks[obj.effectPackNumber].skinnedMeshEffects[obj.effectNumber];
                        }
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public string SeparateCamelCase(string source)
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z](?![A-Z]|$))")); ;
        }
    }
}

