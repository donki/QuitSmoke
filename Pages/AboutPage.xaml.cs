using QuitSmoke.Helpers;
using QuitSmoke.Services;
using QuitSmoke.ViewModels;
using QuitSmoke.Views;

namespace QuitSmoke.Pages
{
    /// <summary>Acerca de. Las etiquetas se enlazan a <see cref="AboutViewModel"/>.</summary>
    public partial class AboutPage : ContentPage
    {
        private readonly AboutViewModel _vm;

        public AboutPage()
        {
            InitializeComponent();
            // En Android, el correo nativo; en el resto, el de MAUI Essentials.
            IEmailService email = new EssentialsEmailService();
#if ANDROID
            email = ServiceHelper.GetService<IEmailService>();
#endif
            BindingContext = _vm = new AboutViewModel(ServiceHelper.GetService<ILocalizationService>(),
                AppInfo.Current.VersionString, email);
            HighlightActiveLanguage();
        }

        private void HighlightActiveLanguage()
        {
            var primary = (Style)Application.Current!.Resources["PrimaryButton"];
            var outline = (Style)Application.Current!.Resources["OutlineButton"];
            SpanishButton.Style = _vm.SpanishActive ? primary : outline;
            EnglishButton.Style = _vm.EnglishActive ? primary : outline;
        }

        private async void OnContactEmailClicked(object? sender, EventArgs e) => await _vm.ContactAsync(new ModernDialogs(this));

        private async void OnSpanishClicked(object? sender, EventArgs e) => await SetLanguageAsync("es");

        private async void OnEnglishClicked(object? sender, EventArgs e) => await SetLanguageAsync("en");

        private async Task SetLanguageAsync(string code)
        {
            var done = _vm.SetLanguageAsync(code, new ModernDialogs(this));
            HighlightActiveLanguage();
            await done;
        }
    }
}
