using WindowSill.API;

namespace WindowSill.UnitConverter.Exchange;

public sealed partial class ExchangeSettingsView : UserControl
{
    private readonly ISettingsProvider _settingsProvider;

    public ExchangeSettingsView(ISettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
        ViewModel = new ExchangeSettingsViewModel(settingsProvider);

        InitializeComponent();
    }

    internal ExchangeSettingsViewModel ViewModel { get; }

    private void MoveUpButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is CurrencyValue currencyValue)
        {
            ViewModel.MoveCurrencyUp(currencyValue);
        }
    }

    private void MoveDownButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is CurrencyValue currencyValue)
        {
            ViewModel.MoveCurrencyDown(currencyValue);
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is CurrencyValue currencyValue)
        {
            ViewModel.DeleteCurrency(currencyValue);
        }
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = this.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "/WindowSill.UnitConverter/Exchange/AddFavoriteTitle".GetLocalizedString(),
            PrimaryButtonText = "/WindowSill.UnitConverter/Exchange/Ok".GetLocalizedString(),
            CloseButtonText = "/WindowSill.UnitConverter/Exchange/Cancel".GetLocalizedString(),
            DefaultButton = ContentDialogButton.Primary,
            IsPrimaryButtonEnabled = false
        };

        dialog.Content
            = new ExchangeAddFavoriteCurrencyContentDialog(
                dialog,
                _settingsProvider.GetSetting(ExchangeSettings.FavoriteIsoCurrencyCodes) ?? [],
                (await ExchangeRateHelper.GetCurrencyNameMapAsync())!);

        await dialog.ShowAsync();

        if (dialog.Tag is CurrencyValue selectedCurrency)
        {
            ViewModel.FavoriteIsoCurrencyCodes.Add(selectedCurrency);
            ViewModel.SaveSettings();
        }
    }
}
