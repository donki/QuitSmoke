# Changelog

Todos los cambios relevantes de QuitSmoke se documentan en este fichero.

El formato sigue [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/)
y el versionado las reglas de la [Constitución del proyecto](constitution/constitucion.md) (§6):
`ApplicationDisplayVersion` (legible) y `ApplicationVersion` (entero incremental) se
actualizan en sincronía antes de cada publicación.

## [No publicado]

### Seguridad
- Se dejan de versionar el keystore de firma, la contraseña del keystore y las
  claves (`*.keystore`, `keystore.password.txt`, `*.pem`); añadidos a `.gitignore`
  (Constitución §5). Los ficheros permanecen en disco para firmar localmente.

### Cambios técnicos / refactorización
- Restaurados los ViewModels (`MainPageViewModel`, `SettingsPageViewModel`,
  `HistoryPageViewModel`) que dejaban la solución sin compilar.
- Eliminada la `AboutPage` duplicada y muerta en `Views/`; la activa es `Pages/AboutPage`.
- Añadido el repositorio de gobernanza `constitution` como submódulo de git.
- Añadidos scripts reproducibles de compilación/firmado (`scripts/build.ps1`) y
  publicación a Google Play (`scripts/publish.ps1`).
- Documentada la justificación de cada permiso de Android (`docs/PERMISSIONS.md`).
- Alineada la versión mostrada en la interfaz con la del proyecto: `1.10.0`.

## [1.10.0] - 2025-09-18
### Añadido
- Configuración de "pantalla siempre encendida" cuando el dispositivo está conectado.
- Spinner de carga en la página de historial.
### Corregido
- Cálculos económicos del historial (gasto, ahorro y promedios).

## [1.x] - 2025-09-08
### Añadido
- Página de historial con estadísticas de 30 días.
### Cambiado
- Mejoras de interfaz de usuario.
- Actualización del SDK de .NET MAUI.

## [1.0.0] - 2025-08-31
### Añadido
- Primera versión: registro de cigarrillos, límite diario, notificaciones,
  seguimiento económico con soporte multidivisa y configuración de horarios.
