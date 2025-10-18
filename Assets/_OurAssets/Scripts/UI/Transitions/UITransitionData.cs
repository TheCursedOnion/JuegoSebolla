using UnityEngine;

namespace CursedOnion.UI.Transitions
{
    [System.Serializable]
    public class UITransitionData
    {
        public float Duration;
        public float InBetweenTime;
        public TransitionType Type;
        public Color Color;
        
        public UITransitionData() {}

        public UITransitionData(float duration, float inBetweenTime, TransitionType type, Color color)
        {
            Duration = duration;
            InBetweenTime = inBetweenTime;
            Type = type;
            Color = color;
        }
    }
}