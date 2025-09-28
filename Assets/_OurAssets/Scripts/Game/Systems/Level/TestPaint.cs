using CursedOnion.Extensions;
using CursedOnion.Helpers;
using CursedOnion.ScriptableObjects;
using UnityEngine;

namespace CursedOnion
{
    public class TestPaint : MonoBehaviour
    {
        [SerializeField] LevelAsset levelAsset;

        [SerializeField] private Vector3 gridPos;

        [SerializeField] private Vector3 gridPos2;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                var tile = levelAsset.LevelGrid.GetTileAtGridPosition(gridPos);
                var tile2 = levelAsset.LevelGrid.GetTileAtGridPosition(gridPos2);
                if(tile == null || tile2 == null) return;
                
                var mesh = GetComponent<MeshFilter>().mesh;
                var range = tile.VertexRange;
                var range2 = tile2.VertexRange;
                
                mesh.Color32Vertices(new IntRange[]{range, range2}, Color.red);
            }
        }
    }
}
