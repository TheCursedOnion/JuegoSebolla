using CursedOnion.Game.Systems.Files;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Grid
{
    public class Grid3dSaveLoader : MonoBehaviour
    {
        [Button]
        public void GenerateGrid3DFile()
        {
            Tilemap[] layers = GetComponentsInChildren<Tilemap>();
            var gridBounds = GetGridBounds(layers);
            
            Grid3d grid3d = new Grid3d(gridBounds.size, gridBounds.origin, layers);

            var grid3DSaveable = new Grid3dSaveableBinary();
            grid3DSaveable.width = (uint)gridBounds.size.x;
            grid3DSaveable.height = (uint)gridBounds.size.y;
            grid3DSaveable.length = (uint)gridBounds.size.z;
            
            BinaryFile file = BinaryFile.Default;
            file.SetSaveableBinary(grid3DSaveable, "grid3d");
            
            FilePanelWindow.SaveBinaryFile(ref file);
        }

        (Vector3 size, Vector3 origin) GetGridBounds(Tilemap[] layers)
        {
            Vector3 min = layers[0].transform.GetChild(0).position;
            Vector3 max = min;
            
            foreach (var layer in layers)
            {
                foreach (Transform tile in layer.transform)
                {
                    Vector3 pos = tile.position;
                    min = Vector3.Min(min, pos);
                    max = Vector3.Max(max, pos);
                }
            }
            
            float width = max.x - min.x + 1;
            float height = layers.Length;
            float length = max.z - min.z + 1;
            Vector3 size = new Vector3(width, height, length);
            
            return (size, min);
        }
        
        [Button]
        public void ReadGrid3DFile()
        {
            var grid3Dfile = new Grid3dSaveableBinary();
            BinaryFile file = BinaryFile.Default;
            file.SetSaveableBinary(grid3Dfile, "grid3d");
            
            FilePanelWindow.LoadBinaryFile(ref file);
            Debug.Log($"Size = {grid3Dfile.width},{grid3Dfile.height},{grid3Dfile.length}");
        }
    }
}