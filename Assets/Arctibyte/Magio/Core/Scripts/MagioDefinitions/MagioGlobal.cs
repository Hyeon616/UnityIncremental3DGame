using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magio
{
    public enum EffectClass
    {
        Default,
        Flame,
        Melt,
        Stone,
        Grass,
        Poison,
        Ice,
        Darkness,
        Electricity,
        Light,
        Water,
        Illusion,
        Air,
        Earth
    }

    public enum VFXSpawnerType
    {
        Mesh,
        SkinnedMesh
    }

    public enum VFXParameterType
    {
        ANIMATION_CURVE,
        BOOL,
        FLOAT,
        GRADIENT,
        INT,
        MATRIX,
        MESH,
        SKINNED_MESH_RENDERER,
        TEXTURE,
        UINT,
        VECTOR2,
        VECTOR3,
        VECTOR4,
        COLOR
    }

    public enum EffectBehaviourMode
    {
        Enable,
        Spread
    }
}
