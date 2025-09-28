using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
{
    [RequireComponent(typeof(MeshFilter))]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] LevelAsset levelAsset;

        [SerializeField] private Vector3[] gridPositions;
        private Mesh mesh;

        void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }
        public void Initialize(LevelAsset levelAsset)
        {
            this.levelAsset = levelAsset;
            GetComponent<MeshFilter>().sharedMesh = levelAsset.Mesh;
            GetComponent<MeshRenderer>().sharedMaterials = levelAsset.MeshMaterials;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                mesh.FillColor32(Color.white);
                Grid3d grid = levelAsset.Grid;
                foreach (Vector3 pos in gridPositions)
                {
                    if (grid.IsGridPositionInBounds(pos))
                        grid.GetTileAtGridPosition(pos).Paint(Color.red);
                }
                /*var tile = levelAsset.Grid.GetTileAtGridPosition(gridPos);
                var tile2 = levelAsset.Grid.GetTileAtGridPosition(gridPos2);
                if(tile == null || tile2 == null) return;
                
                var mesh = GetComponent<MeshFilter>().mesh;
                var range = tile.VertexInGridRange;
                var range2 = tile2.VertexInGridRange;
                
                mesh.Color32Vertices(new IntRange[]{range, range2}, Color.red);*/
            }
        }

        void OnDisable()
        {
            mesh.FillColor32(Color.white);
        }
    }
}
