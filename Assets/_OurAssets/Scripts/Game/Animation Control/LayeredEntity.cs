using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion
{
    public class LayeredEntity : MonoBehaviour
    {
        private static readonly int LookupTexture = Shader.PropertyToID("_LookupTexture");

        [System.Serializable]
        public class AnimationLayer
        {
            public string layerName = "NewLayer";
            public RuntimeAnimatorController animatorController;
            public Material baseMaterial;
            public Texture2D lookupTexture;

            [HideInInspector] public Material materialInstance;
            [HideInInspector] public Animator animator;
            [HideInInspector] public EntityAnimatorController controller;
            [HideInInspector] public SpriteRenderer spriteRenderer;
            [HideInInspector] public GameObject layerObject;
        }

        public List<AnimationLayer> layers = new List<AnimationLayer>();
        [SerializeField] string testAnimation;

        private void Awake()
        {
            InitializeLayers();
        }

        private void InitializeLayers()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];

                if (layer.layerObject == null)
                {
                    layer.layerObject = new GameObject(layer.layerName);
                    layer.layerObject.transform.SetParent(transform);
                    layer.layerObject.transform.localPosition = Vector3.zero;
                }

                if (layer.spriteRenderer == null)
                    layer.spriteRenderer = layer.layerObject.AddComponent<SpriteRenderer>();

                layer.spriteRenderer.sortingOrder = i;

                if (layer.baseMaterial != null)
                {
                    layer.materialInstance = Instantiate(layer.baseMaterial);
                    layer.spriteRenderer.material = layer.materialInstance;

                    if (layer.lookupTexture != null)
                        layer.materialInstance.SetTexture(LookupTexture, layer.lookupTexture);
                }

                if (layer.animator == null)
                {
                    layer.animator = layer.layerObject.AddComponent<Animator>();
                    layer.animator.runtimeAnimatorController = layer.animatorController;
                }

                if (layer.controller == null)
                {
                    layer.controller = layer.layerObject.AddComponent<EntityAnimatorController>();
                    layer.controller.animator = layer.animator;
                }
            }
        }

        public void PlayAnimation(string animationName)
        {
            foreach (var layer in layers)
            {
                if (layer.controller != null)
                    layer.controller.PlayAnimation(animationName);
            }
        }

        public void TestPlayAnimation()
        {
            PlayAnimation(testAnimation);
        }
    }
}
