# Sistema HelpDesk - Proyecto Final para la Evaluación del Curso de Clean Architecture con .Net 9 - Galaxy Training Perú

Sistema de gestión de tickets de soporte técnico desarrollado con .NET 9, siguiendo principios de Clean Architecture y Domain-Driven Design.

## Funcionalidades Principales

### Autenticación y Autorización
- **Registro de usuarios** con confirmación por email
- **Login con JWT** para autenticación segura
- **Sistema de roles**: Admin, Support y Customer
- **Permisos dinámicos** configurables sin reiniciar la aplicación
- **Bloqueo automático** de cuenta tras 5 intentos fallidos
- **Cambio de contraseña** con notificación por email

### Gestión de Tickets
- **Crear tickets** con título, descripción, prioridad, categoría y tipo
- **Asignar tickets** a empleados de soporte
- **Flujo de estados**: Abierto → Asignado → En Progreso → Resuelto → Cerrado
- **Comentarios y respuestas** en cada ticket
- **Historial completo** de cambios de estado
- **Notificaciones automáticas** por email en eventos importantes

### Control de Acceso
- **Clientes** solo pueden ver y gestionar sus propios tickets
- **Soporte** puede ver y gestionar todos los tickets
- **Administradores** tienen acceso completo al sistema

### Auditoría
- **Registro automático** de todas las acciones sobre tickets
- **Trazabilidad completa**: usuario, acción, fecha, IP y navegador
- **Filtros avanzados** por ticket, usuario, acción y fechas
- **Paginación** de resultados

### Notificaciones por Email
- Confirmación de registro
- Ticket asignado a empleado
- Nueva respuesta en ticket
- Ticket cerrado o resuelto
- Cambio de contraseña

## Tecnologías

### Core
- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API RESTful
- **Entity Framework Core** - ORM para acceso a datos
- **SQL Server** - Base de datos relacional

### Autenticación y Seguridad
- **ASP.NET Core Identity** - Gestión de usuarios
- **JWT** - Autenticación stateless
- **Autorización basada en permisos** - Políticas dinámicas

### Patrones y Arquitectura
- **Clean Architecture** - Separación de responsabilidades
- **Domain-Driven Design** - Lógica de negocio en el dominio
- **MediatR** - Patrón CQRS
- **Mapster** - Mapeo de objetos
- **FluentValidation** - Validación de datos

### Observabilidad
- **Serilog** - Logging estructurado (Consola + Archivo)
- **.NET Aspire** - Dashboard de observabilidad integrado
  - Logs en tiempo real
  - Trazas distribuidas
  - Métricas y health checks
  - Dashboard automático en `http://localhost:15888`

## Arquitectura

El proyecto sigue **Clean Architecture** con las siguientes capas:

```
HelpDesk/
├── HelpDesk.Domain/            # Entidades, Value Objects, Eventos
├── HelpDesk.Application/       # Casos de uso, DTOs, Interfaces
├── HelpDesk.Infrastructure/    # Implementaciones, DbContext, Servicios
├── HelpDesk.API/               # Controllers, Middleware
└── HelpDesk.ArchitectureTests/ # Tests de arquitectura
```

## Requisitos Previos

- **.NET 9 SDK**
- **SQL Server 2017+** o **SQL Server Express**
- **Servidor SMTP** (para envío de emails)

## Instalación

### 1. Configurar Base de Datos

Editar `HelpDesk.API/appsettings.json` con la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "HelpDeskConnection": "Server=localhost,1433;Database=HelpDeskDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Aplicar Migraciones

```bash
dotnet ef database update --project HelpDesk.Infrastructure --startup-project HelpDesk.API
```

Esto creará:
- Esquema `security`: Usuarios, Roles, Sesiones, Permisos
- Esquema `helpdesk`: Tickets, Comentarios, Historial, Auditoría
- Esquema `catalog`: Catálogos recursivos
- Datos iniciales: Usuarios Admin y Support, Roles, Permisos

### 3. Configurar Email (Opcional)

Edita `HelpDesk.API/appsettings.json`:

```json
{
  "SmtpSettings": {
    "Host": "smtp.domain.com",
    "Port": 587,
    "Username": "email@domain.com",
    "Password": "password",
    "FromEmail": "noreply@helpdesk.com",
    "FromName": "HelpDesk System",
    "EnableSsl": true
  }
}
```

### 4. Ejecutar la Aplicación

Iniciar sólo la API:
```bash
dotnet run --project HelpDesk.API
```

La API estará disponible en: `https://localhost:5021`

Swagger UI: `https://localhost:5021/swagger`


o iniciar con el Dashboard de Aspire:

```bash
dotnet run --project HelpDesk.AppHost
```

Dashboard de Aspire estará disponible en: https://localhost:17298/login?t=687a5f5632105027c87508e2b5d6e881

Si Aspire solicita Token, deberá ingresar el identificador que aparece después del parámetro t=
el cual se muestra en el Console al iniciar Aspire

```bash
Login to the dashboard at https://localhost:17298
```


## Usuarios de Prueba

Al iniciar la aplicación se crean automáticamente:

- **Admin**: `admin@helpdesk.com` / `Admin@123`
- **Support**: `support@helpdesk.com` / `Support@123`

## Seguridad

- ✅ Contraseñas encriptadas con PBKDF2
- ✅ Tokens JWT con expiración configurable
- ✅ Bloqueo automático de cuenta
- ✅ Validación estricta de permisos
- ✅ Aislamiento de datos por usuario
- ✅ Auditoría completa de acciones

