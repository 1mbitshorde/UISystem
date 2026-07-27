using OneM.LocalizationSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

namespace OneM.UISystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DropdownField))]
    public sealed class DropdownFieldLocalization : MonoBehaviour
    {
        [SerializeField] private DropdownField dropdown;

        private List<Locale> locales;

        private void Reset() => dropdown = GetComponent<DropdownField>();
        private void Awake() => PopulateLanguages();
        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        private void SubscribeEvents() => dropdown.OnValueChanged += HandleLanguageChanged;
        private void UnsubscribeEvents() => dropdown.OnValueChanged -= HandleLanguageChanged;

        private void HandleLanguageChanged(string _)
        {
            if (locales == null || locales.Count == 0) return;

            var locale = locales[dropdown.CurrentIndex];
            LocalizationManager.Select(locale);
        }

        private async void PopulateLanguages()
        {
            locales = await LocalizationManager.GetLocalesAsync();
            var selectedIndex = LocalizationManager.GetLocaleIndex(locales);
            var localesNames = locales.Select(locale => LocalizationManager.GetDisplayName(locale)).ToArray();

            dropdown.SetValues(localesNames);
            dropdown.SetValueWithoutNotify(selectedIndex);
        }
    }
}
