using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
{
    [RequireComponent(typeof(MeshFilter))]
    public class LevelManager : MonoBehaviour
    {
        [Expandable, SerializeField] LevelAsset levelAsset;

        [SerializeField] private Vector3[] gridPositions;
        private Mesh mesh;
        [ShowNonSerializedField] private Vector3 levelOffset;

        void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            
            Renderer meshRenderer = GetComponent<Renderer>();
            levelOffset = levelAsset.Grid.Origin - meshRenderer.bounds.min;
        }
        public void Initialize(LevelAsset levelAsset)
        {
            this.gameObject.name = "LevelManager";
            this.levelAsset = levelAsset;
            GetComponent<MeshFilter>().sharedMesh = levelAsset.Mesh;
            GetComponent<MeshRenderer>().sharedMaterials = levelAsset.MeshMaterials;
            GetComponent<MeshCollider>().sharedMesh = levelAsset.Mesh;
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
            }

            if (Input.GetMouseButtonDown(0))
            {
                mesh.FillColor32(Color.white);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Grid3d grid = levelAsset.Grid;
                    Debug.Log("Hit");
                    Vector3 hitPoint = hit.point + levelOffset - hit.normal * 0.1f;
                    if (VectorConversions.TryWorldToGridPosition(hitPoint, grid, out Vector3 gridPosition))
                    {
                        Debug.Log("A Pintar en: " + gridPosition);
                        grid.GetTileAtGridPosition(gridPosition).Paint(Color.red);
                    }
                }
            }
        }

        void OnDisable()
        {
            mesh.FillColor32(Color.white);
        }
    }
}
