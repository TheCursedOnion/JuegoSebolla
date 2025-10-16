using CursedOnion.Game.Cameras;
using CursedOnion.Game.Inputs;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class LevelSceneInstaller: MonoBehaviour, IInstaller 
    {
        [Expandable, Required, SerializeField] LevelAsset levelAsset;
        [Required, SerializeField] LevelManager levelManager;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(levelAsset, typeof(LevelAsset));
            containerBuilder.AddSingleton(levelManager,  typeof(LevelManager));
        }
    }
}
