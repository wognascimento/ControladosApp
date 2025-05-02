
using ControladosApp.Services;
using ControladosApp.Views;

namespace ControladosApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}
	private async void OnSaidaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SaidaPage());
    }

    private async void OnEntradaClicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new EntradaPage());
        await Database.Init();
        await Navigation.PushAsync(new QRScannerPage(0, true)); // 0 pois Entrada não precisa número
    }

    private async void AbrirSyncPage(object sender, EventArgs e)
    {
        await Database.Init();
        await Navigation.PushAsync(new SyncPage());
    }
}

