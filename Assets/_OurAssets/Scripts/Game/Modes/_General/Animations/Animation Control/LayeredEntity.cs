using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        [SerializeField] GameObject spritesContainer;
        [SerializeField] GameObject animationLayerPrefab;
        [SerializeField] string testAnimation;
        
        private static int LookupTextureId = Shader.PropertyToID("_LookupTexture");
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        private AnimationLayerGroup layerGroup;

        public void Initialize(List<AnimationLayerGroup> animationLayerGroups, int useIndex)
        {
            if (animationLayerGroups == null || animationLayerGroups.Count == 0)
            {
                Debug.LogWarning($"{name}: No se asignaron grupos de animación.");
                return;
            }
            
            AnimationLayerGroup layer = animationLayerGroups[useIndex];
            if (layer.layers == null || layer.layers.Count == 0)
            {
                Debug.LogWarning($"{name}: El grupo '{layer.groupName}' no tiene capas asignadas.");
                return;
            }
            
            Debug.Log(useIndex);
            ProcessLayer(layer);
        }
        void ProcessLayer(AnimationLayerGroup layerGroup)
        {
            this.layerGroup = layerGroup;
            for (int i = 0; i < layerGroup.layers.Count; i++)
            {
                var layer = layerGroup.layers[i];
                
                GameObject animationLayer = Instantiate(animationLayerPrefab, spritesContainer.transform);
                
                SpriteRenderer layerSpriteRenderer = animationLayer.GetComponent<SpriteRenderer>();
                
                if (layer.baseMaterial != null)
                {
                    layerSpriteRenderer.material = Instantiate(layer.baseMaterial);
                    if(layer.lookupTexture != null)
                        layerSpriteRenderer.material.SetTexture(LookupTextureId, layer.lookupTexture);
                    else
                    {
                        animationLayer.transform.position += Vector3.back * (0.001f * i);
                    }
                }

                layerSpriteRenderer.sortingOrder = 0;
                
                Animator animator = animationLayer.GetComponent<Animator>();
                animator.runtimeAnimatorController = layer.animatorController;
            }
        }
        public void PlayAnimation(string animationName)
        {
            for (int i = 0; i < spritesContainer.transform.childCount; i++)
            {
                var layerAnimator = spritesContainer.transform.GetChild(i).GetComponent<EntityAnimatorController>();
                layerAnimator.PlayAnimation(animationName);
            }
        }

        public void TestPlayAnimation()
        {
            PlayAnimation(testAnimation);
        }
    }
}