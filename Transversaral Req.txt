
# About Page Template - .NET MAUI

Este template proporciona una estructura base reutilizable para crear páginas "About" (Acerca de) en aplicaciones .NET MAUI con un diseño moderno y funcionalidad completa.

## Características del Template

- ✅ Diseño responsivo con cards modernas
- ✅ Sección de información de la aplicación
- ✅ Sección de contacto con funcionalidad tap-to-email
- ✅ Sección de donaciones con navegación externa
- ✅ Aviso legal personalizable
- ✅ Manejo robusto de errores
- ✅ Estilos visuales consistentes

## Estructura de Archivos

```
Pages/
├── AboutPage.xaml          # Interfaz de usuario
└── AboutPage.xaml.cs       # Lógica de código
```

## Dependencias Requeridas

Asegúrate de que tu proyecto incluya:

```xml
<PackageReference Include="Microsoft.Maui.Essentials" Version="[latest]" />
```

## Template XAML (AboutPage.xaml)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="[YOUR_NAMESPACE].Pages.AboutPage"
             Title="[PAGE_TITLE]"
             BackgroundColor="#F8F9FA">

    <ScrollView>
        <StackLayout Padding="20" Spacing="20">

            <!-- Header Card -->
            <Frame BackgroundColor="White" 
                   CornerRadius="12" 
                   HasShadow="True" 
                   Padding="0">
                <StackLayout Padding="30,25" Spacing="15">
                    <Label
                        Text="[APP_ICON]"
                        FontSize="60"
                        HorizontalOptions="Center" />

                    <Label
                        Text="[APP_NAME]"
                        FontSize="28"
                        FontAttributes="Bold"
                        TextColor="#2C3E50"
                        HorizontalOptions="Center" />

                    <Label
                        Text="[APP_VERSION]"
                        FontSize="16"
                        TextColor="#7F8C8D"
                        HorizontalOptions="Center" />

                    <Label
                        Text="[APP_DESCRIPTION]"
                        FontSize="14"
                        TextColor="#95A5A6"
                        HorizontalOptions="Center" />

                    <Label
                        Text="[COMPANY_NAME]"
                        FontSize="12"
                        TextColor="#7F8C8D"
                        HorizontalOptions="Center"
                        Margin="0,10,0,0" />
                </StackLayout>
            </Frame>

            <!-- Contact Card -->
            <Frame BackgroundColor="White" 
                   CornerRadius="12" 
                   HasShadow="True" 
                   Padding="0">
                <StackLayout Padding="25,20" Spacing="15">
                    <Label
                        Text="[CONTACT_ICON] [CONTACT_TITLE]"
                        FontSize="18"
                        FontAttributes="Bold"
                        TextColor="#3498DB" />

                    <Button
                        Text="[CONTACT_EMAIL]"
                        FontSize="14"
                        BackgroundColor="Transparent"
                        TextColor="#3498DB"
                        BorderColor="#3498DB"
                        BorderWidth="1"
                        CornerRadius="8"
                        HeightRequest="45"
                        Clicked="OnContactEmailClicked"
                        HorizontalOptions="Fill">
                        <Button.Shadow>
                            <Shadow Brush="#3498DB" Opacity="0.2" Radius="4" Offset="0,2" />
                        </Button.Shadow>
                    </Button>

                    <Label
                        Text="[CONTACT_INSTRUCTION]"
                        FontSize="12"
                        TextColor="#7F8C8D"
                        HorizontalOptions="Center" />
                </StackLayout>
            </Frame>

            <!-- Donation Card (Optional - Remove if not needed) -->
            <Frame BackgroundColor="White" 
                   CornerRadius="12" 
                   HasShadow="True" 
                   Padding="0">
                <StackLayout Padding="25,20" Spacing="15">
                    <Label
                        Text="[DONATION_ICON] [DONATION_TITLE]"
                        FontSize="18"
                        FontAttributes="Bold"
                        TextColor="#E67E22" />

                    <Button
                        Text="[DONATION_BUTTON_TEXT]"
                        FontSize="14"
                        BackgroundColor="#E67E22"
                        TextColor="White"
                        CornerRadius="8"
                        HeightRequest="45"
                        Clicked="OnDonationClicked"
                        HorizontalOptions="Fill">
                        <Button.Shadow>
                            <Shadow Brush="#E67E22" Opacity="0.3" Radius="8" Offset="0,4" />
                        </Shadow>
                    </Button>

                    <Label
                        Text="[DONATION_DESCRIPTION]"
                        FontSize="12"
                        TextColor="#7F8C8D"
                        HorizontalOptions="Center" />
                </StackLayout>
            </Frame>

            <!-- Legal Disclaimer Card (Optional - Customize as needed) -->
            <Frame BackgroundColor="White" 
                   CornerRadius="12" 
                   HasShadow="True" 
                   Padding="0">
                <StackLayout Padding="25,20" Spacing="15">
                    <Label
                        Text="[LEGAL_ICON] [LEGAL_TITLE]"
                        FontSize="18"
                        FontAttributes="Bold"
                        TextColor="#E74C3C" />

                    <Frame BackgroundColor="#FDF2F2" 
                           CornerRadius="8" 
                           Padding="15" 
                           HasShadow="False">
                        <StackLayout Spacing="10">
                            <Label
                                Text="[LEGAL_TEXT_1]"
                                FontSize="13"
                                LineHeight="1.4"
                                TextColor="#2C3E50" />

                            <Label
                                Text="[LEGAL_TEXT_2]"
                                FontSize="13"
                                LineHeight="1.4"
                                TextColor="#2C3E50" />

                            <Frame BackgroundColor="#E74C3C" 
                                   CornerRadius="6" 
                                   Padding="12" 
                                   HasShadow="False">
                                <Label
                                    Text="[WARNING_TEXT]"
                                    FontSize="13"
                                    FontAttributes="Bold"
                                    LineHeight="1.4"
                                    TextColor="White"
                                    HorizontalOptions="Center" />
                            </Frame>
                        </StackLayout>
                    </Frame>
                </StackLayout>
            </Frame>

            <!-- Back Button -->
            <Button
                Text="[BACK_BUTTON_TEXT]"
                FontSize="16"
                FontAttributes="Bold"
                BackgroundColor="#3498DB"
                TextColor="White"
                CornerRadius="10"
                HeightRequest="50"
                Clicked="OnBackClicked"
                HorizontalOptions="Fill"
                Margin="0,10,0,20">
                <Button.Shadow>
                    <Shadow Brush="#3498DB" Opacity="0.3" Radius="8" Offset="0,4" />
                </Button.Shadow>
            </Button>

        </StackLayout>
    </ScrollView>

