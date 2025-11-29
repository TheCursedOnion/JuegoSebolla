using System;
using System.Collections;
using System.Collections.Generic;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.Level.Battle.UI
{
    public class TurnInspector : MonoBehaviour
    {
        [SerializeField] private GameObject separatorPrefab;
        [SerializeField] private GameObject turnIconPrefab;
        [SerializeField] private Transform turnIconContainer;
        [SerializeField] private ScrollRect turnOrderScrollRect;
        
        private ObjectPool<TurnIcon> iconPool;
        private ObjectPool<GameObject> separatorPool;
        
        readonly List<TurnIcon> visualizedIcons = new();
        readonly List<GameObject> separators = new();
        
        bool modifiedLayout = false;
        LevelEvents levelEvents;
        public void Initialize(LevelManager levelManager)
        {
            iconPool = CreatePool(() => Instantiate(turnIconPrefab, turnIconContainer).GetComponent<TurnIcon>());

            separatorPool = CreatePool(() => Instantiate(separatorPrefab, turnIconContainer));

            levelEvents = levelManager.LevelEvents;
            levelEvents.OnMergedUnitListUpdated += ProcessMergedList;
        }

        void OnEnable()
        {
            if(modifiedLayout)
                StartCoroutine(SmoothSetScrollHorizontalValue(0f, 1f));
        }
        void OnDestroy()
        {
            levelEvents.OnMergedUnitListUpdated -= ProcessMergedList;
        }

        ObjectPool<T> CreatePool<T>(Func<T> createFunc) where T : class
        {
            return new ObjectPool<T>(
                createFunc,
                item => SetActive(item, true),
                item => SetActive(item, false),
                item => DestroyObject(item),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 50
            );
        }
        void SetActive<T>(T item, bool active)
        {
            switch (item)
            {
                case GameObject go:
                    go.SetActive(active);
                    break;

                case Component c:
                    c.gameObject.SetActive(active);
                    break;

                default:
                    throw new ArgumentException("Type must be GameObject or Component");
            }
        }

        void DestroyObject<T>(T item)
        {
            switch (item)
            {
                case GameObject go:
                    Destroy(go);
                    break;

                case Component c:
                    Destroy(c.gameObject);
                    break;

                default:
                    throw new ArgumentException("Type must be GameObject or Component");
            }
        }
        
        void ProcessMergedList(List<Unit> mergedUnits)
        {
            int needed = mergedUnits.Count;
            int current = visualizedIcons.Count;
            
            if(needed == 0) return;
            
            if (current > needed)
            {
                for (int i = needed; i < current; i++)
                    iconPool.Release(visualizedIcons[i]);

                visualizedIcons.RemoveRange(needed, current - needed);
            }
            
            for (int i = current; i < needed; i++)
            {
                TurnIcon newIcon = iconPool.Get();
                visualizedIcons.Add(newIcon);
            }
            
            AddIcons(mergedUnits);
            
            ClearSeparators();
            AddSeparators(mergedUnits);
            
            modifiedLayout = true;
        }
        void AddIcons(List<Unit> mergedUnits)
        {
            for (int i = 0; i < mergedUnits.Count; i++)
            {
                Unit unit = mergedUnits[i];
                visualizedIcons[i].AssignUnit(unit);
            }
        }
        
        void ClearSeparators()
        {
            foreach (var separator in separators)
            {
                separatorPool.Release(separator);
            }
            separators.Clear();
        }
        void AddSeparators(List<Unit> mergedUnits)
        {
            if (mergedUnits.Count == 0) return;

            int added = 0;
            var previous = mergedUnits[0];

            for (int i = 1; i < mergedUnits.Count; i++)
            {
                var current = mergedUnits[i];

                bool sideChanged = current.GetSide() != previous.GetSide();
                bool initiativeChanged = current.Stats.InitiativeStat != previous.Stats.InitiativeStat;
                
                if (sideChanged || initiativeChanged)
                {
                    var separator = separatorPool.Get();
                    separators.Add(separator);
                    separator.transform.SetSiblingIndex(i + added);
                    
                    added++;
                }

                previous = current;
            }
        }
        IEnumerator SmoothSetScrollHorizontalValue(float target, float duration)
        {
            yield return new WaitForEndOfFrame();
            modifiedLayout = false;
            
            float start = turnOrderScrollRect.horizontalNormalizedPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = 1 - Mathf.Pow(1 - t, 2);

                turnOrderScrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, easedT);
                yield return null;
            }

            turnOrderScrollRect.horizontalNormalizedPosition = target;
        }

        
    }
}
