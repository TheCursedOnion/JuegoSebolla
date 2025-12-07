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

            UpdateIcons(mergedUnits);

            AnalyzeTurnOrderSeparators(mergedUnits);

            modifiedLayout = true;

            if (layout) layout.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(turnIconContainer as RectTransform);
        }
        void UpdateIcons(List<Unit> mergedUnits)
        {
            int needed = mergedUnits.Count;
            
            while (visualizedIcons.Count > needed)
            {
                var extra = visualizedIcons[visualizedIcons.Count - 1];
                visualizedIcons.RemoveAt(visualizedIcons.Count - 1);
                iconPool.Release(extra);
            }
            
            while (visualizedIcons.Count < needed)
            {
                var icon = iconPool.Get();
                icon.transform.SetParent(turnIconContainer, false);
                visualizedIcons.Add(icon);
            }
            
            for (int i = 0; i < needed; i++)
            {
                var icon = visualizedIcons[i];
                Unit unit = mergedUnits[i];

                icon.AssignUnit(unit);
                icon.EnableCanRequestScroll(false);
                icon.transform.SetSiblingIndex(i);
            }
        }
        
        void AnalyzeTurnOrderSeparators(List<Unit> mergedUnits)
        {
            if (mergedUnits.Count <= 1)
            {
                EnsureSeparatorCount(0);
                return;
            }
            
            List<int> separatorPositions = ComputeSeparatorPositions(mergedUnits);
            EnsureSeparatorCount(separatorPositions.Count);
            
            List<int> realIndices = PlaceExistingSeparators(separatorPositions);
            SetScrollRequests(realIndices);
        }
        List<int> ComputeSeparatorPositions(List<Unit> mergedUnits)
        {
            List<int> positions = new List<int>();

            Unit previous = mergedUnits[0];

            for (int i = 1; i < mergedUnits.Count; i++)
            {
                Unit current = mergedUnits[i];

                bool sideChanged = current.GetSide() != previous.GetSide();
                bool initChanged = current.Stats.InitiativeStat != previous.Stats.InitiativeStat;

                if (sideChanged || initChanged)
                    positions.Add(i);

                previous = current;
            }

            return positions;
        }
        void EnsureSeparatorCount(int needed)
        {
            while (separators.Count > needed)
            {
                var sep = separators[separators.Count - 1];
                separators.RemoveAt(separators.Count - 1);
                separatorPool.Release(sep);
            }

            while (separators.Count < needed)
            {
                var sep = separatorPool.Get();
                sep.transform.SetParent(turnIconContainer, false);
                separators.Add(sep);
            }
        }
        List<int> PlaceExistingSeparators(List<int> separatorPositions)
        {
            List<int> realIndices = new List<int>();
            int offset = 0;

            for (int i = 0; i < separatorPositions.Count; i++)
            {
                int pos = separatorPositions[i];
                int siblingIndex = pos + offset;

                var sep = separators[i];
                sep.transform.SetSiblingIndex(siblingIndex);

                realIndices.Add(siblingIndex);

                offset++;
            }

            return realIndices;
        }

        void SetScrollRequests(List<int> realIndices)
        {
            turnIconContainer.GetChild(0)?.GetComponent<TurnIcon>()?.EnableCanRequestScroll(true);
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
