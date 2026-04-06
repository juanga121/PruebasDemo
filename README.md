# PruebasDemo

Proyecto de pruebas unitarias e integracion para .NET enfocado en la gestion de creditos, implementando arquitectura por capas con enfasis en testabilidad y separacion de responsabilidades.

---

# Descripcion

PruebasDemo es una aplicacion web API desarrollada con .NET 8 que permite gestionar creditos, implementando una arquitectura por capas con enfoque principal en testing.

El proyecto demuestra como estructurar codigo altamente testeable mediante separacion clara de responsabilidades, inversion de dependencias y uso de patrones que facilitan el mocking y la validacion de la logica de negocio.

---

# Objetivo principal

## Testing Completo

Implementacion de estrategias de testing unitario e integracion usando xUnit y Moq:

- Tests unitarios con mocking de dependencias externas  
- Tests de integracion de endpoints HTTP  
- Cobertura de casos exitosos y manejo de errores  
- Validacion de logica de negocio en el flujo de creditos  
- Arquitectura que maximiza la testabilidad  

---

# Caracteristicas secundarias

Como soporte para el testing efectivo, el proyecto implementa:

- Arquitectura por capas (Domain, Application, Infrastructure, API)  
- Repository Pattern generico  
- Entity Framework Core para persistencia  
- Validaciones de logica de negocio en servicios  
- Inyeccion de dependencias  
- Configuracion de CORS

---

# Arquitectura

El proyecto implementa una arquitectura por capas organizada de forma que la logica de negocio se mantenga desacoplada de la infraestructura.

## Infrastructure (Persistencia)

- GenericRepository<T, TKey>  
- Entity Framework Core DataContext  
- Configuracion de base de datos SQL Server  

## Application (Logica de negocio)

- CreditosService  
- Interfaces de repositorio  
- Validaciones y manejo de estados de creditos  

## Domain (Entidades)

- CreditoEntity  
- CreditoDTO  

---

# Estructura del proyecto

```
PruebasDemo

PruebasDemo.Domain
Entidades y modelos del dominio

PruebasDemo.Application
Logica de negocio y servicios

PruebasDemo.Infrastructure
Implementaciones de persistencia y repositorios

PruebasDemo
API y configuracion de la aplicacion

PruebaDemoTest
Tests unitarios
```

---

# Tecnologias utilizadas

## Framework y lenguaje

- .NET 8.0  
- ASP.NET Core 8.0  

## Testing

- xUnit  
- Moq  

## Persistencia

- Entity Framework Core  
- Microsoft.EntityFrameworkCore.SqlServer  

## Patrones y arquitectura

- Repository Pattern  
- Inyeccion de dependencias  

---

# Pruebas unitarias

Los tests unitarios verifican el comportamiento de los servicios de negocio (CreditosService) en completo aislamiento, reemplazando las dependencias externas con mocks del repositorio generico.

## Escenarios cubiertos

- Pagos parciales de creditos  
- Pago total del credito  
- Error cuando el credito no existe  
- Error cuando el credito esta inactivo  
- Validacion de montos invalidos  

## Patron AAA (Arrange - Act - Assert)

Todos los tests siguen el patron AAA:

- **Arrange:** Preparar el escenario de prueba (datos de entrada, configuracion de mocks)  
- **Act:** Ejecutar el metodo o funcion bajo prueba  
- **Assert:** Verificar que el resultado es el esperado  

---

# Pruebas de integracion

Los tests de integracion verifican el flujo completo del sistema, probando la integracion entre controladores, servicios y persistencia.

## Incluyen

- Ejecucion de endpoints HTTP  
- Validacion de respuestas  
- Integracion entre capas  
- Uso de base de datos real o en memoria  

## Patron AAA aplicado a HTTP

- **Arrange:** Preparar el request HTTP (datos de entrada, payload JSON)  
- **Act:** Ejecutar la llamada HTTP real al endpoint  
- **Assert:** Verificar la respuesta HTTP  

---

# Ejecucion de pruebas desde consola

Para ejecutar las pruebas del proyecto desde la consola:

1. Abrir una terminal o consola  
2. Ubicarse en la carpeta raiz del proyecto  
3. Ejecutar:

```bash
dotnet test
```

Este comando compila el proyecto de pruebas y ejecuta todos los tests disponibles.

---

# Ejecucion de pruebas desde Visual Studio

Con el proyecto abierto en Visual Studio:

1. Ir al menu **Ver**  
2. Seleccionar **Explorador de pruebas**  
3. Ejecutar:
   - Todos los tests  
   - Tests individuales  

Tambien es posible visualizar el resultado de cada prueba (correcta o fallida).

---

# Debug en pruebas

Para depurar una prueba:

1. Ubicarse sobre el test  
2. Clic derecho  
3. Seleccionar **Debug Test**  

Esto permite usar breakpoints, inspeccionar variables y analizar el flujo paso a paso.

---

# Ejecucion de la aplicacion

## Desde Visual Studio

- Clonar el repositorio  
- Abrir la solucion  
- Seleccionar el proyecto **PruebasDemo** como inicio  
- Ejecutar la aplicacion  

## Desde consola

```bash
dotnet run
```

---

# En resumen

Este proyecto demuestra como construir una API en .NET centrada en la logica de negocio de creditos, disenada desde su base para ser completamente testeable mediante pruebas unitarias e integracion, garantizando calidad, mantenibilidad y escalabilidad.