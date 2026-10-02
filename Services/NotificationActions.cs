namespace QuitSmoke.Services;

/// <summary>
/// La notificacion persistente con el boton «Fumar»: al arrancar se pide permiso y se pone al dia; al
/// pulsar «Fumar» se apunta un cigarro sin abrir la app. Ningun fallo aqui bloquea el arranque.
/// </summary>
public static class NotificationActions
{
    public static async Task InitAsync(INotificationService? notifications, ISmokingDataService? data)
    {
        try
        {
            if (notifications is null || data is null) return;

            await notifications.RequestPermissionAsync();
            await notifications.UpdatePersistentStatusAsync(await data.GetDataAsync());
        }
        catch
        {
            // no bloquear el arranque si falla la notificacion
        }
    }

    /// <summary>Devuelve true si la accion era «Fumar» y se apunto el cigarro.</summary>
    public static async Task<bool> HandleAsync(int actionId, ISmokingDataService? data, INotificationService? notifications)
    {
        if (actionId != NotificationService.SmokeActionId || data is null || notifications is null)
            return false;
        try
        {
            await data.AddSmokedCigaretteAsync();
            await notifications.UpdatePersistentStatusAsync(await data.GetDataAsync());
            return true;
        }
        catch
        {
            return false;
        }
    }
}
