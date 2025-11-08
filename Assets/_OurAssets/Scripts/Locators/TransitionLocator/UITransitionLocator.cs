using System;
using System.Collections.Generic;
using CursedOnion.Game.Modes.General.UI.Transitions;
using UnityEngine;

namespace CursedOnion.Locators
{
    [CreateAssetMenu(fileName = "UI Transition Locator", menuName = "Game/Locators/UI Transition Locator")]
    public class UITransitionLocator : ScriptableObject
    {
        [System.NonSerialized] Dictionary<TransitionType, UITransition> transitions = new();
        
        public void AddTransition(TransitionType transitionType, UITransition transition)
        {
            transitions.TryAdd(transitionType, transition);
        }
        public UITransition GetTransition(TransitionType transitionType)
        {
            return transitions.GetValueOrDefault(transitionType);
        }
    }
}
