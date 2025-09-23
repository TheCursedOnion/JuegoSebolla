using Reflex.Core;
using UnityEngine;
using CursedOnion.GameLogic.Services.Pause;

namespace CursedOnion
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton("Hola");
            containerBuilder.AddSingleton(new PauseService(), typeof(PauseService));
        }
    }
}
