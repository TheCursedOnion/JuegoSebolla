using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Helpers
{
    public static class VectorConversions
    {
        public static bool TryWorldToGridPosition(Vector3 worldPosition, Grid3d grid, out Vector3 gridPosition)
        {
            gridPosition = worldPosition - grid.Origin;
            gridPosition.Truncate();
            if (grid.IsGridPositionInBounds(gridPosition))
            {
                return true;
            }

            return false;
        }
        public static bool TryGridToWorldPosition(Vector3 gridPosition, Grid3d grid, out Vector3 worldPosition)
        {
            worldPosition = gridPosition + grid.Origin;
            if (grid.IsGridPositionInBounds(gridPosition))
            {
                return true;
            }

            return false;
        }
        public static bool TryWorldPositionToGridIndex(Vector3 worldPosition, Grid3d grid, out int gridIndex)
        {
            gridIndex = -1;
            if (!TryWorldToGridPosition(worldPosition, grid, out var gridPosition)) return false;

            gridIndex = GridPositionToIndex(gridPosition, grid);
            return true;
        }
        public static bool TryGridPositionToIndex(Vector3 gridPosition, Grid3d grid, out int gridIndex)
        {
            gridIndex = -1;
            if(!grid.IsGridPositionInBounds(gridPosition)) return false;
            
            gridIndex = GridPositionToIndex(gridPosition, grid);
            return true;
        }
        private static int GridPositionToIndex(Vector3 gridPosition, Grid3d grid)
        {
            Vector3Int size = grid.Size;
            Vector3Int vectorIndex = gridPosition.ObtainVectorInt();
            return vectorIndex.x + vectorIndex.z * size.x + vectorIndex.y * size.x * size.z;
        }
    }
}
