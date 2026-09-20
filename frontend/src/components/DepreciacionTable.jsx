import { formatearFecha, formatearMoneda } from '../api'

export default function DepreciacionTable({ tabla }) {
  if (!tabla) return null

  const ultimaFila = tabla.filas[tabla.filas.length - 1]
  const totalFilas = tabla.filas.length

  return (
    <div className="card tabla-depreciacion">
      <div id="zona-impresion">
        <header className="cabecera-tabla">
          <h2>Tabla de depreciación — {tabla.nombre}</h2>
          <div className="datos-activo">
            <span>
              <strong>Tipo:</strong> {tabla.categoria}
            </span>
            <span>
              <strong>Precio de compra:</strong> ${formatearMoneda(tabla.precioCompra)}
            </span>
            <span>
              <strong>Fecha de compra:</strong> {formatearFecha(tabla.fechaCompra)}
            </span>
            <span>
              <strong>Fecha de corte:</strong> {formatearFecha(tabla.fechaCorte)}
            </span>
            <span>
              <strong>Valor real final:</strong> ${formatearMoneda(ultimaFila.valorReal)}
            </span>
          </div>
        </header>

        <table>
          <thead>
            <tr>
              <th>#</th>
              <th>Fecha</th>
              <th>Valor Depreciación</th>
              <th>Depreciación Acumulada</th>
              <th>Valor Real</th>
            </tr>
          </thead>
          <tbody>
            {tabla.filas.map((fila, i) => (
              <tr key={i}>
                <td>{i + 1}</td>
                <td>{formatearFecha(fila.fecha)}</td>
                <td>${formatearMoneda(fila.valorDepreciacion)}</td>
                <td>${formatearMoneda(fila.valorDepreciacionAcumulado)}</td>
                <td>${formatearMoneda(fila.valorReal)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <p className="nota-tabla">
          {totalFilas} {totalFilas === 1 ? 'fila' : 'filas'} (mensuales, desde el mes
          de compra hasta la fecha de corte o el fin de la vida útil).
        </p>
      </div>

      <div className="no-print acciones-tabla">
        <button className="boton secundario" type="button" onClick={() => window.print()}>
          Exportar a PDF
        </button>
      </div>
    </div>
  )
}