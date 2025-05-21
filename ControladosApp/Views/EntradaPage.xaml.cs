using ControladosApp.Services;

namespace ControladosApp.Views;

public partial class EntradaPage : ContentPage
{
	public EntradaPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Database.Init();
    }

    private async void OnScanClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QRScannerPage(0, true)); // 0 pois Entrada não precisa número
    }
    
    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        var lista = await Database.GetEntradas();
        var resultado = await ApiClient.EnviarEntradasAsync(lista);

        if (resultado.sucesso)
        {
            await DisplayAlert("Enviado!", $"Inseridos: {resultado.inseridos ?? 0}", "OK");
            // Deleta os que foram enviados
            await Database.DeletarEntradasAsync(lista);
        }
        else
        {
            await DisplayAlert("Erro ao enviar", resultado.mensagem ?? resultado.erro ?? "Erro desconhecido", "Fechar");
        }
    }
   
}