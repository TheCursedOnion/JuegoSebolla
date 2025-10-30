using System;
using CursedOnion.Extensions;
using CursedOnion.Game.Systems.Grid;
using CursedOnion.Game.Systems.Level;
using CursedOnion.ScriptableObjects;
using Reflex.Attributes;
using Reflex.Extensions;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class GridAligner : MonoBehaviour
    { 
        LevelManager levelManager;
        void Start()
        {
            levelManager = gameObject.scene.GetSceneContainer().Resolve<LevelManager>();
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
