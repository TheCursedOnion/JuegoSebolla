using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace CursedOnion.Game.Miscellaneous
{
    [System.Serializable]
    public class TalkEntry
    {
        public float Weight;
        public string TalkKey;
    }
    [CreateAssetMenu(fileName = "TalkData", menuName = "Game/Entity/TalkData")]
    public class TalkData : ScriptableObject
    {
        [SerializeField, MinMaxRangeSlider(5f, 100f)] private Vector2 talkInterval;
        [SerializeField] private List<TalkEntry> talkEntries;
        
        public float GetNewRandomInterval() => Random.Range(talkInterval.x, talkInterval.y);
        
        public bool TryGetRandomTalkKey(out string key)
        {
            key = string.Empty;
            if (talkEntries.Count == 0) return false;
            
            int index = Random.Range(0, talkEntries.Count);
            key = talkEntries[index].TalkKey;
            return true;
        }
        
        private float? totalWeight;
        public bool TryGetWeightedRandomTalkKey(out string key)
        {
            key = string.Empty;
            if (talkEntries.Count == 0) return false;
            
            if(totalWeight == null) CalculateWeight();
            
            float randomWeight = Random.Range(0f, totalWeight.Value);
            float cumulative = 0f;

            foreach (var entry in talkEntries)
            {
                cumulative += entry.Weight;
                if (randomWeight <= cumulative)
                {
                    key = entry.TalkKey;
                    return true;
                }
            }

            return false;
        }
        void CalculateWeight()
        {
            totalWeight = 0f;
            foreach (var entry in talkEntries)
            {
                totalWeight += entry.Weight;
            }
        }
    }
}