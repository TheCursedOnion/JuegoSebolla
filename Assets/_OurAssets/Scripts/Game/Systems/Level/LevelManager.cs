using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    public enum LevelState { InDialog, InBattleEditor, InBattle, Finished }

    [RequireComponent(typeof(MeshFilter))]
    public class LevelManager : MonoBehaviour
    {
        [Expandable] public LevelAsset LevelAsset;
        public Vector3 LevelManagerOrigin => GetComponent<MeshRenderer>().bounds.min;
        
        public LevelEvents LevelEvents;
        public LevelScoreData LevelScoreVariables;
        
        #if UNITY_EDITOR
        public void Initialize(LevelAsset asset)
        {
            gameObject.name = "LevelManager";
            
            LevelAsset = asset;
            GetComponent<MeshCollider>().sharedMesh = asset.Grid.Mesh;
            GetComponent<MeshFilter>().sharedMesh = asset.Grid.Mesh;
            GetComponent<MeshRenderer>().sharedMaterials = asset.MeshMaterials;
        }
        #endif

        public LevelEvents BuildEvents()
        {
            LevelEvents = GetComponent<LevelEvents>();
            LevelScoreVariables = new LevelScoreData(LevelEvents, LevelAsset.LevelData);
            CurrentLevelState = LevelAsset.LevelData.StartingState;

            return LevelEvents;
        }
        void Awake()
        {
            LevelAsset.Grid.StartingOffset = LevelAsset.Grid.Origin - LevelManagerOrigin;
        }

        public bool TryPlacingUnit(int unitPrice)
        {
            bool result = LevelScoreVariables.TakeGold(unitPrice);
            if (result)
            {
                LevelScoreVariables.UpdateUnitCount(1);
            }
            return result;
        }
        public void EraseUnit(int unitPrice)
        {
            LevelScoreVariables.AddGold(unitPrice);
            LevelScoreVariables.UpdateUnitCount(-1);
        }

        public LevelState CurrentLevelState;

        public void SetNewLevelState(LevelState newState)
        {
            if (CurrentLevelState == newState) return;
            LevelEvents.InvokeLevelState(CurrentLevelState, newState);
            CurrentLevelState = newState;
        }
    }

    public class LevelScoreData
    {
        LevelEvents levelEvents;
        
        public int PlacedUnits;
        public int RemainingGold;
        public LevelScoreData(LevelEvents levelEvents, LevelData levelData)
        {
            this.levelEvents = levelEvents;
            RemainingGold = levelData.StartingGold;
        }
        
        public bool AddGold(int gold)
        {
            if (gold < 0)
            {
                return false;
            }
            return ModifyGold(gold);
        }
        public bool TakeGold(int gold)
        {
            if (gold < 0 || RemainingGold < gold)
            {
                return false;
            }
            return ModifyGold(-gold);
        }
        private bool ModifyGold(int amount)
        {
            RemainingGold += amount;
            levelEvents.UpdateGold(RemainingGold);
            return true;
        }
        public void UpdateUnitCount(int add)
        {
            PlacedUnits += add;
            levelEvents.UpdateUnitPlacedCount(PlacedUnits);
        }
    }
}
