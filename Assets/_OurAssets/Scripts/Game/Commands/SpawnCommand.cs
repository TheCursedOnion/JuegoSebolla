using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class SpawnCommand : ICommand
    {
        private LevelManager levelManager;
        private GameObject unitPrefab;
        private StatData unitStatsData;
        
        private Vector3 spawnPosition;
        private Tile3d targetTile;

        public static ICommand Create(CommandParameters parameters)
        {
            try
            {
                if(!parameters.Prefab) throw new ArgumentException($"[SpawnCommand] Spawn Unit prefab cannot be null");
                if(!parameters.EntityStatData) throw new ArgumentException($"[SpawnCommand] Spawn Stat Data cannot be null");
                if(parameters.Position == null) throw new ArgumentException($"[SpawnCommand] Spawn Unit position cannot be null");
                if(parameters.TargetTile == null) throw new ArgumentException($"[SpawnCommand] Spawn Unit target tile cannot be null");
                if(parameters.LevelManager == null) throw new ArgumentException($"[SpawnCommand] Level Manager cannot be null");
                
                return new SpawnCommand(parameters.LevelManager, parameters.Prefab, parameters.EntityStatData, parameters.Position.Value, parameters.TargetTile);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return null;
            }
        }
        private SpawnCommand(LevelManager levelManager, GameObject unitPrefab, StatData data, Vector3 spawnPosition, Tile3d targetTile)
        {
            this.levelManager = levelManager;
            this.unitPrefab = unitPrefab;
            this.unitStatsData = data;
            this.targetTile = targetTile;
            this.spawnPosition = spawnPosition;
        }
        public bool CanExecute()
        {
            // Validaciones
            if (targetTile.GetContainedEntity() != null)
            {
                Debug.LogWarning($"[SpawnCommand] No se puede ejecutar: tile en {spawnPosition} está ocupado");
                return false;
            }
            
            if (!targetTile.GetTileAttributes().CanUnitsSpawnHere)
            {
                Debug.LogWarning($"[SpawnCommand] No se puede ejecutar: tile en {spawnPosition} no permite spawnear unidades");
                return false;
            }
            return true;
        }

        public bool Execute()
        {
            if(!CanExecute()) return false;
            Unit unit = unitPrefab.GetComponent<Unit>();
            return unit != null && unit.TrySpawningUnit(levelManager, unitPrefab, unitStatsData, spawnPosition, BattleSide.Ally);
        }
        
    }
}