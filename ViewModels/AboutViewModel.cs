using QuitSmoke.Services;

namespace QuitSmoke.ViewModels;

/// <summary>Acerca de: version, contacto, idioma, privacidad, licencia y aviso legal.</summary>
public sealed class AboutViewModel : ViewModelBase
{
    public const string ContactEmail = "jsoladelarosa@gmail.com";
    public const string AppName = "QuitSmoke";

    private readonly string _version;
    private readonly IEmailService _email;

    public AboutViewModel(ILocalizationService loc, string version, IEmailService email) : base(loc)
    {
        _version = version;
        _email = email;
    }

    public string Title => L("about_title");
    public string Version => $"{L("version_label")} {_version}";
    public string Description => L("app_description");
    public string ContactTitle => L("contact");
    public string ContactHint => L("contact_hint");
    public string LanguageTitle => L("language");
    public string SpanishText => L("language_es");
    public string EnglishText => L("language_en");
    public string LanguageHint => L("select_language");
    public string PrivacyTitle => L("privacy_title");
    public string PrivacyText => L("privacy_text");
    public string LicenseTitle => L("license_title");
    public string LicenseText => L("license_text");
    public string LicenseLine => L("license_line");
    public string LegalTitle => L("legal_notice");
    public string LegalText1 => L("legal_text_1");
    public string LegalText2 => L("legal_text_2");
    public string LegalWarning => L("legal_warning");

    public bool SpanishActive => Loc.GetCurrentLanguage() == "es";
    public bool EnglishActive => Loc.GetCurrentLanguage() == "en";

    /// <summary>Cambia el idioma, repinta los textos y avisa de que ya esta.</summary>
    public async Task SetLanguageAsync(string code, IUserDialogs dialogs)
    {
        Loc.SetLanguage(code);
        RaiseAllChanged();
        await dialogs.AlertAsync(L("language"), L("language_selected"), L("ok"));
    }

    /// <summary>Correo de contacto con asunto y cuerpo; si no hay cliente de correo, lo dice.</summary>
    public async Task ContactAsync(IUserDialogs dialogs)
    {
        try
        {
            await _email.SendEmailAsync(ContactEmail, L("email_subject"), string.Format(L("email_body"), AppName));
        }
        catch (FeatureNotSupportedException)
        {
            await dialogs.AlertAsync(L("error"), L("email_error"), L("ok"));
        }
        catch (Exception ex)
        {
            await dialogs.AlertAsync(L("error"), $"{L("email_error_message")}: {ex.Message}", L("ok"));
        }
    }
}
