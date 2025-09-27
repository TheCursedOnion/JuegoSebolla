using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CursedOnion.Game.Systems.Grid.Scriptable
{
    [CreateAssetMenu(fileName = "Tile3d Catalog", menuName = "Game/Tile/Catalog")]
    public class Tile3dCatalog : ScriptableObject
    {
        [SerializeField] private List<CatalogEntry> tileDefinitions;

        public ScriptableTile3d GetTileDefinition(uint id)
        {
            return tileDefinitions.FirstOrDefault(entry => entry.GetId() == id)?.GetTileDefinition();
        }

        private void OnValidate()
        {
            foreach (var entry in tileDefinitions)
            {
                uint id = entry.GetTileDefinition() != null ? entry.GetTileDefinition().descriptor.Id : uint.MaxValue;
                entry.SetId(id);
            }
        }
    }
    
    [System.Serializable]
    public class CatalogEntry
    {
        [SerializeField] uint id;
        [SerializeField] ScriptableTile3d tile;
        
        public uint GetId() => id;
        public void SetId(uint newId) => id = newId;
        public ScriptableTile3d GetTileDefinition() => tile;
    }
}
