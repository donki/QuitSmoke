using QuitSmoke.Pages;

namespace QuitSmoke;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Registrar rutas para navegación
        Routing.RegisterRoute("AboutPage", typeof(AboutPage));
    }
}