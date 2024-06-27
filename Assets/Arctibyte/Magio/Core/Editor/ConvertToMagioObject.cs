using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Magio
{
    public class ConvertToMagioObject : EditorWindow
    {
        public enum ConvertSpawnerType
        {
            Mesh,
            SkinnedMesh
        }

        public GameObject magioEnginePrefab;
        public List<GameObject> magioObjs = new List<GameObject>();
        public Dictionary<Material, bool> materialCheckList = new Dictionary<Material, bool>();

        public ConvertSpawnerType spawnerType = ConvertSpawnerType.Mesh;
        public bool multiEffect = false;
        public bool onlyShader = false;


        HashSet<Shader> supportedShaders;
        Vector2 scrollPos = Vector2.zero;

        private void OnEnable()
        {
            supportedShaders = new HashSet<Shader>()
        {
            Shader.Find("Universal Render Pipeline/Lit"),
            Shader.Find("Universal Render Pipeline/Unlit"),
            Shader.Find("HDRP/Lit"),
            Shader.Find("HDRP/Unlit")
        };
        }

        [MenuItem("GameObject/Magio Convert/To Simple Magio Object", false, 10)]
        static void Convert(MenuCommand menuCommand)
        {
            ConvertToMagioObject window = (ConvertToMagioObject)EditorWindow.GetWindow(typeof(ConvertToMagioObject));

            bool skinned = false;
            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponentsInChildren<SkinnedMeshRenderer>().Length != 0)
                {
                    skinned = true;
                    break;
                }
            }

            if (skinned)
            {
                window.spawnerType = ConvertSpawnerType.SkinnedMesh;
            }
            else
            {
                window.spawnerType = ConvertSpawnerType.Mesh;
            }

            window.onlyShader = false;
            window.multiEffect = false;
            window.magioObjs.Clear();
            window.materialCheckList.Clear();
            window.magioObjs.AddRange(Selection.gameObjects.ToList());
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 300);
            window.Show();
        }

        [MenuItem("GameObject/Magio Convert/To Simple Magio Object", true, 10)]
        static bool ConvertValidate(MenuCommand menuCommand)
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Magio Convert/To Multi-effect Magio Object", false, 10)]
        static void ConvertBones(MenuCommand menuCommand)
        {
            ConvertToMagioObject window = (ConvertToMagioObject)EditorWindow.GetWindow(typeof(ConvertToMagioObject));
            bool skinned = false;
            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponentsInChildren<SkinnedMeshRenderer>().Length != 0)
                {
                    skinned = true;
                    break;
                }
            }

            if (skinned)
            {
                window.spawnerType = ConvertSpawnerType.SkinnedMesh;
            }
            else
            {
                window.spawnerType = ConvertSpawnerType.Mesh;
            }
            window.onlyShader = false;
            window.multiEffect = true;
            window.magioObjs.Clear();
            window.materialCheckList.Clear();
            window.magioObjs.AddRange(Selection.gameObjects.ToList());
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 420);
            window.Show();
        }

        [MenuItem("GameObject/Magio Convert/To Multi-effect Magio Object", true, 10)]
        static bool ConvertBonesValidate(MenuCommand menuCommand)
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Magio Convert/Only Materials", false, 10)]
        static void ConvertMaterial(MenuCommand menuCommand)
        {
            ConvertToMagioObject window = (ConvertToMagioObject)EditorWindow.GetWindow(typeof(ConvertToMagioObject));

            window.multiEffect = false;
            window.onlyShader = true;
            window.magioObjs.Clear();
            window.materialCheckList.Clear();
            window.magioObjs.AddRange(Selection.gameObjects.ToList());
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 420);
            window.Show();
        }

        [MenuItem("GameObject/Magio Convert/Only Materials", true, 10)]
        static bool ConvertMaterialValidate(MenuCommand menuCommand)
        {
            return Selection.activeGameObject != null;
        }




        void ConvertGOToMagioGO()
        {
            foreach (GameObject magioObj in magioObjs)
            {
                if (!onlyShader)
                {
                    if (spawnerType == ConvertSpawnerType.SkinnedMesh)
                    {
                        if (magioObj.GetComponentsInChildren<SkinnedMeshRenderer>().Length == 0)
                        {
                            Debug.LogWarning("No Skinned mesh renderer in object children, skipping: " + magioObj.name.ToString());
                            continue;
                        }
                    }
                    Debug.Log("Convert gameobject to magio object: " + magioObj.name.ToString());
                }
                else
                {
                    Debug.Log("Converting materials in: " + magioObj.name.ToString());
                }
            }

            HashSet<Shader> supportedURPLitShaders = new HashSet<Shader>()
            {
                Shader.Find("Universal Render Pipeline/Lit")
            };
            HashSet<Shader> supportedURPUnLitShaders = new HashSet<Shader>()
            {
                Shader.Find("Universal Render Pipeline/Unlit")
            };

            HashSet<Shader> supportedHDRPLitShaders = new HashSet<Shader>()
            {
                Shader.Find("HDRP/Lit")
            };

            HashSet<Shader> supportedHDRPUnLitShaders = new HashSet<Shader>()
            {
                Shader.Find("HDRP/Unlit")
            };

            List<Object> select = new List<Object>();

            foreach (GameObject go in magioObjs)
            {
                foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
                {
                    for (int i = 0; i < rend.sharedMaterials.Length; i++)
                    {
                        if (materialCheckList.ContainsKey(rend.sharedMaterials[i]) && materialCheckList[rend.sharedMaterials[i]])
                        {
                            Material cur = rend.sharedMaterials[i];
                            if (supportedURPUnLitShaders.Contains(cur.shader))
                            {
                                Undo.RecordObject(rend.sharedMaterials[i], "Shader change");
                                Color col = cur.GetColor("_BaseColor");
                                Color emCol = cur.GetColor("_EmissionColor");
                                Texture baseText = cur.GetTexture("_BaseMap");
                                Texture occlusionText = cur.GetTexture("_OcclusionMap");
                                Texture metallicText = cur.GetTexture("_MetallicGlossMap");
                                Texture emissionText = cur.GetTexture("_EmissionMap");
                                Vector2 tiling = cur.GetTextureScale("_BaseMap");
                                Vector2 offset = cur.GetTextureOffset("_BaseMap");

                                rend.sharedMaterials[i].shader = Shader.Find("Arctibyte/Magio");

                                rend.sharedMaterials[i].SetVector("_Tiling", tiling);
                                rend.sharedMaterials[i].SetVector("_Offset", offset);
                                rend.sharedMaterials[i].SetColor("_BaseColor", col);
                                rend.sharedMaterials[i].SetColor("_EmissionColor", emCol);
                                rend.sharedMaterials[i].SetTexture("_BaseMap", baseText);
                                rend.sharedMaterials[i].SetTexture("_OcclusionMap", occlusionText);
                                rend.sharedMaterials[i].SetTexture("_MetallicGlossMap", metallicText);
                                rend.sharedMaterials[i].SetTexture("_EmissionMap", emissionText);

                                if (!emissionText)
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 0);
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 1);
                                }
                            }
                            else if (supportedURPLitShaders.Contains(cur.shader))
                            {
                                Undo.RecordObject(rend.sharedMaterials[i], "Shader change");
                                Color col = cur.GetColor("_BaseColor");
                                Color emCol = cur.GetColor("_EmissionColor");
                                Texture baseText = cur.GetTexture("_BaseMap");
                                float smoothness = cur.GetFloat("_Smoothness");
                                float metallic = cur.GetFloat("_Metallic");
                                float normalStrength = cur.GetFloat("_BumpScale");
                                Texture normalText = cur.GetTexture("_BumpMap");
                                Vector2 tiling = cur.GetTextureScale("_BaseMap");
                                Vector2 offset = cur.GetTextureOffset("_BaseMap");
                                Texture occlusionText = cur.GetTexture("_OcclusionMap");
                                Texture metallicText = cur.GetTexture("_MetallicGlossMap");
                                Texture emissionText = cur.GetTexture("_EmissionMap");

                                rend.sharedMaterials[i].shader = Shader.Find("Arctibyte/Magio");

                                rend.sharedMaterials[i].SetColor("_BaseColor", col);
                                rend.sharedMaterials[i].SetColor("_EmissionColor", emCol);
                                rend.sharedMaterials[i].SetTexture("_BaseMap", baseText);
                                rend.sharedMaterials[i].SetVector("_Tiling", tiling);
                                rend.sharedMaterials[i].SetVector("_Offset", offset);
                                rend.sharedMaterials[i].SetTexture("_NormalMap", normalText);
                                rend.sharedMaterials[i].SetFloat("_Smoothness", smoothness);
                                rend.sharedMaterials[i].SetFloat("_Metallic", metallic);
                                rend.sharedMaterials[i].SetTexture("_OcclusionMap", occlusionText);
                                rend.sharedMaterials[i].SetTexture("_MetallicGlossMap", metallicText);
                                rend.sharedMaterials[i].SetTexture("_EmissionMap", emissionText);

                                if (normalText == null)
                                {
                                    rend.sharedMaterials[i].SetFloat("_NormalStrength", 0);
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetFloat("_NormalStrength", normalStrength);
                                }

                                if (!emissionText)
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 0);
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 1);
                                }
                            }
                            else if (supportedHDRPUnLitShaders.Contains(cur.shader))
                            {
                                Undo.RecordObject(rend.sharedMaterials[i], "Shader change");
                                Color col = cur.GetColor("_UnlitColor");
                                Color emCol = cur.GetColor("_EmissiveColor");
                                Texture baseText = cur.GetTexture("_UnlitColorMap");
                                Vector2 tiling = cur.GetTextureScale("_UnlitColorMap");
                                Vector2 offset = cur.GetTextureOffset("_UnlitColorMap");
                                Texture emissionText = cur.GetTexture("_EmissiveColorMap");

                                rend.sharedMaterials[i].shader = Shader.Find("Arctibyte/MagioHDRP");

                                rend.sharedMaterials[i].SetVector("_Tiling", tiling);
                                rend.sharedMaterials[i].SetVector("_Offset", offset);
                                rend.sharedMaterials[i].SetColor("_BaseColor", col);
                                rend.sharedMaterials[i].SetColor("_EmissionColor", emCol);
                                rend.sharedMaterials[i].SetTexture("_BaseMap", baseText);
                                rend.sharedMaterials[i].SetTexture("_EmissionMap", emissionText);

                                if (!emissionText)
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 0);
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 1);
                                }
                            }
                            else if (supportedHDRPLitShaders.Contains(cur.shader))
                            {
                                Undo.RecordObject(rend.sharedMaterials[i], "Shader change");
                                Color col = cur.GetColor("_BaseColor");
                                Color emCol = cur.GetColor("_EmissionColor");
                                Texture baseText = cur.GetTexture("_BaseColorMap");
                                float smoothness = cur.GetFloat("_Smoothness");
                                float metallic = cur.GetFloat("_Metallic");
                                float normalStrength = cur.GetFloat("_NormalScale");
                                Texture normalText = cur.GetTexture("_NormalMap");
                                Vector2 tiling = cur.GetTextureScale("_BaseColorMap");
                                Vector2 offset = cur.GetTextureOffset("_BaseColorMap");
                                Texture maskMap = cur.GetTexture("_MaskMap");
                                Texture coatMaskMap = cur.GetTexture("_CoatMaskMap");
                                Texture emissionText = cur.GetTexture("_EmissiveColorMap");
                                float coatMask = cur.GetFloat("_CoatMask");

                                float metallicRemapMin = cur.GetFloat("_MetallicRemapMin");
                                float metallicRemapMax = cur.GetFloat("_MetallicRemapMax");
                                float smoothnessRemapMin = cur.GetFloat("_SmoothnessRemapMin");
                                float smoothnessRemapMax = cur.GetFloat("_SmoothnessRemapMax");
                                float AORemapMin = cur.GetFloat("_AORemapMin");
                                float AORemapMax = cur.GetFloat("_AORemapMax");

                                rend.sharedMaterials[i].shader = Shader.Find("Arctibyte/MagioHDRP");

                                rend.sharedMaterials[i].SetColor("_BaseColor", col);
                                rend.sharedMaterials[i].SetColor("_EmissionColor", emCol);
                                rend.sharedMaterials[i].SetTexture("_BaseMap", baseText);
                                rend.sharedMaterials[i].SetVector("_Tiling", tiling);
                                rend.sharedMaterials[i].SetVector("_Offset", offset);
                                rend.sharedMaterials[i].SetTexture("_NormalMap", normalText);

                                rend.sharedMaterials[i].SetFloat("_CoatMask", coatMask);
                                rend.sharedMaterials[i].SetTexture("_MaskMap", maskMap);
                                rend.sharedMaterials[i].SetTexture("_EmissionMap", emissionText);
                                rend.sharedMaterials[i].SetTexture("_CoatMaskMap", coatMaskMap);
                                rend.sharedMaterials[i].SetFloat("_AlphaCutoffEnable", 1);


                                if (normalText == null)
                                {
                                    rend.sharedMaterials[i].SetFloat("_NormalStrength", 0);
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetFloat("_NormalStrength", normalStrength);
                                }

                                if (!emissionText)
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 0);
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetFloat("_EmissionStrength", 1);
                                }

                                if (maskMap == null)
                                {
                                    rend.sharedMaterials[i].SetVector("_SmoothnessRemap", new Vector2(smoothness, smoothness));
                                    rend.sharedMaterials[i].SetVector("_MetallicRemap", new Vector2(metallic, metallic));
                                    rend.sharedMaterials[i].SetVector("_AORemap", new Vector2(0, 1));
                                }
                                else
                                {
                                    rend.sharedMaterials[i].SetVector("_SmoothnessRemap", new Vector2(smoothnessRemapMin, smoothnessRemapMax));
                                    rend.sharedMaterials[i].SetVector("_MetallicRemap", new Vector2(metallicRemapMin, metallicRemapMax));
                                    rend.sharedMaterials[i].SetVector("_AORemap", new Vector2(AORemapMin, AORemapMax));
                                }

                                select.Add(rend.sharedMaterials[i]);
                            }

                        }
                    }
                }
            }

            if (select.Count > 0)
            {
                DelayedSelect(select);
            }


            Object magioEngine = Object.FindObjectOfType<MagioEngine>();
            if (!magioEngine)
            {
                if (!magioEnginePrefab)
                {
                    magioEnginePrefab = AssetDatabase.LoadAssetAtPath("Assets/Arctibyte/Magio/Core/Prefabs/Engine/MagioEngine.prefab", typeof(GameObject)) as GameObject;
                }
                if (!magioEnginePrefab)
                {
                    Debug.LogError("Cannot find flameEngine prefab in path: Assets/Arctibyte/Magio/Core/Prefabs/Engine/MagioEngine.prefab");
                }

                Undo.RegisterCreatedObjectUndo(PrefabUtility.InstantiatePrefab(magioEnginePrefab), "Created go");
            }

            if (!onlyShader)
            {
                foreach (GameObject magioObj in magioObjs)
                {
                    if (multiEffect)
                    {
                        if (!magioObj.GetComponentInChildren<MagioObjectEffect>())
                        {
                            GameObject magioParent = new GameObject();
                            Undo.RegisterCreatedObjectUndo(magioParent, "Created magio effect parent");

                            magioParent.name = "Magio Effects";
                            magioParent.transform.parent = magioObj.transform;
                            magioParent.transform.localPosition = Vector3.zero;
                            magioParent.transform.localRotation = Quaternion.identity;

                            GameObject firstMagioEffect = new GameObject();

                            Undo.RegisterCreatedObjectUndo(firstMagioEffect, "Created magio element");

                            firstMagioEffect.name = "New Magio Effect";
                            firstMagioEffect.transform.parent = magioParent.transform;
                            firstMagioEffect.transform.localPosition = Vector3.zero;
                            firstMagioEffect.transform.localRotation = Quaternion.identity;

                            MagioObjectEffect obj = Undo.AddComponent<MagioObjectEffect>(firstMagioEffect);
                            if (spawnerType == ConvertSpawnerType.SkinnedMesh)
                            {
                                obj.vfxSpawnerType = VFXSpawnerType.SkinnedMesh;
                            }

                            obj.useOnThisGameObject = false;
                            obj.targetGameObject = magioObj;

                            MagioObjectMaster master = Undo.AddComponent<MagioObjectMaster>(magioObj);
                            master.effectParent = magioParent;
                        }
                        else
                        {
                            Debug.Log("Found Magio Object Effect in children of:" + magioObj.name + ". Skipping adding Magio Effects..");
                        }

                    }
                    else
                    {
                        if (!magioObj.GetComponent<MagioObjectEffect>())
                        {
                            MagioObjectEffect obj = Undo.AddComponent<MagioObjectEffect>(magioObj);
                            if (spawnerType == ConvertSpawnerType.SkinnedMesh)
                            {
                                obj.vfxSpawnerType = VFXSpawnerType.SkinnedMesh;
                            }
                        }
                        else
                        {
                            Debug.Log("Found Magio Object Effect in:" + magioObj.name + ". Skipping adding Magio Effects..");
                        }
                    }
                }
            }

        }

        async void DelayedSelect(List<Object> select)
        {
            Object[] curSelect = Selection.objects;
            Selection.objects = select.ToArray();

            await System.Threading.Tasks.Task.Delay(200);
            Selection.objects = curSelect;
        }


        void OnGUI()
        {
            if (magioObjs.Count <= 0)
                this.Close();

            if (magioObjs.Count > 0)
            {
                if (!onlyShader)
                {
                    spawnerType = (ConvertSpawnerType)EditorGUILayout.EnumPopup("Spawner type:", spawnerType);
                    GUILayout.Space(20);
                }

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Select materials you want to convert to Magio shader.", EditorStyles.wordWrappedLabel);
                    GUILayout.Space(20);

                    scrollPos =
                        EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(300), GUILayout.Height(130));
                    bool foundMats = false;


                    foreach (GameObject go in magioObjs)
                    {
                        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

                        foreach (Renderer rend in rends)
                        {

                            for (int i = 0; i < rend.sharedMaterials.Length; i++)
                            {
                                if (supportedShaders.Contains(rend.sharedMaterials[i].shader))
                                {
                                    foundMats = true;
                                    if (!materialCheckList.ContainsKey(rend.sharedMaterials[i])) materialCheckList.Add(rend.sharedMaterials[i], true);


                                    bool check = EditorGUILayout.ToggleLeft(rend.gameObject.name + ": " + rend.sharedMaterials[i].name, materialCheckList[rend.sharedMaterials[i]]);

                                    if (check)
                                    {
                                        materialCheckList[rend.sharedMaterials[i]] = true;
                                    }
                                    else
                                    {
                                        materialCheckList[rend.sharedMaterials[i]] = false;
                                    }
                                }
                            }
                        }
                    }

                    EditorGUILayout.EndScrollView();

                    if (foundMats)
                    {
                        EditorGUILayout.LabelField("Selected materials will be converted. Do you want to continue?", EditorStyles.wordWrappedLabel);
                        GUILayout.Space(10);
                    }

                    if (GUILayout.Button("Convert!"))
                    {
                        ConvertGOToMagioGO();
                        this.Close();
                    }
                }
                EditorGUILayout.EndVertical();
            }

        }
    }
}
