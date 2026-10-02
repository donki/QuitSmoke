using QuitSmoke.Services;

namespace QuitSmoke.Views;

/// <summary>Dialogos con ModernDialog sobre una pagina (nunca AlertDialog nativo).</summary>
public sealed class ModernDialogs(Page page) : IUserDialogs
{
    public Task AlertAsync(string title, string message, string ok) =>
        SocShared.ModernDialog.AlertAsync(page, title, message, ok);

    public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel) =>
        SocShared.ModernDialog.AlertAsync(page, title, message, accept, cancel);
}
