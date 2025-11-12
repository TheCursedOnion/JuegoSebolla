using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Game.Modes.General.Animations
{
    [System.Serializable]
    public class AnimationLayer
    {
        public string layerName = "NewLayer";
        public RuntimeAnimatorController animatorController;
        public Material baseMaterial;
        public Texture2D lookupTexture;
    }
    
    [ExecuteAlways]
    public class LayeredEntity : MonoBehaviour
    {
        [SerializeField] GameObject animationLayerPrefab;
        [SerializeField] string testAnimation;
        
        private static int LookupTextureId = Shader.PropertyToID("_LookupTexture");
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        private AnimationLayerGroup layerGroup;
        public void InitializeLayers(AnimationLayerGroup layerGroup)
        {
            this.layerGroup = layerGroup;
            for (int i = 0; i < layerGroup.layers.Count; i++)
            {
                var layer = layerGroup.layers[i];
                
                GameObject animationLayer = Instantiate(animationLayerPrefab, transform);
                
                SpriteRenderer layerSpriteRenderer = animationLayer.GetComponent<SpriteRenderer>();

                if (layer.baseMaterial)
                {
                    layerSpriteRenderer.material = Instantiate(layer.baseMaterial);
                    layerSpriteRenderer.material.SetTexture(LookupTextureId, layer.lookupTexture);
                }

                layerSpriteRenderer.sortingOrder = i;
                
                Animator animator = animationLayer.GetComponent<Animator>();
                animator.runtimeAnimatorController = layer.animatorController;
            }
        }
        public void PlayAnimation(string animationName)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var layerAnimator = transform.GetChild(i).GetComponent<EntityAnimatorController>();
                layerAnimator.PlayAnimation(animationName);

            }
        }

        public void TestPlayAnimation()
        {
            PlayAnimation(testAnimation);
        }
    }
}