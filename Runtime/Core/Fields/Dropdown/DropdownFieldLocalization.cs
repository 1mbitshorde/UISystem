using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
            Select(locale);
        }

        private async void PopulateLanguages()
        {
            locales = await GetLocalesAsync();
            var selectedIndex = GetLocaleIndex(locales);
            var localesNames = locales.Select(locale => GetDisplayName(locale)).ToArray();

            dropdown.SetValues(localesNames);
            dropdown.SetValueWithoutNotify(selectedIndex);
        }

        private static void Select(Locale locale) =>
            LocalizationSettings.SelectedLocale = locale;

        private static async Awaitable<List<Locale>> GetLocalesAsync()
        {
            await LocalizationSettings.SelectedLocaleAsync.Task;
            return LocalizationSettings.AvailableLocales.Locales;
        }

        private static int GetLocaleIndex(List<Locale> locales)
        {
            var currentLocale = LocalizationSettings.SelectedLocale;
            return locales.FindIndex(locale => locale == currentLocale);
        }

        private static string GetDisplayName(Locale locale)
        {
            var name = locale.Identifier.CultureInfo != null ?
                locale.Identifier.CultureInfo.NativeName :
                locale.ToString();

            TryCaptalizeFirstLetter(ref name);

            return name;
        }

        private static void TryCaptalizeFirstLetter(ref string text)
        {
            if (text.Length > 1) text = char.ToUpper(text[0]) + text[1..];
        }
    }
}
