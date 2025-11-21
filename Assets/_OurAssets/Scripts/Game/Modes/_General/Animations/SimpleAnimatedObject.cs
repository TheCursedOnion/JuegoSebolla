using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CursedOnion.Game.Modes.General.Animations
{
    public class SimpleAnimatedObject : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        
        private Vector3 originalPosition;
        private Vector3 originalScale;
        private Quaternion originalRotation;
        
        private readonly Dictionary<string, LTDescr> tweenDescriptors = new();
        private void Awake()
        {
            StoreOriginalValues();
        }

        private void StoreOriginalValues()
        {
            originalPosition = targetTransform.localPosition;
            originalScale = targetTransform.localScale;
            originalRotation = targetTransform.localRotation;
        }
        
        private void SafeTween(string key, LTDescr tween)
        {
            
            if (tweenDescriptors.TryGetValue(key, out LTDescr existingTween) && LeanTween.isTweening(existingTween.id))
                LeanTween.cancel(existingTween.id);

            tween.setOnComplete(() =>
            {
                tweenDescriptors.Remove(key);
            });

            tweenDescriptors[key] = tween;
        }
        
        #region Métodos de Escala
        /// <summary>
        /// Escala el objeto uniformemente
        /// </summary>
        public void UniformScaleTo(float targetScale, LeanTweenType easing, float duration)
        {
            SafeTween("scale",
                LeanTween.scale(targetTransform.gameObject, Vector3.one * targetScale, duration).setEase(easing));
        }
        
        /// <summary>
        /// Escala el objeto a un tamaño específico
        /// </summary>
        public void ScaleTo(Vector3 targetScale, LeanTweenType easing, float duration)
        {
            SafeTween("scale",
                LeanTween.scale(targetTransform.gameObject, targetScale, duration).setEase(easing));
        }
        
        /// <summary>
        /// Pulso de escala uniforme (escala y vuelve al original)
        /// </summary>
        public void UniformScalePulse(float pulseScale, LeanTweenType easing, float duration)
        {
            SafeTween("scale",
                LeanTween.scale(targetTransform.gameObject, Vector3.one * pulseScale, duration * 0.5f)
                .setEase(easing)
                .setLoopPingPong(1));
        }
        
        /// <summary>
        /// Pulso de escala (escala y vuelve al original)
        /// </summary>
        public void ScalePulse(Vector3 pulseScale, LeanTweenType easing, float duration)
        {
            SafeTween("scale",
                LeanTween.scale(targetTransform.gameObject, pulseScale, duration * 0.5f)
                .setEase(easing)
                .setLoopPingPong(1));
        }

        /// <summary>
        /// Escala el objeto RectTransform a un tamaño específico
        /// </summary>
        public void RectScaleTo(Vector2 targetScale, LeanTweenType easing, float duration)
        {
            if(targetTransform is not RectTransform rectTransform) return;
            SafeTween("scale",
                LeanTween.size(rectTransform, targetScale, duration)
                    .setEase(easing)
                );
        }

        /// <summary>
        /// Restaura la escala original
        /// </summary>
        public void ScaleToOriginal(float duration, LeanTweenType easing = LeanTweenType.notUsed)
        {
            SafeTween("scale",
                LeanTween.scale(targetTransform.gameObject, originalScale, duration).setEase(easing));
        }

        #endregion

        #region Métodos de Traslación

        /// <summary>
        /// Mueve el objeto a una posición específica (local)
        /// </summary>
        public void MoveToLocal(Vector3 targetPosition, LeanTweenType easing, float duration)
        {
            RectTransform rect = targetTransform as RectTransform;
            if (rect != null)
            {
                Vector2 startPos = rect.anchoredPosition;
                Vector2 targetPos = new Vector2(targetPosition.x, targetPosition.y); // z se ignora para UI

                SafeTween("move",
                    LeanTween.value(rect.gameObject, startPos, targetPos, duration)
                        .setEase(easing)
                        .setOnUpdate((Vector2 val) => rect.anchoredPosition = val)
                );
            }
            else
            {
                SafeTween("move", 
                    LeanTween.moveLocal(targetTransform.gameObject, targetPosition, duration).setEase(easing)
                );
            }
        }

        /// <summary>
        /// Mueve el objeto a una posición específica (global)
        /// </summary>
        public void MoveTo(Vector3 targetPosition, LeanTweenType easing, float duration)
        {
            SafeTween("move",
                LeanTween.move(targetTransform.gameObject, targetPosition, duration).setEase(easing));
        }

        /// <summary>
        /// Mueve el objeto en una dirección específica
        /// </summary>
        public void MoveInDirection(Vector3 direction, float distance, LeanTweenType easing, float duration)
        {
            Vector3 targetPosition = targetTransform.position + direction.normalized * distance;
            SafeTween("move",
                LeanTween.move(targetTransform.gameObject, targetPosition, duration).setEase(easing));
        }

        /// <summary>
        /// Mueve el objeto arriba
        /// </summary>
        public void MoveUp(float distance, LeanTweenType easing, float duration)
        {
            MoveInDirection(Vector3.up, distance, easing, duration);
        }

        /// <summary>
        /// Mueve el objeto abajo
        /// </summary>
        public void MoveDown(float distance, LeanTweenType easing, float duration)
        {
            MoveInDirection(Vector3.down, distance, easing, duration);
        }

        /// <summary>
        /// Mueve el objeto a la derecha
        /// </summary>
        public void MoveRight(float distance, LeanTweenType easing, float duration)
        {
            MoveInDirection(Vector3.right, distance, easing, duration);
        }

        /// <summary>
        /// Mueve el objeto a la izquierda
        /// </summary>
        public void MoveLeft(float distance, LeanTweenType easing, float duration)
        {
            MoveInDirection(Vector3.left, distance, easing, duration);
        }

        /// <summary>
        /// Mueve el objeto adelante
        /// </summary>
        public void MoveForward(float distance, LeanTweenType easing, float duration)
        {
            MoveInDirection(Vector3.forward, distance, easing, duration);
        }

        /// <summary>
        /// Mueve el objeto atrás
        /// </summary>
        public void MoveBack(float distance, LeanTweenType easing, float duration)
        {
            MoveInDirection(Vector3.back, distance, easing, duration);
        }

        /// <summary>
        /// Restaura la posición original
        /// </summary>
        public void MoveToOriginal(float duration, LeanTweenType easing = LeanTweenType.notUsed)
        {
            SafeTween("move",
                LeanTween.moveLocal(targetTransform.gameObject, originalPosition, duration).setEase(easing));
        }

        /// <summary>
        /// Mueve el objeto en trayectoria curva (Bezier)
        /// </summary>
        public void MoveBezier(Vector3[] path, LeanTweenType easing, float duration)
        {
            SafeTween("move",
                LeanTween.move(targetTransform.gameObject, path, duration).setEase(easing));
        }

        /// <summary>
        /// Mueve el objeto siguiendo un camino con curva suave
        /// </summary>
        public void MoveSpline(Vector3[] path, LeanTweenType easing, float duration)
        {
            SafeTween("move",
                LeanTween.moveSpline(targetTransform.gameObject, path, duration).setEase(easing));
        }

        #endregion

        #region Métodos de Rotación

        /// <summary>
        /// Rota el objeto a una rotación específica (Euler)
        /// </summary>
        public void RotateTo(Vector3 targetRotation, LeanTweenType easing, float duration)
        {
            SafeTween("rotate",
                LeanTween.rotate(targetTransform.gameObject, targetRotation, duration).setEase(easing));
        }

        /// <summary>
        /// Rota el objeto localmente
        /// </summary>
        public void RotateToLocal(Vector3 targetRotation, LeanTweenType easing, float duration)
        {
            SafeTween("rotate",
                LeanTween.rotateLocal(targetTransform.gameObject, targetRotation, duration).setEase(easing));
        }

        /// <summary>
        /// Rota el objeto alrededor de un eje
        /// </summary>
        public void RotateAround(Vector3 axis, float degrees, LeanTweenType easing, float duration)
        {
            Vector3 currentRotation = targetTransform.localEulerAngles;
            Vector3 targetRotation = currentRotation + axis.normalized * degrees;
            SafeTween("rotate",
                LeanTween.rotateLocal(targetTransform.gameObject, targetRotation, duration).setEase(easing));
        }

        /// <summary>
        /// Rota el objeto en el eje X
        /// </summary>
        public void RotateX(float degrees, LeanTweenType easing, float duration)
        {
            RotateAround(Vector3.right, degrees, easing, duration);
        }

        /// <summary>
        /// Rota el objeto en el eje Y
        /// </summary>
        public void RotateY(float degrees, LeanTweenType easing, float duration)
        {
            RotateAround(Vector3.up, degrees, easing, duration);
        }

        /// <summary>
        /// Rota el objeto en el eje Z
        /// </summary>
        public void RotateZ(float degrees, LeanTweenType easing, float duration)
        {
            RotateAround(Vector3.forward, degrees, easing, duration);
        }

        /// <summary>
        /// Rota el objeto 360 grados continuamente
        /// </summary>
        public void RotateSpin(Vector3 axis, float duration, LeanTweenType easing, int loops = -1)
        {
            SafeTween("rotate",
                LeanTween.rotateAround(targetTransform.gameObject, axis, 360f, duration).setLoopCount(loops)
                    .setEase(easing));
        }

        /// <summary>
        /// Restaura la rotación original
        /// </summary>
        public void RotateToOriginal(float duration, LeanTweenType easing = LeanTweenType.notUsed)
        {
            SafeTween("rotate",
                LeanTween.rotate(targetTransform.gameObject, originalRotation.eulerAngles, duration).setEase(easing));
        }

        #endregion

        #region Métodos de Fade (Para UI y Sprites)

        /// <summary>
        /// Fade para Canvas Group (UI)
        /// </summary>
        public void FadeCanvasGroup(float targetAlpha, LeanTweenType easing, float duration)
        {
            CanvasGroup canvasGroup = targetTransform.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                SafeTween("canvasGroup",
                    LeanTween.alphaCanvas(canvasGroup, targetAlpha, duration).setEase(easing));
            }
            else
            {
                Debug.LogWarning($"No se encontró CanvasGroup en {targetTransform.name}");
            }
        }

        /// <summary>
        /// Fade In (aparición gradual)
        /// </summary>
        public void FadeIn(LeanTweenType easing, float duration)
        {
            FadeCanvasGroup(1f, easing, duration);
        }

        /// <summary>
        /// Fade Out (desaparición gradual)
        /// </summary>
        public void FadeOut(LeanTweenType easing, float duration)
        {
            FadeCanvasGroup(0f, easing, duration);
        }
        
        public void ChangeImageAlpha(float targetAlpha, LeanTweenType easing, float duration)
        {
            Image image = targetTransform.GetComponent<Image>();
            if (image != null)
            {
                Color targetColor = image.color;
                targetColor.a = targetAlpha;
                SafeTween("image",
                    LeanTween.color(targetTransform.GetComponent<RectTransform>(), targetColor, duration)
                        .setEase(easing));
            }
            else
            {
                Debug.LogWarning($"No se encontró componente Image en {targetTransform.name}");
            }
        }

        /// <summary>
        /// Fade In para UI Image
        /// </summary>
        public void FadeImageIn(LeanTweenType easing, float duration)
        {
            ChangeImageAlpha(1f, easing, duration);
        }

        /// <summary>
        /// Fade Out para UI Image
        /// </summary>
        public void FadeImageOut(LeanTweenType easing, float duration)
        {
            ChangeImageAlpha(0f, easing, duration);
        }


        /// <summary>
        /// Fade para material (3D objects)
        /// </summary>
        public void FadeMaterial(float targetAlpha, LeanTweenType easing, float duration)
        {
            Renderer renderer = targetTransform.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                Color color = renderer.material.color;
                color.a = targetAlpha;
                SafeTween("render3D",
                    LeanTween.color(targetTransform.gameObject, color, duration).setEase(easing));
            }
        }

        #endregion

        #region Métodos de Color

        /// <summary>
        /// Cambia el color del objeto (Renderer)
        /// </summary>
        public void ChangeColor(Color targetColor, LeanTweenType easing, float duration)
        {
            Renderer renderer = targetTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                SafeTween("render3D",
                    LeanTween.color(targetTransform.gameObject, targetColor, duration).setEase(easing));
            }
        }

        /// <summary>
        /// Cambia el color de un Sprite Renderer
        /// </summary>
        public void ChangeSpriteColor(Color targetColor, LeanTweenType easing, float duration)
        {
            SpriteRenderer spriteRenderer = targetTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                SafeTween("render2D",
                    LeanTween.color(targetTransform.gameObject, targetColor, duration).setEase(easing));
            }
        }
        
        /// <summary>
        /// Cambia el color de un Image
        /// </summary>
        public void ChangeImageColor(Color targetColor, LeanTweenType easing, float duration)
        {
            Image image = targetTransform.GetComponent<Image>();
            if (image != null)
            {
                targetColor.a = image.color.a;
                SafeTween("image",
                    LeanTween.color(targetTransform.GetComponent<RectTransform>(), targetColor, duration)
                    .setEase(easing));
            }
        }

        #endregion

        #region Métodos Combinados y Especiales

        /// <summary>
        /// Efecto de "pop" (aparición con escala)
        /// </summary>
        public void PopIn(float duration)
        {
            targetTransform.localScale = Vector3.zero;
            SafeTween("specialScale",
                LeanTween.scale(targetTransform.gameObject, originalScale, duration)
                    .setEase(LeanTweenType.easeOutBack));
        }

        /// <summary>
        /// Efecto de "pop" al desaparecer
        /// </summary>
        public void PopOut(float duration)
        {
            SafeTween("specialScale",
                LeanTween.scale(targetTransform.gameObject, Vector3.zero, duration)
                    .setEase(LeanTweenType.easeInBack));
        }

        /// <summary>
        /// Efecto de sacudida (shake)
        /// </summary>
        public void Shake(float intensity, float duration)
        {
            Vector3 originalPos = targetTransform.localPosition;
            SafeTween("specialMove",
                LeanTween.moveLocal(targetTransform.gameObject, originalPos + Random.insideUnitSphere * intensity,
                        duration * 0.1f)
                    .setEase(LeanTweenType.easeShake)
                    .setLoopPingPong((int)(duration / 0.1f))
                    .setOnComplete(() => targetTransform.localPosition = originalPos));
        }

        /// <summary>
        /// Efecto de rebote vertical (bounce loop)
        /// </summary>
        public void BounceLoop(float height, float duration)
        {
            Vector3 startPos = targetTransform.localPosition;
            SafeTween("specialMove",
                LeanTween.moveLocalY(targetTransform.gameObject, startPos.y + height, duration)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setLoopPingPong());
        }

        /// <summary>
        /// Efecto de flotación suave
        /// </summary>
        public void FloatLoop(float height, float duration)
        {
            Vector3 startPos = targetTransform.localPosition;
            SafeTween("specialMove",
                LeanTween.moveLocalY(targetTransform.gameObject, startPos.y + height, duration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong());
        }

        /// <summary>
        /// Efecto de balanceo (swing)
        /// </summary>
        public void SwingLoop(float angle, float duration)
        {
            Vector3 startRotation = targetTransform.localEulerAngles;
            SafeTween("specialRotate",
                LeanTween.rotateZ(targetTransform.gameObject, startRotation.z + angle, duration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong());
        }

        /// <summary>
        /// Animación de pulso continuo
        /// </summary>
        public void PulseLoop(float scaleMultiplier, float duration)
        {
            Vector3 targetScale = originalScale * scaleMultiplier;
            SafeTween("specialScale",
                LeanTween.scale(targetTransform.gameObject, targetScale, duration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong());
        }

        #endregion

        #region Métodos de Control

        /// <summary>
        /// Cancela todas las animaciones en este objeto
        /// </summary>
        public void CancelAllAnimations()
        {
            LeanTween.cancel(targetTransform.gameObject);
        }

        /// <summary>
        /// Pausa todas las animaciones
        /// </summary>
        public void PauseAllAnimations()
        {
            LeanTween.pause(targetTransform.gameObject);
        }

        /// <summary>
        /// Reanuda todas las animaciones
        /// </summary>
        public void ResumeAllAnimations()
        {
            LeanTween.resume(targetTransform.gameObject);
        }

        /// <summary>
        /// Resetea el objeto a sus valores originales instantáneamente
        /// </summary>
        public void ResetToOriginal()
        {
            CancelAllAnimations();
            targetTransform.localPosition = originalPosition;
            targetTransform.localScale = originalScale;
            targetTransform.localRotation = originalRotation;
        }

        /// <summary>
        /// Resetea el objeto a sus valores originales con animación
        /// </summary>
        public void ResetToOriginalAnimated(float duration)
        {
            CancelAllAnimations();
            LeanTween.moveLocal(targetTransform.gameObject, originalPosition, duration);
            LeanTween.scale(targetTransform.gameObject, originalScale, duration);
            LeanTween.rotateLocal(targetTransform.gameObject, originalRotation.eulerAngles, duration);
        }
        
        public void ResetToOriginalAnimated(LeanTweenType easing, float duration)
        {
            CancelAllAnimations();
            LeanTween.moveLocal(targetTransform.gameObject, originalPosition, duration).setEase(easing);
            LeanTween.scale(targetTransform.gameObject, originalScale, duration).setEase(easing);
            LeanTween.rotateLocal(targetTransform.gameObject, originalRotation.eulerAngles, duration).setEase(easing);
        }

        #endregion

        #region Métodos Auxiliares Públicos

        /// <summary>
        /// Establece el transform objetivo manualmente
        /// </summary>
        public void SetTargetTransform(Transform newTarget)
        {
            targetTransform = newTarget;
            StoreOriginalValues();
        }

        /// <summary>
        /// Actualiza los valores originales guardados
        /// </summary>
        public void UpdateOriginalValues()
        {
            StoreOriginalValues();
        }
        #endregion
        
        private void OnDisable()
        {
            //ResetToOriginalAnimated(0f);
        }
    }
}
