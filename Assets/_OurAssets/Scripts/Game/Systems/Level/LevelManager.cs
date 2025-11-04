using System;
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
        [SerializeField] TurnSystem turnSystem;
        public TurnSystem GetTurnSystem() => turnSystem;
        
        
        [SerializeField, Inject, ReadOnly] LevelAsset levelAsset;
        [SerializeField, ReadOnly] LevelState currentLevelState;
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

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            levelAsset.Grid.ResetPaint();
        }
    }
}
