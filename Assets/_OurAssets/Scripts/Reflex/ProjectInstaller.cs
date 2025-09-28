using Reflex.Core;
using UnityEngine;
using CursedOnion.Game.Logic.Services.Pause;

namespace CursedOnion.Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(new PauseService(), typeof(PauseService));
        }
    }
}
