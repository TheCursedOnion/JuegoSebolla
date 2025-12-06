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
            var layout = turnIconContainer.GetComponent<LayoutGroup>();
            if (layout) layout.enabled = false;
            
            ClearIcons();
            AddIcons(mergedUnits);
            
            AnalyzeTurnOrderSeparators(mergedUnits);
            
            modifiedLayout = true;
            
            if (layout) layout.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(turnIconContainer as RectTransform);
        }
        void ClearIcons()
        {
            foreach (var icon in visualizedIcons)
                iconPool.Release(icon);
            visualizedIcons.Clear();
        }
        void AddIcons(List<Unit> mergedUnits)
        {
            for (int i = 0; i < mergedUnits.Count; i++)
            {
                var icon = iconPool.Get();
                Unit unit = mergedUnits[i];
                icon.AssignUnit(unit);
                icon.EnableCanRequestScroll(false);
                icon.transform.SetSiblingIndex(i);
                visualizedIcons.Add(icon);
            }
        }
        
        void AnalyzeTurnOrderSeparators(List<Unit> mergedUnits)
        {
            ClearSeparators();

            if (mergedUnits.Count <= 1) return;

            List<int> separatorPositions = new List<int>();
            InsertSeparatorPositions(separatorPositions, mergedUnits);

            List<int> realIndices = PlaceSeparators(separatorPositions);

            SetScrollRequests(realIndices);
        }

        void ClearSeparators()
        {
            foreach (var separator in separators)
                separatorPool.Release(separator);

            separators.Clear();
        }

        void InsertSeparatorPositions(List<int> separatorPositions, List<Unit> mergedUnits)
        {
            Unit previous = mergedUnits[0];

            for (int i = 1; i < mergedUnits.Count; i++)
            {
                Unit current = mergedUnits[i];

                bool sideChanged = current.GetSide() != previous.GetSide();
                bool initChanged = current.Stats.InitiativeStat != previous.Stats.InitiativeStat;

                if (sideChanged || initChanged)
                    separatorPositions.Add(i);

                previous = current;
            }
        }

        List<int> PlaceSeparators(List<int> separatorPositions)
        {
            List<int> realIndices = new List<int>();
            int offset = 0;

            foreach (int pos in separatorPositions)
            {
                int siblingIndex = pos + offset;

                var sep = separatorPool.Get();
                sep.transform.SetParent(turnIconContainer, false);
                sep.transform.SetSiblingIndex(siblingIndex);

                separators.Add(sep);
                realIndices.Add(siblingIndex);

                offset++;
            }

            return realIndices;
        }

        void SetScrollRequests(List<int> realIndices)
        {
            foreach (int sepIndex in realIndices)
            {
                int iconIndex = sepIndex + 1;

                if (iconIndex < turnIconContainer.childCount)
                {
                    var icon = turnIconContainer.GetChild(iconIndex)?.GetComponent<TurnIcon>();
                    icon?.EnableCanRequestScroll(true);
                }
            }
        }
        
        public void FocusOnIcon(TurnIcon elementToCenter)
        {
            if(elementToCenter == null || gameObject.activeInHierarchy == false) return;
            
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
