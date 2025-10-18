using UltEvents;
using UnityEngine;

namespace CursedOnion.UI.Buttons
{
    public class UIButton : MonoBehaviour
    {
        public void InvokeOnClick() => OnClick.Invoke();
        [SerializeField] UltEvent OnClick;
    }
}
