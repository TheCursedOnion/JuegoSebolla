using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Grid
{
    public class Grid3dEditor : MonoBehaviour
    {
        [Required] [SerializeField] GameObject tilemapPrefab;
        
        [Button]
        public void StackTilemapLayer()
        {
            var newTilemap = GameObject.Instantiate(tilemapPrefab, this.transform);
            newTilemap.transform.SetSiblingIndex(0);
            newTilemap.name = $"Layer {this.transform.childCount - 1}";
            newTilemap.transform.localPosition = new Vector3(0, this.transform.childCount - 1, 0);
        }
        
        [Button]
        public void RemoveTilemapLayer()
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i);
                child.name = $"Layer {this.transform.childCount - 1 - i}";
            }
        }
    }
}
