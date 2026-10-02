namespace QuitSmoke.Services;

// Lo que la logica necesita de la interfaz y del sistema, por interfaz: la app usa ModernDialog y
// MAUI Essentials; las pruebas, dobles (constitucion General 8.6).

/// <summary>Dialogos de la app (ModernDialog en la app real, nunca AlertDialog nativo).</summary>
public interface IUserDialogs
{
    Task AlertAsync(string title, string message, string ok);
    Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);
}

/// <summary>Abrir un enlace en el navegador.</summary>
public interface ILinkOpener
{
    Task OpenAsync(string url);
}

/// <summary>Correo con MAUI Essentials, donde no hay servicio propio de la plataforma.</summary>
public sealed class EssentialsEmailService : IEmailService
{
    public Task SendEmailAsync(string email, string subject, string body) =>
        Email.ComposeAsync(new EmailMessage { Subject = subject, To = [email], Body = body });
}

/// <summary>Enlaces con el navegador del sistema.</summary>
public sealed class BrowserLinkOpener : ILinkOpener
{
    public Task OpenAsync(string url) => Browser.Default.OpenAsync(new Uri(url), BrowserLaunchMode.SystemPreferred);
}
