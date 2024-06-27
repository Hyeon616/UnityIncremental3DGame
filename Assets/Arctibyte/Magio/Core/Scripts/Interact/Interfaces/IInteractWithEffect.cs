using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Implement this interface and you can catch collisions/touchs with effects. Example: SimpleInteractWithEffect.cs. Example in demoscene: Main camera, move to effect.
    /// </summary>
    public interface IInteractWithEffect
    {
        /// <summary>
        /// Called on collision with the effect SpreadPhysics
        /// </summary>
        /// <param name="magioObject">object which collides</param>
        void OnCollisionWithEffect(GameObject magioObject);
    }
}

