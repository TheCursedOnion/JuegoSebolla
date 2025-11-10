using CursedOnion.Game.Systems.Grid;
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
            HashSet<Vector3> closedList = new HashSet<Vector3>();

            Node startNode = new Node(startGrid, null, 0, Heuristic(startGrid, targetGrid));
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(n => n.f).First();
                openList.Remove(currentNode);
                closedList.Add(currentNode.gridPos);

                if (currentNode.gridPos == targetGrid)
                {
                    return ReconstructPath(currentNode, levelGrid);
                }

                // Explorar vecinos
                foreach (Vector3 neighbourPos in GetNeighbours(currentNode.gridPos, levelGrid))
                {
                    if (closedList.Contains(neighbourPos))
                        continue;

                    Tile3d tile = levelGrid.GetTileAtGridPosition(neighbourPos);
                    if (tile == null || tile.GetContainedEntity() != null)
                        continue; // Tile obstaculo

                    float tentativeG = currentNode.g + Vector3.Distance(currentNode.gridPos, neighbourPos);

                    Node existingNode = openList.FirstOrDefault(n => n.gridPos == neighbourPos);
                    if (existingNode != null && tentativeG >= existingNode.g)
                        continue;

                    float h = Heuristic(neighbourPos, targetGrid);
                    Node neighbourNode = new Node(neighbourPos, currentNode, tentativeG, h);

                    if (existingNode != null)
                        openList.Remove(existingNode);

                    openList.Add(neighbourNode);
                }
            }

            Debug.Log("No se encontro un camino.");
            return null;
        }

        private List<Vector3> GetNeighbours(Vector3 gridPos, Grid3d levelGrid)
        {
            List<Vector3> neighbours = new List<Vector3>();
            List<Vector3> directions = new List<Vector3>
        {
            new Vector3( 1, 0,  0),
            new Vector3(-1, 0,  0),
            new Vector3( 0, 0,  1),
            new Vector3( 0, 0, -1),

            // Diagonales
            //new Vector3( 1, 0,  1),
            //new Vector3( 1, 0, -1),
            //new Vector3(-1, 0,  1),
            //new Vector3(-1, 0, -1)
        };

            foreach (var dir in directions)
            {
                Vector3 neighbour = gridPos + dir;

                if (levelGrid.GetTileAtGridPosition(neighbour) != null)
                    neighbours.Add(neighbour);
            }

            return neighbours;
        }


        private float Heuristic(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        private List<Vector3> ReconstructPath(Node endNode, Grid3d levelGrid)
        {
            List<Vector3> path = new List<Vector3>();
            Node current = endNode;

            while (current != null)
            {
                path.Add(current.gridPos);
                current = current.parent;
            }

            path.Reverse();

            // Convertir a coordenadas del mundo
            for (int i = 0; i < path.Count; i++)
            {
                if (levelGrid.TryGridToWorldPosition(path[i], out Vector3 worldPos))
                {
                    worldPos += new Vector3(0.5f, 0f, 0.5f);
                    path[i] = worldPos;
                }
            }

            return path;
        }

        private class Node
        {
            public Vector3 gridPos;
            public Node parent;
            public float g, h, f;

            public Node(Vector3 pos, Node parent, float g, float h)
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
