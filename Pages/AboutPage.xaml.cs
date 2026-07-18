using QuitSmoke.Services;
using QuitSmoke.Helpers;

namespace QuitSmoke.Pages
{
    public partial class AboutPage : ContentPage
    {
        // CONFIGURACIÓN
        private const string ContactEmail = "jsoladelarosa@gmail.com";
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
            string L(string key) => _localizationService.GetString(key);

            Title = L("about_title");

            // Cabecera
            VersionLabel.Text = $"{L("version_label")} {AppInfo.Current.VersionString}";
            DescriptionLabel.Text = L("app_description");

            // Contacto
            ContactTitleLabel.Text = L("contact");
            ContactHintLabel.Text = L("contact_hint");

            // Apoyo
            SupportTitleLabel.Text = L("support_development");
            SupportButton.Text = L("support_button");
            SupportHintLabel.Text = L("support_description");

            // Idioma
            LanguageTitleLabel.Text = L("language");
            SpanishButton.Text = L("language_es");
            EnglishButton.Text = L("language_en");
            LanguageHintLabel.Text = L("select_language");
            HighlightActiveLanguage();

            // Privacidad
            PrivacyTitleLabel.Text = L("privacy_title");
            PrivacyTextLabel.Text = L("privacy_text");

            // Licencia
            LicenseTitleLabel.Text = L("license_title");
            LicenseTextLabel.Text = L("license_text");
            LicenseLineLabel.Text = L("license_line");

            // Aviso Legal
            LegalTitleLabel.Text = L("legal_notice");
            LegalText1Label.Text = L("legal_text_1");
            LegalText2Label.Text = L("legal_text_2");
            LegalWarningLabel.Text = L("legal_warning");
        }

        private void HighlightActiveLanguage()
        {
            var active = _localizationService.GetCurrentLanguage();
            var primaryStyle = (Style)Application.Current!.Resources["PrimaryButton"];
            var outlineStyle = (Style)Application.Current!.Resources["OutlineButton"];

            SpanishButton.Style = active == "es" ? primaryStyle : outlineStyle;
            EnglishButton.Style = active == "en" ? primaryStyle : outlineStyle;
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
                    TitleMode = BrowserTitleMode.Show
                };

                await Browser.OpenAsync(uri, browserLaunchOptions);
            }
            catch (FeatureNotSupportedException)
            {
                // Fallback: copy URL to clipboard if browser is not available
                try
                {
                    await Clipboard.SetTextAsync(DonationUrl);
                    await DisplayAlert(_localizationService.GetString("browser_not_available"),
                        $"{_localizationService.GetString("link_copied")}:\n{DonationUrl}",
                        "OK");
                }
                catch
                {
                    await DisplayAlert("Error",
                        $"{DonationUrl}",
                        "OK");
                }
            }
            catch (Exception)
            {
                // Fallback: copy URL to clipboard on any other error
                try
                {
                    await Clipboard.SetTextAsync(DonationUrl);
                    await DisplayAlert(_localizationService.GetString("browser_not_available"),
                        $"{_localizationService.GetString("link_copied")}:\n{DonationUrl}",
                        "OK");
                }
                catch
                {
                    await DisplayAlert("Error",
                        $"{DonationUrl}",
                        "OK");
                }
            }
        }

        private async void OnSpanishClicked(object? sender, EventArgs e)
        {
            _localizationService.SetLanguage("es");
            UpdateUI();
            await DisplayAlert(_localizationService.GetString("language"),
                _localizationService.GetString("language_selected"), "OK");
        }

        private async void OnEnglishClicked(object? sender, EventArgs e)
        {
            _localizationService.SetLanguage("en");
            UpdateUI();
            await DisplayAlert(_localizationService.GetString("language"),
                _localizationService.GetString("language_selected"), "OK");
        }
    }
}
