# Sistema de Gestión de Citas Médicas - Hospital San Vicente

## Descripción General

Este sistema web desarrollado en C# con ASP.NET Core MVC permite al Hospital San Vicente digitalizar la gestión de citas médicas, pacientes y médicos. El sistema centraliza la información, automatiza la programación de citas, evita duplicidades y facilita el acceso seguro y eficiente a los datos, eliminando la dependencia de registros manuales.

## Funcionalidades

### Gestión de Pacientes
- Registrar nuevos pacientes con nombre, documento, edad, teléfono y correo.
- Editar la información de los pacientes.
- Validar que el documento de identidad sea único para evitar duplicados.
- Listar todos los pacientes registrados.

### Gestión de Médicos
- Registrar médicos con nombre, documento, especialidad, teléfono y correo.
- Editar la información de los médicos.
- Validar que el documento de identidad sea único y que no existan médicos con la misma combinación de nombre y especialidad.
- Listar todos los médicos registrados y filtrar por especialidad.

### Gestión de Citas Médicas
- Agendar citas médicas asignando paciente, médico, fecha y hora.
- Validar que un médico o paciente no tenga más de una cita en el mismo horario.
- Cancelar citas cambiando su estado a "Cancelada".
- Marcar citas como atendidas cambiando su estado a "Atendida".
- Listar citas médicas por paciente y por médico.
- Enviar correo de confirmación al paciente al agendar una cita.
- Registrar historial de envío de correos electrónicos con su estado ("Enviado", "No enviado").

### Persistencia y Validaciones
- Uso de Entity Framework Core con PostgreSQL para persistencia.
- LINQ, Listas y Diccionarios para la gestión y consulta de información.
- Validaciones de negocio y manejo de errores con mensajes claros y bloques try-catch.

## Estructura del Proyecto

- **Models/**  
  - `Appointment.cs`: Modelo de citas médicas.
  - `Doctor.cs`: Modelo de médicos.
  - `Patient.cs`: Modelo de pacientes.
  - `EmailHistory.cs`: Historial de correos electrónicos.
  - `ErrorViewModel.cs`: Modelo para manejo de errores en vistas.

- **Controllers/**  
  - `AppointmentController.cs`: Lógica de citas médicas.
  - `DoctorController.cs`: Lógica de médicos.
  - `PatientController.cs`: Lógica de pacientes.
  - `EmailHistoryController.cs`: Lógica de historial de correos.
  - `HomeController.cs`: Página principal y privacidad.

- **Views/**  
  - Carpetas para cada entidad con vistas para listar, editar, detalles, etc.
  - Carpeta `Shared` con layouts y vistas comunes.

- **Data/**  
  - `PostgresDbContext.cs`: Contexto de base de datos PostgreSQL.

## Requisitos Previos

- .NET 8 SDK
- PostgreSQL
- Visual Studio Code (recomendado)

## Instalación y Ejecución

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/JhonRojas1997/modulo5_3_jhonr
   cd Modulo5_3_JhonR
   ```

2. **Configurar la base de datos**
   - Crear una base de datos PostgreSQL (por ejemplo, `hospital_db`).
   - Editar la cadena de conexión en `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=hospital_db;Username=tu_usuario;Password=tu_contraseña"
     }
     ```

3. **Aplicar migraciones**
   ```bash
   dotnet ef database update
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```
   Accede a al link en tu navegador.

## Ejemplos de Uso

- Registrar un paciente desde la vista `/Patient/Create`.
- Listar médicos y filtrar por especialidad en `/Doctor/Index`.
- Agendar una cita en `/Appointment/Create` y recibir confirmación por correo.
- Consultar historial de correos en `/EmailHistory/Index`.

## Diagramas

- **Diagrama de Clases:**  
  ![Diagrama de Clases](docs/diagrama_clases.png)
- **Diagrama de Casos de Uso:**  
  ![Diagrama de Casos de Uso](docs/diagrama_casos_uso.png)

## Información del Coder

- **Nombre:** Jhon Rojas.
- **Clan:** VanRossum Csharp
- **Correo:** jfrojas1997@gmailcom
- **Documento de Identidad:** [1019129435]

## Notas

- El sistema valida la unicidad de documentos y evita duplicidad de citas.
- Los mensajes de error son claros y amigables.
- El historial de correos muestra el estado de cada envío.

---

Para dudas o soporte, consulta la documentación o contacta al coder.


