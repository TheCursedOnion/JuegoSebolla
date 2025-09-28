using CursedOnion.ScriptableObjects;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class LevelSceneInstaller: MonoBehaviour, IInstaller 
    {
        [SerializeField] LevelAsset levelAsset;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(levelAsset, typeof(LevelAsset));
        }
    }
}
