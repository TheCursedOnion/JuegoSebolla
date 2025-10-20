using System;
using CursedOnion.Game.Events;
using CursedOnion.Game.Logic;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class MapLevelSelector : MonoBehaviour
    {
        [Inject] private MapEvents mapEvents;
        [SerializeField] private Vector3 offsetFromLevelPlatforms;
        private void OnEnable()
        {
            mapEvents.OnLevelPlatformChange += MoveToLevel;
            //TODO: Moverse al último nivel completado/jugado
            Debug.Log("Aquí debería comprobarse qué nivel se completo el último pero de momento no hay nada de guardado");
        }

        private void OnDisable()
        {
            mapEvents.OnLevelPlatformChange -= MoveToLevel;
        }

        void MoveToLevel(LevelPlatform levelPlatform)
        {
            transform.position = levelPlatform.transform.position;
            transform.position += offsetFromLevelPlatforms;
        }
    }
}
