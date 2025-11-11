using CursedOnion.Game.Systems.Grid;
using CursedOnion.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public abstract class UnitController : MonoBehaviour
    {
        [SerializeReference, SubclassSelector] protected AStarPathFinder PathFinder = new AStarPathFinderMod();
        public AStarPathFinder GetPathFinder() => PathFinder;
        public abstract void ProcessTurn(Unit unit);
    }

    [System.Serializable]
    public class AStarPathFinderMod : AStarPathFinder
    {
        // Clase modificada para pruebas unitarias si es necesario
    }

    [System.Serializable]
    public class AStarPathFinder
    {
        public List<Vector3> FindPath(Vector3 startGrid, Vector3 targetGrid, Grid3d levelGrid)
        {
            List<Node> openList = new List<Node>();
            HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();

            Vector3Int start = Vector3Int.FloorToInt(startGrid);
            Vector3Int target = Vector3Int.FloorToInt(targetGrid);

            Node startNode = new Node(start, null, 0, Heuristic(start, target));
            openList.Add(startNode);


            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(n => n.f).First();
                openList.Remove(currentNode);
                closedList.Add(currentNode.gridPos);

                if (currentNode.gridPos == target)
                    return ReconstructPath(currentNode, levelGrid);

                foreach (Vector3Int neighbourPos in GetNeighbours(currentNode.gridPos, levelGrid))
                {
                    if (closedList.Contains(neighbourPos))
                        continue;

                    Tile3d tile = levelGrid.GetTileAtGridPosition(neighbourPos);
                    if (tile == null || tile.GetContainedEntity() != null)
                        continue;

                    float tentativeG = currentNode.g + 1; // movimiento básico = 1

                    Node existingNode = openList.FirstOrDefault(n => n.gridPos == neighbourPos);
                    if (existingNode != null && tentativeG >= existingNode.g)
                        continue;

                    float h = Heuristic(neighbourPos, target);
                    Node neighbourNode = new Node(neighbourPos, currentNode, tentativeG, h);

                    if (existingNode != null)
                        openList.Remove(existingNode);

                    openList.Add(neighbourNode);
                }

            }
            Debug.Log("No se encontro un camino.");
            return null;
        }

        private List<Vector3Int> GetNeighbours(Vector3Int currentAirPos, Grid3d levelGrid)
        {
            List<Vector3Int> neighbours = new();
            Vector3Int[] directions =
            {
            new( 1, 0, 0),
            new(-1, 0, 0),
            new( 0, 0, 1),
            new( 0, 0,-1)
        };

            // Tile de suelo debajo de la posición actual
            Vector3Int groundPos = currentAirPos + Vector3Int.down;
            Tile3d groundTile = levelGrid.GetTileAtGridPosition(groundPos);
            if (groundTile == null)
                return neighbours;

            DirectionFlag groundExits = groundTile.GetExitDirections();

            foreach (var dir in directions)
            {
                // Tile de aire a la que se movería el personaje
                Vector3Int nextAirPos = currentAirPos + dir;
                Vector3Int nextGroundPos = nextAirPos + Vector3Int.down;

                Tile3d nextGroundTile = levelGrid.GetTileAtGridPosition(nextGroundPos);
                if (nextGroundTile == null)
                    continue;

                var nextDesc = nextGroundTile.GetTileDescriptor();
                if (nextDesc.IsAirBlock)
                    continue;

                DirectionFlag moveDir = DirectionHelper.GetDirectionFlag(dir);
                DirectionFlag opposite = DirectionHelper.GetDirectionFlag(-dir);

                DirectionFlag nextEntries = nextGroundTile.GetEntryDirections();

                // Debe poder salir del suelo actual y entrar en el suelo destino
                if ((groundExits & moveDir) != 0 && (nextEntries & opposite) != 0)
                {
                    neighbours.Add(nextAirPos);
                }
            }

            return neighbours;
        }


        private float Heuristic(Vector3Int a, Vector3Int b)
        {
            // Distancia Manhattan en grid 3D
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        private List<Vector3> ReconstructPath(Node endNode, Grid3d levelGrid)
        {
            List<Vector3> path = new();
            Node current = endNode;

            while (current != null)
            {
                if (levelGrid.TryGridToWorldPosition(current.gridPos, out Vector3 worldPos))
                {
                    worldPos += new Vector3(0.5f, 0f, 0.5f);
                    path.Add(worldPos);
                }

                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        private class Node
        {
            public Vector3Int gridPos;
            public Node parent;
            public float g, h, f;

            public Node(Vector3Int pos, Node parent, float g, float h)
            {
                this.gridPos = pos;
                this.parent = parent;
                this.g = g;
                this.h = h;
                this.f = g + h;
            }
        }
    }
}
