using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI.Transitions
{
    [System.Serializable]
    public class UITransitionData
    {
        public float Duration;
        public float InBetweenTime;
        public TransitionType Type;
        public Color Color;
        public Func<Task> MidPointAction;
        public Action EndPointAction;
        public UITransitionData() {}

        public UITransitionData(float duration, float inBetweenTime, TransitionType type, Color color, Func<Task> midPointAction, Action endPointAction)
        {
            Duration = duration;
            InBetweenTime = inBetweenTime;
            Type = type;
            Color = color;
            MidPointAction = midPointAction;
            EndPointAction = endPointAction;
        }
    }
}