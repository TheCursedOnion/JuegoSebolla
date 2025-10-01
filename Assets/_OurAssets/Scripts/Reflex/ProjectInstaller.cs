using Reflex.Core;
using UnityEngine;
using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Settings;

namespace CursedOnion.Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] GameSettings gameSettings;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(new PauseService(), typeof(PauseService));
            containerBuilder.AddSingleton(gameSettings, typeof(GameSettings));
        }
    }
}
