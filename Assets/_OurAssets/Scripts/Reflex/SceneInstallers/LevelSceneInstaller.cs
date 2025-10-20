using CursedOnion.Game.Cameras;
using CursedOnion.Game.Events;
using CursedOnion.Game.Inputs;
using CursedOnion.ScriptableObjects;
using CursedOnion.UI.Canvases.Level;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class LevelSceneInstaller: MonoBehaviour, IInstaller 
    {
        [Expandable, Required, SerializeField] LevelAsset levelAsset;
        [Required, SerializeField] LevelManager levelManager;
        [Required, SerializeField] LevelUICanvas levelUICanvas;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(levelAsset, typeof(LevelAsset));
            containerBuilder.AddSingleton(levelManager,  typeof(LevelManager));
            containerBuilder.AddSingleton(levelUICanvas, typeof(LevelUICanvas));
            containerBuilder.AddSingleton(new LevelEvents(true), typeof(LevelEvents));
        }
    }
}
