# AlMaximo - Sistema de Gestión de Préstamos Empresariales

## 🏢 Descripción del Sistema

**AlMaximo** es un sistema integral de gestión de préstamos empresariales desarrollado para administrar y controlar los préstamos otorgados a empleados. El sistema permite realizar operaciones CRUD completas, seguimiento de pagos, cálculo automático de intereses y mantiene un historial detallado de todas las transacciones y modificaciones realizadas.

### 🎯 Objetivos Principales

- **Gestión Completa de Préstamos**: Crear, consultar, modificar y eliminar préstamos
- **Control de Empleados**: Administración de catálogos de empleados y autorizadores
- **Seguimiento Financiero**: Cálculo automático de intereses y control de saldos
- **Auditoría Completa**: Historial detallado de cambios con trazabilidad total
- **Interfaz Intuitiva**: Dashboard moderno y responsive para facilitar la gestión

---

## 🏗️ Arquitectura del Sistema

El sistema AlMaximo está construido utilizando una **arquitectura de capas** siguiendo los principios de **Clean Architecture**, separando claramente las responsabilidades y garantizando la mantenibilidad, escalabilidad y testabilidad del código.

### 📊 Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    FRONTEND (Angular 18)                    │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ Components  │  │  Services   │  │   Models    │        │
│  │   - List    │  │ - Prestamos │  │   - DTOs    │        │
│  │   - Form    │  │ - Catalogos │  │ - Interfaces│        │
│  │ - Historial │  │   - HTTP    │  │             │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
                              │
                         HTTP/REST API
                              │
┌─────────────────────────────────────────────────────────────┐
│                   BACKEND (.NET Core 8)                    │
├─────────────────────────────────────────────────────────────┤
│                    Controllers (API)                       │
│  ┌─────────────────┐  ┌─────────────────┐                 │
│  │ PrestamosController │  CatalogosController │           │
│  └─────────────────┘  └─────────────────┘                 │
├─────────────────────────────────────────────────────────────┤
│                  Application Layer                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐│
│  │   Services      │  │   Validators    │  │  Mappings   ││
│  │ - PrestamoSvc   │  │ - FluentValid.  │  │ - AutoMapper││
│  │ - CatalogosSvc  │  │                 │  │             ││
│  └─────────────────┘  └─────────────────┘  └─────────────┘│
├─────────────────────────────────────────────────────────────┤
│                     Core Layer                             │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐│
│  │    Entities     │  │   Interfaces    │  │    DTOs     ││
│  │ - Prestamo      │  │ - IRepository   │ │ - PrestamoDto││
│  │ - Empleado      │  │ - IService      │ │ - CatalogDto ││
│  │ - Historial     │  │                 │  │             ││
│  └─────────────────┘  └─────────────────┘  └─────────────┘│
├─────────────────────────────────────────────────────────────┤
│                Infrastructure Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐                 │
│  │  Repositories   │  │   Data Context  │                 │
│  │ - PrestamoRepo  │  │ - EF DbContext  │                 │
│  │ - CatalogosRepo │  │ - Migrations    │                 │
│  └─────────────────┘  └─────────────────┘                 │
└─────────────────────────────────────────────────────────────┘
                              │
                        Entity Framework
                              │
┌─────────────────────────────────────────────────────────────┐
│                   DATABASE (SQL Server)                    │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Prestamos  │  │  Empleados  │  │ Prestamos   │        │
│  │             │  │             │  │ Historial   │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 Inyección de Dependencias

El sistema utiliza el **contenedor de IoC integrado de .NET Core** para gestionar la inyección de dependencias, garantizando un acoplamiento débil y facilitando las pruebas unitarias.

### 📋 Configuración de Servicios

#### Program.cs - Configuración Principal

```csharp
// Entity Framework Configuration
builder.Services.AddDbContext<AlMaximoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository Pattern - Scoped Lifetime
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
builder.Services.AddScoped<ICatalogosRepository, CatalogosRepository>();

// Business Logic Services - Scoped Lifetime
builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<ICatalogosService, CatalogosService>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddScoped<IValidator<CreatePrestamoDto>, PrestamoValidator>();
```

### 🏭 Patrones de Inyección Utilizados

#### 1. **Constructor Injection**
```csharp
public class PrestamosController : ControllerBase
{
    private readonly IPrestamoService _prestamoService;

    public PrestamosController(IPrestamoService prestamoService)
    {
        _prestamoService = prestamoService;
    }
}
```

#### 2. **Repository Pattern con DI**
```csharp
public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePrestamoDto> _validator;

    public PrestamoService(
        IPrestamoRepository prestamoRepository,
        IMapper mapper,
        IValidator<CreatePrestamoDto> validator)
    {
        _prestamoRepository = prestamoRepository;
        _mapper = mapper;
        _validator = validator;
    }
}
```

#### 3. **DbContext Injection**
```csharp
public class PrestamoRepository : IPrestamoRepository
{
    private readonly AlMaximoDbContext _context;

    public PrestamoRepository(AlMaximoDbContext context)
    {
        _context = context;
    }
}
```

### 🔄 Lifetimes de Servicios

- **Scoped**: Repositorios y Servicios (una instancia por request HTTP)
- **Singleton**: AutoMapper, Logging
- **Transient**: Validadores (nueva instancia cada vez que se solicita)

---

## 🛠️ Tecnologías y Frameworks

### Backend (.NET Core 8)
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Base de Datos**: SQL Server
- **Validación**: FluentValidation
- **Mapeo**: AutoMapper
- **Logging**: Serilog
- **API**: RESTful Web API

