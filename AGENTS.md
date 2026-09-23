# Depreciacion App

Proyecto académico, en pareja. Los dos integrantes conocen Java y SQL, NO conocen
React ni .NET Core. Sin contenedores (confirmado por el docente).

## Stack

- Frontend: React (Vite)
- Backend: .NET Core Web API, dos servicios independientes (arquitectura de microservicios)
- Base de datos: SQL Server
- Autenticación: JWT

## Arquitectura de sistema (microservicios)

Dos servicios .NET Core independientes, cada uno con su propio proyecto y puerto, sin
API Gateway ni contenedores. React consume ambos directamente por HTTP.

1. **AuthService** (puerto 5001): login, genera y firma JWT.
2. **AssetsService** (puerto 5002): CRUD de activos, cálculo de depreciación, valida
   el JWT en cada request (mismo secreto que AuthService).

Cada servicio tiene SU PROPIA base de datos SQL Server (AuthDB, AssetsDB).

Justificación de microservicios (las 4 características que pide el docente):
- **Business Function**: AuthService = autenticación, AssetsService = gestión y
  cálculo de activos. Cada uno resuelve una sola responsabilidad de negocio.
- **API Communication**: ambos exponen API REST; el frontend habla con cada uno por
  HTTP, nunca comparten memoria ni proceso.
- **Independent Operation**: cada uno corre con su propio `dotnet run`, su propio
  puerto, su propia base de datos — se puede apagar uno sin tumbar al otro.
- **Request Handling**: el frontend decide a qué servicio enrutar según la operación
  (login → AuthService; activos/depreciación → AssetsService); no hay gateway, el
  enrutamiento es explícito en `frontend/src/api.js`.

## Onion Architecture dentro de cada microservicio

Cada microservicio son 4 PROYECTOS .NET separados (no solo carpetas), para que la
regla de dependencia se cumpla a nivel de compilador:

```
<Servicio>.Domain/            <- SIN dependencias de nadie
  Entities/                     (Activo, Categoria, Usuario)
  Services/                     (DepreciacionService: la regla de negocio pura)

<Servicio>.Application/       <- referencia SOLO a Domain
  Interfaces/                   ("puertos": IActivoRepository, ICategoriaRepository...)
  DTOs/                         (contrato JSON de entrada/salida)
  UseCases/                     (orquestan: llaman a Domain + a las interfaces)

<Servicio>.Infrastructure/    <- referencia a Application + Domain
  Persistence/                  (DbContext + implementaciones de las interfaces,
                                  ej. ActivoRepository : IActivoRepository)

<Servicio>.Api/                <- referencia a los tres
  Controllers/                  (HTTP <-> UseCases, nada de lógica)
  Program.cs                    (Dependency Injection: registra cada interfaz con su
                                  implementación concreta)
```

Regla de dependencia (innegociable, se valida con `dotnet list <proyecto> reference`):
Domain no referencia a nadie. Application solo a Domain. Infrastructure a Application
y Domain. Api a los tres.

Por qué Onion y no capas simples: permite invertir la dependencia hacia la base de
datos (Application define la interfaz, Infrastructure la implementa), así el Domain
y el Application nunca importan `Microsoft.EntityFrameworkCore` — se puede cambiar el
motor de persistencia sin tocar la lógica de negocio ni los casos de uso.

Comandos para crear la estructura de un servicio (repetir cambiando el nombre):

```
cd backend/AssetsService
dotnet new classlib -n AssetsService.Domain
dotnet new classlib -n AssetsService.Application
dotnet new classlib -n AssetsService.Infrastructure
dotnet new webapi -n AssetsService.Api -controllers

dotnet add AssetsService.Application reference AssetsService.Domain
dotnet add AssetsService.Infrastructure reference AssetsService.Application AssetsService.Domain
dotnet add AssetsService.Api reference AssetsService.Application AssetsService.Infrastructure AssetsService.Domain

dotnet add AssetsService.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
```

## Reglas del refactor (el agente `refactor` debe seguir esto sin excepción)

1. Primero corregir la fórmula de depreciación (ver sección siguiente) DENTRO del
   `DepreciacionService` actual, antes de mover nada de proyecto. Correr `dotnet test`.
2. Crear los proyectos de Onion Architecture, migrar clase por clase, validando
   `dotnet build` + `dotnet test` después de cada paso.
3. No mezclar en un mismo commit la corrección de fórmula con el movimiento de
   proyectos — deben poder aislarse si algo falla.
4. Trabajar solo en `feature/onion-architecture`, creada desde `develop` actualizado.
5. Commits atómicos, mensajes `refactor: ...` o `fix: ...` según corresponda.

## Fórmula de depreciación (CORREGIDA — tabla anual con resto parcial en meses)

La tabla es por AÑO COMPLETO, no por mes. Si la fecha de corte no coincide con un
aniversario exacto de la compra, la última fila es un período PARCIAL calculado en
meses completos (no un año completo), usando la depreciación mensual solo para ese
resto.

Valor depreciable: el activo termina su vida útil en el 10% de su valor de compra.
```
VD  = VALOR_COMPRA - VALOR_COMPRA * 0.10
VDA = VD / VIDA_UTIL_ANIOS       (depreciación por año completo)
VDM = VDA / 12                   (depreciación por mes, SOLO para la fila parcial final)
```

Vida útil por tipo (los únicos 4 tipos válidos): Tecnología 3 años, Vehículos 5 años,
Edificios 20 años, Muebles 3 años.

