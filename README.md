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
