using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;

namespace ControladosApp.Services;

public static class SoundPlayer
{
    static IAudioManager audioManager => AudioManager.Current;

    public static async Task PlaySucesso()
    {
        try
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("sucess.wav"));
            player.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao tocar som de sucesso: {ex.Message}");
        }
    }

    public static async Task PlayErro()
    {
        try
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("error.wav"));
            player.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao tocar som de erro: {ex.Message}");
        }
    }
}
