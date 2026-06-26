using QuitSmoke.Services;
using QuitSmoke.Helpers;

namespace QuitSmoke.Pages
{
    public partial class AboutPage : ContentPage
    {
        // CONFIGURACIÓN
        private const string ContactEmail = "soporte@quitsmoke.app";
        private const string DonationUrl = "https://ko-fi.com/josepsola";
        private const string EmailSubject = "Contacto desde QuitSmoke";
        private const string AppName = "QuitSmoke";
        
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService? _emailService;

        public AboutPage()
        {
            InitializeComponent();
            _localizationService = new LocalizationService();
            
            // Obtener el servicio de email si está disponible (solo en Android)
#if ANDROID
            _emailService = ServiceHelper.GetService<IEmailService>();
#endif
            UpdateUI();
        }

        private void UpdateUI()
        {
            Title = _localizationService.GetString("about_title");
            // Las actualizaciones de UI se harán mediante binding o código
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnContactEmailClicked(object? sender, EventArgs e)
        {
            try
            {
                var currentLanguage = _localizationService.GetCurrentLanguage();
                var body = currentLanguage == "es" 
                    ? $"Hola,\n\nMe pongo en contacto desde la aplicación {AppName} para:\n\n[Escribe aquí tu mensaje]\n\nGracias."
                    : $"Hello,\n\nI'm contacting you from the {AppName} application to:\n\n[Write your message here]\n\nThank you.";

                // Intentar usar el servicio nativo de Android primero
                if (_emailService != null)
                {
                    await _emailService.SendEmailAsync(ContactEmail, EmailSubject, body);
                }
                else
                {
                    // Fallback a MAUI Essentials
                    var message = new EmailMessage
                    {
                        Subject = EmailSubject,
                        To = new List<string> { ContactEmail },
                        Body = body
                    };

                    await Email.ComposeAsync(message);
                }
            }
            catch (FeatureNotSupportedException)
            {
                var errorMessage = _localizationService.GetString("email_error");
                await DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                var errorMessage = _localizationService.GetString("email_error_message");
                await DisplayAlert("Error", $"{errorMessage}: {ex.Message}", "OK");
            }
        }

        private async void OnDonationClicked(object? sender, EventArgs e)
        {
            try
            {
                var uri = new Uri(DonationUrl);
                var browserLaunchOptions = new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = Color.FromArgb("#E67E22"),
                    PreferredControlColor = Color.FromArgb("#FFFFFF")
                };

                await Browser.OpenAsync(uri, browserLaunchOptions);
            }
            catch (FeatureNotSupportedException)
            {
                // Fallback: copy URL to clipboard if browser is not available
                try
                {
                    await Clipboard.SetTextAsync(DonationUrl);
                    await DisplayAlert("Navegador no disponible", 
                        $"Enlace copiado al portapapeles:\n{DonationUrl}", 
                        "OK");
                }
                catch
                {
                    await DisplayAlert("Error", 
                        $"No se pudo abrir el navegador. Enlace: {DonationUrl}", 
                        "OK");
                }
            }
            catch (Exception ex)
            {
                // Fallback: copy URL to clipboard on any other error
                try
                {
                    await Clipboard.SetTextAsync(DonationUrl);
                    await DisplayAlert("Error al abrir enlace", 
                        $"No se pudo abrir el navegador ({ex.Message}), enlace copiado al portapapeles.", 
                        "OK");
                }
                catch
                {
                    await DisplayAlert("Error", 
                        $"No se pudo abrir el enlace: {DonationUrl}", 
                        "OK");
                }
            }
        }

        private async void OnSpanishClicked(object? sender, EventArgs e)
        {
            _localizationService.SetLanguage("es");
            UpdateUI();
            await DisplayAlert("Idioma", _localizationService.GetString("language_selected"), "OK");
        }

        private async void OnEnglishClicked(object? sender, EventArgs e)
        {
            _localizationService.SetLanguage("en");
            UpdateUI();
            await DisplayAlert("Language", _localizationService.GetString("language_selected"), "OK");
        }
    }
}