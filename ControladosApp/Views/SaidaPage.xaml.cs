
using ControladosApp.Services;

namespace ControladosApp.Views;

public partial class SaidaPage : ContentPage
{
    private int numeroRequisicao;

    public SaidaPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Database.Init();
    }

    private void OnNumeroChanged(object sender, TextChangedEventArgs e)
    {
        scanButton.IsEnabled = int.TryParse(numeroEntry.Text, out numeroRequisicao);
    }

    private async void OnScanClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QRScannerPage(numeroRequisicao, false));
    }
    
    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        var lista = await Database.GetRequisicoes();
        var resultado = await ApiClient.EnviarRequisicoesAsync(lista);

        if (resultado.sucesso)
        {
            await DisplayAlert("Enviado!", $"Inseridos: {resultado.inseridos ?? 0}", "OK");
            //  Deleta os que foram enviados
            await Database.DeletarRequisicoesAsync(lista);
        }
        else
        {
            await DisplayAlert("Erro ao enviar", resultado.mensagem ?? resultado.erro ?? "Erro desconhecido", "Fechar");
        }
    }
    
}