# Justificación de Permisos de Android

Conforme a la Constitución del proyecto (§5 "Seguridad" y §9 "Estándares de
Calidad"), cada permiso declarado en
[`Platforms/Android/AndroidManifest.xml`](../Platforms/Android/AndroidManifest.xml)
debe estar justificado y aplicar el principio de mínimo privilegio.

| Permiso | Justificación | ¿Imprescindible? |
|---|---|---|
| `POST_NOTIFICATIONS` | Mostrar recordatorios y advertencias cuando el usuario se acerca o excede su límite diario de cigarrillos (función central de la app). Requerido en Android 13+ (API 33). | Sí |
| `WAKE_LOCK` | Permitir que las notificaciones locales programadas se entreguen con fiabilidad y, opcionalmente, mantener la pantalla encendida mientras el dispositivo está conectado (función "pantalla siempre encendida"). | Sí |
| `RECEIVE_BOOT_COMPLETED` | Reprogramar las notificaciones locales tras reiniciar el dispositivo (lo gestiona `Plugin.LocalNotification`); sin él, los recordatorios se perderían tras un reinicio. | Sí |
| `INTERNET` | Envío del correo de feedback/soporte mediante `EmailService`. No se transmiten datos de consumo del usuario: la privacidad de los datos se mantiene en el dispositivo (§3). | Revisar |
| `ACCESS_NETWORK_STATE` | Comprobar conectividad antes de intentar el envío de correo, evitando fallos silenciosos (§3 "Confiabilidad operativa"). | Revisar |

## Notas de cumplimiento

- **Privacidad primero (§3):** los datos de consumo se almacenan localmente en el
  dispositivo y no se envían a ningún servidor. `INTERNET` se usa exclusivamente
  para la acción explícita del usuario de enviar feedback por correo.
- **Mínimo privilegio (§3):** si en una revisión futura se elimina la función de
  envío de correo, deben retirarse también `INTERNET` y `ACCESS_NETWORK_STATE`.
- **Revisión obligatoria (§7, §9):** este fichero debe revisarse antes de cada
  publicación y mantenerse sincronizado con el `AndroidManifest.xml`.
