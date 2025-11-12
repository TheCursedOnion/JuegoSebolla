using CursedOnion.Game.Systems.Grid;
using UnityEngine;

namespace CursedOnion.Game.Modes.General
{
    public class HighlightPlane : MonoBehaviour
    {
        private static int highlightColorId = Shader.PropertyToID("_HighlightColor");
        Material highlightMaterial;
        void Awake()
        {
            highlightMaterial = GetComponent<MeshRenderer>().material;
        }

        public void SetHighlightAt(Vector3 worldPosition, Tile3d tile, Color color)
        {
            highlightMaterial.SetColor(highlightColorId, color);
            transform.position = worldPosition + new Vector3(0, 0.01f, 0);
            //Debug.Log("PINTADA TILE: " );
            //tile.DebugTile();
        }
        
    }
}
