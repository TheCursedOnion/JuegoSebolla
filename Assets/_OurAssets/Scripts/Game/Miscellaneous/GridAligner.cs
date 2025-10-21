using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class GridAligner : MonoBehaviour
    { 
        [Inject] LevelManager levelManager;
        void Awake()
        {
            Center();
        }
        void Center()
        {
            Vector3 originManager = levelManager.LevelManagerOrigin;
            
            originManager.Truncate();
            Vector3 decimalOffset = levelManager.LevelManagerOrigin - originManager;
            
            Vector3 position = transform.position;
            position = position.CenterOnTile();
            transform.position = position + decimalOffset;
        }
    }
}
