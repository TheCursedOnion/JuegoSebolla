using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class EraseCommand : ICommand
    {
        private Unit eraseUnit;
        private LevelEvents levelEvents;
        public static EraseCommand Create(LevelEvents levelEvents, Tile3d eraseTile)
        {
            return new EraseCommand(levelEvents, eraseTile.GetContainedEntity());
        }
        private EraseCommand(LevelEvents levelEvents, SimpleEntity eraseEntity)
        {
            this.eraseUnit = eraseEntity as Unit;
            this.levelEvents = levelEvents;
        }
        public bool CanExecute()
        {
            return eraseUnit && levelEvents != null;
        }
        public bool Execute()
        {
            if(!CanExecute()) return false;
            return eraseUnit.TryErasingUnit(levelEvents);
        }
        
    }
}