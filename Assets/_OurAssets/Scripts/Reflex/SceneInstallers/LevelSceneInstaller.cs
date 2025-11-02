using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using CursedOnion.UI.Canvases.Level;
using NaughtyAttributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class LevelSceneInstaller: MonoBehaviour, IInstaller 
    {
        [Expandable, SerializeField] LevelAsset levelAsset;
        [Expandable, SerializeField] LevelData levelData;
        [HorizontalLine(height: 2f , color: EColor.Blue)]
        [SerializeField] LevelManager levelManager;
        [SerializeField] LevelUICanvas levelUICanvas;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(levelAsset, typeof(LevelAsset));
            containerBuilder.AddSingleton(levelManager,  typeof(LevelManager));
            containerBuilder.AddSingleton(levelUICanvas, typeof(LevelUICanvas));
            containerBuilder.AddSingleton(new LevelEvents(levelData), typeof(LevelEvents));
        }
    }
}
