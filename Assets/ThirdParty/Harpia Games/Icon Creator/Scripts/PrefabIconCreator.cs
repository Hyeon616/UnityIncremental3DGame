using System.Collections;
using UnityEngine;

namespace ItemIconCreator
{
    [ExecuteInEditMode]
    public class PrefabIconCreator : IconCreator
    {
        [Header("Itens")]
        public GameObject[] itensToShot;

        public Transform itemPosition;

        private GameObject instantiatedItem;

        public override void BuildIcons()
        {
            StartCoroutine(BuildAllIcons());
        }

        public override bool CheckConditions()
        {
            if (base.CheckConditions() == false) return false;

            if (itensToShot.Length == 0)
            {
                Debug.LogError("There's no prefab to shoot");
                return false;
            }

            if (itemPosition == null)
            {
                Debug.LogError("Item position is null");
                return false;
            }

            return true;
        }

        protected override void Update()
        {
            
                if (preview && !isCreatingIcons)
                {
                    if (instantiatedItem != null)
                    {
                        if (dynamicFov) UpdateFOV(instantiatedItem);
                        if (lookAtObjectCenter) LookAtTargetCenter(instantiatedItem);
                        
                        instantiatedItem.transform.position = itemPosition.transform.position;
                        instantiatedItem.transform.rotation = itemPosition.transform.rotation;
                    }
                    else if (instantiatedItem == null && itensToShot.Length > 0)
                    {
                        if (itemPosition.childCount > 0 && itemPosition.GetChild(0).GetComponent<MeshRenderer>() != null)                        
                            instantiatedItem = itemPosition.GetChild(0).gameObject;                        
                        else                        
                            instantiatedItem = Instantiate(itensToShot[0], itemPosition.transform.position, itemPosition.transform.rotation, itemPosition);                        
                    }
                }
               
           

            base.Update();
        }

        public IEnumerator BuildAllIcons()
        {
            Initialize();

            for (int i = 0; i < itensToShot.Length; i++)
            {
                if (instantiatedItem != null) DestroyImmediate(instantiatedItem);

                if(whiteCam  != null) whiteCam.enabled = false;
                if (blackCam != null) blackCam.enabled = false;

                instantiatedItem = Instantiate(itensToShot[i], itemPosition.transform.position, itemPosition.transform.rotation);

                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(itensToShot.Length, i, itensToShot[i].name, true, nextIconKey);

                currentObject = instantiatedItem.transform;

                if (dynamicFov) UpdateFOV(instantiatedItem);
                if (lookAtObjectCenter) LookAtTargetCenter(instantiatedItem);

                if (mode == Mode.Manual)
                {
                    CanMove = true;
                    yield return new WaitUntil(() => Input.GetKeyDown(nextIconKey));
                    CanMove = false;
                }

                if (IconCreatorCanvas.instance != null)
                {
                    IconCreatorCanvas.instance.SetTakingPicture();
                    yield return null;
                    yield return null;
                }

                yield return CaptureFrame(itensToShot[i].name, i);
            }

            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false,nextIconKey);

            RevealInFinder();

            DeleteCameras();
        }
    }
}