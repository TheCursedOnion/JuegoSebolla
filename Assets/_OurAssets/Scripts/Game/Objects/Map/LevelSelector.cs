using System;
using CursedOnion.Game.Logic;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Objects
{
    public class LevelSelector : MonoBehaviour
    {
        [Inject] private MediatorEvents mediatorEvents;
        [SerializeField] private Vector3 offsetFromLevelPlatforms;
        private void OnEnable()
        {
            mediatorEvents.OnLevelInspectionChange += MoveToLevel;
            //TODO: Moverse al último nivel completado/jugado
            Debug.Log("Aquí debería comprobarse qué nivel se completo el último pero de momento no hay nada de guardado");
        }

        private void OnDisable()
        {
            mediatorEvents.OnLevelInspectionChange -= MoveToLevel;
        }

        void MoveToLevel(LevelPlatform levelPlatform)
        {
            transform.position = levelPlatform.transform.position;
            transform.position += offsetFromLevelPlatforms;
        }
    }
}
