using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class Unit : CommandableEntity
    {
        // Character UI 
        public CharacterUI uiScript;
        
        public bool IsEnemy;
        public UnitController UnitController;

        // Pathfinding
        private Coroutine moveCoroutine;
        
        private Grid3d levelGrid;
        private TurnSystem turnSystem;
        public void Awake()
        {
            var container = this.gameObject.scene.GetSceneContainer();
            
            var levelAsset = container.Resolve<LevelAsset>();
            levelGrid = levelAsset.Grid;
            
            var levelManager = container.Resolve<LevelManager>();
            turnSystem = levelManager.GetTurnSystem();
        }

        public override void Damage(int damage)
        {
            int finalDamage = Mathf.Clamp(damage - GetStats().DefenseStat, 0, damage);
            
            Stats.CurrentHealthStat -= finalDamage;
            if (Stats.CurrentHealthStat <= 0) Die();
        }

        protected override void DoAttack(SimpleEntity target, bool undo)
        {
            if (undo)
            {
                
            }
            else
            {
                target.Damage(GetStats().AttackStat);
            }
        }

        /*public void Move(Vector3 newPosition) 
        {
            uiScript.SetButtonsFalse();
            Debug.Log(characterName + id + " Moving to " + newPosition);
            canMove = false;
            hasMoved = true;
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveAlongPath(GetPath(transform.position, newPosition)));
        }*/

        protected override void DoMove(Vector3 newPosition, bool undo)
        {
            if (undo)
            {
                transform.position = newPosition;
            }
            else
            {
                
            }
        }
        
        #region PathFinding
        // Pathfinding method (Bresenham's 3D line algorithm)
        public static List<Vector3> GetPath(Vector3 start, Vector3 end) // From repository with MIT License
        {
            List<Vector3> path = new List<Vector3>();

            float x1 = start.x, y1 = start.y, z1 = start.z;
            float x2 = end.x, y2 = end.y, z2 = end.z;
            
            float dx = Mathf.Abs(x2 - x1), dy = Mathf.Abs(y2 - y1), dz = Mathf.Abs(z2 - z1);
            float xs = x2 > x1 ? 1 : -1;
            float ys = y2 > y1 ? 1 : -1;
            float zs = z2 > z1 ? 1 : -1;
            
            float p1, p2;
            
            float x = x1, y = y1, z = z1;

            // X is the driving axis
            if (dx >= dy && dx >= dz)
            {
                p1 = 2 * dy - dx;
                p2 = 2 * dz - dx;
                while (x != x2)
                {
                    path.Add(new Vector3(x, y, z));
                    x += xs;
                    if (p1 >= 0)
                    {
                        y += ys;
                        p1 -= 2 * dx;
                    }
                    if (p2 >= 0)
                    {
                        z += zs;
                        p2 -= 2 * dx;
                    }
                    p1 += 2 * dy;
                    p2 += 2 * dz;
                }
            }
            // Y is the driving axis
            else if (dy >= dx && dy >= dz)
            {
                p1 = 2 * dx - dy;
                p2 = 2 * dz - dy;
                while (y != y2)
                {
                    path.Add(new Vector3(x, y, z));
                    y += ys;
                    if (p1 >= 0)
                    {
                        x += xs;
                        p1 -= 2 * dy;
                    }
                    if (p2 >= 0)
                    {
                        z += zs;
                        p2 -= 2 * dy;
                    }
                    p1 += 2 * dx;
                    p2 += 2 * dz;
                }
            }
            // Z is the driving axis
            else
            {
                p1 = 2 * dy - dz;
                p2 = 2 * dx - dz;
                while (z != z2)
                {
                    path.Add(new Vector3(x, y, z));
                    z += zs;
                    if (p1 >= 0)
                    {
                        y += ys;
                        p1 -= 2 * dz;
                    }
                    if (p2 >= 0)
                    {
                        x += xs;
                        p2 -= 2 * dz;
                    }
                    p1 += 2 * dy;
                    p2 += 2 * dx;
                }
            }

            path.Add(new Vector3(x2, y2, z2)); // Añade el destino
            return path;
        }
        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            foreach (var pos in path)
            {
                transform.position = new Vector3(pos.x, pos.y, pos.z);
                yield return new WaitForSeconds(0.5f);
            }

            //UpdateCharacterUI();
        }
        #endregion


    }
}
