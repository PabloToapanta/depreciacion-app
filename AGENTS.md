# Depreciacion App

Proyecto académico, en pareja, plazo de 5 días. Los dos integrantes conocen Java y SQL,
NO conocen React ni .NET Core. Sin contenedores (confirmado por el docente).

## Stack

- Frontend: React (Vite)
- Backend: .NET Core Web API, dos servicios independientes (arquitectura de microservicios simplificada)
- Base de datos: SQL Server
- Autenticación: JWT

## Arquitectura

Dos servicios .NET Core independientes, cada uno con su propio proyecto y puerto,
sin API Gateway ni contenedores. React consume ambos directamente por HTTP.

1. **AuthService** (puerto 5001)
   - Login de usuario/contraseña
   - Genera y firma JWT
   - Tabla `Usuarios` (usuario, hash de contraseña con BCrypt)

2. **AssetsService** (puerto 5002)
   - CRUD de activos
   - Cálculo de la tabla de depreciación
   - Valida el JWT en cada request (mismo secreto/clave que AuthService)
   - Tabla `Activos` (tipo, precio_compra, fecha_compra)

Cada servicio tiene SU PROPIA base de datos SQL Server (no comparten base):

- AuthService → base `AuthDB` con la tabla `Usuarios`
- AssetsService → base `AssetsDB` con la tabla `Activos`

## Reglas de negocio (definición oficial del proyecto — NO usar otras fuentes ni otros porcentajes)

Valor depreciable (VD): el activo termina su vida útil en el 10% de su valor de compra,
sin importar el tipo.

```
VD = VALOR_COMPRA - VALOR_COMPRA * 0.10
```

Vida útil por tipo de activo (los únicos 4 tipos válidos):

| Tipo        | Vida útil |
|-------------|-----------|
| Tecnología  | 3 años    |
| Vehículos   | 5 años    |
| Edificios   | 20 años   |
| Muebles     | 3 años    |

Depreciación anual y mensual:

```
VDA = VD / TIEMPO_DE_VIDA_UTIL   (en años)
VDM = VDA / 12
```

## Datos de entrada del proceso de depreciación

1. Tipo de activo (uno de los 4 de la tabla)
2. Fecha de compra
3. Precio del activo
4. Fecha de corte (OBLIGATORIA): el usuario siempre la ingresa. La tabla se calcula
   desde el mes de compra hasta la fecha de corte (o hasta el fin de la vida útil si
   la fecha de corte la supera).

## Salida esperada: tabla mensual

Columnas: **Fecha | Valor Depreciación | Valor Depreciación Acumulado | Valor Real**

- La primera fila (mes de compra) tiene depreciación = 0, acumulado = 0, valor real = precio de compra.
- Cada fila siguiente salta un mes, suma VDM a la depreciación acumulada, y resta esa
  acumulada al precio de compra para el valor real.
- Se detiene en la fecha de corte, o al llegar al fin de la vida útil del activo (valor real = 10% del precio de compra).

Ejemplo de verificación (usar para probar que el cálculo está bien):
activo Tecnología, $900, comprado 01/01/2023 → VDA = 270, VDM = 22.5.
01/01/2023: dep=0, acumulado=0, real=900.00
01/02/2023: dep=22.5, acumulado=22.5, real=877.50
01/03/2023: dep=22.5, acumulado=45, real=855.00
...
01/01/2026: acumulado=810, real=90.00 (fin de vida útil, 10% de 900)

## Export a PDF

La tabla final debe poder exportarse a PDF desde el frontend (o generarse en el backend,
lo que sea más rápido de integrar con el tiempo disponible).

## Convenciones de código

- C#: PascalCase para clases/métodos, camelCase para variables locales.
- React: componentes funcionales con hooks, nada de clases.
- Sin Redux, sin TypeScript, sin librerías de estado complejas — el alcance no las necesita.

## Comandos

- Backend: `dotnet run` dentro de cada carpeta de servicio
- Frontend: `npm run dev`

## Flujo de trabajo con Git (Git Flow simplificado)

- `main`: siempre la versión funcional, la que se entrega al final.
- `develop`: rama de integración diaria, aquí se juntan persona A y B.
- `feature/*`: una por tarea del TASKS.md, sale de `develop` y vuelve a `develop`.
  Ejemplos: `feature/auth-login`, `feature/auth-jwt`, `feature/assets-calculo-depreciacion`,
  `feature/frontend-tabla`, `feature/export-pdf`.
- `release/*`: se usa una sola vez, el día 5, cuando `develop` ya esté estable — se congela
  `release/v1.0`, se hacen las últimas pruebas y correcciones ahí, y se mergea a `main` y de
  vuelta a `develop`.
- `hotfix/*`: solo si, después de mergear a `main`, aparece un bug grave antes de la
  demo/entrega. Si no pasa, no se usa — y eso también se explica en el informe.

Flujo de trabajo diario para cada tarea:

1. `git checkout develop && git pull`
2. `git checkout -b feature/nombre-tarea`
3. Trabajar, con commits pequeños y descriptivos
   (`feat: agrega endpoint de login`, `fix: corrige cálculo de depreciación mensual`)
4. `git push origin feature/nombre-tarea`
5. Pull request hacia `develop` — con solo 2 personas, el otro revisa rápido (no hace falta
   proceso pesado, pero sí que el compañero vea el código antes de mergear, para que ambos
   entiendan todo el sistema).

Verificación de integración (el coordinador DEBE revisarla en cada sesión de git):

- Commits atómicos: un commit = un cambio lógico (`feat:`, `fix:`, `chore:`). Nada de
  commits gigantes con archivos sin relación, ni mensajes vagos como "cambios" o "fix".
- Antes de cada commit: confirmar que se está en la rama correcta (`feature/*` o `develop`,
  NUNCA `main`). Si el commit se hizo en la rama equivocada, corregir antes de continuar.
- `main` NO recibe código hasta el día 5, y solo vía `release/v1.0`. Si alguien pushea a
  `main` antes, es un error de integración: detener y corregir.
- Antes de crear una feature branch: `git checkout develop && git pull` para partir de la
  versión más reciente (evita conflictos de merge al final).
- Los cambios a `develop` entran por pull request revisada por el compañero, no por push
  directo. La única excepción son los archivos de coordinación (AGENTS.md, TASKS.md,
  opencode.json, .opencode/) que el coordinador actualiza directamente en `develop`.
- Después de mergear una feature branch a `develop`: borrar la rama feature local y remota
  para no acumular ramas muertas.

El coordinador debe recordar este flujo al inicio de cada sesión y verificar que los commits
se hagan en la rama correcta.

## Estado del proyecto

Ver `TASKS.md` para el tablero de tareas y quién está haciendo qué.
