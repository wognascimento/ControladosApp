namespace ControladosApp.Models;

public class ApiResponse
{
    public bool sucesso { get; set; }
    public int? inseridos { get; set; }
    public string erro { get; set; }
    public string codigo { get; set; }
    public string mensagem { get; set; }
    public string onde { get; set; }
}
