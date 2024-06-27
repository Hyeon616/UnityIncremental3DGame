using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Spreading physics. Creates overlap box and uses math to spread the effect if necessary.
    /// </summary>
    public class SpreadPhysics : MonoBehaviour
    {

        struct CurOnTouchMaterials
        {
            public GameObject obj;
            public Material mat;
            public OAVAShaderCompatibilitySO shaderComp;
        }

        struct CurMagioObjs
        {
            public MagioObjectEffect magioObj;
            public Vector3 pos;
        }

        public MagioObjectEffect myMagioObj;

        private readonly Dictionary<MagioObjectEffect, CurMagioObjs> magioObjsAndPositions = new Dictionary<MagioObjectEffect, CurMagioObjs>();
        private readonly HashSet<IInteractWithEffect> interactWithFires = new HashSet<IInteractWithEffect>();
        private readonly Dictionary<GameObject, CurOnTouchMaterials> onTouchObjs = new Dictionary<GameObject, CurOnTouchMaterials>();

        private void FixedUpdate()
        {
            foreach(MagioObjectEffect magioObj in magioObjsAndPositions.Keys.ToList())
            {
                if (magioObj)
                {
                    magioObj.TryToAnimateEffect(magioObjsAndPositions[magioObj].pos, Time.fixedDeltaTime);
                }
                else
                {
                    magioObjsAndPositions.Remove(magioObj);
                }
            }

            foreach (IInteractWithEffect interactable in interactWithFires.ToList())
            {
                if (interactable != null && !interactable.Equals(null))
                {
                    interactable.OnCollisionWithEffect(myMagioObj.gameObject);
                }
                else
                {
                    interactWithFires.Remove(interactable);
                }
            }

            foreach(GameObject matGO in onTouchObjs.Keys.ToList())
            {
                if (matGO)
                {
                    CurOnTouchMaterials mats = onTouchObjs[matGO];
                    foreach (OAVAShaderCompatibilitySO.ShaderProperty prop in mats.shaderComp.onTouchChangeProperties)
                    {
                        if (mats.mat.HasProperty(prop.name))
                        {
                            mats.mat.SetFloat(prop.name, Mathf.MoveTowards(mats.mat.GetFloat(prop.name), prop.targetValue, Time.deltaTime * prop.speedMultiplier));
                        }
                    }
                }
                else
                {
                    onTouchObjs.Remove(matGO);
                }
                
            }
        }


        /// <summary>
        /// Starts the spread.
        /// </summary>
        public void StartTrigger()
        {
            InvokeRepeating(nameof(SpreadPhysicsUpdate), Random.Range(0, 1/MagioEngine.instance.SpreadPhysicsCheckFrequency), 1/MagioEngine.instance.SpreadPhysicsCheckFrequency);
        }

        /// <summary>
        /// One update call for spread physics.
        /// </summary>
        public void SpreadPhysicsUpdate()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            magioObjsAndPositions.Clear();
            onTouchObjs.Clear();
            interactWithFires.Clear();
     
            if (myMagioObj.HasEffectEnded()) return;

            foreach(Collider col in myMagioObj.spreadingColliders)
            {
                Vector3 center = Vector3.zero;
                Vector3 extents = Vector3.zero;
                if (col.GetType() == typeof(BoxCollider))
                {
                    BoxCollider box = col as BoxCollider;
                    center = col.transform.TransformPoint(box.center);
                    extents = Vector3.Scale(box.size, box.transform.lossyScale) / 2;
                }
                else if (col.GetType() == typeof(SphereCollider))
                {
                    SphereCollider sphere = col as SphereCollider;

                    center = col.transform.TransformPoint(sphere.center);
                    extents = Vector3.Scale(new Vector3(sphere.radius * 2, sphere.radius * 2, sphere.radius * 2), sphere.transform.lossyScale) / 2;
                }
                else if (col.GetType() == typeof(CapsuleCollider))
                {
                    CapsuleCollider capsule = col as CapsuleCollider;
                    center = col.transform.TransformPoint(capsule.center);
                    
                    float x = capsule.radius * 2;
                    float y = capsule.radius * 2;
                    float z = capsule.radius * 2;
                    if (capsule.height > capsule.radius * 2)
                    {
                        float height = capsule.height - capsule.radius * 2;
                        if (capsule.direction == 0)
                        {
                            x += height;
                        }
                        else if (capsule.direction == 1)
                        {
                            y += height;
                        }
                        else if (capsule.direction == 2)
                        {
                            z += height;
                        }
                    }

                    extents = Vector3.Scale(new Vector3(x, y, z), capsule.transform.lossyScale) / 2;
                }
                else
                {
                    continue;
                }

                Collider[] collidedCols = Physics.OverlapBox(center, extents + (myMagioObj.effectSpreadAreaAddition / 2), col.transform.rotation, myMagioObj.spreadLayerMask);
                foreach (Collider collided in collidedCols)
                {
                    ProcessCollidedObject(collided, extents + (myMagioObj.effectSpreadAreaAddition / 2), center);
                }
            }
            
        }

        private void ProcessCollidedObject(Collider other, Vector3 dist, Vector3 center)
        {
            MagioObjectMaster magioObjProps = other.gameObject.GetComponentInParent<MagioObjectMaster>();

            if (MagioEngine.instance.enableMeshPrefabReplacers)
            {
                MagioMeshPrefabReplacer replacer = other.gameObject.GetComponentInParent<MagioMeshPrefabReplacer>();

                if(replacer)
                {
                    GameObject replaced = replacer.TryToSwitchToMagioPrefab(myMagioObj.effectClass);

                    if (replaced)
                    {
                        magioObjProps = replaced.gameObject.GetComponentInChildren<MagioObjectMaster>();

                        if (!magioObjProps)
                        {
                            replaced.gameObject.GetComponentInChildren<MagioObjectEffect>().Setup();
                            magioObjProps = replaced.gameObject.GetComponentInChildren<MagioObjectMaster>();
                        }

                        if (magioObjProps)
                        {
                            magioObjProps.Setup();
                        }
                    }
                }
            }

            
            if (magioObjProps)
            {
                foreach (MagioObjectEffect magioO in magioObjProps.magioObjects)
                {
                    if (magioO != null && magioO != myMagioObj && magioO.targetGameObject != myMagioObj.targetGameObject && !magioObjsAndPositions.ContainsKey(magioO) && magioO.effectBehaviourType == EffectBehaviourMode.Spread && magioO.effectClass == myMagioObj.effectClass)
                    {
                        Vector3 closestPoint = other.ClosestPointOnBounds(center);
                        if (Vector3.Distance(closestPoint, myMagioObj.GetEffectOrigin()) <= myMagioObj.effectSpread)
                        {
                            if (myMagioObj.GetNullifyRadius() < 0.05 || Vector3.Distance(closestPoint, myMagioObj.GetNullifyCenter()) > myMagioObj.GetNullifyRadius())
                            {
                                if (!magioO.effectEnabled || magioO.canBeReAnimated != MagioObjectEffect.CanBeReanimated.No)
                                {
                                    magioObjsAndPositions.Add(magioO, new CurMagioObjs { magioObj = magioO, pos = closestPoint });
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            
            
            IInteractWithEffect interactable = other.gameObject.GetComponentInParent<IInteractWithEffect>();
             
            if(interactable != null && !interactable.Equals(null) &&!interactWithFires.Contains(interactable))
            {
                Vector3 closestPoint = other.ClosestPointOnBounds(center);
                if (Vector3.Distance(closestPoint, myMagioObj.GetEffectOrigin()) <= myMagioObj.effectSpread)
                {
                    if (myMagioObj.GetNullifyRadius() < 0.05 || Vector3.Distance(closestPoint, myMagioObj.GetNullifyCenter()) > myMagioObj.GetNullifyRadius())
                    {
                        interactWithFires.Add(interactable);
                    }
                }
            }
            

            if (MagioEngine.instance.VegetationStudioProCompatible)
            {
                MagioEngine.instance.MaskInstanceAndSpawnAPrefabIfNecessary(other.gameObject, myMagioObj.effectClass);
            }

            if (MagioEngine.instance.unityTerrainCompatible)
            {
                MagioEngine.instance.MaskUnityTerrainTreeAndInstanceAPrefabIfNecessary(other.gameObject, other.ClosestPointOnBounds(center), myMagioObj.effectClass, dist.magnitude);
            }
            

            if (!MagioEngine.instance.enableOnTouchChecks)
            {
                return;
            }

            // If other object is a collider object for LOD group or flammable child use the parent instead
            GameObject supportedGameObject = other.gameObject;
            LODGroup parent = other.gameObject.GetComponentInParent<LODGroup>();
            if (parent)
            {
                if(parent.transform != other.gameObject.transform)
                    supportedGameObject = other.gameObject.transform.parent.gameObject;
            }
            else
            {
                MagioObjectEffect parent2 = other.gameObject.GetComponentInParent<MagioObjectEffect>();
                if (parent2)
                {
                    if (parent2.transform != other.gameObject.transform)
                        supportedGameObject = other.gameObject.transform.parent.gameObject;
                }
            }

            if(supportedGameObject == myMagioObj.gameObject)
            {
                return;
            }

            if (onTouchObjs.ContainsKey(supportedGameObject))
            {
                foreach (Renderer rend in supportedGameObject.GetComponentsInChildren<Renderer>())
                {
                    for (int a = 0; a < rend.materials.Length; a++)
                    {
                        List<OAVAShaderCompatibilitySO> compShaders = MagioEngine.instance.GetCompatibleShaders();
                        Material matRef = rend.materials[a];
                        foreach (OAVAShaderCompatibilitySO shaderComp in compShaders)
                        {
                            if (matRef.HasProperty(shaderComp.ShaderCheckProperty) ||
                                (shaderComp.ShaderCheckProperty.Equals(string.Empty) && rend.materials[a].shader.name.Equals(shaderComp.ShaderName)))
                            {
                                CurOnTouchMaterials onTouch = new CurOnTouchMaterials
                                {
                                    obj = supportedGameObject,
                                    mat = matRef,
                                    shaderComp = shaderComp
                                };
                                onTouchObjs.Add(supportedGameObject, onTouch);

                                break;
                            }
                        }
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach (Collider col in myMagioObj.spreadingColliders)
            {
                if (col.GetType() == typeof(BoxCollider))
                {
                    BoxCollider box = col as BoxCollider;
                    Gizmos.matrix = box.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(box.center, box.size + Vector3.Scale(myMagioObj.effectSpreadAreaAddition, new Vector3(1 / box.transform.lossyScale.x, 1 / box.transform.lossyScale.y, 1 / box.transform.lossyScale.z)));
                }
                else if (col.GetType() == typeof(SphereCollider))
                {
                    SphereCollider sphere = col as SphereCollider;
                    Gizmos.matrix = sphere.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(sphere.center, new Vector3(sphere.radius * 2, sphere.radius * 2, sphere.radius * 2) + Vector3.Scale(myMagioObj.effectSpreadAreaAddition, new Vector3(1 / col.transform.lossyScale.x, 1 / col.transform.lossyScale.y, 1 / col.transform.lossyScale.z)));
                }
                else if (col.GetType() == typeof(CapsuleCollider))
                {
                    CapsuleCollider capsule = col as CapsuleCollider;
                    Gizmos.matrix = capsule.transform.localToWorldMatrix;
                    float x = capsule.radius * 2;
                    float y = capsule.radius * 2;
                    float z = capsule.radius * 2;
                    if (capsule.height > capsule.radius * 2)
                    {
                        float height = capsule.height - capsule.radius * 2;
                        if (capsule.direction == 0)
                        {
                            x += height;
                        }
                        else if (capsule.direction == 1)
                        {
                            y += height;
                        }
                        else if (capsule.direction == 2)
                        {
                            z += height;
                        }
                    }

                    Gizmos.DrawWireCube(capsule.center, new Vector3(x, y, z) + Vector3.Scale(myMagioObj.effectSpreadAreaAddition, new Vector3(1 / col.transform.lossyScale.x, 1 / col.transform.lossyScale.y, 1 / col.transform.lossyScale.z)));
                }
            }

        }
    }
}
