using Android.Content;
using QuitSmoke.Services;
using AndroidX.Core.Content;

namespace QuitSmoke.Platforms.Android.Services;

public class EmailService : IEmailService
{
    private readonly ILocalizationService _loc;

    public EmailService(ILocalizationService loc)
    {
        _loc = loc;
    }

    public Task SendEmailAsync(string email, string subject, string body)
    {
        var intent = new Intent(Intent.ActionSend);
        intent.SetType("message/rfc822");
        intent.PutExtra(Intent.ExtraEmail, new string[] { email });
        intent.PutExtra(Intent.ExtraSubject, subject);
        intent.PutExtra(Intent.ExtraText, body);

        // Crear un chooser para mostrar todas las aplicaciones de email disponibles
        var chooserIntent = Intent.CreateChooser(intent, _loc.GetString("email_chooser_title"));
        chooserIntent!.SetFlags(ActivityFlags.NewTask);

        // Los fallos llegan a AboutPage, que los muestra con su propio texto traducido
        // ("email_error_message") delante del detalle.
        var context = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (context == null)
            throw new InvalidOperationException(_loc.GetString("email_no_activity"));

        context.StartActivity(chooserIntent);
        return Task.CompletedTask;
    }
}
