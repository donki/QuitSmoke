using QuitSmoke.Helpers;
using QuitSmoke.Services;
using QuitSmoke.ViewModels;

namespace QuitSmoke.Views;

/// <summary>Historico. Las etiquetas se enlazan a <see cref="HistoryViewModel"/>.</summary>
public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _vm;

    public HistoryPage()
    {
        InitializeComponent();
        BindingContext = _vm = new HistoryViewModel(ServiceHelper.GetService<ISmokingDataService>(),
            ServiceHelper.GetService<ILocalizationService>());
        _ = _vm.LoadAsync(new ModernDialogs(this));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync(new ModernDialogs(this));
    }

    private async void OnRefreshClicked(object sender, EventArgs e) => await _vm.LoadAsync(new ModernDialogs(this));
}