</ContentPage>
```

## Template C# Code-Behind (AboutPage.xaml.cs)

```csharp
using Microsoft.Maui.Essentials;

namespace [YOUR_NAMESPACE].Pages
{
    public partial class AboutPage : ContentPage
    {
        // CONFIGURACIÓN - Personaliza estos valores
        private const string ContactEmail = "[YOUR_EMAIL]";
        private const string DonationUrl = "[YOUR_DONATION_URL]"; // Ko-fi, PayPal, etc.
        private const string EmailSubject = "[EMAIL_SUBJECT]";

        public AboutPage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnContactEmailClicked(object? sender, EventArgs e)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = EmailSubject,
                    To = new List<string> { ContactEmail }
                };

                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "[EMAIL_NOT_AVAILABLE_MESSAGE]", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"[EMAIL_ERROR_MESSAGE]: {ex.Message}", "OK");
            }
        }

        private async void OnDonationClicked(object? sender, EventArgs e)
        {
            try
            {
                var uri = new Uri(DonationUrl);
                var browserLaunchOptions = new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Show,
                    PreferredToolbarColor = Color.FromArgb("#E67E22"),
                    PreferredControlColor = Color.FromArgb("#FFFFFF")
                };

                await Browser.OpenAsync(uri, browserLaunchOptions);
            }
            catch (FeatureNotSupportedException)
            {
                // Fallback: copy URL to clipboard if browser is not available
                try
                {
                    await Clipboard.SetTextAsync(DonationUrl);
                    await DisplayAlert("[BROWSER_NOT_AVAILABLE_TITLE]", 
                        $"[BROWSER_NOT_AVAILABLE_MESSAGE]:\n{DonationUrl}", 
                        "OK");
                }
                catch
                {
                    await DisplayAlert("Error", 
                        $"[BROWSER_ERROR_MESSAGE]: {DonationUrl}", 
                        "OK");
                }
            }
            catch (Exception ex)
            {
                // Fallback: copy URL to clipboard on any other error
                try
                {
                    await Clipboard.SetTextAsync(DonationUrl);
                    await DisplayAlert("[LINK_ERROR_TITLE]", 
                        $"[LINK_ERROR_MESSAGE] ({ex.Message}), [CLIPBOARD_MESSAGE].", 
                        "OK");
                }
                catch
                {
                    await DisplayAlert("Error", 
                        $"[FINAL_ERROR_MESSAGE]: {DonationUrl}", 
                        "OK");
                }
            }
        }
    }
}
```

## Guía de Personalización

### 1. Placeholders a Reemplazar en XAML

| Placeholder | Descripción | Ejemplo |
|-------------|-------------|---------|
| `[YOUR_NAMESPACE]` | Namespace de tu aplicación | `MyApp` |
| `[PAGE_TITLE]` | Título de la página | `"Acerca de"` |
| `[APP_ICON]` | Emoji o icono de la app | `"📖"` |
| `[APP_NAME]` | Nombre de la aplicación | `"Mi Aplicación"` |
| `[APP_VERSION]` | Versión actual | `"Versión 1.0"` |
| `[APP_DESCRIPTION]` | Descripción breve | `"Una aplicación increíble"` |
| `[COMPANY_NAME]` | Nombre de la empresa/desarrollador | `"Mi Empresa"` |
| `[CONTACT_ICON]` | Icono de contacto | `"📧"` |
| `[CONTACT_TITLE]` | Título de sección de contacto | `"Contacto"` |
| `[CONTACT_EMAIL]` | Email de contacto | `"contacto@miapp.com"` |
| `[CONTACT_INSTRUCTION]` | Instrucción para el usuario | `"Toca para enviar un correo"` |
| `[DONATION_ICON]` | Icono de donación | `"☕"` |
| `[DONATION_TITLE]` | Título de sección de donación | `"Apoya el Desarrollo"` |
| `[DONATION_BUTTON_TEXT]` | Texto del botón de donación | `"Ko-fi.com - Invítame un café"` |
| `[DONATION_DESCRIPTION]` | Descripción de la donación | `"Tu apoyo ayuda a mejorar la app"` |
| `[LEGAL_ICON]` | Icono legal | `"⚖️"` |
| `[LEGAL_TITLE]` | Título de sección legal | `"Aviso Legal"` |
| `[LEGAL_TEXT_1]` | Primer párrafo legal | `"Este software se proporciona..."` |
| `[LEGAL_TEXT_2]` | Segundo párrafo legal | `"En ningún caso los autores..."` |
| `[WARNING_TEXT]` | Texto de advertencia | `"⚠️ Uso bajo su propio riesgo"` |
| `[BACK_BUTTON_TEXT]` | Texto del botón volver | `"← Volver"` |

### 2. Placeholders a Reemplazar en C#

| Placeholder | Descripción | Ejemplo |
|-------------|-------------|---------|
| `[YOUR_NAMESPACE]` | Namespace de tu aplicación | `MyApp` |
| `[YOUR_EMAIL]` | Tu email de contacto | `"contacto@miapp.com"` |
| `[YOUR_DONATION_URL]` | URL de donación | `"https://ko-fi.com/usuario"` |
| `[EMAIL_SUBJECT]` | Asunto del email | `"Contacto desde Mi App"` |
| `[EMAIL_NOT_AVAILABLE_MESSAGE]` | Mensaje cuando email no disponible | `"Cliente de correo no disponible"` |
| `[EMAIL_ERROR_MESSAGE]` | Mensaje de error de email | `"No se pudo abrir el cliente de correo"` |
| `[BROWSER_NOT_AVAILABLE_TITLE]` | Título cuando navegador no disponible | `"Navegador no disponible"` |
| `[BROWSER_NOT_AVAILABLE_MESSAGE]` | Mensaje cuando navegador no disponible | `"Enlace copiado al portapapeles"` |
| `[BROWSER_ERROR_MESSAGE]` | Mensaje de error de navegador | `"No se pudo abrir el navegador"` |
| `[LINK_ERROR_TITLE]` | Título de error de enlace | `"Error al abrir enlace"` |
| `[LINK_ERROR_MESSAGE]` | Mensaje de error de enlace | `"No se pudo abrir el navegador"` |
| `[CLIPBOARD_MESSAGE]` | Mensaje de portapapeles | `"enlace copiado al portapapeles"` |
| `[FINAL_ERROR_MESSAGE]` | Mensaje de error final | `"No se pudo abrir el enlace"` |

## Instrucciones de Implementación

### Paso 1: Crear los Archivos

1. Crea la carpeta `Pages` en tu proyecto si no existe
2. Copia el template XAML y guárdalo como `AboutPage.xaml`
3. Copia el template C# y guárdalo como `AboutPage.xaml.cs`

### Paso 2: Personalizar el Contenido

1. Reemplaza todos los placeholders `[PLACEHOLDER]` con tus valores específicos
2. Ajusta los colores si deseas un esquema diferente:
   - Azul principal: `#3498DB`
   - Naranja donación: `#E67E22`
   - Rojo legal: `#E74C3C`
   - Gris texto: `#2C3E50`, `#7F8C8D`, `#95A5A6`

