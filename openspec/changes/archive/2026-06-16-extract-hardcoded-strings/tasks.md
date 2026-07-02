# Tasks: Extract Hardcoded Strings

## Review Workload Forecast

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: size-exception
400-line budget risk: Low

| Field | Value |
|-------|-------|
| Estimated changed lines | ~410 |
| 400-line budget risk | Low |
| Chained PRs recommended | No |
| Suggested split | Single PR |
| Delivery strategy | ask-on-risk |

**Note**: ~255 of ~410 lines are boilerplate deletions (`CreditoMensajes.resx` + `.Designer.cs`). Cognitive review load is ~100 lines of actual refactor — well under budget.

## Phase 1: Foundation — New Constants

- [x] 1.1 Create `PruebaDemoTest/Constants/TestConstants.cs` — named consts for Monto, Saldo, MontoPago, TasaInteres, Meses values used across tests
- [x] 1.2 Create `PruebasDemo/Constants/ApiConstants.cs` — `AllowAll`, `CorsOriginLocal`, `LogPath`, `OutputTemplate`, `TestingEnv`, `TestingDbName`
- [x] 1.3 Add `ErrorNoControlado = "Error no controlado"` to `PruebasDemo.Application/Resources/Constants/LogTemplates.cs`

## Phase 2: Resource Consolidation

- [x] 2.1 Update `PruebasDemo/Controllers/CreditoController.cs` — using `PruebasDemo.Application.Resources` → `Mensajes.*` instead of `PruebasDemo.Resources` → `CreditoMensajes.*`
- [x] 2.2 Delete `PruebasDemo/Resources/CreditoMensajes.resx` and `CreditoMensajes.Designer.cs`

## Phase 3: Core Refactor — Wire Constants

- [x] 3.1 Update `PruebasDemo/Middlewares/ExceptionMiddleware.cs` — add `using` for LogTemplates, replace `"Error no controlado"` with `LogTemplates.ErrorNoControlado`
- [x] 3.2 Update `PruebasDemo/Program.cs` — replace `"AllowAll"`, `"http://localhost:4200"`, `"logs/log-.txt"`, output template strings with `ApiConstants.*`
- [x] 3.3 Update `PruebasDemo/Configuration/DataBaseConfiguration.cs` — replace `"Testing"` / `"PruebasDemo_TestingDb"` with `ApiConstants.*`
- [x] 3.4 Update `PruebaDemoTest/PruebasIntegracion/CustomWebApplicationFactory.cs` — replace `"Testing"` with `ApiConstants.TestingEnv`

## Phase 4: Test Refactor

- [x] 4.1 Update `CreditosServiceTest.cs` — replace assertion strings (`"Crédito no encontrado"`, `"El crédito no está activo"`, `"El monto de pago debe ser mayor a cero"`, `"El monto de pago excede el saldo del crédito"`) with `Mensajes.*`; replace numeric literals with `TestConstants.*`
- [x] 4.2 Update `CreditosIntegrationTest.cs` — replace magic numbers (`Monto`, `TasaInteres`, `Meses`, `Saldo`, `MontoPago`) with `TestConstants.*`

## Phase 5: Verification

- [x] 5.1 Run `dotnet build` — zero compilation errors
- [x] 5.2 Run `dotnet test` — all tests pass (0 failures)
- [x] 5.3 Verify no remaining references to `CreditoMensajes` or `PruebasDemo.Resources` elsewhere in solution
