using CursedOnion.Game.Inputs;
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
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(new PauseService(), typeof(PauseService));
            containerBuilder.AddSingleton(gameSettings, typeof(GameSettings));
            containerBuilder.AddSingleton(InputReaderCollection, typeof(InputReaderCollection));
            containerBuilder.AddSingleton(new CommandManager(), typeof(CommandManager));
        }
    }
}
