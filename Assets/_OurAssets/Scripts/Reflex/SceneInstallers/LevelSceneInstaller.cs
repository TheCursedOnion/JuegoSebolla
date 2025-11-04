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
        private LevelEvents levelEvents;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            levelEvents = new LevelEvents(levelData);
            containerBuilder.AddSingleton(levelAsset, typeof(LevelAsset));
            containerBuilder.AddSingleton(levelManager,  typeof(LevelManager));
            containerBuilder.AddSingleton(levelUICanvas, typeof(LevelUICanvas));
            containerBuilder.AddSingleton(levelEvents, typeof(LevelEvents));
        }

        void Start()
        {
            levelEvents.InvokeInitialCalls();
        }
    }
}
