namespace ControladosApp.Models;

public class ApiResponse
{
    public bool Sucesso { get; set; }
    public int? Inseridos { get; set; }
    public string Erro { get; set; }
    public string Codigo { get; set; }
    public string Mensagem { get; set; }
    public string Onde { get; set; }
}
