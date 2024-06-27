using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Magio
{
    /// <summary>
    /// Ability to invoke events
    /// </summary>
    public class MagioEventInvoker : MonoBehaviour
    {
        /// Called when effect is being nullified currently
        public UnityEvent BeingNullified;

        // Called when object has been nullified and is currently being fade out
        public UnityEvent Nullified;

        // Called when object has been faded out
        public UnityEvent FadedOut;

        // Called When object is ignited.
        public UnityEvent Ignited;
    }

}
