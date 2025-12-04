using System;
using System.Collections;
using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity;
using CursedOnion.Game.Systems.Level;
using CursedOnion.Helpers;
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
            levelEvents = levelManager.LevelEvents;
            levelEvents.OnMergedUnitListUpdated += ProcessMergedList;
            
            iconPool = PoolHelper.CreatePool(CreateIcon);
            separatorPool = PoolHelper.CreatePool(() => Instantiate(separatorPrefab, turnIconContainer));
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

        TurnIcon CreateIcon()
        {
           var icon = Instantiate(turnIconPrefab, turnIconContainer).GetComponent<TurnIcon>();
           icon.Initialize(levelEvents, this);
           return icon;
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
                visualizedIcons[i].EnableCanRequestScroll(false);
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

            turnIconContainer.GetChild(0).GetComponent<TurnIcon>().EnableCanRequestScroll(true);
                
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
                    turnIconContainer.GetChild(i + added + 1).GetComponent<TurnIcon>().EnableCanRequestScroll(true);
                    
                    added++;
                }

                previous = current;
            }
        }
        
        public void FocusOnIcon(TurnIcon elementToCenter)
        {
            if(elementToCenter == null) return;
            
            StartCoroutine(turnOrderScrollRect.FocusOnItemCoroutine(elementToCenter.GetComponent<RectTransform>(), 0.5f));
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
