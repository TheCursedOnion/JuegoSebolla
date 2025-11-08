using CursedOnion.Game.Events;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Modes.General.UI.Events;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class ProjectServiceInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(new PauseService(), typeof(PauseService));
            containerBuilder.AddSingleton(new SceneService(), typeof(SceneService));
            containerBuilder.AddSingleton(new ScriptAnimationService(), typeof(ScriptAnimationService));
        }
    }
}