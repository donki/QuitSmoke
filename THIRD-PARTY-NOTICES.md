# Avisos de Terceros (Third-Party Notices)

QuitSmoke se distribuye bajo la licencia **MIT** (ver [LICENSE](LICENSE)).
Conforme a la Constitución del proyecto (§4), este fichero inventaría las
dependencias de terceros, su uso, su licencia y su titular. Todas las
dependencias usan licencias **permisivas** compatibles con MIT.

| Dependencia | Versión | Uso | Licencia | Titular |
|---|---|---|---|---|
| Microsoft.Maui.Controls | 9.0.0 | Framework de interfaz multiplataforma (.NET MAUI) | MIT | Microsoft / .NET Foundation |
| Microsoft.Maui.Controls.Compatibility | 9.0.0 | Compatibilidad de controles heredados de MAUI | MIT | Microsoft / .NET Foundation |
| Microsoft.Extensions.Logging.Debug | 9.0.0 | Proveedor de logging de depuración (solo Debug) | MIT | Microsoft / .NET Foundation |
| Plugin.LocalNotification | 11.1.0 | Notificaciones locales (recordatorios y alertas) | MIT | Eddy Zavaleta (thudugala) |

## Notas

- **APIs del sistema operativo:** el uso de las APIs de Android (a través de los
  bindings de .NET para Android) no contamina la licencia del proyecto (§4).
- **Revisión antes de añadir:** antes de incorporar una dependencia nueva debe
  verificarse su licencia (permisiva: MIT/BSD/Apache-2.0) y registrarse en esta
  tabla (§4). No se permiten dependencias con licencia copyleft (GPL/AGPL/LGPL).

Las licencias completas de cada dependencia están disponibles en sus respectivos
repositorios de NuGet/GitHub.
