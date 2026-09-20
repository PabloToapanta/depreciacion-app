import { useState } from 'react'
import { crearActivo } from '../api'

export default function ActivoForm({ categorias, onCreado }) {
  const [categoriaId, setCategoriaId] = useState('')
  const [nombre, setNombre] = useState('')
  const [precio, setPrecio] = useState('')
  const [fechaCompra, setFechaCompra] = useState('')
  const [fechaCorte, setFechaCorte] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')

    const precioNumero = Number(precio)
    if (precioNumero <= 0) {
      setError('El precio debe ser mayor que 0')
      return
    }
    if (new Date(fechaCorte) < new Date(fechaCompra)) {
      setError('La fecha de corte no puede ser anterior a la fecha de compra')
      return
    }

    setLoading(true)
    try {
      const activo = await crearActivo({
        categoriaId: Number(categoriaId),
        nombre: nombre.trim(),
        precioCompra: precioNumero,
        fechaCompra,
        fechaCorte,
      })
      onCreado(activo)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <form className="card form-activo" onSubmit={handleSubmit}>
      <h2>Registrar activo</h2>

      <div className="campo">
        <label htmlFor="categoria">Tipo de activo</label>
        <select
          id="categoria"
          value={categoriaId}
          onChange={(e) => setCategoriaId(e.target.value)}
          required
        >
          <option value="" disabled>
            Seleccione un tipo…
          </option>
          {categorias.map((c) => (
            <option key={c.id} value={c.id}>
              {c.nombre} (vida útil: {c.vidaUtilAnios} años)
            </option>
          ))}
        </select>
      </div>

      <div className="campo">
        <label htmlFor="nombre">Nombre del activo</label>
        <input
          id="nombre"
          type="text"
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
          placeholder="Ej. Laptop Dell"
          required
        />
      </div>

      <div className="campo">
        <label htmlFor="precio">Precio de compra</label>
        <input
          id="precio"
          type="number"
          min="0.01"
          step="0.01"
          value={precio}
          onChange={(e) => setPrecio(e.target.value)}
          placeholder="Ej. 900"
          required
        />
      </div>

      <div className="fila-campos">
        <div className="campo">
          <label htmlFor="fechaCompra">Fecha de compra</label>
          <input
            id="fechaCompra"
            type="date"
            value={fechaCompra}
            onChange={(e) => setFechaCompra(e.target.value)}
            required
          />
        </div>

        <div className="campo">
          <label htmlFor="fechaCorte">Fecha de corte</label>
          <input
            id="fechaCorte"
            type="date"
            value={fechaCorte}
            onChange={(e) => setFechaCorte(e.target.value)}
            required
          />
        </div>
      </div>

      {error && <p className="error" aria-live="polite">{error}</p>}

      <button className="boton primario" type="submit" disabled={loading}>
        {loading ? 'Guardando…' : 'Guardar y calcular'}
      </button>
    </form>
  )
}