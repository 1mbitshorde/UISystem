using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityLocalizedString = UnityEngine.Localization.LocalizedString;

namespace OneM.UISystem
{
    /// <summary>
    /// Field for a Horizontal Carousel Dropdown.
    /// </summary>
    /// <remarks>
    /// Values can be set using an Enum type or a custom array of values.<br/>
    /// Player can navigate between values using the gamepad/keyboard left/right buttons.<br/>
    /// Available values are displayed in a horizontal carousel using small dots.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ColorTransition))]
    public sealed class DropdownField : AbstractField<string>, IClickable
    {
        [Space]
        [Tooltip("The Enum to use in the Dropdown Values. Use the fully qualified enum type name (e.g. UnityEngine.RuntimePlatform)")]
        [SerializeField] private string enumTypeName;
        [SerializeField] private Label displayValue;
        [SerializeField] private DropdownDots dots;

        [Header("BUTTONS")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [Header("LOCALIZATION")]
        [SerializeField, Tooltip("[Optional] The Localization Keys where the Dropdown values are stored.")]
        private UnityLocalizedString[] localizationKeys;

        public event Action OnClicked;

        public int CurrentIndex { get; private set; }

        private Array enumValues;
        private AudioHandler audioHandler;

        protected override void Awake()
        {
            base.Awake();
            if (IsRunning()) FindComponents();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsRunning()) SubscribeEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (IsRunning()) UnsubscribeEvents();
        }

        #region MOVE
        public void MoveLeft() => Move(-1);
        public void MoveRight() => Move(1);

        public void Move(int direction)
        {
            if (enumValues.Length == 0) return;

            CurrentIndex = (CurrentIndex + direction + enumValues.Length) % enumValues.Length;
            UpdateDisplay();

            if (audioHandler) audioHandler.PlayTabSelection();
        }

        private void SubscribeEvents()
        {
            leftButton.onClick.AddListener(HandleLeftButtonClicked);
            rightButton.onClick.AddListener(HandleRightButtonClicked);
        }

        private void UnsubscribeEvents()
        {
            leftButton.onClick.RemoveListener(HandleLeftButtonClicked);
            rightButton.onClick.RemoveListener(HandleRightButtonClicked);
        }

        private void HandleLeftButtonClicked() => MoveLeft();
        private void HandleRightButtonClicked() => MoveRight();
        #endregion

        #region ENUM
        public bool HasEnumSource() => !string.IsNullOrEmpty(enumTypeName);
        public bool HasEnumValues() => enumValues != null && enumValues.Length > 0;
        public bool TryGetValue<TEnum>(out TEnum value) where TEnum : struct => Enum.TryParse(Value, out value);
        public string GetCurrentValue() => HasEnumValues() ? enumValues.GetValue(CurrentIndex).ToString() : string.Empty;

        /// <summary>
        /// Set the Dropbox values using the given enum type.
        /// </summary>
        /// <param name="enumType">
        /// The Enum to use in the Dropdown Values. Use the enum type full name (e.g. UnityEngine.RuntimePlatform)
        /// </param>
        public void SetEnumSource(string enumType)
        {
            var isEnum = TryGetEnumType(enumType, out Type type);
            if (!isEnum)
            {
                Debug.LogError($"Could not load enum '{enumType}'. Ensure you are using the Full Name (Namespace.TypeName).");
                return;
            }

            SetValues(Enum.GetValues(type));
        }

        /// <summary>
        /// <inheritdoc cref="SetEnumSource(string)"/>
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        public void SetEnumSource<T>() where T : Enum
        {
            var type = typeof(T);
            SetValues(Enum.GetValues(type));
        }

        /// <summary>
        /// Set the Dropbox values using the given values.
        /// </summary>
        /// <param name="values">The Dropdown values.</param>
        public void SetValues(Array values)
        {
            dots.Destroy();
            CurrentIndex = 0;
            enumValues = values;

            dots.Spawn(enumValues.Length);
            UpdateDisplay();
        }

        public void Clear()
        {
            dots.Destroy();
            CurrentIndex = 0;
            enumValues = Array.Empty<string>();
            displayValue.Text = string.Empty;
        }

        private bool TryGetEnumType(string typeName, out Type type)
        {
            type = Type.GetType(typeName) ??
                AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(typeName))
                    .FirstOrDefault(t => t != null);
            return type?.IsEnum == true;
        }
        #endregion

        #region INTERFACES
        // Triggered when Mouse clicks on it
        public void OnPointerClick(PointerEventData evt)
        {
            var isLeftClick = evt.button == PointerEventData.InputButton.Left;
            if (isLeftClick) Press();
        }

        // Triggered when Gamepad/Keyboard submits or Mouse clicks on it
        public void OnSubmit(BaseEventData _) => Press();

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    MoveRight();
                    break;

                case MoveDirection.Left:
                    MoveLeft();
                    break;
            }
        }

        private void Press()
        {
            if (IsAvailable()) OnClicked?.Invoke();
        }
        #endregion

        #region INITIALIZATION
        protected override void SetupInput() => dots = GetComponentInChildren<DropdownDots>();

        private void FindComponents()
        {
            if (HasEnumSource()) SetEnumSource(enumTypeName);
            audioHandler = GetComponentInParent<AudioHandler>();
        }

        private void UpdateDisplay()
        {
            Value = GetCurrentValue();
            dots.Select(CurrentIndex);

            var hasLocalization = TryGetCurrentLocalization(out var localization);
            if (hasLocalization) displayValue.UpdateLocalization(localization);
            else
            {
                displayValue.ClearLocalization();
                displayValue.Text = Value;
            }
        }

        private bool TryGetCurrentLocalization(out UnityLocalizedString localization)
        {
            var hasLocalization = localizationKeys.Length > CurrentIndex;
            localization = hasLocalization ? localizationKeys[CurrentIndex] : null;
            return localization != null;
        }
        #endregion
    }
}