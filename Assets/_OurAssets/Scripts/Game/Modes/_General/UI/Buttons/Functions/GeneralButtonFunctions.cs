using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.UI
{
    public class GeneralButtonFunctions : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        public void SaveSettings()
        {
            variableLocator ??= gameObject.scene.GetSceneContainer().Resolve<RuntimeVariableLocator>();
            if (variableLocator.AutoCloudSave != null)
            {
                _ = variableLocator.AutoCloudSave.SaveGame();
            }
        }
    }
}