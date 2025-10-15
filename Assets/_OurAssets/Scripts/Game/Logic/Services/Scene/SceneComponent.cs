using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Logic.Services
{
    public class SceneComponent : MonoBehaviour
    {
        [Inject] SceneService sceneService;

        public void ChangeScene(string sceneName)
        {
            sceneService.ChangeScene(sceneName);
        }
    }
}