### Paso 3: Configurar Navegación

Añade la navegación a tu AboutPage desde tu página principal:

```csharp
private async void OnAboutClicked(object sender, EventArgs e)
{
    await Navigation.PushAsync(new AboutPage());
}
```

### Paso 4: Personalización Opcional

#### Remover Sección de Donaciones
Si no necesitas donaciones, elimina todo el Frame de "Donation Card" del XAML y el método `OnDonationClicked` del C#.

#### Personalizar Colores
Modifica los valores de color en el XAML:
```xml
<!-- Cambiar color principal -->
TextColor="#TU_COLOR_AQUI"
BackgroundColor="#TU_COLOR_AQUI"
```

#### Añadir Más Secciones
Puedes añadir más cards siguiendo el mismo patrón:
```xml
<Frame BackgroundColor="White" CornerRadius="12" HasShadow="True" Padding="0">
    <StackLayout Padding="25,20" Spacing="15">
        <!-- Tu contenido aquí -->
    </StackLayout>
</Frame>
```

### Paso 5: Actualizar Referencias del Proyecto

Si es necesario, actualiza tu archivo `.csproj` para incluir las nuevas páginas:

```xml
<ItemGroup>
  <MauiXaml Include="Pages\AboutPage.xaml" />
</ItemGroup>

<ItemGroup>
  <Compile Include="Pages\AboutPage.xaml.cs">
    <DependentUpon>AboutPage.xaml</DependentUpon>
  </Compile>
</ItemGroup>
```

