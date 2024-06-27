using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Magio
{
    public class FPSCounter : MonoBehaviour
    {
        int frameCount = 0;
        float dt = 0.0f;
        float fps = 0.0f;
        float updateRate = 4.0f;  // 4 updates per sec.
        public Text fpsText;


        void Update()
        {
            frameCount++;
            dt += Time.deltaTime;
            if (dt > 1.0f / updateRate)
            {
                fps = frameCount / dt;
                frameCount = 0;
                dt -= 1.0f / updateRate;
                fpsText.text = Mathf.Round(fps).ToString() + " fps";
            }
        }
    }
}

