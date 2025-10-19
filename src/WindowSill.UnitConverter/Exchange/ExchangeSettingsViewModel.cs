using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;

namespace WindowSill.UnitConverter.Exchange;

internal sealed partial class ExchangeSettingsViewModel : ObservableObject
{
    private readonly ISettingsProvider _settingsProvider;

    internal ExchangeSettingsViewModel(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;

        IsLoading = true;
        LoadFavoriteCurrenciesAsync().ForgetSafely();
    }

    internal ObservableCollection<CurrencyValue> FavoriteIsoCurrencyCodes { get; } = [];

    [ObservableProperty]
    internal partial bool IsLoading { get; set; }

    internal void MoveCurrencyUp(CurrencyValue currencyValue)
    {
        int currentIndex = FavoriteIsoCurrencyCodes.IndexOf(currencyValue);
        if (currentIndex > 0)
        {
            FavoriteIsoCurrencyCodes.RemoveAt(currentIndex);
            FavoriteIsoCurrencyCodes.Insert(currentIndex - 1, currencyValue);
        }

        SaveSettings();
    }

    internal void MoveCurrencyDown(CurrencyValue currencyValue)
    {
        int currentIndex = FavoriteIsoCurrencyCodes.IndexOf(currencyValue);
        if (currentIndex >= 0 && currentIndex < FavoriteIsoCurrencyCodes.Count - 1)
        {
            FavoriteIsoCurrencyCodes.RemoveAt(currentIndex);
            FavoriteIsoCurrencyCodes.Insert(currentIndex + 1, currencyValue);
        }

        SaveSettings();
    }

    internal void DeleteCurrency(CurrencyValue currencyValue)
    {
        FavoriteIsoCurrencyCodes.Remove(currencyValue);
        SaveSettings();
    }

    internal void SaveSettings()
    {
        string[] newOrdering = FavoriteIsoCurrencyCodes.Select(s => s.IsoCurrency).ToArray();
        _settingsProvider.SetSetting(ExchangeSettings.FavoriteIsoCurrencyCodes, newOrdering);
    }

    private async Task LoadFavoriteCurrenciesAsync()
    {
        await Task.Run(async () =>
        {
            string[] favoriteCurrencyCodes = _settingsProvider.GetSetting(ExchangeSettings.FavoriteIsoCurrencyCodes) ?? [];

            var loadedCurrencies = new List<CurrencyValue>();
            for (int i = 0; i < favoriteCurrencyCodes.Length; i++)
            {
                string isoCode = favoriteCurrencyCodes[i];
                string? currencyName = await ExchangeRateHelper.GetCurrencyNameAsync(isoCode);
                loadedCurrencies.Add(new CurrencyValue(1.0, currencyName ?? isoCode, isoCode.ToUpper()));
            }

            await ThreadHelper.RunOnUIThreadAsync(async () =>
            {
                FavoriteIsoCurrencyCodes.Clear();
                foreach (CurrencyValue currency in loadedCurrencies)
                {
                    FavoriteIsoCurrencyCodes.Add(currency);
                }

                if ((await ExchangeRateHelper.GetCurrencyNameMapAsync()) is not null)
                {
                    IsLoading = false;
                }
            });
        });
    }
}
