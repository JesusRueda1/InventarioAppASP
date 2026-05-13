# 📦 Sistema de Inventario Simple
**ASP.NET Core 8 MVC · Entity Framework Core · MySQL/MariaDB · Tailwind · jQuery · Tabulator**

---

## 🗂 Estructura del Proyecto

```
InventarioApp/
├── .vscode/
│   ├── launch.json          ← Configuración de depuración en VS Code
│   └── tasks.json           ← Tareas build/watch
├── Controllers/
│   ├── LoginController.cs
│   ├── HomeController.cs
│   ├── CategoriasController.cs
│   ├── ProductosController.cs
│   ├── ComprasController.cs
│   └── VentasController.cs
├── Data/
│   └── ApplicationDbContext.cs   ← DbContext de EF Core
├── Models/
│   ├── Usuario.cs
│   ├── Categoria.cs
│   ├── Producto.cs
│   └── Compra.cs            ← También contiene Venta, DetalleCompra, DetalleVenta, LoginViewModel
├── Properties/
│   └── launchSettings.json
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml   ← Layout principal con sidebar
│   ├── Login/Index.cshtml
│   ├── Home/Index.cshtml    ← Dashboard
│   ├── Categorias/Index.cshtml
│   ├── Productos/Index.cshtml
│   ├── Compras/Index.cshtml
│   └── Ventas/Index.cshtml  ← POS
├── appsettings.json
├── Program.cs
├── InventarioApp.csproj
└── database.sql             ← Script SQL para HeidiSQL
```

---

## ⚙️ Requisitos previos

| Herramienta | Versión mínima | Descarga |
|---|---|---|
| .NET SDK | 8.0 | https://dotnet.microsoft.com/download |
| MySQL o MariaDB | 8.0 / 10.6 | https://dev.mysql.com/downloads/ |
| HeidiSQL | Cualquiera | https://www.heidisql.com/ |
| Visual Studio Code | Cualquiera | https://code.visualstudio.com/ |
| Extensión C# (VS Code) | — | ID: `ms-dotnettools.csharp` |

---

## 🚀 Pasos para ejecutar el proyecto

### 1. Crear la base de datos con HeidiSQL

1. Abre **HeidiSQL** y conéctate a tu servidor MySQL/MariaDB.
2. Ve al menú **Archivo → Ejecutar archivo SQL...** y selecciona `database.sql`.
3. También puedes abrir una pestaña de **Query**, pegar todo el contenido de `database.sql` y presionar **F9**.
4. Verifica que se creó la base de datos `inventario` con sus 7 tablas.

### 2. Configurar la cadena de conexión

Abre `appsettings.json` y ajusta `user` y `password` según tu instalación:

```json
"DefaultConnection": "server=localhost;database=inventario;user=root;password=;"
```

> Si tu MySQL tiene contraseña: `...user=root;password=tuPassword;`

### 3. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 4. Compilar el proyecto

```bash
dotnet build
```

### 5. Ejecutar la aplicación

```bash
dotnet run
```

La aplicación estará disponible en: **http://localhost:5000**

> Para recargar automáticamente los cambios en desarrollo usa:
> ```bash
> dotnet watch run
> ```

---

## 🔑 Credenciales de prueba

| Campo    | Valor            |
|----------|-----------------|
| Correo   | admin@demo.com  |
| Password | Admin123        |

---

## 📋 Funcionalidades del sistema

| Módulo | Descripción |
|---|---|
| **Login** | Autenticación con cookie segura y hash BCrypt |
| **Dashboard** | Tarjetas con totales: productos, categorías, stock bajo, ventas |
| **Categorías** | CRUD completo con tabla Tabulator y modal AJAX |
| **Productos** | CRUD con búsqueda, alertas de stock < 5, tabla con colores |
| **Compras** | Registrar entrada de mercancía con múltiples productos; actualiza stock automáticamente |
| **POS / Ventas** | Punto de venta con carrito, buscador en tiempo real, comprobante; descuenta stock |

---

## 🏗 Arquitectura del proyecto

```
Cliente (Razor + jQuery + Tailwind)
        │
        │  HTTP (GET/POST/DELETE) + JSON
        ▼
Controllers (ASP.NET Core MVC)
        │
        │  LINQ + Entity Framework Core
        ▼
ApplicationDbContext
        │
        │  Pomelo MySQL Driver
        ▼
Base de datos MySQL/MariaDB
```

- **Controladores** exponen endpoints JSON consumidos por **jQuery AJAX**.
- **Tabulator** renderiza y pagina las tablas desde esos endpoints.
- **BCrypt** hashea y verifica contraseñas.
- **Cookie Authentication** protege todas las rutas con `[Authorize]`.

---

## 🛠 Comandos útiles de .NET CLI

