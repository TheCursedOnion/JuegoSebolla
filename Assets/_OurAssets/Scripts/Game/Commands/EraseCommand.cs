using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class EraseCommand : ICommand
    {
        private readonly Tile3d targetTile;
        public static EraseCommand Create(CommandParameters parameters)
        {
            try
            {
                if(parameters.TargetTile == null) throw new ArgumentException($"[EraseCommand] Target tile cannot be null");
                return new EraseCommand(parameters.TargetTile);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                throw;
            }
        }
        private EraseCommand(Tile3d targetTile)
        {
            this.targetTile = targetTile;
        }
        public void OnPrepare()
        {
            
        }
        public bool CanExecute()
        {
            return targetTile.GetContainedEntity() as Unit && targetTile.GetTileAttributes().CanUnitsSpawnHere;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;
            return ((Unit)targetTile.GetContainedEntity()).TryErasingUnit();
        }
        
    }
}