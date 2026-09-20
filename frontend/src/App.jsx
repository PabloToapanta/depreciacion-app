import { useEffect, useState } from 'react'
import {
  clearToken,
  getToken,
  listarActivos,
  listarCategorias,
  obtenerDepreciacion,
  setToken,
} from './api'
import Login from './components/Login'
import ActivoForm from './components/ActivoForm'
import DepreciacionTable from './components/DepreciacionTable'
import './App.css'

function App() {
  const [sesion, setSesion] = useState(() => {
    const token = getToken()
    return token ? { token, usuario: localStorage.getItem('depreciacion_usuario') } : null
  })
  const [categorias, setCategorias] = useState([])
  const [activos, setActivos] = useState([])
  const [tabla, setTabla] = useState(null)
  const [cargando, setCargando] = useState(false)
  const [errorGlobal, setErrorGlobal] = useState('')

  useEffect(() => {
    if (!sesion) return
    let activo = true

    async function cargarDatos() {
      setCargando(true)
      setErrorGlobal('')
      try {
        const [cats, acts] = await Promise.all([listarCategorias(), listarActivos()])
        if (!activo) return
        setCategorias(cats)
        setActivos(acts)
      } catch (err) {
        if (!activo) return
        setErrorGlobal(err.message)
      } finally {
        if (activo) setCargando(false)
      }
    }

    cargarDatos()
    return () => {
      activo = false
    }
  }, [sesion])

  function handleLogin(data) {
    setToken(data.token)
    localStorage.setItem('depreciacion_usuario', data.nombreCompleto || data.username)
    setSesion({ token: data.token, usuario: data.nombreCompleto || data.username })
  }

  function handleLogout() {
    clearToken()
    localStorage.removeItem('depreciacion_usuario')
    setSesion(null)
    setTabla(null)
  }

  async function cargarDepreciacion(id) {
    setCargando(true)
    setErrorGlobal('')
    try {
      const data = await obtenerDepreciacion(id)
      setTabla(data)
    } catch (err) {
      setErrorGlobal(err.message)
    } finally {
      setCargando(false)
    }
  }

  async function handleActivocreado() {
    setErrorGlobal('')
    try {
      const acts = await listarActivos()
      setActivos(acts)
    } catch (err) {
      setErrorGlobal(err.message)
    }
  }

  if (!sesion) {
    return <Login onLogin={handleLogin} />
  }

  return (
    <div className="app">
      <header className="barra-superior">
        <h1>Depreciación App</h1>
        <div className="usuario">
          <span className="nombre-usuario">{sesion.usuario}</span>
          <button className="boton enlace" type="button" onClick={handleLogout}>
            Cerrar sesión
          </button>
        </div>
      </header>

      {errorGlobal && <p className="error banner">{errorGlobal}</p>}

      <main className="contenido">
        <section className="columna-izquierda">
          {cargando && !categorias.length ? (
            <p className="aviso">Cargando datos…</p>
          ) : (
            <ActivoForm
              categorias={categorias}
              onCreado={(activo) => {
                cargarDepreciacion(activo.id)
                handleActivocreado()
              }}
            />
          )}

          {activos.length > 0 && (
            <div className="card lista-activos no-print">
              <h2>Activos registrados</h2>
              <ul>
                {activos.map((a) => (
                  <li key={a.id}>
                    <button
                      className="item-activo"
                      type="button"
                      onClick={() => cargarDepreciacion(a.id)}
                    >
                      <span className="nombre-activo">{a.nombre}</span>
                      <span className="detalle-activo">
                        {a.categoriaNombre} · ${Number(a.precioCompra).toFixed(2)}
                      </span>
                    </button>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </section>

        <section className="columna-derecha">
          {cargando && <p className="aviso">Calculando…</p>}
          {!cargando && tabla && <DepreciacionTable tabla={tabla} />}
          {!cargando && !tabla && (
            <p className="aviso">
              Registra un activo o selecciona uno de la lista para ver su tabla de
              depreciación.
            </p>
          )}
        </section>
      </main>
    </div>
  )
}

export default App