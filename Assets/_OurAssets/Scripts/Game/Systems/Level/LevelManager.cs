using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game;
using CursedOnion.Game.Commands;
using CursedOnion.Game.Events;
using CursedOnion.Game.Handlers;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
{
    [RequireComponent(typeof(MeshFilter))]
    public class LevelManager : MonoBehaviour
    {
        public EntityCommandHandler CommandHandler { get; } = new EntityCommandHandler();
        
        [SerializeField] TurnSystem turnSystem;
        public TurnSystem GetTurnSystem() => turnSystem;
        
        [SerializeField, Inject, ReadOnly] LevelAsset levelAsset;
        public Vector3 LevelManagerOrigin => GetComponent<MeshRenderer>().bounds.min;
        
        public void Initialize(LevelAsset asset)
        {
            gameObject.name = "LevelManager";
            
            levelAsset = asset;
            GetComponent<MeshCollider>().sharedMesh = asset.Grid.Mesh;
            GetComponent<MeshFilter>().sharedMesh = asset.Grid.Mesh;
            GetComponent<MeshRenderer>().sharedMaterials = asset.MeshMaterials;
        }

        void Awake()
        {
            levelAsset.Grid.StartingOffset = levelAsset.Grid.Origin - LevelManagerOrigin;
            
            /*Mesh mesh = GetComponent<MeshFilter>().mesh;
            levelAsset.Grid.PaintTileAtGridPosition(new Vector3(0,0,0), Color.red);
            levelAsset.Grid.PaintTileAtGridPosition(new Vector3(1,0,0), Color.blue);
            levelAsset.Grid.PaintTileAtGridPosition(new Vector3(2,0,0), Color.yellow);
            levelAsset.Grid.PaintTileAtGridPosition(new Vector3(3,0,0), Color.green);
            
            GetComponent<MeshFilter>().mesh = levelAsset.Grid.PaintAllTiles(Color.magenta);*/
        }

        private void OnDisable()
        {
            levelAsset.Grid.ResetPaint();
        }
    }
}
