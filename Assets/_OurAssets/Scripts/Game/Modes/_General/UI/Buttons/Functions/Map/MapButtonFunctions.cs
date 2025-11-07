using CursedOnion.Game.Logic.Services;
using CursedOnion.Game.Objects;
using CursedOnion.Game.Modes.General.UI.Transitions;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.General.UI.Buttons.Functions
{
    public class MapButtonFunctions : MonoBehaviour
    {
        [Inject] MapManager mapManager;

        public void PlayLevel(float duration, float inBetweenTime, TransitionType type, Color color)
        {
            if (mapManager.TryGetSelectedLevelScene(out string sceneName))
            {
                UITransitionData transitionData = new UITransitionData(duration, inBetweenTime, type, color, null, null);
                GetComponent<SceneServiceUser>().ChangeScene(sceneName, transitionData);
            }
        }
        public void MoveToNextLevel()
        {
            mapManager.MoveToNextLevel();
        }

        public void MoveToPreviousLevel()
        {
            mapManager.MoveToPreviousLevel();
        }
    }
}
