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
- [x] done | ambos | Usuarios de acceso por servicio (auth_user, assets_user) con permisos solo sobre su base

## Día 2 — Login con JWT

- [x] done | Pablo | Endpoint de login en AuthService (valida usuario/clave)
- [x] done | Pablo | Generación y firma de JWT
- [x] done | Leslie | Tutorial rápido de React (useState, fetch)
- [x] done | Leslie | Pantalla de login en React que consume AuthService

## Día 3 — Lógica de depreciación + formulario

- [x] done | Pablo | Endpoint POST activo en AssetsService
- [x] done | Pablo | Endpoint GET tabla de depreciación (aplica fórmulas de AGENTS.md)
- [x] done | Leslie | Formulario de activo (tipo, precio, fecha compra, fecha corte)
- [x] done | Leslie | Tabla en React que muestra el resultado
- [x] done | Leslie | Endpoint GET /api/categorias en AssetsService (para el select del formulario, sin hardcodear ids)

## Día 4 — Integración + PDF (ambos)

- [x] done | ambos | JWT viajando en header Authorization desde React
- [x] done | Pablo | AssetsService valida el JWT antes de responder
- [x] done | ambos | Botón de exportar a PDF funcionando (window.print + CSS de impresión)
- [x] done | Pablo | Resolver CORS entre servicios (config + verificación con curl; falta prueba final desde React)

## Día 5 — Pruebas y documentación (ambos)

- [ ] todo | ambos | Probar con el ejemplo de verificación de AGENTS.md
- [ ] todo | ambos | Probar 2-3 casos con los 4 tipos de activo
- [ ] todo | ambos | Diagrama de arquitectura simple para el informe
- [ ] todo | ambos | Ensayar demo
