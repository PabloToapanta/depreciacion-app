const AUTH_URL = 'http://localhost:5001'
const ASSETS_URL = 'http://localhost:5002'
const TOKEN_KEY = 'depreciacion_token'

// ---------- Sesion (token en localStorage) ----------
export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token) {
  localStorage.setItem(TOKEN_KEY, token)
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY)
}

// ---------- Helpers de fetch ----------
async function request(url, options = {}) {
  const res = await fetch(url, options)

  if (!res.ok) {
    let mensaje = `Error ${res.status}`
    try {
      const body = await res.json()
      if (body.mensaje) mensaje = body.mensaje
    } catch {
      // el cuerpo no es JSON (o esta vacio)
    }
    throw new Error(mensaje)
  }

  return res.json()
}

function authHeaders() {
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${getToken()}`,
  }
}

// ---------- AuthService (puerto 5001) ----------
export async function login(username, password) {
  return request(`${AUTH_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  })
}

// ---------- AssetsService (puerto 5002) ----------
export async function listarCategorias() {
  return request(`${ASSETS_URL}/api/categorias`, { headers: authHeaders() })
}

export async function listarActivos() {
  return request(`${ASSETS_URL}/api/activos`, { headers: authHeaders() })
}

export async function crearActivo(activo) {
  return request(`${ASSETS_URL}/api/activos`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify(activo),
  })
}

export async function obtenerDepreciacion(id) {
  return request(`${ASSETS_URL}/api/activos/${id}/depreciacion`, {
    headers: authHeaders(),
  })
}

// ---------- Formateo ----------
export function formatearFecha(iso) {
  if (!iso) return ''
  const fecha = new Date(iso)
  if (Number.isNaN(fecha.getTime())) return iso
  return fecha.toLocaleDateString('es-ES', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

export function formatearMoneda(monto) {
  return Number(monto).toFixed(2)
}