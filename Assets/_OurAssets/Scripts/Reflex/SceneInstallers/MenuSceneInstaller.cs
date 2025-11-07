using System;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Settings;
using NaughtyAttributes;
using Reflex.Attributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class MenuSceneInstaller : MonoBehaviour, IInstaller 
    {
        [SerializeField] UIEvents uiEvents;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(uiEvents, typeof(UIEvents));
        }
    }
}
