using ControladosApp.Models;
using ControladosApp.Services;
using CommunityToolkit.Maui.Alerts;
using System.Collections.ObjectModel;

namespace ControladosApp.Views;

public partial class SyncPage : ContentPage
{
    public ObservableCollection<QRCodeItem> Pendentes { get; set; } = new();

    public SyncPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarPendentes();
    }

    private async Task CarregarPendentes()
    {
        Pendentes.Clear();

        var entradas = await Database.GetEntradasNaoSincronizadas();
        foreach (var e in entradas)
            Pendentes.Add(new QRCodeItem { Barcode = e.Barcode });

        var requisicoes = await Database.GetRequisicoesNaoSincronizadas();
        foreach (var r in requisicoes)
            Pendentes.Add(new QRCodeItem { Barcode = $"[Req {r.NumeroRequisicao}] {r.Barcode}" });
    }

    private async void OnSyncClicked(object sender, EventArgs e)
    {
        SyncButton.IsEnabled = false;
        EnvioIndicator.IsVisible = true;
        EnvioIndicator.IsRunning = true;

        await Snackbar.Make("Sincronizando coletas...", null, "", TimeSpan.FromSeconds(2)).Show();

        var entradas = await Database.GetEntradasNaoSincronizadas();
        var requisicoes = await Database.GetRequisicoesNaoSincronizadas();

        var mensagens = new List<string>();
        bool falhou = false;

        if (entradas.Any())
        {
            var resultadoEntradas = await ApiClient.EnviarEntradasAsync(entradas);
            if (resultadoEntradas?.Sucesso == true)
                mensagens.Add($"Entradas sincronizadas: {resultadoEntradas.Inseridos}");
            else
            {
                falhou = true;
                mensagens.Add($"Erro em entradas: {resultadoEntradas?.Mensagem ?? resultadoEntradas?.Erro ?? "Erro desconhecido"}");
            }
        }

        if (requisicoes.Any())
        {
            var resultadoRequisicoes = await ApiClient.EnviarRequisicoesAsync(requisicoes);
            if (resultadoRequisicoes?.Sucesso == true)
                mensagens.Add($"Requisições sincronizadas: {resultadoRequisicoes.Inseridos}");
            else
            {
                falhou = true;
                mensagens.Add($"Erro em requisições: {resultadoRequisicoes?.Mensagem ?? resultadoRequisicoes?.Erro ?? "Erro desconhecido"}");
            }
        }

        EnvioIndicator.IsRunning = false;
        EnvioIndicator.IsVisible = false;
        SyncButton.IsEnabled = true;

        string finalMsg = string.Join("\n", mensagens);
        await Snackbar.Make(finalMsg, null, "", TimeSpan.FromSeconds(4)).Show();

        if (!falhou)
            await CarregarPendentes();
    }
}
