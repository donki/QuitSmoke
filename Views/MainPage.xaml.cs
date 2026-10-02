using QuitSmoke.Helpers;
using QuitSmoke.Services;
using QuitSmoke.ViewModels;

namespace QuitSmoke.Views;

/// <summary>Inicio. Las etiquetas se enlazan a <see cref="MainViewModel"/>, donde esta la logica.</summary>
public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    private readonly ModernDialogs _dialogs;

    public MainPage()
    {
        InitializeComponent();
        _dialogs = new ModernDialogs(this);
        BindingContext = _vm = new MainViewModel(ServiceHelper.GetService<ISmokingDataService>(),
            ServiceHelper.GetService<INotificationService>(), ServiceHelper.GetService<ILocalizationService>());
        _ = _vm.LoadAsync(_dialogs);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync(_dialogs);

        // §15: comprobacion de version al arrancar, no bloqueante y silenciosa.
        _ = ServiceHelper.GetService<UpdateService>().CheckAndPromptAsync(_dialogs);
    }

    private async void OnSmokeClicked(object sender, EventArgs e) => await _vm.SmokeAsync(_dialogs);

    private async void OnRefreshClicked(object sender, EventArgs e) => await _vm.RefreshAsync(_dialogs);
}
