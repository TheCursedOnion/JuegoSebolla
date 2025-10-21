using System.Linq;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid.Scriptable;
using CursedOnion.ScriptableObjects;
using CursedOnion.Tools;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CursedOnion.Game.Systems.Grid
{
    public class Grid3dSaveLoader : MonoBehaviour
    {
        [Required] public GameObject levelManagerPrefab;
        
        [Button]
        public void GenerateGrid3DFile()
        {
            Tilemap[] layers = GetComponentsInChildren<Tilemap>();
            
            if(!TryGetGridBounds(layers, out var gridBounds))
                return;

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            if(meshGenerator == null) return;
        
            Grid3d grid3d = new Grid3d(gridBounds.size, gridBounds.origin, layers);
            GridMesh gridMesh = meshGenerator.GenerateGridMesh(grid3d);
            
            
            LevelAsset levelAsset = ScriptableObject.CreateInstance<LevelAsset>();
            levelAsset.SetupLevelAsset(gridMesh, grid3d);
            
            levelAsset.Save();
            SpawnLevelManager(levelAsset);
        }

        bool TryGetGridBounds(Tilemap[] layers, out (Vector3 size, Vector3 origin) gridBounds)
        {
            bool hasMinBounds = false;
            
            gridBounds.size =  gridBounds.origin = Vector3.zero;
            Vector3
                max = Vector3.zero,
                min = Vector3.zero;
            
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (layers[i].transform.childCount > 0)
                {
                    min = max = layers[i].transform.GetChild(0).position;
                    hasMinBounds = true;
                    break;
                }
            }
            if(!hasMinBounds) return false;

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
            
            gridBounds.size = new Vector3(width, height, length);
            gridBounds.origin = min;
            
            return true;
        }
        void SpawnLevelManager(LevelAsset levelAsset)
        {
            GameObject levelManager = Instantiate(levelManagerPrefab);
            levelManager.GetComponent<LevelManager>().Initialize(levelAsset);
        }
        /*
         public void TestSaveBinary()
         {
            var grid3DSaveable = new Grid3dSaveableBinary();
            grid3DSaveable.width = 1;
            grid3DSaveable.height = 2;
            grid3DSaveable.length = 3;
            
            BinaryFile file = BinaryFile.Default;
            file.SetSaveableBinary(grid3DSaveable, "grid3d");
            
            FilePanelWindow.SaveBinaryFile(ref file);
        }
        public void TestLoadBinary()
        {
            var grid3Dfile = new Grid3dSaveableBinary();
            BinaryFile file = BinaryFile.Default;
            file.SetSaveableBinary(grid3Dfile, "grid3d");
            
            FilePanelWindow.LoadBinaryFile(ref file);
            Debug.Log($"Size = {grid3Dfile.width},{grid3Dfile.height},{grid3Dfile.length}");
        }
        */
    }
}