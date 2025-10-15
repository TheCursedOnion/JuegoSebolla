using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

namespace CursedOnion
{
    public class Character : MonoBehaviour, IEntity
    {
        // Character type 
        public CharacterData data;

        // Character Model (test)
        public GameObject characterModel3D;

        // Character UI (test)
        public GameObject characterUI;
        public CharacterUI uiScript;

        // Character Stats
        public string characterName { get; set; }
        public int HP { get; set; }
        public int attackStat { get; set; }
        public int defenseStat { get; set; }
        public int speedStat { get; set; }
        public int movementStat { get; set; }
        public int priceStat { get; set; }
        public int id { get; set; }
        public bool isEnemy { get; set; }

        // Character Variables
        public bool hasDied = false;
        public bool canMove = false;
        public bool canAttack = false;

        // Pathfinding
        private Coroutine moveCoroutine;

        public void SetCharacterData()
        {
            characterName = data.SetCharacterName();
            HP = data.SetRandomHP();
            attackStat = data.SetRandomAttack();
            defenseStat = data.SetRandomDefense();
            speedStat = data.SetRandomSpeed();
            movementStat = data.SetMovement();
            priceStat = data.SetPrice();
            characterModel3D = data.SetModel();
            characterUI = data.SetUI();
            CreateCharacterUI();
        }

        public void DoTurn()
        {
            Debug.Log(characterName + " id: " + id + " est� haciendo su turno...");
            uiScript.gameObject.SetActive(true);
        }

        public void Attack(IEntity target)
        {
            var targetObj = target as Character;
            Debug.Log(characterName + id + " Attacking! " + targetObj.characterName + targetObj.id);
            targetObj.HP -= Mathf.Max(1, this.attackStat - targetObj.defenseStat);
            Debug.Log(targetObj.characterName + targetObj.id + "was Attacked " + "HP now: " + targetObj.HP);
            if (targetObj.HP <= 0)
            {
                targetObj.hasDied = true;
                targetObj.Die();
            }
            canAttack = false;
            UpdateCharacterUI();
        }

        public void Move(Vector3 newPosition) 
        {
            Debug.Log(characterName + id + " Moving to " + newPosition);
            canMove = false;
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveAlongPath(GetPath(transform.position, newPosition)));
        }

        public void Die()
        {
            Debug.Log(characterName + id + " has died.");
            this.gameObject.SetActive(false);
        }

        public void CreateCharacterUI()
        {
            if (characterUI != null)
            {
                GameObject uiInstance = Instantiate(characterUI, transform);

                uiScript = uiInstance.GetComponent<CharacterUI>();
                if (uiScript != null)
                {
                    uiScript.SetCharacter(this);
                    uiScript.gameObject.SetActive(false);
                }
            }
        }

        public void UpdateCharacterUI() 
        {
            uiScript.UpdateUI();
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
            
            UpdateCharacterUI();
        }
        #endregion
    }
}
