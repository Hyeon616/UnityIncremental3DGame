using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Inverts the effect crawl speed for fade out period.
    /// </summary>
    public class InvertCrawlForFadeOut : MonoBehaviour
    {
        public MagioObjectEffect effect;
        bool inverted = false;

        // Start is called before the first frame update
        void Start()
        {
            if (!effect) effect = GetComponent<MagioObjectEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            if(effect.effectEnabledTimer > effect.fadeOutStart_s && !inverted)
            {
                effect.effectCrawlSpeed = -effect.effectCrawlSpeed;
                inverted = true;
                Destroy(this);
            }
        }
    }

}
