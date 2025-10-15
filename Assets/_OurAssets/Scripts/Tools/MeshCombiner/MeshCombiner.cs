using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Tools
{
    public class MeshCombiner : MonoBehaviour
    {
        [Button]
        public CombinedMesh CombineTilemapMeshes(bool spawnObject = true)
        {
            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
            CombinedMesh combinedMesh = new CombinedMesh(filters);
            
            return combinedMesh;
        }
            
    }
}
