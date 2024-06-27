using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    /// <summary>
    /// Represents global nullify ruleset. Effects will interact with each other according to these rules.
    /// Origin will nullify the target.
    /// Target cannot ignite if origin is enabled.
    /// </summary>
    [CreateAssetMenu(fileName = "NewNullifyRuleSet", menuName = "Magio/Nullify Ruleset", order = 1)]
    public class NullifyRuleSet : ScriptableObject
    {
        [System.Serializable]
        public struct NullifyRule
        {
            [Tooltip("What class will affect the other class? e.g. Fire(origin) burns Plant (Target) after it has spreaded 0.3m (nullifyLagBehind_m) in the grass ")]
            public EffectClass originClass;

            [Tooltip("The affected class. e.g. Fire(origin) burns Plant (Target) after it has spreaded 0.3m (nullifyLagBehind_m) in the grass ")]
            public EffectClass targetClass;

            [Min(0)]
            [Tooltip("How much distance it needs to go forward before nullify? e.g. Fire(origin) burns Plant (Target) after it has spreaded 0.3m (nullifyLagBehind_m) in the grass ")]
            public float nullifyLagBehind_m;
        }

        [Tooltip("Collection of nullify rules. See more information inside the rule.")]
        public List<NullifyRule> nullifyRules = new List<NullifyRule>();

        public Dictionary<EffectClass, Dictionary<EffectClass, float>> nullifyRuleDict = new Dictionary<EffectClass, Dictionary<EffectClass, float>>();

        private void OnEnable()
        {
            foreach(NullifyRule rules in nullifyRules)
            {
                if (!nullifyRuleDict.ContainsKey(rules.originClass))
                {
                    nullifyRuleDict.Add(rules.originClass, new Dictionary<EffectClass, float>());
                }
                if (!nullifyRuleDict[rules.originClass].ContainsKey(rules.targetClass))
                {
                    nullifyRuleDict[rules.originClass].Add(rules.targetClass, rules.nullifyLagBehind_m);
                }
            }
        }

        /// <summary>
        /// Gets nullify rule parameters if it exists. Returns -1 if rule does not exist.
        /// </summary>
        /// <param name="origin">origin class</param>
        /// <param name="target">Target class</param>
        /// <returns>-1 if rule does not exist. Else NullifyLagBehind_m</returns>
        public float GetNullifyRuleLagBehind(EffectClass origin, EffectClass target)
        {
            if (nullifyRuleDict.ContainsKey(origin))
            {
                if (nullifyRuleDict[origin].ContainsKey(target))
                {
                    return nullifyRuleDict[origin][target];
                }
            }

            return -1;
        }
        
    }
}

