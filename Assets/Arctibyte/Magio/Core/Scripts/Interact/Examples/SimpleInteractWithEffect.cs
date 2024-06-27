using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// A template for interacting with MagioEffect using IInteractWithEffect interface.
    /// </summary>
    public class SimpleInteractWithEffect : MonoBehaviour, IInteractWithEffect
    {
        public void OnCollisionWithEffect(GameObject magioObject)
        {
            MagioObjectEffect obj = magioObject.GetComponent<MagioObjectEffect>();
            Debug.Log("Object: " + gameObject.name + " Interacted with effect object: " + magioObject.name + " Which had a tag: " + magioObject.tag + " Which has Effect class: " + obj.effectClass);
        }
    }
}

