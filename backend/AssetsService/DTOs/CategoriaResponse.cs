namespace AssetsService.DTOs;

// Respuesta del listado de categorias (las 4 de AGENTS.md).
public class CategoriaResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int VidaUtilAnios { get; set; }
}