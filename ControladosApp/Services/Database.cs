using ControladosApp.Models;
using SQLite;

namespace ControladosApp.Services;

public static class Database
{
    static SQLiteAsyncConnection db;

    public static async Task Init()
    {
        if (db != null) return;

        try
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "qrcodes.db");
            db = new SQLiteAsyncConnection(dbPath);

            await db.CreateTableAsync<QRCodeRequisicao>();
            await db.CreateTableAsync<QRCodeEntrada>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
            await AlertService.ShowAlert("Erro de Banco de Dados", $"Erro ao inicializar o banco de dados: {ex.Message}");
            // Aqui você pode fazer um DisplayAlert se quiser avisar visualmente
        }
    }

    public static async Task<int> SaveRequisicaoQRCode(QRCodeRequisicao item)
    {
        try
        {
            return await db.InsertAsync(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar QRCode de Requisição: {ex.Message}");
            await AlertService.ShowAlert("Erro de Banco de Dados", $"Erro ao salvar QRCode de Requisição: {ex.Message}");
            return 0; // Retorna 0 para indicar falha
        }
    }

    public static async Task<int> SaveEntradaQRCode(QRCodeEntrada item)
    {
        try
        {
            return await db.InsertAsync(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar QRCode de Entrada: {ex.Message}");
            await AlertService.ShowAlert("Erro de Banco de Dados", $"Erro ao salvar QRCode de Entrada: {ex.Message}");
            return 0;
        }
    }

    public static async Task<List<QRCodeRequisicao>> GetRequisicoes()
    {
        try
        {
            return await db.Table<QRCodeRequisicao>().ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar Requisições: {ex.Message}");
            await AlertService.ShowAlert("Erro de Banco de Dados", $"Erro ao buscar Requisições: {ex.Message}");
            return []; // Retorna lista vazia
        }
    }

    public static async Task<List<QRCodeEntrada>> GetEntradas()
    {
        try
        {
            return await db.Table<QRCodeEntrada>().ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar Entradas: {ex.Message}");
            await AlertService.ShowAlert("Erro de Banco de Dados", $"Erro ao buscar Entradas: {ex.Message}");
            return [];
        }
    }

    public static async Task DeletarRequisicoesAsync(IEnumerable<QRCodeRequisicao> requisicoes)
    {
        await Init(); // Garante que o DB está inicializado
        foreach (var item in requisicoes)
        {
            await db.DeleteAsync(item);
        }
    }

    public static async Task DeletarEntradasAsync(IEnumerable<QRCodeEntrada> entradas)
    {
        await Init(); // Garante que o DB está inicializado
        foreach (var item in entradas)
        {
            await db.DeleteAsync(item);
        }
    }

    public static Task<int> DeletarEntradaAsync(QRCodeEntrada entrada)
    {
        return db.DeleteAsync(entrada);
    }

    public static Task<int> DeletarRequisicaoAsync(QRCodeRequisicao requisicao)
    {
        return db.DeleteAsync(requisicao);
    }

    public static Task<List<QRCodeEntrada>> GetEntradasNaoSincronizadas()
    {
        return db.Table<QRCodeEntrada>().Where(e => !e.Sincronizado).ToListAsync();
    }

    public static Task<List<QRCodeRequisicao>> GetRequisicoesNaoSincronizadas()
    {
        return db.Table<QRCodeRequisicao>().Where(r => !r.Sincronizado).ToListAsync();
    }

    public static async Task MarcarEntradasComoSincronizadas(List<QRCodeEntrada> entradas)
{
    foreach (var entrada in entradas)
    {
        entrada.Sincronizado = true;
        await db.UpdateAsync(entrada);
    }
}

public static async Task MarcarRequisicoesComoSincronizadas(List<QRCodeRequisicao> requisicoes)
{
    foreach (var requisicao in requisicoes)
    {
        requisicao.Sincronizado = true;
        await db.UpdateAsync(requisicao);
    }
}
}
