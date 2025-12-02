using System;
using CursedOnion.Extensions;
using CursedOnion.Locators;
using Reflex.Attributes;
using UnityEngine;

namespace CursedOnion
{
    public class CameraBounds : MonoBehaviour
    {
        [Inject] private RuntimeVariableLocator runtimeVariableLocator;
        [SerializeField] private BoxCollider area;
        private Transform cameraGuide;
        void Awake()
        {
            gameObject.InjectDependencies();
            cameraGuide = runtimeVariableLocator.GlobalCamera.CameraGuide.transform;
        }

        void LateUpdate()
        {
            Bounds b = area.bounds;
            Vector3 pos = cameraGuide.position;

            pos.x = Mathf.Clamp(pos.x, b.min.x, b.max.x);
            pos.y = Mathf.Clamp(pos.y, b.min.y, b.max.y);
            pos.z = Mathf.Clamp(pos.z, b.min.z, b.max.z);

            cameraGuide.position = pos;
        }
    }
}
