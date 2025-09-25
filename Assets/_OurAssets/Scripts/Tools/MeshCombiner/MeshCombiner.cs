using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public class MeshCombiner : MonoBehaviour
    {
        [Button]
        public CombinedMesh CombineMeshes(bool spawnObject = true)
        {
            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

            CombinedMesh combinedMesh = new CombinedMesh(filters);
            
            #if UNITY_EDITOR
                combinedMesh.Save();
            #endif
            
            if(spawnObject)
                combinedMesh.SpawnObject();
            
            return combinedMesh;
        }
            
    }
}
