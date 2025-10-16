using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
{
    [RequireComponent(typeof(MeshFilter))]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField, Inject, ReadOnly] LevelAsset levelAsset;
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
            levelAsset.Grid.StartingOffset = levelAsset.Grid.Origin - GetComponent<MeshRenderer>().bounds.min;
        }
        

    }
}
