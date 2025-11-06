using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Game.Modes.General.UI.Events;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class LevelSceneInstaller: MonoBehaviour, IInstaller 
    {
        [Expandable, SerializeField] LevelAsset levelAsset;
        [HorizontalLine(height: 2f , color: EColor.Blue)]
        [SerializeField] LevelManager levelManager;
        [SerializeField] UIEvents uiEvents;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(levelAsset, typeof(LevelAsset));
            
            containerBuilder.AddSingleton(uiEvents, typeof(UIEvents));
            
            levelManager.BuildEvents();
            containerBuilder.AddSingleton(levelManager,  typeof(LevelManager));
            containerBuilder.AddSingleton(levelManager.LevelEvents, typeof(LevelEvents));
        }

        void Start()
        {
            //levelManager.LevelEvents.InvokeInitialCalls();
        }
    }
}
