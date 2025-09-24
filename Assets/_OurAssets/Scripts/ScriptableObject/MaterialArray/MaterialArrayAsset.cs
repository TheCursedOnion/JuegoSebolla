using UnityEngine;

namespace CursedOnion.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MaterialArrayAsset", menuName = "Game/FileAsset/MaterialArrayAsset")]
    public class MaterialArrayAsset : ScriptableObject
    {
        public Material[] materials;
    }
}
