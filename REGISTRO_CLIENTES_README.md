# Sistema de Registro de Clientes - SGHR

## Funcionalidad Implementada

Se ha implementado el sistema completo de registro de clientes para la plataforma web de reservas de hotel.

## Archivos Creados

### 1. Capa de Datos (SGHR)
- **SGHR/Models/Cliente.cs**: Modelo de entidad Cliente con todas las propiedades requeridas
- **SGHR/Context/SGHRDbContext.cs**: Contexto de Entity Framework con configuración de la entidad Cliente

### 2. Capa Web (SGHRWeb)
- **SGHRWeb/Models/RegistroClienteViewModel.cs**: ViewModel con validaciones completas
- **SGHRWeb/Controllers/ClienteController.cs**: Controlador para manejar el registro
- **SGHRWeb/Views/Cliente/Registro.cshtml**: Vista del formulario de registro
- **SGHRWeb/wwwroot/css/registro.css**: Estilos personalizados para validación
- **SGHRWeb/wwwroot/js/registro.js**: Script JavaScript para validación en tiempo real

### 3. Migraciones
- Base de datos creada con tabla `Clientes` e índice único en el campo Email

## Validaciones Implementadas

### Frontend (Cliente-Side)
✅ Campos obligatorios marcados con asterisco rojo
✅ Nombre y Apellido: máximo 50 caracteres, no permiten números
✅ Email: formato válido (usuario@dominio.com), máximo 100 caracteres
✅ Teléfono: acepta números y prefijos internacionales (+, -, (), espacios), máximo 25 caracteres
✅ Contraseña: oculta al ingresar, máximo 30 caracteres
✅ Confirmación de contraseña
✅ Bordes rojos en campos con errores
✅ Validación en tiempo real al perder el foco del campo
✅ Prevención de envío si hay campos vacíos

### Backend (Server-Side)
✅ Validación de todos los campos obligatorios
✅ Validación de longitudes máximas
✅ Validación de formato de email
✅ Validación de formato de teléfono
✅ Validación para evitar emails duplicados
✅ Validación de que nombre y apellido no contengan números

## Cadena de Conexión

La aplicación está configurada para usar **SQLite** (base de datos local):
```
Data Source=SGHR.db
```

El archivo `SGHR.db` se crea automáticamente en la carpeta `SGHRWeb` al ejecutar la aplicación por primera vez.
**No necesitas instalar SQL Server ni ningún otro servidor de base de datos.**

## Cómo Usar

1. **Ejecutar la aplicación**:
   ```bash
   dotnet run --project SGHRWeb
   ```

2. **Acceder al registro**:
   - Navega a: `https://localhost:xxxx/Cliente/Registro`
   - O haz clic en "Registrarse" en el menú de navegación

3. **Probar validaciones**:
   - Intenta dejar campos vacíos → verás mensajes de error
   - Ingresa un email inválido → verás error de formato
   - Ingresa números en nombre/apellido → verás error de validación
   - Ingresa un email ya registrado → verás error de duplicado
   - Todos los campos con errores tendrán borde rojo

## Estructura de Base de Datos

### Tabla: Clientes
| Campo         | Tipo          | Restricciones                    |
|---------------|---------------|----------------------------------|
| Id            | int           | Primary Key, Identity            |
| Nombre        | nvarchar(50)  | NOT NULL, sin números            |
| Apellido      | nvarchar(50)  | NOT NULL, sin números            |
| Email         | nvarchar(100) | NOT NULL, UNIQUE, formato válido |
| Telefono      | nvarchar(25)  | NOT NULL                         |
| Contrasena    | nvarchar(30)  | NOT NULL                         |
| FechaRegistro | datetime2     | NOT NULL                         |

## Notas de Seguridad

⚠️ **IMPORTANTE**: En un entorno de producción, debes implementar:
- Hash de contraseñas (usar `BCrypt`, `Argon2` o `PBKDF2`)
- HTTPS obligatorio
- Rate limiting para prevenir ataques de fuerza bruta
- Tokens anti-CSRF (ya implementado con `@Html.AntiForgeryToken()`)
- Validación adicional del lado del servidor

## Criterios de Aceptación Cumplidos

✅ El sistema rechaza emails que no cumplan el formato usuario@dominio.com
✅ El campo teléfono acepta números y prefijos internacionales
✅ Los campos nombre y apellido no permiten números
✅ El sistema muestra error si nombre o apellido superan los 50 caracteres
✅ El sistema impide el envío del formulario si existen campos obligatorios vacíos
✅ No se permite registrar dos usuarios con el mismo email
✅ Los campos obligatorios están marcados con asterisco rojo
✅ Los campos con errores de validación tienen borde rojo
✅ La contraseña se muestra oculta al ingresarla

## Próximos Pasos Recomendados

1. Implementar hash de contraseñas
2. Crear página de inicio de sesión
3. Agregar autenticación con JWT o Cookies
4. Implementar recuperación de contraseña
5. Agregar confirmación de email
6. Crear pruebas unitarias para las validaciones
