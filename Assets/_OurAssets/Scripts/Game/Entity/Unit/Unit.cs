using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public class Unit : CommandableEntity
    {
        // Character UI 
        [SerializeField] GameObject unitUI;
        public GameObject GetUI() => unitUI;
        
        public bool IsEnemy;
        public UnitController UnitController;

        // Pathfinding
        private Coroutine moveCoroutine;
        
        private Grid3d levelGrid;
        private TurnSystem turnSystem;
        public void Start()
        {
            var container = this.gameObject.scene.GetSceneContainer();
            
            var levelAsset = container.Resolve<LevelAsset>();
            levelGrid = levelAsset.Grid;
            
            var levelManager = container.Resolve<LevelManager>();
            turnSystem = levelManager.GetTurnSystem();
            
            levelGrid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);

            // Controllers
            if (UnitController == null)
            {
                UnitController = GetComponent<UnitController>();
                if (UnitController == null)
                {
                    UnitController = IsEnemy
                        ? gameObject.AddComponent<AIUnitController>()
                        : gameObject.AddComponent<PlayerUnitController>();
                }
            }

            Debug.Log("El set de stats es temporal");
            Stats.SetStats(Data);
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

        public override bool ValidateAttack(SimpleEntity target)
        {
            return target != null;
        }


        protected override void DoMove(Vector3 newPosition, bool undo)
        {
            
            if (undo)
            {
                transform.position = newPosition;
            }
            else
            {
                Debug.Log($"{gameObject.name}: Me muevo a {newPosition}");
                if (moveCoroutine != null)
                    StopCoroutine(moveCoroutine);

                if (!levelGrid.TryWorldToGridPosition(transform.position, out Vector3 startGrid))
                {
                    Debug.LogError($"TryWorldToGridPosition falló para start world position: {transform.position}");
                    return;
                }

                var path = UnitController.PathFinder.FindPath(startGrid, newPosition, levelGrid);

                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning("No se encontró camino (FindPath devolvió null/empty).");
                    return;
                }

                moveCoroutine = StartCoroutine(MoveAlongPath(path));
            }
        }

        public override bool ValidateMove(Vector3 newPosition)
        {
            return true;
        }

        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            foreach (var pos in path)
            {
                transform.position = new Vector3(pos.x, pos.y, pos.z);
                yield return new WaitForSeconds(0.25f);
            }
        }

    }
}
