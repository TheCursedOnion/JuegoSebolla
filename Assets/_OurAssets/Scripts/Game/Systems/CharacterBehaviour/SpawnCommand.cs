using System;
using CursedOnion.Game.Entity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CursedOnion.Game.Commands
{
    public class SpawnCommand : IStackableCommand
    {
        private GameObject spawnEntity;
        private Vector3 spawnPosition;
        
        public static SpawnCommand Create(GameObject spawnEntity, Vector3 spawnPosition)
        {
            if(!spawnEntity) throw new ArgumentException($"Spawn Unit cannot be null");
            return new SpawnCommand(spawnEntity, spawnPosition);
        }
        private SpawnCommand(GameObject spawnEntity, Vector3 spawnPosition)
        {
            this.spawnEntity = spawnEntity;
            this.spawnPosition = spawnPosition;
        }

        public bool Execute()
        {
            Debug.Log(spawnEntity.name + " spawned at " + spawnPosition);
            Object.Instantiate(spawnEntity, spawnPosition, Quaternion.identity);
            return true;
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}