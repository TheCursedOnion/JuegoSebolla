using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace CursedOnion.Tools
{
    public class MeshGenerator : MonoBehaviour
    {
        public GridMesh GenerateGridMesh(Grid3d gridData)
        {
            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
            GridMesh gridMesh = new GridMesh(filters, gridData);
            gridData.SetMeshForGrid(gridMesh.Mesh);
            return gridMesh;
        }
            
    }
}
