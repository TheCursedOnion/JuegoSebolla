using CursedOnion.Game.Systems.Grid;
using UnityEngine;
using UnityEngine.Pool;

namespace CursedOnion.Game.Modes.General
{
    [System.Serializable]
    public class GridHighlighter : MonoBehaviour
    {
        [SerializeField] GameObject highlightPlanePrefab;
        private ObjectPool<HighlightPlane> pool;
        private Grid3d grid;
        public void Initialize(Grid3d grid)
        {
            this.grid = grid;
            pool = new ObjectPool<HighlightPlane>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 50
            );
        }
        
        HighlightPlane CreatePooledItem()
        {
            var go = Object.Instantiate(highlightPlanePrefab);
            go.SetActive(false);
            return go.GetComponent<HighlightPlane>();
        }

        void OnTakeFromPool(HighlightPlane plane)
        {
            plane.gameObject.SetActive(true);
        }
        void OnReturnedToPool(HighlightPlane plane)
        {
            plane.gameObject.SetActive(false);
        }

        void OnDestroyPoolObject(HighlightPlane plane)
        {
            Destroy(plane.gameObject);
        }
        
        public HighlightPlane PlaceHighlightPlaneAt(Vector3 worldPosition, Color color)
        {
            Debug.Log(pool != null);
            
            HighlightPlane plane = pool.Get();
            
            plane.SetHighlightAt(worldPosition, grid.GetTileAtWorldPosition(worldPosition) , color);
            return plane;
        }
        public void RetrieveHighlightPlane(HighlightPlane plane)
        {
            if(plane == null) return;
            pool.Release(plane);
        }
    }
}