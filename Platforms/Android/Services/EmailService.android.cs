using Android.Content;
using QuitSmoke.Services;
using AndroidX.Core.Content;

namespace QuitSmoke.Platforms.Android.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("message/rfc822");
            intent.PutExtra(Intent.ExtraEmail, new string[] { email });
            intent.PutExtra(Intent.ExtraSubject, subject);
            intent.PutExtra(Intent.ExtraText, body);

            // Crear un chooser para mostrar todas las aplicaciones de email disponibles
            var chooserIntent = Intent.CreateChooser(intent, "Enviar email con:");
            chooserIntent!.SetFlags(ActivityFlags.NewTask);

            var context = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (context != null)
            {
                context.StartActivity(chooserIntent);
            }
            else
            {
                throw new InvalidOperationException("No se pudo obtener el contexto de la actividad actual");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al abrir cliente de email: {ex.Message}", ex);
        }
    }
}