using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Adds ability to Nullify/Extinguish the effect using particles.
    /// </summary>
    public class ParticleNullify : MonoBehaviour
    {
        [Tooltip("How large area can one particle nullify")]
        public float particleNullifyRadius = 1f;

        [Tooltip("How much the area is incremented if new area is not hit. (simulates water puddling/sliding on ground)")]
        public float incrementalPower = 0.0005f;

        [Tooltip("This class will be affected by this particle. For multiple classes add multiple scripts.")]
        public EffectClass affectedClass = EffectClass.Default;

        private ParticleSystem part;
        private List<ParticleCollisionEvent> collisionEvents;

        void Start()
        {
            part = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
        }

        void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            Magio.MagioObjectMaster magioTarget = other.GetComponentInParent<Magio.MagioObjectMaster>();
            if (magioTarget)
            {
                foreach (MagioObjectEffect magioObj in magioTarget.magioObjects)
                {
                    int i = 0;
                    if (magioObj && magioObj.effectClass == affectedClass)
                    {
                        while (i < numCollisionEvents)
                        {
                            Vector3 pos = collisionEvents[i].intersection;
                            magioObj.IncrementalNullify(pos, particleNullifyRadius, incrementalPower);
                            i++;
                        }
                    }
                }
            }

        }
    }
}
