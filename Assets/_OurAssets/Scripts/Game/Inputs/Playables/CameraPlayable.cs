using System;
using CursedOnion.Extensions;
using CursedOnion.Helpers;
using NaughtyAttributes;
using Reflex.Attributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedOnion.Game.Inputs
{
    public class CameraPlayable : MonoBehaviour, IPlayable
    {
        [Inject] public InputReaderCollection InputReaderCollection { get; set; }
        [Inject] LevelManager levelManager;
        
        CinemachinePanTilt cinemachinePanTilt;
        private void Awake()
        {
            cinemachinePanTilt = GetComponent<CinemachinePanTilt>();
        }

        public void OnEnable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            if (InputReaderCollection == null)
            {
                transform.localEulerAngles = new Vector3(-30, 0, 0);
                return;
            }
                
            reader.RotateCamera += RotateCamera;
        }
        
        public void OnDisable()
        {
            BattleInputReader reader = InputReaderCollection.GetReader<BattleInputReader>();
            reader.RotateCamera -= RotateCamera;
        }

        void RotateCamera(DirectionFlag direction)
        {
            float rotateAmmount = direction == DirectionFlag.Left ? 45 : -45;
            
            cinemachinePanTilt.PanAxis.Center += rotateAmmount;
            
            cinemachinePanTilt.PanAxis.Center %= 360f;
            if (cinemachinePanTilt.PanAxis.Center < 0)
                cinemachinePanTilt.PanAxis.Center += 360f;
            
            levelManager.UpdateCameraPanAngles(cinemachinePanTilt.PanAxis.Center);
        }
    }
}