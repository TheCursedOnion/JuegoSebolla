using System;
using CursedOnion.Game.Settings;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CursedOnion.Game.Settings
{
    public class GlobalVolume : MonoBehaviour
    {
        [Inject] GameSettings gameSettings;
        [SerializeField] Volume volume;
        void Awake()
        {
            var instancedVolume = gameSettings.GetGlobalVolume();
            if (instancedVolume != null && instancedVolume != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            gameSettings.SetGlobalVolume(this);
            
            volume ??= GetComponent<Volume>();
        }
        public Volume GetVolume() => volume;

        
    }
}
