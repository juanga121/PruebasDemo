# Proposal: Extract Hardcoded Strings

## Intent

Eliminate hardcoded strings and magic numbers by referencing existing resource files (`Mensajes.resx`) and introducing named constants. Reduces duplication, improves maintainability, and aligns with the pattern already established by `Mensajes.resx` and `LogTemplates.cs`.

## Scope

### In Scope
- Consolidate `CreditoMensajes.resx` (API) into `Mensajes.resx` (Application), update controller references
- Replace hardcoded assertion strings in `CreditosServiceTest.cs` with `Mensajes.*` references
- Extract magic numbers in unit tests (`Monto`, `Saldo`, `MontoPago`, `TasaInteres`, `Meses`) into named constants
- Extract equivalent magic numbers in integration tests
- Extract hardcoded log message `"Error no controlado"` from `ExceptionMiddleware.cs`
- Extract hardcoded config strings from `Program.cs` (CORS policy name, origin URL, log path, output templates)
- Extract `"Testing"` / `"PruebasDemo_TestingDb"` from `DataBaseConfiguration.cs` & `CustomWebApplicationFactory.cs`
- All existing tests must continue passing

### Out of Scope
- Adding new tests or test scenarios
- Refactoring architecture or business logic
- Adding localization/internationalization support
- Splitting tests into new files

## Capabilities

### New Capabilities
None — pure refactor, no new spec-level behavior.

### Modified Capabilities
None — no spec-level requirements change, only implementation.

## Approach

1. **Resource consolidation**: Remove `CreditoMensajes.resx` + `.Designer.cs`, update `CreditoController` to use `Mensajes.*` equivalents
2. **Test strings**: Replace hardcoded assertion strings with `Mensajes.*` in `CreditosServiceTest.cs`
3. **Test constants**: Add a shared `TestConstants` class in the test project for magic numeric values
4. **Config constants**: Add a `Constants/` class in API project for `"Testing"`, `"AllowAll"`, origins, DB name, log path, output templates
5. **Log message**: Move `"Error no controlado"` to `LogTemplates.cs` as a `const`

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `PruebasDemo/Resources/CreditoMensajes.resx` | Removed | Duplicate, consolidated into Mensajes.resx |
| `PruebasDemo/Resources/CreditoMensajes.Designer.cs` | Removed | Duplicate, consolidated |
| `PruebasDemo/Controllers/CreditoController.cs` | Modified | Switch to `Mensajes.*` |
| `PruebaDemoTest/.../CreditosServiceTest.cs` | Modified | Use `Mensajes.*` + constants |
| `PruebaDemoTest/.../CreditosIntegrationTest.cs` | Modified | Use constants |
| `PruebasDemo/Middlewares/ExceptionMiddleware.cs` | Modified | Log constant |
| `PruebasDemo/Program.cs` | Modified | Config constants |
| `PruebasDemo/Configuration/DataBaseConfiguration.cs` | Modified | Env/DB constants |
| `PruebaDemoTest/.../CustomWebApplicationFactory.cs` | Modified | Env constant |
| `PruebasDemo.Application/Resources/Constants/LogTemplates.cs` | Modified | Add log message const |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Consolidation breaks controller response messages | Low | Run tests, verify response strings unchanged |
| Magic number replacement changes test semantics | Low | Values identical, only moved to const |
| Designer.cs regeneration after .resx removal | Low | Rebuild project after removal |

## Rollback Plan

`git revert` the single commit. All changes are mechanical with zero business logic change — no data migration or config rollback needed.

## Dependencies

None.

## Success Criteria

- [ ] `dotnet test` passes with 0 failures
- [ ] No hardcoded strings remain in `CreditosServiceTest.cs`
- [ ] `CreditoMensajes.resx` removed, controller uses `Mensajes.*`
- [ ] All magic numbers in test files extracted to constants
- [ ] `"Error no controlado"` replaced with `LogTemplates` constant
- [ ] Hardcoded config strings (CORS, log path, env name, DB name) extracted to constants