### Frontend (Angular 18)
- **Framework**: Angular 18 (Standalone Components)
- **UI Framework**: Bootstrap 5.3
- **Iconos**: Font Awesome 6
- **HTTP Client**: Angular HttpClient
- **Formularios**: Reactive Forms
- **Routing**: Angular Router

### Base de Datos
- **Motor**: SQL Server 2019+
- **Migraciones**: Entity Framework Migrations
- **Índices**: Optimizados para consultas frecuentes
- **Integridad**: Constraints y Foreign Keys

---

## 📁 Estructura del Proyecto

```
AlMaximo/
├── AlMaximo/                          # Backend .NET Core
│   ├── Controllers/
│   │   └── Api/
│   │       ├── PrestamosController.cs
│   │       └── CatalogosController.cs
│   ├── Core/                          # Dominio y Contratos
│   │   ├── Entities/                  # Entidades de dominio
│   │   ├── DTOs/                      # Data Transfer Objects
│   │   └── Interfaces/                # Contratos de servicios
│   ├── Application/                   # Lógica de negocio
│   │   ├── Services/                  # Servicios de aplicación
│   │   ├── Validators/                # Validaciones FluentValidation
│   │   └── Mappings/                  # Perfiles AutoMapper
│   ├── Infrastructure/                # Acceso a datos
│   │   ├── Data/                      # DbContext y configuraciones
│   │   └── Repositories/              # Implementación de repositorios
│   └── Migrations/                    # Migraciones EF
├── Presentation/ClientApp/            # Frontend Angular
│   ├── src/app/
│   │   ├── components/prestamos/      # Componentes de préstamos
│   │   ├── services/                  # Servicios HTTP
│   │   └── models/                    # Interfaces TypeScript
│   └── dist/                          # Build de producción
```

---

## ⚙️ Instalación y Configuración

### Prerrequisitos
- .NET 8 SDK
- Node.js 18+ y NPM
- SQL Server 2019+
- Visual Studio Code o Visual Studio 2022

### 🚀 Configuración del Backend

1. **Clonar el repositorio**
```bash
git clone <repository-url>
cd AlMaximo
```

2. **Configurar cadena de conexión**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AlMaximo;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3. **Ejecutar migraciones**
```bash
cd AlMaximo
dotnet ef database update
```

4. **Ejecutar aplicación**
```bash
dotnet run
# API disponible en: https://localhost:7001
```

### 🎨 Configuración del Frontend

1. **Instalar dependencias**
```bash
cd Presentation/ClientApp
npm install
```

2. **Configurar endpoint API**
```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api'
};
```

3. **Ejecutar aplicación**
```bash
npm start
# Aplicación disponible en: http://localhost:4200 o http://localhost:4201
```

---

## 🚀 Funcionalidades Principales

### 💰 Gestión de Préstamos
- **Crear Préstamo**: Formulario con validaciones y cálculo automático de intereses
- **Listar Préstamos**: Vista paginada con búsqueda y filtros
- **Editar Préstamo**: Modificación con validaciones de negocio
- **Eliminar Préstamo**: Soft delete con validaciones de estado

### 👥 Gestión de Empleados
- **Catálogo de Empleados**: Lista completa con datos personales
- **Empleados Autorizadores**: Gestión de permisos de autorización
- **Búsqueda Avanzada**: Filtros por nómina, nombre, departamento

### 📊 Historial y Auditoría
- **Trazabilidad Completa**: Registro de todos los cambios realizados
- **Historial por Préstamo**: Vista detallada de modificaciones
- **Información de Usuario**: Tracking de quién realizó cada cambio
- **Timeline Visual**: Interfaz cronológica de eventos

### 🔍 Características Adicionales
- **Validaciones Robustas**: FluentValidation en backend + Angular Validators
- **Interfaz Responsive**: Bootstrap 5 con diseño mobile-first
- **API RESTful**: Endpoints bien documentados y estructurados
- **Manejo de Errores**: Logging detallado y mensajes de usuario amigables

---

## 🧪 Testing y Calidad

### Estrategia de Testing
- **Unit Tests**: Servicios y repositorios con MSTest
- **Integration Tests**: Controllers y base de datos
- **Frontend Tests**: Jasmine y Karma para componentes Angular

### Principios de Calidad
- **SOLID Principles**: Aplicados en toda la arquitectura
- **Clean Code**: Nomenclatura clara y código autodocumentado
- **Error Handling**: Manejo consistente de excepciones
- **Logging**: Trazabilidad completa con Serilog

---

## 📊 Endpoints Principales de la API

### Préstamos
- `GET /api/prestamos` - Listar préstamos con paginación
- `GET /api/prestamos/{id}` - Obtener préstamo por ID  
- `GET /api/prestamos/{id}/historial` - Obtener historial de cambios
- `POST /api/prestamos` - Crear nuevo préstamo
- `PUT /api/prestamos/{id}` - Actualizar préstamo
- `DELETE /api/prestamos/{id}` - Eliminar préstamo

### Catálogos
- `GET /api/catalogos/empleados` - Listar empleados
- `GET /api/catalogos/empleados-autorizadores` - Listar autorizadores  
- `GET /api/catalogos/tipos-pago` - Listar tipos de pago

---

## 🌐 URLs de Acceso

- **Frontend Angular**: `http://localhost:4200` o `http://localhost:4201`
- **Backend API**: `https://localhost:7001`
- **Swagger Documentation**: `https://localhost:7001/swagger`

---

## 📝 Licencia

Este proyecto está desarrollado para uso interno empresarial.

---

## 👨‍💻 Contribución

Para contribuir al proyecto:

1. Fork del repositorio
2. Crear branch feature (`git checkout -b feature/AmazingFeature`)
3. Commit de cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

---
