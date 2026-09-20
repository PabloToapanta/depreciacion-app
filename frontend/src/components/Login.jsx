import { useState } from 'react'
import { login } from '../api'

export default function Login({ onLogin }) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const data = await login(username.trim(), password)
      onLogin(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="card login-card">
      <h1>Depreciación App</h1>
      <p className="subtitulo">
        Inicia sesión para registrar activos y calcular su depreciación.
      </p>

      <form onSubmit={handleSubmit}>
        <div className="campo">
          <label htmlFor="username">Usuario</label>
          <input
            id="username"
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="admin"
            autoComplete="username"
            required
          />
        </div>

        <div className="campo">
          <label htmlFor="password">Contraseña</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="••••••••"
            autoComplete="current-password"
            required
          />
        </div>

        {error && <p className="error" aria-live="polite">{error}</p>}

        <button className="boton primario" type="submit" disabled={loading}>
          {loading ? 'Ingresando…' : 'Ingresar'}
        </button>
      </form>
    </div>
  )
}