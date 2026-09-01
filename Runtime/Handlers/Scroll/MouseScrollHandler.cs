using UnityEngine;
using UnityEngine.UI;

namespace OneM.UISystem
{
    /// <summary>
    /// Reads the Vertical Wheel from the active Mouse to scroll a local ScrollRect component.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class MouseScrollHandler : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [Min(0f)] public float Speed = 15f;

        private void Reset() => scrollRect = GetComponent<ScrollRect>();
        private void Update() => TryUpdateVerticalPosition();

        private void TryUpdateVerticalPosition()
        {
            var hasInput = InputSystem.InputSystem.TryGetMouseScrollValue(out var input);
            if (!hasInput || Mathf.Approximately(input.y, 0f)) return;

            var currentScroll = scrollRect.verticalNormalizedPosition;
            var newScroll = currentScroll + (input.y * Speed * Time.unscaledDeltaTime);

            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newScroll);
        }
    }
}