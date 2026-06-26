# 🚭 QuitSmoke

Una aplicación móvil desarrollada en .NET MAUI para ayudar a las personas a reducir y controlar su consumo de cigarrillos de manera gradual y efectiva.

## 📱 Características

### 🎯 Control de Consumo
- **Límite diario personalizable**: Establece tu meta de cigarrillos por día
- **Registro de consumo**: Registra cada cigarrillo fumado con timestamp
- **Advertencias inteligentes**: Notificaciones cuando excedes tu límite diario
- **Confirmación doble**: Sistema de confirmación para evitar exceder el límite

### 📊 Estadísticas Detalladas
- **Historial de 30 días**: Visualización gráfica de tu progreso
- **Estadísticas de tiempo**: Tiempo mínimo, máximo y promedio entre cigarrillos
- **Porcentaje de reducción**: Calcula tu progreso hacia la meta
- **Análisis económico**: Seguimiento de gastos y ahorros

### 💰 Seguimiento Económico
- **Configuración de precios**: Precio por cajetilla y cigarrillos por cajetilla
- **Múltiples divisas**: Soporte para 14 divisas diferentes
- **Total gastado**: Cálculo del dinero gastado en el período
- **Total ahorrado**: Dinero ahorrado vs. fumar el máximo diario
- **Porcentaje de ahorro**: Relación entre dinero ahorrado y gastado
- **Promedio diario**: Gasto promedio por día

### ⚙️ Configuración Avanzada
- **Horarios personalizados**: Configura tus horas de despertar y dormir
- **Notificaciones**: Recordatorios y alertas personalizables
- **Optimización de batería**: Configuración para evitar restricciones del sistema
- **Pantalla siempre encendida**: Mantiene la pantalla activa cuando está conectado por USB

## 🛠️ Tecnologías Utilizadas

- **.NET MAUI**: Framework multiplataforma para desarrollo móvil
- **C#**: Lenguaje de programación principal
- **Arquitectura code-behind ligero**: la interfaz delega la lógica de negocio en servicios (sin capa MVVM)
- **Inyección de dependencias**: servicios registrados en `MauiProgram`
- **Plugin.LocalNotification**: Para notificaciones locales
- **Microsoft.Maui.Graphics**: Para gráficos y visualizaciones

## 📋 Requisitos

### Para Desarrollo
- Visual Studio 2022 17.8+ o Visual Studio Code
- .NET 9.0 SDK
- Workload de .NET MAUI instalado
- Android SDK (para desarrollo Android)

### Para Usuarios
- Android 7.0 (API level 24) o superior
- Aproximadamente 50 MB de espacio libre

## 🚀 Instalación y Configuración

### Clonar el Repositorio
```bash
git clone https://github.com/tu-usuario/QuitSmoke.git
cd QuitSmoke
```

### Restaurar Dependencias
```bash
dotnet restore
```

### Compilar el Proyecto
```bash
dotnet build
```

### Ejecutar en Android
```bash
dotnet build -t:Run -f net9.0-android
```

### Instalar en Dispositivo Android
```bash
dotnet build -t:Install -f net9.0-android
```

## 📖 Uso de la Aplicación

### Primera Configuración
1. **Configura tu límite diario** en la página de configuración
2. **Establece tus horarios** de despertar y dormir
3. **Configura los precios** de los cigarrillos para seguimiento económico
4. **Selecciona tu divisa** preferida

### Registro Diario
1. **Registra cada cigarrillo** usando el botón principal
2. **Revisa tu progreso** en tiempo real
3. **Consulta estadísticas** en la página de historial
4. **Ajusta tu límite** según tu progreso

### Análisis de Progreso
- **Página principal**: Estado actual del día y próximo cigarrillo recomendado
- **Página de historial**: Estadísticas detalladas de los últimos 30 días
- **Gráficas**: Visualización de tu consumo diario
- **Métricas económicas**: Análisis de gastos y ahorros

## 🎨 Capturas de Pantalla

*[Aquí puedes agregar capturas de pantalla de la aplicación]*

## 🤝 Contribuir

Las contribuciones son bienvenidas. Para contribuir:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Roadmap

- [ ] Soporte para iOS
- [ ] Sincronización en la nube
- [ ] Estadísticas avanzadas
- [ ] Gamificación y logros
- [ ] Soporte para múltiples usuarios
- [ ] Exportación de datos
- [ ] Modo oscuro
- [ ] Widget para pantalla de inicio

## 🐛 Reportar Problemas

Si encuentras algún problema o tienes sugerencias, por favor:

1. Revisa los [issues existentes](https://github.com/tu-usuario/QuitSmoke/issues)
2. Si no existe, crea un [nuevo issue](https://github.com/tu-usuario/QuitSmoke/issues/new)
3. Proporciona la mayor información posible

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 👨‍💻 Autor

Desarrollado con ❤️ para ayudar a las personas a mejorar su salud.

## 🙏 Agradecimientos

- Comunidad de .NET MAUI
- Contribuidores de código abierto
- Usuarios que proporcionan feedback

---

**⚠️ Nota Importante**: Esta aplicación es una herramienta de apoyo para reducir el consumo de cigarrillos. Para dejar de fumar completamente, se recomienda consultar con profesionales de la salud.

**🏥 Recursos de Ayuda**:
- [Línea Nacional para Dejar de Fumar](https://www.smokefree.gov/)
- [Organización Mundial de la Salud - Tabaco](https://www.who.int/news-room/fact-sheets/detail/tobacco)