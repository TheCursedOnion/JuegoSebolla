using CursedOnion.Locators;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Installers
{
    public class ProjectLocatorInstaller : MonoBehaviour, IInstaller
    {
        [Expandable, SerializeField] CameraLocator cameraLocator;
        [Expandable, SerializeField] private UITransitionLocator uiTransitionLocator;
        [Expandable, SerializeField] private RuntimeVariableLocator variableLocator;
        [Expandable, SerializeField] private CloudSaveLocator cloudSaveLocator;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(cameraLocator, typeof(CameraLocator));
            containerBuilder.AddSingleton(uiTransitionLocator, typeof(UITransitionLocator));
            containerBuilder.AddSingleton(cloudSaveLocator, typeof(CloudSaveLocator));
            
            variableLocator.Initialize();
            containerBuilder.AddSingleton(variableLocator, typeof(RuntimeVariableLocator));
        }
    }
}