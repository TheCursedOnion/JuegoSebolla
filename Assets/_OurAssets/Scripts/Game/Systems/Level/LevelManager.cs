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
        [Expandable, SerializeField, Inject] LevelAsset levelAsset;

        [SerializeField] private Vector3[] gridPositions;
        private Mesh mesh;
        private Renderer meshRenderer;

        void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            
            meshRenderer = GetComponent<Renderer>();
            UpdateOffset();
        }
        public void Initialize(LevelAsset asset)
        {
            gameObject.name = "LevelManager";
            levelAsset = asset;
            
            GetComponent<MeshCollider>().sharedMesh = asset.Grid.Mesh;
            GetComponent<MeshFilter>().sharedMesh = asset.Grid.Mesh;
            
            GetComponent<MeshRenderer>().sharedMaterials = asset.MeshMaterials;
        }

        void UpdateOffset()
        {
            levelAsset.Grid.StartingOffset = levelAsset.Grid.Origin - meshRenderer.bounds.min;
        }
        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.C))
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
                UpdateOffset();
                mesh.FillColor32(Color.white);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Grid3d grid = levelAsset.Grid;
                    
                    Vector3 hitPoint = hit.point - hit.normal * 0.1f;
                    if (grid.TryWorldToGridPosition(hitPoint, out Vector3 gridPosition))
                    {
                        grid.GetTileAtGridPosition(gridPosition).Paint(Color.red);
                    }
                    Debug.Log("Hit en " + hit.point + "grid "+ gridPosition);
                }
            }*/
        }

        void OnDisable()
        {
            mesh.FillColor32(Color.white);
        }
    }
}
