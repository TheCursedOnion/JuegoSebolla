using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Game.Systems.Grid;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class LevelSceneInstaller: MonoBehaviour, IInstaller 
    {
        [SerializeField] LevelManager levelManager;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            levelManager ??= GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
            
            levelManager.SetLevelProperties();
            containerBuilder.AddSingleton(levelManager,  typeof(LevelManager));
            containerBuilder.AddSingleton(levelManager.Grid, typeof(Grid3d));
            containerBuilder.AddSingleton(levelManager.LevelEvents, typeof(LevelEvents));
        }
    }
}
