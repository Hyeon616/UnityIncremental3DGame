using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemIconCreator
{
    [ExecuteInEditMode]
    public class MaterialIconCreator : IconCreator
    {
        public Renderer targetRenderer;
        public Material[] materials;

        public override void BuildIcons()
        {
            StartCoroutine(BuildIconsRotine());
        }

        public override bool CheckConditions()
        {
            if( base.CheckConditions() == false) return false;

            if (materials.Length == 0)
            {
                Debug.LogError("There's no materials");
                return false;
            }

            if (targetRenderer == null)
            {
                Debug.LogError("There's no target renderer");
                return false;
            }

            return true;
        }


        IEnumerator BuildIconsRotine()
        {
            Initialize();
     

            if (dynamicFov) UpdateFOV(targetRenderer.gameObject);
            if (lookAtObjectCenter) LookAtTargetCenter(targetRenderer.gameObject);

            currentObject = targetRenderer.transform;

            yield return CaptureFrame(targetRenderer.name, 0);


            for (int i = 0; i < materials.Length; i++)
            {
                

                targetRenderer.material = materials[i];
                targetRenderer.materials[0] = materials[i];
                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo( materials.Length, i, materials[i].name, true, nextIconKey);


                if(whiteCam != null) whiteCam.enabled = false;
                if (whiteCam != null) blackCam.enabled = false;

                if(mode == Mode.Manual)
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
                    yield return null;
                }
                yield return CaptureFrame(materials[i].name, i);
            }

            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0,0,"",false, nextIconKey);
            RevealInFinder();
            DeleteCameras();
        }

        private void Reset()
        {
            targetRenderer = null;
            materials = new Material[0];
        }

        protected override void Update()
        {
      
                if (preview && !isCreatingIcons)
                {
                    if (targetRenderer != null)
                    {
                        if (dynamicFov) UpdateFOV(targetRenderer.gameObject);
                        if (lookAtObjectCenter) LookAtTargetCenter(targetRenderer.gameObject);
                    }
                return;
                }
               
           

            base.Update();
        }
    }
}
