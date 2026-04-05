# ClienteService.Crear

## Unidad bajo prueba

- Clase: `ClienteService`
- Metodo: `Crear(bool esConsumidorFinal, string nit, string razonSocial, string? correoElectronico)`
- Dependencias simuladas:
  - `IClienteRepository`
  - `IValidacion<Cliente>`

## Objetivo

Verificar que el servicio registre un cliente correctamente y que responda de forma adecuada cuando:

- la validacion falla
- ya existe un cliente con el mismo NIT
- el repositorio no inserta el registro
- todo el flujo es exitoso

## Flujo de control simplificado

```text
Inicio
  |
Construir cliente
  |
Validar cliente
  |---- falla ----> retornar error de validacion
  |
Validar duplicado
  |---- falla ----> retornar error de duplicado
  |
Insertar cliente
  |---- falla ----> retornar error de insercion
  |
Retornar OK
```

## Complejidad ciclomatica

Decisiones principales del metodo:

1. `validacion.IsFailure`
2. `validacionDuplicado.IsFailure`
3. `_repository.Insert(cliente) <= 0`

Formula:

```text
M = P + 1
M = 3 + 1
M = 4
```

Complejidad ciclomatica: `4`

## Caminos independientes

1. La validacion falla.
2. La validacion pasa, pero existe un cliente duplicado.
3. La validacion y duplicado pasan, pero el repositorio no inserta.
4. La validacion y duplicado pasan, y el repositorio inserta correctamente.

## Casos de prueba derivados

| ID | Camino | Configuracion de mocks | Resultado esperado |
|----|--------|------------------------|--------------------|
| CP-01 | Falla validacion | `Validar` retorna `Fail("Error de validacion.")` | El metodo retorna failure con ese error y no inserta |
| CP-02 | Falla por duplicado | `Validar` retorna `Ok`, `ObtenerPorNit` retorna un cliente existente | El metodo retorna failure con mensaje de duplicado y no inserta |
| CP-03 | Falla insercion | `Validar` retorna `Ok`, `ObtenerPorNit` retorna `null`, `Insert` retorna `0` | El metodo retorna failure con mensaje de insercion |
| CP-04 | Flujo exitoso | `Validar` retorna `Ok`, `ObtenerPorNit` retorna `null`, `Insert` retorna `1` | El metodo retorna `Ok` |

## Observaciones

- Este metodo es un buen candidato para pruebas unitarias porque concentra reglas de negocio y depende de interfaces.
- El uso de `Moq` permite aislar la logica del servicio sin conectarse a base de datos.
- La cobertura de estos caminos ayuda a justificar la prueba desde el enfoque de caja blanca visto en clase.
