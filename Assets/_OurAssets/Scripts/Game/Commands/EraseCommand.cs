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
        private Unit eraseUnit;
        private LevelManager levelManager;
        public static EraseCommand Create(LevelManager levelEvents, Tile3d eraseTile)
        {
            return new EraseCommand(levelEvents, eraseTile.GetContainedEntity());
        }
        private EraseCommand(LevelManager levelManager, SimpleEntity eraseEntity)
        {
            this.eraseUnit = eraseEntity as Unit;
            this.levelManager = levelManager;
        }
        public void OnPrepare()
        {
            
        }
        public bool CanExecute()
        {
            return eraseUnit && levelManager != null;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;
            return eraseUnit.TryErasingUnit(levelManager);
        }
        
    }
}