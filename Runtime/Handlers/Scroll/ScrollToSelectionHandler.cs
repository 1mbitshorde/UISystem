using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneM.UISystem
{
    /// <summary>
    /// Automatically scrolls a ScrollRect content area to keep the currently focused 
    /// UI element visible when navigating with a gamepad, keyboard or mouse wheel.
    /// </summary>
    /// <remarks>
    /// Put this component inside the Scroll Rect Content Game Object.
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class ScrollToSelectionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField, Min(0f)] private float scrollSpeed = 10f;

        private RectTransform scrollRectTransform;
        private Vector2 targetPosition;
        private bool isPointerOver;

        private void Reset()
        {
            content = GetComponent<RectTransform>();
            scrollRect = GetComponentInParent<ScrollRect>();
        }

        private void Awake()
        {
            targetPosition = content.anchoredPosition;
            scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        }

        private void Update()
        {
            HandleMouseWheel();
            TryKeepTargetInView();
            UpdateAnchoredPosition();
        }

        public void OnPointerEnter(PointerEventData _) => isPointerOver = true;
        public void OnPointerExit(PointerEventData _) => isPointerOver = false;

        private void HandleMouseWheel()
        {
            if (!isPointerOver || scrollRect == null) return;

            var hasScroll = InputSystem.InputSystem.TryGetMouseScrollValue(out var scrollValue);
            if (!hasScroll) return;

            var verticalScroll = Mathf.Abs(scrollValue.y);
            if (verticalScroll > 0.01f) targetPosition = content.anchoredPosition;
        }

        private void TryKeepTargetInView()
        {
            if (!IsScrollNeeded() || isPointerOver) return;

            var current = EventSystem.current.currentSelectedGameObject;
            if (current == null || !current.transform.IsChildOf(content)) return;

            var hasTarget = current.TryGetComponent<RectTransform>(out var target);
            if (hasTarget) UpdateTargetPosition(target);
        }

        private bool IsScrollNeeded()
        {
            var viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRectTransform;
            return content.rect.height > viewport.rect.height;
        }

        private void UpdateAnchoredPosition()
        {
            var isSmallDistance = Vector2.Distance(content.anchoredPosition, targetPosition) < 0.01f;
            if (isSmallDistance)
            {
                content.anchoredPosition = targetPosition;
                return;
            }

            content.anchoredPosition = Vector2.Lerp(
                content.anchoredPosition,
                targetPosition,
                Time.unscaledDeltaTime * scrollSpeed
            );
        }

        private void UpdateTargetPosition(RectTransform target)
        {
            var targetBounds = GetBoundsInContentSpace(target);
            var viewportBounds = GetViewportBoundsInContentSpace();

            if (targetBounds.max.y > viewportBounds.max.y)
            {
                var diff = targetBounds.max.y - viewportBounds.max.y;
                targetPosition -= new Vector2(0, diff);
            }
            else if (targetBounds.min.y < viewportBounds.min.y)
            {
                var diff = viewportBounds.min.y - targetBounds.min.y;
                targetPosition += new Vector2(0, diff);
            }
        }

        private Bounds GetBoundsInContentSpace(RectTransform target)
        {
            var targetCorners = new Vector3[4];
            target.GetWorldCorners(targetCorners);

            var min = Vector3.positiveInfinity;
            var max = Vector3.negativeInfinity;

            foreach (var corner in targetCorners)
            {
                var local = content.InverseTransformPoint(corner);
                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }

            return new Bounds((min + max) * 0.5f, max - min);
        }

        private Bounds GetViewportBoundsInContentSpace()
        {
            var viewport = scrollRect.viewport != null ? scrollRect.viewport : scrollRectTransform;
            var viewportCorners = new Vector3[4];
            var min = Vector3.positiveInfinity;
            var max = Vector3.negativeInfinity;

            viewport.GetWorldCorners(viewportCorners);

            foreach (var corner in viewportCorners)
            {
                var local = content.InverseTransformPoint(corner);
                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }

            return new Bounds((min + max) * 0.5f, max - min);
        }
    }
}