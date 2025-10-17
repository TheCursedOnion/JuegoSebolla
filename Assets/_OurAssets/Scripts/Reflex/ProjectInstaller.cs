using System.Collections.Generic;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Logic;
using Reflex.Core;
using UnityEngine;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Settings;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace CursedOnion.Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [Expandable, SerializeField] InputReaderCollection InputReaderCollection;
        [Expandable, SerializeField] GameSettings gameSettings;
        [Expandable, SerializeField] RuntimeSettings runtimeSettings;
        [Expandable, SerializeField] MediatorEvents mediatorEvents;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            InputReaderCollection.Initialize();
            containerBuilder.AddSingleton(gameSettings, typeof(GameSettings));
            containerBuilder.AddSingleton(runtimeSettings, typeof(RuntimeSettings));
            containerBuilder.AddSingleton(InputReaderCollection, typeof(InputReaderCollection));
            containerBuilder.AddSingleton(mediatorEvents, typeof(MediatorEvents));
        }
    }
}