## Características Avanzadas

### Manejo de Errores Robusto
El template incluye manejo completo de errores para:
- Cliente de email no disponible
- Navegador no disponible
- Errores de red
- Fallback a portapapeles

### Diseño Responsivo
- Usa `ScrollView` para contenido largo
- Cards con sombras para mejor UX
- Espaciado consistente
- Botones con altura fija para mejor accesibilidad

### Accesibilidad
- Contraste de colores adecuado
- Tamaños de fuente legibles
- Botones con altura mínima recomendada
- Textos descriptivos

## Solución de Problemas

### Error: "FeatureNotSupportedException"
- **Causa**: La funcionalidad no está disponible en la plataforma
- **Solución**: El template incluye fallbacks automáticos

### Error: "Namespace not found"
- **Causa**: Namespace incorrecto en el XAML
- **Solución**: Verifica que `[YOUR_NAMESPACE]` coincida con tu proyecto

### Error: "Method not found"
- **Causa**: Método del evento no existe
- **Solución**: Verifica que todos los métodos `Clicked` estén implementados

## Ejemplo de Implementación Completa

Para una aplicación llamada "TextEditor":

**AboutPage.xaml** (fragmento):
```xml
x:Class="TextEditor.Pages.AboutPage"
Title="Acerca de TextEditor"
<!-- ... -->
<Label Text="📝" FontSize="60" />
<Label Text="TextEditor Pro" FontSize="28" />
<Label Text="Versión 2.1.0" FontSize="16" />
```

**AboutPage.xaml.cs** (fragmento):
```csharp
namespace TextEditor.Pages
{
    public partial class AboutPage : ContentPage
    {
        private const string ContactEmail = "soporte@texteditor.com";
        private const string DonationUrl = "https://ko-fi.com/texteditor";
        private const string EmailSubject = "Contacto desde TextEditor Pro";
        // ...
    }
}
```


**Versión del Template**: 1.0  
**Compatibilidad**: .NET MAUI 9.0+  
**Última actualización**: 2025

En castellano e inglés. Por defecto que salga en la versión del SO. Si la aplicación no tiene idioma del SO,
que se ponga en ingles. En la pantalla de configuración o en el about si no hay configuración poner una opción 
para cambiar el idioma.

El Layout debe ser profesional y agradable. Crea iconos sobre la temática de la aplicación.

Pon las pages en una carpeta llamada pages. No quiero viewmodels

Cuando hagan click en email haz un intent con el email y el nombre de la aplicación