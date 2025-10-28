using System.Collections.Generic;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs;
using CursedOnion.Game.Logic;
using Reflex.Core;
using UnityEngine;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace CursedOnion.Installers
{
    public class ProjectMiscellaneousInstaller : MonoBehaviour, IInstaller
    {
        [Expandable, SerializeField] InputReaderCollection inputReaderCollection;
        [Expandable, SerializeField] GameSettings gameSettings;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            inputReaderCollection.Initialize();
            containerBuilder.AddSingleton(gameSettings, typeof(GameSettings));
            containerBuilder.AddSingleton(inputReaderCollection, typeof(InputReaderCollection));
            containerBuilder.AddSingleton(new CommandManager(), typeof(CommandManager));
        }
    }
}
