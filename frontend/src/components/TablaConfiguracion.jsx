import { formatearFecha } from '../api'

export default function TablaConfiguracion({ configuracion }) {
  if (!configuracion) return null

  return (
    <div className="card tabla-configuracion">
      <h2>Información del sistema</h2>
      <table>
        <tbody>
          <tr>
            <th>Nombre del sistema</th>
            <td>{configuracion.nombreSistema}</td>
          </tr>
          <tr>
            <th>Nombres y apellidos</th>
            <td>{configuracion.nombresApellidos}</td>
          </tr>
          <tr>
            <th>Versión</th>
            <td>{configuracion.version}</td>
          </tr>
          <tr>
            <th>Fecha</th>
            <td>{formatearFecha(configuracion.fecha)}</td>
          </tr>
        </tbody>
      </table>
    </div>
  )
}