using ControladosApp.Models;
using System.Text;
using System.Text.Json;

namespace ControladosApp.Services;

public static class ApiClient
{
    private static readonly HttpClient client = new();
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };

    /*
    public static async Task<ApiResponse> EnviarRequisicoesAsync(List<QRCodeRequisicao> requisicoes)
    {
        try
        {
            var json = JsonSerializer.Serialize(requisicoes);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.cipolatti.com.br:44366/api/VolumeControlado/ControladosRequisicao", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<ApiResponse>(responseContent, jsonOptions);

            return resultado ?? new ApiResponse { Sucesso = false, Erro = "Resposta nula da API." };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Sucesso = false,
                Erro = "Falha ao comunicar com a API.",
                Mensagem = ex.Message
            };
        }
    }
    
    public static async Task<ApiResponse> EnviarEntradasAsync(List<QRCodeEntrada> entradas)
    {
        try
        {
            var json = JsonSerializer.Serialize(entradas);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.cipolatti.com.br:44366/api/VolumeControlado/ControladosRetorno", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<ApiResponse>(responseContent, jsonOptions);

            return resultado ?? new ApiResponse { Sucesso = false, Erro = "Resposta nula da API." };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Sucesso = false,
                Erro = "Falha ao comunicar com a API.",
                Mensagem = ex.Message
            };
        }
    }
    */

    public static async Task<ApiResponse> EnviarRequisicoesAsync(List<QRCodeRequisicao> requisicoes)
    {
        try
        {
            var json = JsonSerializer.Serialize(requisicoes);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.cipolatti.com.br:44366/api/VolumeControlado/ControladosRequisicao", content);
            var body = await response.Content.ReadAsStringAsync();

            var resultado = JsonSerializer.Deserialize<ApiResponse>(body);

            if (response.IsSuccessStatusCode && resultado?.Sucesso == true)
            {
                await Database.MarcarRequisicoesComoSincronizadas(requisicoes);
            }

            return resultado ?? new ApiResponse
            {
                Sucesso = false,
                Erro = "Resposta nula da API."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Sucesso = false,
                Erro = "Erro ao enviar requisições.",
                Mensagem = ex.Message
            };
        }
    }

    public static async Task<ApiResponse> EnviarEntradasAsync(List<QRCodeEntrada> entradas)
    {
        try
        {
            var json = JsonSerializer.Serialize(entradas);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.cipolatti.com.br:44366/api/VolumeControlado/ControladosRetorno", content);

            var body = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<ApiResponse>(body);

            if (response.IsSuccessStatusCode && resultado?.Sucesso == true)
            {
                await Database.MarcarEntradasComoSincronizadas(entradas);
            }

            return resultado ?? new ApiResponse { Sucesso = false, Erro = "Resposta nula da API." };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Sucesso = false,
                Erro = "Erro ao enviar entradas.",
                Mensagem = ex.Message
            };
        }
    }
}

