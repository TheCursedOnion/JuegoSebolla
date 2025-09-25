using CursedOnion.Game.Systems.Files;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Grid3D
{
    public class Tilemap3DController : MonoBehaviour
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
        
        [Button]
        public void GenerateGrid3DFile()
        {
            Tilemap[] layers = GetComponentsInChildren<Tilemap>();
            
            int height = layers.Length;
            
            Vector3 min = layers[0].transform.GetChild(0).position;
            Vector3 max = min;

            foreach (var layer in layers)
            {
                foreach (Transform child in layer.transform)
                {
                    Vector3 pos = child.position;
                    min = Vector3.Min(min, pos);
                    max = Vector3.Max(max, pos);
                }
            }

            float width = max.x - min.x + 1;
            float depth = max.z - min.z + 1;

            var grid3DSaveable = new Grid3DSaveableBinary();
            grid3DSaveable.width = (uint)width;
            grid3DSaveable.height = (uint)height;
            grid3DSaveable.depth = (uint)depth;
            
            BinaryFile file = BinaryFile.Default;
            file.SetSaveableBinary(grid3DSaveable, "grid3d");
            
            FilePanelWindow.SaveBinaryFile(ref file);
        }
        
        [Button]
        public void ReadGrid3DFile()
        {
            var grid3Dfile = new Grid3DSaveableBinary();
            BinaryFile file = BinaryFile.Default;
            file.SetSaveableBinary(grid3Dfile, "grid3d");
            
            FilePanelWindow.LoadBinaryFile(ref file);
            Debug.Log($"Size = {grid3Dfile.width},{grid3Dfile.height},{grid3Dfile.depth}");
        }
    }
}
