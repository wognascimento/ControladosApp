using Microsoft.Maui.Controls;

namespace ControladosApp.Services;

public static class AlertService
{
    public static async Task ShowAlert(string title, string message, string cancel = "OK")
    {
        try
        {
            // Tenta pegar a página atual
            //var currentPage = Application.Current?.MainPage;
            var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (currentPage is not null)
            {
                await currentPage.DisplayAlert(title, message, cancel);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao tentar mostrar alerta: {ex.Message}");
        }
    }
}
