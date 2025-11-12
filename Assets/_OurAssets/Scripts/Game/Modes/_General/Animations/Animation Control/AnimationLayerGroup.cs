using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    [System.Serializable]
    public class AnimationLayerGroup
    {
        public string groupName;
        public List<LayeredEntity.AnimationLayer> layers;
    }

    public class EntityData : ScriptableObject
    {
        [SerializeField] private string entityName;
        [SerializeField] private List<AnimationLayerGroup> animationLayerGroups;

        public List<AnimationLayerGroup> GetAnimationLayerGroups() => animationLayerGroups;
    }
}
