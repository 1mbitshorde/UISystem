using UnityEngine;
using UnityEngine.UI;

namespace OneM.UISystem
{
    /// <summary>
    /// Reads the right stick Y-axis from the active Gamepad to scroll a local ScrollRect component.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class GamepadScrollHandler : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [Min(0f)] public float Speed = 5f;

        private void Reset() => scrollRect = GetComponent<ScrollRect>();
        private void Update() => TryUpdateVerticalPosition();

        private void TryUpdateVerticalPosition()
        {
            var hasInput = InputSystem.InputSystem.TryGetGamepadRightStickValue(out var input);
            if (!hasInput || Mathf.Abs(input.y) < 0.1f) return;

            scrollRect.verticalNormalizedPosition += input.y * Speed * Time.unscaledDeltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }
    }
}