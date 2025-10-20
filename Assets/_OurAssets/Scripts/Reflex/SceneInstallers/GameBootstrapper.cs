using CursedOnion.Game.Inputs;
using NaughtyAttributes;
using Reflex.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace CursedOnion.Installers
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Scene, SerializeField] string startingScene;
        void Start()
        {
            SceneScope.OnSceneContainerBuilding += InstallExtra;
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(startingScene, LoadSceneMode.Single);
            if (loadSceneAsync != null)
            {
                loadSceneAsync.completed += (operation) => SceneScope.OnSceneContainerBuilding -= InstallExtra;
            }
            else
            {
                Debug.LogError($"Starting Scene not found: {startingScene}");
                Application.Quit();
            }
        }
        
        void InstallExtra(Scene scene, ContainerBuilder builder)
        {
        }
    }
}
