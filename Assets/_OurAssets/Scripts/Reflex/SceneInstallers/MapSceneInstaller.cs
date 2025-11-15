using CursedOnion.Game.Events;
using CursedOnion.Game.Objects;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class MapSceneInstaller : MonoBehaviour, IInstaller 
    {
        [SerializeField, Required] private MapManager mapManager;
        //TODO: Hacer MapCanvasUIController
        //[SerializeField] private GameObject mapCanvas;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            mapManager ??= GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
            
            containerBuilder.AddSingleton(mapManager, typeof(MapManager));
            containerBuilder.AddSingleton(mapManager.MapEvents, typeof(MapEvents));
        }
    }
}
