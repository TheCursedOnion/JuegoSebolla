using System;
using System.Collections.Generic;
using CursedOnion.Extensions;
using CursedOnion.Game.Entity;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Modes.General.Animations
{
    [System.Serializable]
    public class AnimationLayer
    {
        public string layerName = "NewLayer";
        [FormerlySerializedAs("animatorController")] public RuntimeAnimatorController runtimeAnimatorController;
        public Material baseMaterial;
        public Texture2D lookupTexture;
    }
    
    [ExecuteAlways]
    public class LayeredEntity : MonoBehaviour
    {
        [SerializeField] GameObject spritesContainer;
        [SerializeField] GameObject animationLayerPrefab;
        [SerializeField] string testAnimation;
        [SerializeField] SimpleEntity entity;
        
        private static int LookupTextureId = Shader.PropertyToID("_LookupTexture");
        
        private AnimatorController animatorController;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
        
        void Awake()
        {
            entity = GetComponent<SimpleEntity>();
        }
        
        public void Initialize(List<AnimationLayerGroup> animationLayerGroups, int useGroupIndex)
        {
            if (animationLayerGroups == null || animationLayerGroups.Count == 0)
            {
                Debug.LogWarning($"{name}: No se asignaron grupos de animación.");
                return;
            }
            
            AnimationLayerGroup group = animationLayerGroups[useGroupIndex];
            if (group.layers == null || group.layers.Count == 0)
            {
                Debug.LogWarning($"{name}: El grupo '{group.groupName}' no tiene capas asignadas.");
                return;
            }
            
            ProcessLayerGroup(group);
        }
        
        void ProcessLayerGroup(AnimationLayerGroup group)
        {
            animatorController = spritesContainer.GetOrAddComponent<AnimatorController>();
            animatorController.SetupController(entity);
            
            for (int i = 0; i < group.layers.Count; i++)
            {
                AnimationLayer layer = group.layers[i];
                
                var animationLayer = CreateAnimationLayer();
                
                SetupRenderer(animationLayer, layer, i);
                
                var animator = SetupAnimator(animationLayer, layer);
                animatorController.AddAnimator(animator);
            }
        }
        private GameObject CreateAnimationLayer()
        {
            return Instantiate(animationLayerPrefab, spritesContainer.transform);
        }
        private Animator SetupAnimator(GameObject layerObject, AnimationLayer layer)
        {
            layerObject.GetOrAddComponent<AnimationListener>().SetController(animatorController);
            
            Animator animator = layerObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = layer.runtimeAnimatorController;
            return animator;
        }
        private void SetupRenderer(GameObject layerObject, AnimationLayer layer, int indexOrder)
        {
            SpriteRenderer spriteRenderer = layerObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 0;
            spriteRenderer.material = Instantiate(layer.baseMaterial);

            if (layer.lookupTexture != null)
                spriteRenderer.material.SetTexture(LookupTextureId, layer.lookupTexture);
            
            layerObject.transform.position += -transform.forward * (0.001f * indexOrder);
        }
        
        public void PlayAnimation(string animationName)
        {
            animatorController.PlayAnimation(animationName);
        }
        public void TestPlayAnimation()
        {
            PlayAnimation(testAnimation);
        }
    }
}