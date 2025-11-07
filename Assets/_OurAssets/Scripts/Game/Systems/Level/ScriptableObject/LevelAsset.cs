using CursedOnion.Game.Systems.Files;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Tools;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Level
{
    [CreateAssetMenu(fileName = "LevelAsset", menuName = "Game/FileAsset/LevelAsset")]
    public class LevelAsset : ScriptableObject, ISaveableAsset
    {
        public Material[] MeshMaterials;
        
        [HorizontalLine(color : EColor.Blue, height: 4)]
        public LevelData LevelData;
        
        [HorizontalLine(color : EColor.Blue, height: 4)]
        public Grid3d Grid;

        public void SetupLevelAsset(GridMesh gridMesh, Grid3d grid3d)
        {
            this.MeshMaterials = gridMesh.MaterialsArray;
            this.Grid = grid3d;
        }
        public void Save()
        {
            AssetFile file = AssetFile.DefaultFile(Grid.Mesh, "asset");
            file.SetFileName("Mesh");
            file.SetSaveTitle("Guardar Mesh");
            file.SetSaveMessage("Elige dónde guardar el Mesh del Nivel");
            file.SaveAsset();
            
            file.SetAssetObject(this, "asset");
            file.SetFileName("Level");
            file.SetSaveTitle("Guardar Nivel");
            file.SetSaveMessage("Elige dónde guardar el Nivel");
            file.SaveAsset();
            
            Grid.DebugGrid();
        }
    }
}
