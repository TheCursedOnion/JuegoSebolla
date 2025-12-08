using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Audio;
using CursedOnion.Game.Settings;
using CursedOnion.Locators;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Dialog
{
    public class DialogInvoker : MonoBehaviour
    {
        [Inject] protected RuntimeVariableLocator VariableLocator;
        [Inject] protected AudioGallery AudioGallery;

        protected void Awake()
        {
            gameObject.InjectDependencies();
        }

        protected bool RequestDialog(DialogBlock block)
        {
            if(block == null || string.IsNullOrEmpty(block.Name)) return false;
            return VariableLocator.GetDialogController().PlayDialog(block, gameObject.scene.GetSceneContainer());
        }
    }
}