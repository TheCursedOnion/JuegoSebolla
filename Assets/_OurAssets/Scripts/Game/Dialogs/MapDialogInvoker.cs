using Ami.BroAudio;
using CursedOnion.Game.Audio;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Dialog
{
    public class MapDialogInvoker : DialogInvoker
    {
        public DialogBlock StartingDialogBlock;
        [SerializeField] SoundID mapMusic;
        public void Start()
        {
            if (!RequestDialog(StartingDialogBlock))
            {
                AudioGallery.PlayMusic(mapMusic);
            }
        }
    }
}