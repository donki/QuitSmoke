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
                ["language_selected"] = "Idioma actualizado",
                // Comunes
                ["ok"] = "OK",
                ["cancel"] = "Cancelar",
                ["error"] = "Error",
                // Comprobacion de version (§15)
                ["update_available"] = "Actualización disponible",
                ["update_message"] = "Hay una versión más reciente ({0}). Tienes la {1}.\n¿Quieres actualizar?",
                ["update_now"] = "Actualizar",
                ["update_later"] = "Ahora no",
                // Menu (flyout)
                ["nav_home"] = "Principal",
                ["nav_history"] = "Histórico",
                ["nav_settings"] = "Configuración",
                ["nav_about"] = "Acerca de",
                ["flyout_subtitle"] = "Don't Give Up",
                // MainPage
                ["main_tip_smart"] = "Consejo Smart",
                ["main_progress_title"] = "Progreso del día",
                ["main_smoked"] = "Fumados",
                ["main_remaining"] = "Restantes",
                ["main_last"] = "Último",
                ["main_next"] = "Próximo",
                ["main_time_since"] = "Sin fumar",
                ["main_smoke_button"] = "Fumar Cigarro",
                ["main_limit_reached_button"] = "Límite alcanzado",
                ["main_refresh"] = "Actualizar datos",
                ["main_daily_stats"] = "Estadísticas del día",
                ["main_time_between"] = "Entre cigarros",
                ["main_awake_hours"] = "Horas despierto",
                ["main_error_loading"] = "Error cargando datos",
                ["main_limit_title"] = "Límite alcanzado",
                ["main_limit_question"] = "Ya has fumado {0} cigarros hoy. ¿Quieres continuar?",
                ["main_limit_yes"] = "Sí, fumar",
                ["main_limit_no"] = "No, esperar",
                ["main_confirm_title"] = "Confirmación final",
                ["main_confirm_question"] = "¿Estás seguro? Esto excederá tu límite diario.",
                ["main_confirm_yes"] = "Sí, estoy seguro",
                ["main_registered_title"] = "Registrado",
                ["main_registered_message"] = "Cigarro registrado correctamente",
                ["main_error_register"] = "Error registrando cigarro",
                ["main_placeholder"] = "--",
                // SettingsPage
                ["settings_title"] = "Configuración",
                ["settings_subtitle"] = "Preferencias y permisos",
                ["settings_battery_save"] = "Ahorro batería",
                ["settings_unverified"] = "Sin verificar",
                ["settings_autostart"] = "Autostart",
                ["settings_manual"] = "Manual",
                ["settings_permissions_title"] = "Configuración de Permisos",
                ["settings_check_permissions"] = "Verificar Estado de Permisos",
                ["settings_configure_all"] = "Configurar Todos los Permisos",
                ["settings_battery"] = "Batería",
                ["settings_permissions_hint"] = "Para un funcionamiento óptimo, configure todos los permisos para ejecución en segundo plano.",
                ["settings_max_per_day"] = "Cigarros máximos por día",
                ["settings_time_between"] = "Tiempo entre cigarros",
                ["settings_schedule"] = "Horario de vigilia",
                ["settings_wake_time"] = "Hora de despertar:",
                ["settings_sleep_time"] = "Hora de dormir:",
                ["settings_price_config"] = "Configuración de Precios",
                ["settings_pack_price"] = "Precio de cajetilla:",
                ["settings_per_pack"] = "Cigarros por cajetilla:",
                ["settings_currency"] = "Divisa:",
                ["settings_price_per_cig"] = "Precio por cigarrillo",
                ["settings_save"] = "Guardar Configuración",
                ["settings_reduce_max"] = "Reducir Máximo (Progreso gradual)",
                ["settings_saved_title"] = "Configuración",
                ["settings_saved_message"] = "Configuración guardada correctamente",
                ["settings_permissions_result_title"] = "Permisos",
                ["settings_permissions_result_message"] = "Se abrieron los ajustes del sistema. Configura batería, autoinicio y segundo plano.",
                ["settings_battery_excluded"] = "Excluida del ahorro",
                ["settings_battery_optimized"] = "Optimizada (recomendado excluir)",
                ["settings_check_system"] = "Revisar en ajustes del sistema",
                ["settings_android_only"] = "Solo disponible en Android.",
                ["settings_error_loading"] = "Error cargando configuración",
                // HistoryPage
                ["history_title"] = "Historial",
                ["history_loading"] = "Cargando historial...",
                ["history_general_stats"] = "Estadísticas Generales",
                ["history_total_cigarettes"] = "Total cigarros",
                ["history_days_recorded"] = "Días registrados",
                ["history_avg_per_day"] = "Promedio/día",
                ["history_reduction"] = "Reducción lograda",
                ["history_summary_loading"] = "Cargando estadísticas...",
                ["history_economic_stats"] = "Estadísticas Económicas",
                ["history_total_spent"] = "Total gastado",
                ["history_total_saved"] = "Total ahorrado",
                ["history_savings_pct"] = "% de ahorro",
                ["history_refresh"] = "Actualizar datos",
                ["history_summary"] = "Reducción lograda: {0}% ({1}/{2} cigarros en {3} días)",
                ["history_error_loading"] = "Error cargando historial"
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
                ["language_selected"] = "Language updated",
                // Common
                ["ok"] = "OK",
                ["cancel"] = "Cancel",
                ["error"] = "Error",
                // Version check (§15)
                ["update_available"] = "Update available",
                ["update_message"] = "A newer version is available ({0}). You have {1}.\nDo you want to update?",
                ["update_now"] = "Update",
                ["update_later"] = "Not now",
                // Menu (flyout)
                ["nav_home"] = "Home",
                ["nav_history"] = "History",
                ["nav_settings"] = "Settings",
                ["nav_about"] = "About",
                ["flyout_subtitle"] = "Don't Give Up",
                // MainPage
                ["main_tip_smart"] = "Smart Tip",
                ["main_progress_title"] = "Today's progress",
                ["main_smoked"] = "Smoked",
                ["main_remaining"] = "Remaining",
                ["main_last"] = "Last",
                ["main_next"] = "Next",
                ["main_time_since"] = "Smoke-free",
                ["main_smoke_button"] = "Smoke Cigarette",
                ["main_limit_reached_button"] = "Limit reached",
                ["main_refresh"] = "Refresh data",
                ["main_daily_stats"] = "Today's statistics",
                ["main_time_between"] = "Between cigarettes",
                ["main_awake_hours"] = "Awake hours",
                ["main_error_loading"] = "Error loading data",
                ["main_limit_title"] = "Limit reached",
                ["main_limit_question"] = "You have already smoked {0} cigarettes today. Do you want to continue?",
                ["main_limit_yes"] = "Yes, smoke",
                ["main_limit_no"] = "No, wait",
                ["main_confirm_title"] = "Final confirmation",
                ["main_confirm_question"] = "Are you sure? This will exceed your daily limit.",
                ["main_confirm_yes"] = "Yes, I'm sure",
                ["main_registered_title"] = "Registered",
                ["main_registered_message"] = "Cigarette registered successfully",
                ["main_error_register"] = "Error registering cigarette",
                ["main_placeholder"] = "--",
                // SettingsPage
                ["settings_title"] = "Settings",
                ["settings_subtitle"] = "Preferences and permissions",
                ["settings_battery_save"] = "Battery saver",
                ["settings_unverified"] = "Unverified",
                ["settings_autostart"] = "Autostart",
                ["settings_manual"] = "Manual",
                ["settings_permissions_title"] = "Permission Settings",
                ["settings_check_permissions"] = "Check Permission Status",
                ["settings_configure_all"] = "Configure All Permissions",
                ["settings_battery"] = "Battery",
                ["settings_permissions_hint"] = "For optimal operation, configure all permissions for background execution.",
                ["settings_max_per_day"] = "Maximum cigarettes per day",
                ["settings_time_between"] = "Time between cigarettes",
                ["settings_schedule"] = "Waking hours",
                ["settings_wake_time"] = "Wake-up time:",
                ["settings_sleep_time"] = "Sleep time:",
                ["settings_price_config"] = "Price Settings",
                ["settings_pack_price"] = "Pack price:",
                ["settings_per_pack"] = "Cigarettes per pack:",
                ["settings_currency"] = "Currency:",
                ["settings_price_per_cig"] = "Price per cigarette",
                ["settings_save"] = "Save Settings",
                ["settings_reduce_max"] = "Reduce Maximum (gradual progress)",
                ["settings_saved_title"] = "Settings",
                ["settings_saved_message"] = "Settings saved successfully",
                ["settings_permissions_result_title"] = "Permissions",
                ["settings_permissions_result_message"] = "System settings opened. Configure battery, autostart and background.",
                ["settings_battery_excluded"] = "Excluded from saver",
                ["settings_battery_optimized"] = "Optimized (excluding recommended)",
                ["settings_check_system"] = "Check in system settings",
                ["settings_android_only"] = "Only available on Android.",
                ["settings_error_loading"] = "Error loading settings",
                // HistoryPage
                ["history_title"] = "History",
                ["history_loading"] = "Loading history...",
                ["history_general_stats"] = "General Statistics",
                ["history_total_cigarettes"] = "Total cigarettes",
                ["history_days_recorded"] = "Days recorded",
                ["history_avg_per_day"] = "Average/day",
                ["history_reduction"] = "Reduction achieved",
                ["history_summary_loading"] = "Loading statistics...",
                ["history_economic_stats"] = "Economic Statistics",
                ["history_total_spent"] = "Total spent",
                ["history_total_saved"] = "Total saved",
                ["history_savings_pct"] = "% savings",
                ["history_refresh"] = "Refresh data",
                ["history_summary"] = "Reduction achieved: {0}% ({1}/{2} cigarettes in {3} days)",
                ["history_error_loading"] = "Error loading history"
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