using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Modes.General.UI.Transitions;
using CursedOnion.Game.Systems.Level;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Extensions
{
    public class UIMapButton : MonoBehaviour
    {
        [SerializeField] SceneServiceUser sceneServiceUser;
        public void GoToCorrespondingMapScene(float duration, float inBetweenTime, TransitionType type, Color color)
        {
            var container = gameObject.scene.GetSceneContainer();
            LevelManager levelManager = container.Resolve<LevelManager>();
            string currentMap = levelManager.LevelAsset.LevelData.CorrespondingMapSceneName;
            
            sceneServiceUser.ChangeScene(currentMap, duration, inBetweenTime, type, color);
        }
    }
}
