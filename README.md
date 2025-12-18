# SiaInteractive – Backend API

Este repositorio contiene el backend de la aplicación desarrollado en .NET, 
expuesto como una Web API REST.

El objetivo es proveer operaciones CRUD para entidades de dominio (Products, Categories),
aplicando buenas prácticas de arquitectura, validación, manejo de errores y testing.

## Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AutoMapper
- FluentValidation
- Serilog
- MSTest + Moq
- EF Core InMemory (tests)

## Arquitectura

El backend está organizado siguiendo una separación por capas:

- **API**  
  Controllers, configuración de middleware y exposición HTTP.

- **Application**  
  Lógica de negocio, DTOs, validaciones y orquestación de casos de uso.  
  Esta capa depende únicamente de **abstracciones** (interfaces) para el acceso a datos.

- **Infrastructure**  
  Implementaciones concretas de acceso a datos mediante Entity Framework Core y repositorios, 
  que satisfacen las interfaces definidas y utilizadas por la capa Application.

- **Domain**  
  Entidades de dominio y relaciones (Product, Category).
  
Esta separación permite:
- Facilitar el testing unitario
- Reducir el acoplamiento
- Mantener la lógica de negocio independiente del framework

# Diagrama

API => Application => Infrastructure => Database

## Modelo de datos

- **Product**
  - ProductID
  - Name
  - Description
  - Image

- **Category**
  - Id
  - Name

- **ProductCategory**
  - ProductID
  - CategoryID

La relación Product–Category se implementa mediante una tabla intermedia 

## Validaciones y manejo de errores

- Las validaciones de entrada se realizan con **FluentValidation**
- Cada operación valida los DTOs antes de ejecutar la lógica de negocio
- Las respuestas incluyen información de errores de validación cuando corresponde
- El manejo de errores se centraliza mediante middleware.

## Manejo global de excepciones

La API utiliza un middleware global de manejo de excepciones para centralizar la
traducción de errores a respuestas HTTP consistentes en formato JSON.

Comportamiento:
- `ValidationException` → **400 Bad Request**
- `KeyNotFoundException` → **404 Not Found**
- `DbUpdateConcurrencyException` → **409 Conflict** 
- `DbUpdateException` → **409 Conflict** 
- `InvalidOperationException` → **409 Conflict**
- Otros errores no controlados → **500 Internal Server Error**

Todas las respuestas de error se devuelven con el wrapper estándar:
`{ Data: "...", IsSuccess: false, Message: "...", ValidationErrors: []}`

El middleware registra los eventos con niveles adecuados:
- Warning para errores esperables (validación, no encontrado, conflictos)
- Error para fallas inesperadas

## Testing

Se implementaron distintos niveles de testing:

### Unit Tests
- **Application layer**
  - Se testean los casos de uso de Product
  - Dependencias mockeadas (repositories, validators, mapper)
  - Se validan flujos exitosos y escenarios de error

- **Repository layer**
  - Se testea el acceso a datos con EF Core InMemory
  - Se valida persistencia, queries, paginación y relaciones many-to-many

### Integration Test
- Se incluyó un test de integración mínimo para Product
- Utiliza `WebApplicationFactory` y EF Core InMemory
- Valida el pipeline completo:
  - Routing
  - Model binding
  - Validación
  - Persistencia
  - Respuesta HTTP
  
## Nota

Los tests de integración se mantuvieron intencionalmente mínimos para evitar duplicar 
la cobertura ya lograda por los unit tests y mantener tiempos de ejecución bajos.

## Ejecución del proyecto

1. Correr el script de creation de base de datos y tablas
2. Configurar la cadena de conexión en `appsettings.json`
3. Ejecutar el proyecto
4. La API quedará disponible en: 
	* http://localhost:5083/swagger 
	* https://localhost:7255/swagger

## Decisiones técnicas

- EF Core InMemory se utiliza en tests por simplicidad y velocidad. Para escenarios productivos o mayor fidelidad, podría utilizarse SQLite in-memory.
- Los repositories retornan booleanos simples para operaciones de escritura, manteniendo la lógica de error en capas superiores.
- Los controllers se mantienen simples, delegando la lógica de negocio a la capa Application.