```bash
# Crear nuevo proyecto MVC (referencia)
dotnet new mvc -n MiProyecto

# Agregar paquete NuGet
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.0

# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con recarga automática
dotnet watch run

# Limpiar artefactos de compilación
dotnet clean
```

---

## ❓ Problemas frecuentes

| Problema | Solución |
|---|---|
| `Unable to connect to MySQL` | Verifica que MySQL esté corriendo y la cadena de conexión sea correcta |
| Puerto 5000 ocupado | Cambia `applicationUrl` en `Properties/launchSettings.json` |
| `BCrypt` no encontrado | Ejecuta `dotnet restore` |
| La tabla no carga | Abre las herramientas de desarrollador del navegador (F12) y revisa la consola |

---

## 📖 Documentación del Proyecto (Wiki)

La documentación completa del sistema se encuentra en la carpeta [`docs/wiki/`](docs/wiki/).

### Tabla de Contenido

#### 1. Visión General
- [Visión General del Proyecto](docs/wiki/01-Visión-General-del-Proyecto.md)
- [Configuración del Entorno y Puesta en Marcha](docs/wiki/02-Configuración-del-Entorno-y-Puesta-en-Marcha.md)
- [Arquitectura General de la Aplicación](docs/wiki/03-Arquitectura-General-de-la-Aplicación.md)

#### 2. Capa de Datos
- [Capa de Datos y Esquema de Base de Datos](docs/wiki/04-Capa-de-Datos-y-Esquema-de-Base-de-Datos.md)
- [ApplicationDbContext y Configuración de EF Core](docs/wiki/05-ApplicationDbContext-y-Configuración-de-EF-Core.md)
- [Modelos de Dominio: Catálogo e Inventario](docs/wiki/06-Modelos-de-Dominio-Catálogo-e-Inventario.md)
- [Modelos de Dominio: Transacciones, Pagos y Kárdex](docs/wiki/07-Modelos-de-Dominio-Transacciones-Pagos-y-Kárdex.md)
- [Modelos de Identidad: Usuario, Rol y Permiso](docs/wiki/08-Modelos-de-Identidad-Usuario-Rol-y-Permiso.md)

#### 3. Autenticación y Autorización
- [Autenticación y Autorización](docs/wiki/09-Autenticación-y-Autorización.md)
- [Flujo de Autenticación con Cookies](docs/wiki/10-Flujo-de-Autenticación-con-Cookies.md)
- [Control de Acceso Basado en Permisos (RBAC)](docs/wiki/11-Control-de-Acceso-Basado-en-Permisos-RBAC.md)
- [Gestión de Usuarios y Roles](docs/wiki/12-Gestión-de-Usuarios-y-Roles.md)

#### 4. Módulos de Negocio: Inventario y Catálogo
- [Módulos de Negocio: Inventario y Catálogo](docs/wiki/13-Módulos-de-Negocio-Inventario-y-Catálogo.md)
- [Gestión de Productos](docs/wiki/14-Gestión-de-Productos.md)
- [Gestión de Categorías](docs/wiki/15-Gestión-de-Categorías.md)
- [Ajuste de Kárdex (Stock Manual)](docs/wiki/16-Ajuste-de-Kárdex-Stock-Manual.md)

#### 5. Módulos de Negocio: Compras y Ventas
- [Módulos de Negocio: Compras y Ventas (POS)](docs/wiki/17-Módulos-de-Negocio-Compras-y-Ventas-POS.md)
- [Módulo de Compras](docs/wiki/18-Módulo-de-Compras.md)
- [Módulo de Ventas (Terminal POS)](docs/wiki/19-Módulo-de-Ventas-Terminal-POS.md)
- [Gestión de Pagos y Cartera](docs/wiki/20-Gestión-de-Pagos-y-Cartera-Cuentas-por-CobrarPagar.md)

#### 6. Reportes y Auditoría
- [Reportes, Auditoría y Monitoreo](docs/wiki/21-Reportes-Auditoría-y-Monitoreo.md)
- [Dashboard y Reportes de Negocio](docs/wiki/22-Dashboard-y-Reportes-de-Negocio.md)
- [Sistema de Auditoría Global](docs/wiki/23-Sistema-de-Auditoría-Global.md)

#### 7. Interfaz de Usuario
- [Interfaz de Usuario y Componentes Frontend](docs/wiki/24-Interfaz-de-Usuario-y-Componentes-Frontend.md)
- [Layout Compartido y Navegación](docs/wiki/25-Layout-Compartido-y-Navegación.md)
- [Patrones de UI: Tablas, Modales y Notificaciones](docs/wiki/26-Patrones-de-UI-Tablas-Modales-y-Notificaciones.md)

#### 8. Referencia
- [Glosario de Términos y Conceptos Clave](docs/wiki/27-Glosario-de-Términos-y-Conceptos-Clave.md)
