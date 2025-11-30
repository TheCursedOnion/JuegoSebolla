using CursedOnion.Game.Commands;
using CursedOnion.Game.Inputs;
using Reflex.Core;
using UnityEngine;
using CursedOnion.Game.Modes.General.UI.Events;
using CursedOnion.Game.Settings;
using NaughtyAttributes;

namespace CursedOnion.Installers
{
    public class ProjectMiscellaneousInstaller : MonoBehaviour, IInstaller
    {
        [Expandable, SerializeField] InputReaderCollection inputReaderCollection;
        [Expandable, SerializeField] GameSettings gameSettings;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            gameSettings.Initialize();
            containerBuilder.AddSingleton(gameSettings, typeof(GameSettings));
            
            inputReaderCollection.Initialize();
            containerBuilder.AddSingleton(inputReaderCollection, typeof(InputReaderCollection));
            
            containerBuilder.AddSingleton(new CommandManager(), typeof(CommandManager));
            
            containerBuilder.AddSingleton(new UIEvents(), typeof(UIEvents));
        }
    }
}
