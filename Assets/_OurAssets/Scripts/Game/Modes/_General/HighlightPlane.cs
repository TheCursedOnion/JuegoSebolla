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

            bool isStair = !tile.IsFullTile() && !tile.IsEmptyTile();
            
            float xRotation = isStair ? -45f : 0f;
            float yRotation = tile.GetYRotation();
            
            float zScale = isStair ? 0.144f : 0.1f;
            
            transform.position = worldPosition + tile.GetDisplayOffset() + new Vector3(0, 0.002f, 0);
            transform.eulerAngles = new Vector3(xRotation, yRotation, 0);
            
            var newScale = transform.localScale;
            newScale.z = zScale;
            transform.localScale = newScale;
        }
        
    }
}
