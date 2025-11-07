using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CursedOnion.Game.Entity.UI;
using CursedOnion.Game.Events;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using NaughtyAttributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace CursedOnion.Game.Entity
{
    public enum BattleSide
    {
        Neutral,
        Ally,
        Enemy
    }
    public class Unit : CommandableEntity
    {
        // Character UI
        [SerializeField] GameObject unitUI;
        public GameObject GetUI() => unitUI;
        
        [ReadOnly] public bool PlacedManually = false;
        
        public UnitController UnitController;
        
        public BattleSide Side;

        // Ability Status
        private float nextAttackMultiplier = 1f;
        private float additionalHP = 0f;
        private bool isConfused = false;
        private int confusedTurnsRemaining = 0;

        public bool TrySpawningUnit(LevelManager manager, GameObject unitPrefab, Vector3 atPosition, BattleSide side)
        {
            SetLevelVariables(manager);
            
            bool isPlaced = LevelManager.TryPlacingUnit(Data.GetPrice());
            if (isPlaced)
            {
                Unit spawnedUnit = Instantiate(unitPrefab, atPosition, Quaternion.identity).GetComponent<Unit>();
                spawnedUnit.SetSide(side);
                spawnedUnit.PlacedManually = true;
            }
            return isPlaced;
        }
        void SetSide(BattleSide side)
        {
            Side = side;
            
            if (UnitController !=null) Destroy(UnitController);
            
            UnitController = Side switch
            {
                BattleSide.Enemy => gameObject.AddComponent<AIUnitController>(),
                BattleSide.Ally => gameObject.AddComponent<PlayerUnitController>(),
                _ => null
            };
        }
        
        public bool TryErasingUnit(LevelManager manager)
        {
            bool canBeErased = PlacedManually && Side == BattleSide.Ally;
            if (canBeErased)
            {
                manager.EraseUnit(Data.GetPrice());
                Dispose();
            }
            return canBeErased;
        }
        
        public void Start()
        {
            var container = this.gameObject.scene.GetSceneContainer();
            SetLevelVariables(container.Resolve<LevelManager>());
            
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);

            //Debug.Log("El set de stats es temporal");
            Stats.SetStats(Data);

            if(UnitController == null)
            {
                SetSide(Side);
            }
        }

        public void ApplyConfusion(int turns)
        {
            isConfused = true;
            confusedTurnsRemaining = turns;
        }

        public void UpdateStatusEffects()
        {
            if (isConfused)
            {
                confusedTurnsRemaining--;
                if (confusedTurnsRemaining <= 0)
                {
                    isConfused = false;
                }
            }
        }

        public void SetAdditionalHP(float factor) 
        {
            additionalHP = GetStats().MaxHealthStat * factor / 100f;
        }

        public override void Damage(int damage)
        {
            int finalDamage = Mathf.Clamp(damage - GetStats().DefenseStat, 0, damage);

            if (additionalHP > 0)
            {
                if (finalDamage <= additionalHP)
                {
                    additionalHP -= finalDamage;
                    return; 
                }
                else
                {
                    finalDamage -= Mathf.FloorToInt(additionalHP);
                    additionalHP = 0; 
                }
            }

            Stats.CurrentHealthStat -= finalDamage;
            if (Stats.CurrentHealthStat <= 0) Die();
        }
        public void SetNextAttackMultiplier(float multiplier)
        {
            nextAttackMultiplier = multiplier;
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
            if (target == null)
            {
                Debug.LogWarning("ValidateAttack falló: target es null");
            }
                     
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
                
                if (!Grid.TryWorldToGridPosition(transform.position, out Vector3 startGrid))
                {
                    Debug.LogError($"TryWorldToGridPosition falló para start world position: {transform.position}");
                    return;
                }

                if (!Grid.TryWorldToGridPosition(newPosition, out Vector3 targetGrid))
                {
                    Debug.LogError($"TryWorldToGridPosition falló para target world position: {newPosition}");
                    return;
                }

                var path = UnitController.GetPathFinder().FindPath(startGrid, targetGrid, Grid);

                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning("No se encontró camino (FindPath devolvió null/empty).");
                    return;
                }

                StartCoroutine(MoveAlongPath(path));
            }
        }

        public override bool ValidateMove(Vector3 newPosition)
        {
            return true;
        }

        private IEnumerator MoveAlongPath(List<Vector3> path)
        {
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(null);
            foreach (var pos in path)
            {
                transform.position = new Vector3(pos.x, pos.y, pos.z);
                yield return new WaitForSeconds(0.25f);
            }
            Grid.GetTileAtWorldPosition(transform.position).SetContainedEntity(this);
        }

    }
}
