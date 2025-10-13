using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Objects
{
    public class TileSelector : MonoBehaviour
    {
        [SerializeField, ReadOnly] Vector3 gridPosition;
        [Inject] LevelAsset levelAsset;
        
        public void MovePosition(Vector3 moveDirection)
        {
            Vector3 newPosition = transform.position + moveDirection;
            int result = TrySetAtPosition(newPosition);
            Debug.Log("Resultado: " + result);
            switch (result)
            {
                case 1: MovePosition(moveDirection - Vector3.up); break;
                case 2: MovePosition(moveDirection + Vector3.up); break;
            }
        }

        public void PlaceAtMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Grid3d grid = levelAsset.Grid;
                Vector3 hitPoint = hit.point + hit.normal * 0.1f;
                TrySetAtPosition(hitPoint);
            }
        }

        public void InspectSelectedElement()
        {
            Grid3d grid = levelAsset.Grid;
            Tile3d tile = grid.GetTileAtGridPosition(gridPosition);
            
            Debug.Log(tile.GetContainedEntity());
        }

        int TrySetAtPosition(Vector3 position)
        {
            Grid3d grid = levelAsset.Grid;
            
            if (!grid.TryWorldToGridPosition(position, out Vector3 gridPos)) return -1;

            Tile3d tile = grid.GetTileAtGridPosition(gridPos);
            if (!tile.IsEmptyTile())
            {
                return 2;
            }
            
            Vector3 belowPos = position - Vector3.up;
            if (!grid.TryWorldToGridPosition(belowPos, out Vector3 belowGridPos)) return -1;

            Tile3d belowTile = grid.GetTileAtGridPosition(belowGridPos);
            
            if (belowTile.IsEmptyTile()) return 1;
            
            gridPosition = gridPos;
            transform.position = position.CenterOnTile();
            return 0;
        }
    }
}
