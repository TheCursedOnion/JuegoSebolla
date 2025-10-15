using System;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CursedOnion.Game.Cameras
{
    public class GlobalCamera : MonoBehaviour
    {
        public static GlobalCamera Instance { get; private set; }
        
        public Camera Camera;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Instance.PlaceTransform(this.transform);
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        void PlaceTransform(Transform other)
        {
            transform.position = other.position;
            transform.rotation = other.rotation;
        }
    }
}
