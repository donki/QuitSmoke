using System.Globalization;

namespace QuitSmoke.Services;

public interface ILocalizationService
{
    string GetCurrentLanguage();
    void SetLanguage(string languageCode);
    string GetString(string key);
}

public class LocalizationService : ILocalizationService
{
    private string _currentLanguage;
    private readonly Dictionary<string, Dictionary<string, string>> _translations;

    public LocalizationService()
    {
        // Detectar idioma del sistema
        _currentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        
        // Si no es español, usar inglés por defecto
        if (_currentLanguage != "es")
            _currentLanguage = "en";

        _translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["es"] = new Dictionary<string, string>
            {
                ["about_title"] = "Acerca de",
                ["app_description"] = "Tu compañero para reducir el consumo de cigarrillos de forma gradual y efectiva",
                ["developed_with_love"] = "Desarrollado con ❤️ para tu salud",
                ["contact"] = "Contacto",
                ["contact_instruction"] = "Toca para enviar un correo con tus comentarios o sugerencias",
                ["support_development"] = "Apoya el Desarrollo",
                ["support_description"] = "Tu apoyo ayuda a mantener y mejorar la aplicación",
                ["legal_notice"] = "Aviso Legal",
                ["legal_text_1"] = "Esta aplicación es una herramienta de apoyo para reducir el consumo de cigarrillos. No sustituye el consejo médico profesional.",
                ["legal_text_2"] = "Para dejar de fumar completamente, se recomienda consultar con profesionales de la salud especializados en cesación tabáquica.",
                ["legal_warning"] = "⚠️ Esta app es una herramienta de apoyo, no un tratamiento médico",
                ["language"] = "Idioma / Language",
                ["select_language"] = "Selecciona tu idioma preferido",
                ["back"] = "← Volver",
                ["email_error"] = "Cliente de correo no disponible en este dispositivo",
                ["email_error_message"] = "No se pudo abrir el cliente de correo",
                ["browser_not_available"] = "Navegador no disponible",
                ["link_copied"] = "Enlace copiado al portapapeles",
                ["language_selected"] = "Español seleccionado (funcionalidad en desarrollo)"
            },
            ["en"] = new Dictionary<string, string>
            {
                ["about_title"] = "About",
                ["app_description"] = "Your companion to reduce cigarette consumption gradually and effectively",
                ["developed_with_love"] = "Developed with ❤️ for your health",
                ["contact"] = "Contact",
                ["contact_instruction"] = "Tap to send an email with your comments or suggestions",
                ["support_development"] = "Support Development",
                ["support_description"] = "Your support helps maintain and improve the app",
                ["legal_notice"] = "Legal Notice",
                ["legal_text_1"] = "This application is a support tool to reduce cigarette consumption. It does not replace professional medical advice.",
                ["legal_text_2"] = "To quit smoking completely, it is recommended to consult with health professionals specialized in tobacco cessation.",
                ["legal_warning"] = "⚠️ This app is a support tool, not a medical treatment",
                ["language"] = "Language / Idioma",
                ["select_language"] = "Select your preferred language",
                ["back"] = "← Back",
                ["email_error"] = "Email client not available on this device",
                ["email_error_message"] = "Could not open email client",
                ["browser_not_available"] = "Browser not available",
                ["link_copied"] = "Link copied to clipboard",
                ["language_selected"] = "English selected (feature in development)"
            }
        };
    }

    public string GetCurrentLanguage() => _currentLanguage;

    public void SetLanguage(string languageCode)
    {
        if (_translations.ContainsKey(languageCode))
        {
            _currentLanguage = languageCode;
            Preferences.Set("app_language", languageCode);
        }
    }

    public string GetString(string key)
    {
        // Cargar idioma guardado si existe
        var savedLanguage = Preferences.Get("app_language", _currentLanguage);
        if (_translations.ContainsKey(savedLanguage))
            _currentLanguage = savedLanguage;

        if (_translations.ContainsKey(_currentLanguage) && _translations[_currentLanguage].ContainsKey(key))
            return _translations[_currentLanguage][key];

        // Fallback a inglés si no se encuentra la clave
        if (_translations.ContainsKey("en") && _translations["en"].ContainsKey(key))
            return _translations["en"][key];

        return key; // Devolver la clave si no se encuentra traducción
    }
}