using System;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using Reflex.Core;
using UnityEngine;

namespace CursedOnion.Game.Commands
{
    public class CommandParameters
    {
        public Container Container;
        //On Execution
        public bool ExecuteOnce;
       
        //Execution Data
        public SimpleEntity Subject;
        
        public Vector3? Position;
        public SimpleEntity Target;
        public Tile3d TargetTile;

        public GameObject Prefab;
        public StatData EntityStatData;

        //Execution Dependencies
        public LevelManager LevelManager;
        
        //Simple Execution Logic
        public Func<bool> ExecuteCondition;
        public Action ExecuteAction;
        
        public static void CombineParameters(CommandParameters parameters, CommandParameters other)
        {
            if(parameters == null || other == null) return;
            
            parameters.Container ??= other.Container;
            
            parameters.Subject ??= other.Subject;
            
            parameters.Target ??= other.Target;
            parameters.Position ??= other.Position;
            parameters.TargetTile ??= other.TargetTile;
            
            parameters.Prefab ??= other.Prefab;
            parameters.EntityStatData ??= other.EntityStatData;
            
            parameters.LevelManager ??= other.LevelManager;
            
            parameters.ExecuteAction ??= other.ExecuteAction;
            parameters.ExecuteCondition ??= other.ExecuteCondition;
            parameters.ExecuteOnce |= other.ExecuteOnce;
        }

        private static void ResetParameters(CommandParameters parameters)
        {
            parameters.Position = null;
            parameters.Target = null;
            parameters.EntityStatData = null;
            parameters.Prefab = null;
            parameters.Subject = null;
            parameters.ExecuteOnce = true;
            parameters.ExecuteAction = null;
            parameters.TargetTile = null;
            parameters.ExecuteCondition = null;
            parameters.LevelManager = null;
        }
        private CommandParameters()
        {
            
        }
        public class Builder
        {
            private CommandParameters parameters = new CommandParameters();
            public Builder SetContainer(Container container)
            {
                parameters.Container = container;
                return this;
            }
            public Builder SetCommandSubject(SimpleEntity commandSubject)
            {
                parameters.Subject = commandSubject;
                return this;
            }
            public Builder SetPosition(Vector3 position)
            {
                parameters.Position = position;
                return this;
            }
            public Builder SetTargetEntity(SimpleEntity target)
            {
                parameters.Target = target;
                return this;
            }
            public Builder SetTargetTile(Tile3d targetTile)
            {
                parameters.TargetTile = targetTile;
                return this;
            }
            
            public Builder SetPrefab(GameObject prefab)
            {
                parameters.Prefab = prefab;
                return this;
            }
            public Builder SetEntityStatData(StatData data)
            {
                parameters.EntityStatData = data;
                return this;
            }
            public Builder SetExecuteOnce(bool executeOnce)
            {
                parameters.ExecuteOnce = executeOnce;
                return this;
            }
            public Builder SetSimpleAction(System.Action simpleAction)
            {
                parameters.ExecuteAction = simpleAction;
                return this;
            }
            public Builder SetExecuteCondition(Func<bool> condition)
            {
                parameters.ExecuteCondition = condition;
                return this;
            }
            public Builder SetLevelManager(LevelManager levelManager)
            {
                parameters.LevelManager = levelManager;
                return this;
            }
            public Builder Reset()
            {
                ResetParameters(parameters);
                return this;
            }
            public CommandParameters Build()
            {
                return parameters;
            }
        }
    }
}