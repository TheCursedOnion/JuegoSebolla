using UltEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CursedOnion.Game.General.UI
{
    public class UIInteractiveElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("Eventos de Puntero")]
        [Tooltip("Se invoca cuando el puntero entra en el área del objeto")]
        public UltEvent OnPointerEnterEvent;
        
        [Tooltip("Se invoca cuando el puntero sale del área del objeto")]
        public UltEvent OnPointerExitEvent;
        
        [Tooltip("Se invoca cuando se presiona el botón del mouse sobre el objeto")]
        public UltEvent OnPointerDownEvent;
        
        [Tooltip("Se invoca cuando se suelta el botón dentro del objeto (donde se presionó)")]
        public UltEvent OnPointerClickInsideEvent;
        
        [Tooltip("Se invoca cuando se suelta el botón fuera del objeto")]
        public UltEvent OnPointerUpOutsideEvent;
        
        [Tooltip("Se invoca cuando se suelta el botón (dentro o fuera)")]
        public UltEvent OnPointerUpEvent;
            
        protected bool isPointerInside = false;
        protected bool isPressed = false;
        
        /// <summary>
        /// Cuando el puntero entra en el área del objeto
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            OnPointerEnterEvent?.Invoke();
            
            Debug.Log($"[PointerEventDetector] Puntero entró en: {gameObject.name}");
        }

        /// <summary>
        /// Cuando el puntero sale del área del objeto
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            OnPointerExitEvent?.Invoke();
            
            Debug.Log($"[PointerEventDetector] Puntero salió de: {gameObject.name}");
        }

        /// <summary>
        /// Cuando se presiona el botón del mouse sobre el objeto
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            OnPointerDownEvent?.Invoke();
            
            Debug.Log($"[PointerEventDetector] Botón presionado en: {gameObject.name}");
        }

        /// <summary>
        /// Cuando se suelta el botón del mouse (dentro o fuera)
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            
            // Verificar si se soltó dentro o fuera del objeto
            if (isPointerInside)
            {
                Debug.Log($"[PointerEventDetector] Botón soltado DENTRO de: {gameObject.name}");
            }
            else
            {
                OnPointerUpOutsideEvent?.Invoke();
                Debug.Log($"[PointerEventDetector] Botón soltado FUERA de: {gameObject.name}");
            }
            
            OnPointerUpEvent?.Invoke();
        }

        /// <summary>
        /// Cuando se completa un click (presionar y soltar dentro del mismo objeto)
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickInsideEvent?.Invoke();
            
            Debug.Log($"[PointerEventDetector] Click completado en: {gameObject.name}");
        }

        /// <summary>
        /// Método público para verificar si el puntero está dentro
        /// </summary>
        public bool IsPointerInside() => isPointerInside;

        /// <summary>
        /// Método público para verificar si está presionado
        /// </summary>
        public bool IsPressed() => isPressed;

    }
}