Algoritmo:
1. `finVidaUtil = fechaCompra.AddYears(vidaUtilAnios)`
2. `limite = min(fechaCorte, finVidaUtil)`
3. Fila inicial: fecha de compra, depreciación 0, acumulado 0, valor real = precio.
4. Para cada aniversario `n = 1, 2, 3...` (`fechaCompra.AddYears(n)`) que sea `<= limite`:
   agregar una fila con depreciación = VDA (año completo), acumular, valor real =
   precio - acumulado. Si el aniversario coincide exactamente con `limite`, terminar
   (no hay resto parcial).
5. Si sobra un período menor a un año entre el último aniversario registrado (o la
   fecha de compra si no hubo ningún año completo) y `limite`: contar los MESES
   COMPLETOS de calendario en ese resto (no días, no meses de 30 días fijos — usar
   diferencia de año/mes y ajustar por el día), y agregar una última fila con
   depreciación = meses × VDM.

Conteo de meses completos entre dos fechas `desde` y `hasta`:
```
meses = (hasta.Year - desde.Year) * 12 + (hasta.Month - desde.Month)
si hasta.Day < desde.Day: meses = meses - 1
```

### Ejemplo de verificación oficial (usar para validar el cálculo)

Activo Tecnología, $900, comprado 22/09/2026, fecha de corte 03/08/2027.
- VDA = (900 × 0.9) / 3 = 270. VDM = 22.5.
- Entre 22/09/2026 y 03/08/2027 no cabe ni un año completo (el primer aniversario,
  22/09/2027, es posterior a la fecha de corte).
- Meses completos entre 22/09/2026 y 03/08/2027 = 10 (el mes 11 cerraría el
  22/08/2027, posterior al corte).
- Tabla resultante: fila inicial (22/09/2026: 0, 0, 900) y una única fila final
  (03/08/2027: depreciación 225, acumulado 225, valor real 675).

### Segundo ejemplo (con años completos + resto)

Activo Edificios, $100000, comprado 01/01/2023, corte 01/06/2026.
- VDA = (100000 × 0.9) / 20 = 4500. VDM = 375.
- Años completos que caben: 01/01/2024, 01/01/2025, 01/01/2026 (3 años completos).
- Resto: de 01/01/2026 a 01/06/2026 = 5 meses completos → fila final con
  depreciación = 5 × 375 = 1875.
- Tabla: 5 filas (inicial + 3 años completos + 1 fila parcial).

## Datos de entrada del proceso de depreciación

1. Tipo de activo (uno de los 4 válidos)
2. Fecha de compra
3. Precio del activo
4. Fecha de corte (obligatoria, la ingresa el usuario)

## Salida esperada: tabla anual (con posible fila final parcial)

Columnas: **Fecha | Valor Depreciación | Valor Depreciación Acumulado | Valor Real**

## Export a PDF

La tabla final debe poder exportarse a PDF desde el frontend (actualmente con
`window.print()` y reglas CSS `@media print`).

## Convenciones de código

- C#: PascalCase para clases/métodos, camelCase para variables locales.
- React: componentes funcionales con hooks, nada de clases.
- Sin Redux, sin TypeScript, sin librerías de estado complejas.

## Comandos

- Backend (por servicio, dentro de `<Servicio>.Api/`): `dotnet run`
- Frontend: `npm run dev`
- Pruebas backend: `dotnet test` (dentro de `backend/`, corre toda la solución)
- Pruebas frontend: `npm run test` (dentro de `frontend/`)
- Verificar la regla de dependencia: `dotnet list <Proyecto>.csproj reference`

## Flujo de trabajo con Git (Git Flow simplificado)

- `main`: siempre la versión funcional, la que se entrega al final.
- `develop`: rama de integración diaria, aquí se juntan persona A y B.
- `feature/*`: una por tarea. Ejemplos: `feature/auth-login`, `feature/onion-architecture`,
  `feature/fix-formula-depreciacion`, `feature/frontend-tabla`, `feature/export-pdf`.
- `release/*`: se usa una sola vez, al final, cuando `develop` ya esté estable.
- `hotfix/*`: solo si aparece un bug grave después de mergear a `main`.

Flujo diario:
1. `git checkout develop && git pull`
2. `git checkout -b feature/nombre-tarea`
3. Commits pequeños y descriptivos (`feat:`, `fix:`, `refactor:`, `chore:`)
4. `git push origin feature/nombre-tarea`
5. Pull request hacia `develop`, revisada por el compañero antes de mergear.

Verificación de integración (el coordinador debe revisarla en cada sesión):
- Commits atómicos, un commit = un cambio lógico.
- Nunca commitear directo en `main`; `main` solo recibe código vía `release/*`.
- Antes de crear una feature branch: `git checkout develop && git pull`.
- Los archivos de coordinación (`AGENTS.md`, `TASKS.md`, `opencode.json`, `.opencode/`,
  `Docs/`, `informe/`) el coordinador los actualiza directamente en `develop`.
- Borrar ramas feature locales y remotas después de mergear.

## Documentación (carpeta Docs) e informe (carpeta informe)

- `Docs/` — documentación operativa: arquitectura, modelo de datos, guía de lectura de
  código, guía de defensa, registro de sesiones. Mantenida por el coordinador y el
  agente `documentador`.
- `informe/` — el informe técnico formal en LaTeX (`informe/main.tex`) y los diagramas
  Mermaid fuente (`informe/diagramas/*.mmd`). Mantenido por el agente `informe`. El
  informe solo se genera como código fuente — nunca se compila automáticamente, ni los
  diagramas Mermaid se renderizan automáticamente; eso lo hace el usuario.

## Estado del proyecto

Ver `TASKS.md` para el tablero de tareas y quién está haciendo qué.
