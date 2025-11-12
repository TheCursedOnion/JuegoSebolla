using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using Reflex.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CursedOnion.Game.Commands
{
    public class SpawnCommand : ICommand
    {
        private GameObject unitPrefab;
        
        private Vector3 spawnPosition;
        private Tile3d targetTile;

        public static SpawnCommand Create(GameObject spawnEntity, Vector3 spawnPosition, Tile3d spawnTile)
        {
            if(!spawnEntity) throw new ArgumentException($"Spawn Unit cannot be null");
            return new SpawnCommand(spawnEntity, spawnPosition, spawnTile);
        }
        private SpawnCommand(GameObject unitPrefab, Vector3 spawnPosition, Tile3d targetTile)
        {
            this.unitPrefab = unitPrefab;
            this.targetTile = targetTile;
            this.spawnPosition = spawnPosition;
        }
        
        public void OnPrepare()
        {
            
        }
        public bool CanExecute()
        {
            // Validaciones
            if (unitPrefab == null)
            {
                Debug.LogWarning("[SpawnCommand] No se puede ejecutar: prefab es nulo");
                return false;
            }

            if (targetTile == null)
            {
                Debug.LogWarning("[SpawnCommand] No se puede ejecutar: tile objetivo es nulo");
                return false;
            }

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
            return unit != null && unit.TrySpawningUnit(unitPrefab, spawnPosition, BattleSide.Ally);
        }
        
    }
}