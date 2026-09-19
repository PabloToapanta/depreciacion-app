# Tablero de tareas — Depreciacion App

Formato: cada tarea tiene dueño, estado y fecha. El agente coordinador debe leer y
actualizar este archivo en cada sesión. Estados válidos: `todo`, `doing`, `done`, `blocked`.

## Día 1 — Setup y arquitectura (ambos, mob programming)

- [x] done | ambos | Instalar .NET SDK, SQL Server, Node.js
- [x] done | ambos | Crear tablas Usuarios y Activos en SQL Server
- [x] done | ambos | Crear proyecto AuthService (.NET) vacío que responde
- [x] done | ambos | Crear proyecto AssetsService (.NET) vacío que responde
- [x] done | ambos | Crear proyecto React con Vite vacío
- [x] done | ambos | Repo de GitHub creado, primer commit

## Día 2 — Login con JWT

- [ ] todo | Leslie | Endpoint de login en AuthService (valida usuario/clave)
- [ ] todo | Leslie | Generación y firma de JWT
- [ ] todo | Pablo | Tutorial rápido de React (useState, fetch)
- [ ] todo | Pablo | Pantalla de login en React que consume AuthService

## Día 3 — Lógica de depreciación + formulario

- [ ] todo | Leslie | Endpoint POST activo en AssetsService
- [ ] todo | Leslie | Endpoint GET tabla de depreciación (aplica fórmulas de AGENTS.md)
- [ ] todo | Pablo | Formulario de activo (tipo, precio, fecha compra, fecha corte)
- [ ] todo | Pablo | Tabla en React que muestra el resultado

## Día 4 — Integración + PDF (ambos)

- [ ] todo | ambos | JWT viajando en header Authorization desde React
- [ ] todo | ambos | AssetsService valida el JWT antes de responder
- [ ] todo | ambos | Botón de exportar a PDF funcionando
- [ ] todo | ambos | Resolver CORS entre servicios

## Día 5 — Pruebas y documentación (ambos)

- [ ] todo | ambos | Probar con el ejemplo de verificación de AGENTS.md
- [ ] todo | ambos | Probar 2-3 casos con los 4 tipos de activo
- [ ] todo | ambos | Diagrama de arquitectura simple para el informe
- [ ] todo | ambos | Ensayar demo
