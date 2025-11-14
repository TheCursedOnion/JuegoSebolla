using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CursedOnion.Game.Systems.Level
{
    public class LevelGraphicsController : MonoBehaviour
    {
        UniversalRenderPipelineAsset pipelineAsset;
        LevelEvents levelEvents;
        public void Initialize(LevelEvents levelEvents)
        {
            pipelineAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            EnableBlackAndWhiteEditorMode(false);
            
            this.levelEvents = levelEvents;
            levelEvents.OnStatDataSelected += ValidateStatData;
            levelEvents.OnLevelStateChange += ProcessLevelChange;
        }
        private void OnDisable()
        {
            levelEvents.OnStatDataSelected -= ValidateStatData;
            levelEvents.OnLevelStateChange -= ProcessLevelChange;
            EnableBlackAndWhiteEditorMode(false);
        }

        void ProcessLevelChange(LevelState previousState, LevelState newState)
        {
            EnableBlackAndWhiteEditorMode(false);
        }
        void ValidateStatData(StatData data)
        {
            EnableBlackAndWhiteEditorMode(data != null);
        }
        void EnableBlackAndWhiteEditorMode(bool enable)
        {
            pipelineAsset.EnableRenderFeature<FullScreenPassRendererFeature>(enable);
        }
        
    }
}
