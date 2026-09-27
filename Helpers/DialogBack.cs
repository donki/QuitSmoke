namespace QuitSmoke.Helpers;

/// <summary>
/// Atrás con un SocShared.ModernDialog a la vista (Mobile §7): el diálogo se descarta como si se
/// pulsara su botón de cancelar, y si solo tiene uno (un aviso), ese. ModernDialog no expone cómo
/// cerrarse desde fuera; se reconoce por el StyleId de su capa y se pulsa el botón que toca.
/// </summary>
public static class DialogBack
{
    private const string OverlayId = "__modernDialogOverlay";

    public static bool TryDismiss(ContentPage page)
    {
        if (page.Content is not Grid host)
            return false;

        var overlay = host.Children.OfType<Grid>().LastOrDefault(g => g.StyleId == OverlayId);
        if (overlay is null)
            return false;

        var buttons = Descendants(overlay).OfType<Button>().ToList();
        if (buttons.Count == 0)
        {
            host.Remove(overlay);
            return true;
        }

        // Cancelar: en las listas de opciones es el último (texto rojo); en las confirmaciones, el
        // que no lleva el color de acento. Un aviso solo tiene el de aceptar.
        var accent = Application.Current?.Resources.TryGetValue("Primary", out var v) == true ? v as Color : null;
        var cancel = buttons.LastOrDefault(b => b.TextColor?.ToArgbHex() == Color.FromArgb("#C0392B").ToArgbHex())
            ?? (buttons.Count > 1 ? buttons.FirstOrDefault(b => accent is null || b.BackgroundColor != accent) : null)
            ?? buttons[0];
        cancel.SendClicked();
        return true;
    }

    private static IEnumerable<IView> Descendants(IView view)
    {
        yield return view;
        if (view is Microsoft.Maui.ILayout layout)
            foreach (var child in layout)
                foreach (var d in Descendants(child))
                    yield return d;
        else if (view is IContentView cv && cv.PresentedContent is IView inner)
            foreach (var d in Descendants(inner))
                yield return d;
    }
}
