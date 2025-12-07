using CursedOnion.Game.Audio;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Dialog
{
    public class MapDialogInvoker : MonoBehaviour
    {
        [Inject] RuntimeVariableLocator variableLocator;
        public DialogBlock StartingDialogBlock;
        public void Start()
        {
            var dialogController = variableLocator.GetDialogController();


            bool dialogPlayed = false;
            if (StartingDialogBlock != null && !string.IsNullOrEmpty(StartingDialogBlock.Name))
            {
               dialogPlayed = dialogController.PlayDialog(StartingDialogBlock, gameObject.scene.GetSceneContainer());
            }
            if(!dialogPlayed) variableLocator.MusicPlayer.RequestMusic(MusicType.Map);
        }
    }
}