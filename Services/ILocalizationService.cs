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
                ["about_by"] = "Socratic",
                ["version_label"] = "Versión",
                // Contacto
                ["contact"] = "Contacto",
                ["contact_hint"] = "Toca para enviar un correo electrónico",
                // Apoyo
                ["support_development"] = "Apoya el Desarrollo",
                ["support_button"] = "Ko-fi.com - Invítame un café",
                ["support_description"] = "Tu apoyo ayuda a mantener y mejorar la aplicación",
                // Idioma
                ["language"] = "Idioma",
                ["language_es"] = "🇪🇸 Español",
                ["language_en"] = "🇺🇸 English",
                ["select_language"] = "Selecciona tu idioma preferido",
                // Privacidad
                ["privacy_title"] = "Privacidad",
                ["privacy_text"] = "Esta aplicación no recopila tus datos personales ni los envía a los desarrolladores. La información se procesa en tu dispositivo para la función propia de la app.",
                // Licencia
                ["license_title"] = "Licencia",
                ["license_text"] = "Esta aplicación es software libre distribuido bajo licencia MIT.",
                ["license_line"] = "MIT License · Copyright © 2026 Socratic",
                // Aviso Legal
                ["legal_notice"] = "Aviso Legal",
                ["legal_text_1"] = "Este software se proporciona «tal cual», sin garantías de ningún tipo. El usuario es responsable del uso adecuado de la aplicación y del cumplimiento de las leyes locales.",
                ["legal_text_2"] = "En ningún caso los autores serán responsables de daños directos, indirectos, incidentales o consecuentes que resulten del uso de este software.",
                ["legal_warning"] = "⚠️ Uso bajo su propio riesgo",
                ["back"] = "← Volver",
                ["email_error"] = "Cliente de correo no disponible en este dispositivo",
                ["email_error_message"] = "No se pudo abrir el cliente de correo",
                ["browser_not_available"] = "Navegador no disponible",
                ["link_copied"] = "Enlace copiado al portapapeles",
                ["language_selected"] = "Idioma actualizado"
            },
            ["en"] = new Dictionary<string, string>
            {
                ["about_title"] = "About",
                ["app_description"] = "Your companion to reduce cigarette consumption gradually and effectively",
                ["about_by"] = "Socratic",
                ["version_label"] = "Version",
                // Contact
                ["contact"] = "Contact",
                ["contact_hint"] = "Tap to send an email",
                // Support
                ["support_development"] = "Support Development",
                ["support_button"] = "Ko-fi.com - Buy me a coffee",
                ["support_description"] = "Your support helps maintain and improve the app",
                // Language
                ["language"] = "Language",
                ["language_es"] = "🇪🇸 Español",
                ["language_en"] = "🇺🇸 English",
                ["select_language"] = "Select your preferred language",
                // Privacy
                ["privacy_title"] = "Privacy",
                ["privacy_text"] = "This app does not collect your personal data or send it to the developers. Information is processed on your device for the app's own purpose.",
                // License
                ["license_title"] = "License",
                ["license_text"] = "This app is free software distributed under the MIT license.",
                ["license_line"] = "MIT License · Copyright © 2026 Socratic",
                // Legal Notice
                ["legal_notice"] = "Legal Notice",
                ["legal_text_1"] = "This software is provided 'as is', without warranty of any kind. The user is responsible for proper use of the app and compliance with local laws.",
                ["legal_text_2"] = "In no event shall the authors be liable for any direct, indirect, incidental or consequential damages arising from the use of this software.",
                ["legal_warning"] = "⚠️ Use at your own risk",
                ["back"] = "← Back",
                ["email_error"] = "Email client not available on this device",
                ["email_error_message"] = "Could not open email client",
                ["browser_not_available"] = "Browser not available",
                ["link_copied"] = "Link copied to clipboard",
                ["language_selected"] = "Language updated"
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