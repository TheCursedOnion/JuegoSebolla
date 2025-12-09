using System;
using CursedOnion.Game.Localization;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace CursedOnion.Game.Miscellaneous
{
    public class TextParticle : MonoBehaviour
    {
        [SerializeField] LocalizedText localizedText;
        [SerializeField] TextMeshPro textMesh;
        
        [Header("Animation Settings")]
        public float verticalDistance = 2f;
        public float moveUpDuration = 1f;
        
        [Header("")]
        public float fadeInDuration = 0.25f;
        public float holdDuration = 0.4f;
        public float fadeOutDuration = 0.35f;
        
        [Header("")]
        public float horizontalShakeAmplitude = 0.2f;
        public float horizontalShakeFrequency = 6f;
        
        LTSeq sequence;
        TextParticleManager manager;
        bool isCancelled = false;
        public void Initialize(TextParticleManager manager)
        {
            this.manager = manager;
            //Debug.LogWarning("Me creo");
        }
        public void SetKey(string key)
        {
            localizedText.SetKey(key);
        }
        public void SetText(string text)
        {
            localizedText.SetText(text);
        }
        public void SetColor(Color color)
        {
            textMesh.color = color;
        }

        void OnDisable()
        {
            CancelAnimation();
        }

        void CancelAnimation()
        {
            isCancelled = true;
            LeanTween.cancel(gameObject);
            LeanTween.cancel(textMesh.gameObject);
        }
        public void PlayAnimation()
        {
            if (this == null || gameObject == null) return;
            
            CancelAnimation();
            isCancelled = false;
            
            Color color = textMesh.color;
            color.a = 0;
            textMesh.color = color;
            
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * verticalDistance;
            
            sequence = LeanTween.sequence();
            
            LeanTween.move(gameObject, endPos, moveUpDuration)
                .setEaseOutQuad()
                .setOnUpdate((Vector3 pos) =>
                {
                    float oscillation = Mathf.Sin(Time.time * horizontalShakeFrequency) * horizontalShakeAmplitude;
                    Vector3 vertical = Vector3.up * (pos.y - startPos.y);
                    Vector3 horizontal = transform.right * oscillation;

                    transform.position = startPos + vertical + horizontal;
                });
            
            sequence.append(
                LeanTween.value(gameObject, 0f, 1f, fadeInDuration)
                    .setOnUpdate(alpha =>
                    {
                        var c = textMesh.color;
                        c.a = alpha;
                        textMesh.color = c;
                    })
            );
            
            
            sequence.append(holdDuration);
            
            sequence.append(
                LeanTween.value(gameObject, 1f, 0f, fadeOutDuration)
                    .setOnUpdate(alpha =>
                    {
                        var c = textMesh.color;
                        c.a = alpha;
                        textMesh.color = c;
                    })
            );
            
            sequence.append(() =>
            {
                if (!isCancelled && this != null && gameObject != null)
                    manager.ReturnParticle(gameObject);
            });
        }
    }
}