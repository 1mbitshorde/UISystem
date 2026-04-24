using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneM.UISystem
{
    /// <summary>
    /// Field for a text input.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TextField : AbstractField<string>
    {
        [Space]
        [SerializeField] private TMP_InputField input;

        /// <summary>
        /// Event fired when the Input Field submits its data.
        /// </summary>
        public event Action<string> OnSubmitted;

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

        protected override void SetupInput()
        {
            input = GetComponentInChildren<TMP_InputField>();
            input.interactable = true;
            input.transition = Transition.None;
            input.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (IsAvailable()) input.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (IsAvailable()) input.OnDeselect(eventData);
        }

        private void SubscribeEvents()
        {
            input.onSubmit.AddListener(HandleInputSubmit);
            input.onValueChanged.AddListener(HandleInputValueChanged);
        }

        private void UnsubscribeEvents()
        {
            input.onSubmit.RemoveListener(HandleInputSubmit);
            input.onValueChanged.RemoveListener(HandleInputValueChanged);
        }

        private void HandleInputValueChanged(string value) => Value = value;
        private void HandleInputSubmit(string value) => OnSubmitted?.Invoke(value);
    }
}