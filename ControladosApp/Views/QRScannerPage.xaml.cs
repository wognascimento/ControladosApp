using CommunityToolkit.Maui.Alerts;
using ControladosApp.Models;
using ControladosApp.Services;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace ControladosApp.Views;

public partial class QRScannerPage : ContentPage
{
    private readonly int _numeroRequisicao;
    private readonly bool _entrada;
    private bool _isProcessingQRCode = false;
    //public ObservableCollection<string> QRCodeCapturados { get; set; } = new();
    public ObservableCollection<QRCodeItem> QRCodeCapturados { get; set; } = [];

    public QRScannerPage(int numeroRequisicao, bool entrada)
    {
        InitializeComponent();
        BindingContext = this; // Isso conecta a lista à tela

        _numeroRequisicao = numeroRequisicao;
        _entrada = entrada;

        //  Remove a seta de voltar
        NavigationPage.SetHasBackButton(this, false);

        // (Opcional) Remove toda a NavigationBar
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        QRCodeCapturados.Clear(); // limpa lista atual (evita duplicata)

        if (_entrada)
        {
            var entradas = await Database.GetEntradas();
            foreach (var item in entradas)
            {
                QRCodeCapturados.Add(new QRCodeItem
                {
                    Barcode = item.Barcode,
                    IsUltimo = false
                });
            }
        }
        else
        {
            var requisicoes = await Database.GetRequisicoes();
            var filtradas = requisicoes
                .Where(r => r.NumeroRequisicao == _numeroRequisicao)
                .ToList();

            foreach (var item in filtradas)
            {
                QRCodeCapturados.Add(new QRCodeItem
                {
                    Barcode = item.Barcode,
                    IsUltimo = false
                });
            }
        }

        // Atualiza o contador
        CapturasLabel.Text = $"Capturados: {QRCodeCapturados.Count}";
    }


    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessingQRCode) return;

        _isProcessingQRCode = true;

        foreach (var barcode in e.Results)
        {
            if (!string.IsNullOrEmpty(barcode.Value))
            {

                if (QRCodeCapturados.Any(item => item.Barcode == barcode.Value))
                {
                    _isProcessingQRCode = false;
                    return; // Ignora duplicado
                }

                if (_entrada)
                {
                    await Database.SaveEntradaQRCode(new QRCodeEntrada
                    {
                        Barcode = barcode.Value
                    });
                }
                else
                {
                    await Database.SaveRequisicaoQRCode(new QRCodeRequisicao
                    {
                        NumeroRequisicao = _numeroRequisicao,
                        Barcode = barcode.Value
                    });
                }

                // Antes de adicionar novo, limpar todos os "último"
                foreach (var item in QRCodeCapturados)
                {
                    item.IsUltimo = false;
                }

                // Adicionar novo QR Code como o último capturado
                QRCodeCapturados.Add(new QRCodeItem
                {
                    Barcode = barcode.Value,
                    IsUltimo = true
                });

                //QRCodeList.ScrollTo(QRCodeCapturados.Last(), position: ScrollToPosition.End, animate: true);
                //CapturasLabel.Text = $"Capturados: {QRCodeCapturados.Count}";

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    QRCodeList.ScrollTo(QRCodeCapturados.Last(), position: ScrollToPosition.End, animate: true);
                    CapturasLabel.Text = $"Capturados: {QRCodeCapturados.Count}";

                    await SoundPlayer.PlaySucesso();
                    await Snackbar.Make($"QR Capturado: {barcode.Value}", null, "", TimeSpan.FromSeconds(2)).Show();
                    await AnimateFinalizarButton();
                    await AnimateFinalizarButtonColor();
                });


            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await SoundPlayer.PlayErro();
                    await Snackbar.Make("Falha ao capturar QR Code.", null, "", TimeSpan.FromSeconds(3)).Show();
                });
            }
        }

        _isProcessingQRCode = false;
    }

    private async void OnFinalizarClicked(object sender, EventArgs e)
    {
        FinalizarButton.IsEnabled = false;
        EnvioIndicator.IsVisible = true;
        EnvioIndicator.IsRunning = true;

        await Snackbar.Make("Enviando coletas para o servidor...", null, "", TimeSpan.FromSeconds(2)).Show();

        ApiResponse resultado;

        if (_entrada)
        {
            var entradas = await Database.GetEntradasNaoSincronizadas();
            resultado = await ApiClient.EnviarEntradasAsync(entradas);
        }
        else
        {
            var saidas = await Database.GetRequisicoesNaoSincronizadas();
            resultado = await ApiClient.EnviarRequisicoesAsync(saidas);
        }

        EnvioIndicator.IsRunning = false;
        EnvioIndicator.IsVisible = false;
        FinalizarButton.IsEnabled = true;

        if (resultado is not null && resultado.sucesso)
        {
            await Snackbar.Make($"Coletas enviadas com sucesso! Inseridos: {resultado.inseridos}", null, "", TimeSpan.FromSeconds(3)).Show();
            await Navigation.PopAsync();
        }
        else
        {
            string mensagem = resultado?.mensagem ?? resultado?.erro ?? "Erro desconhecido.";
            await Snackbar.Make($"Erro: {mensagem}", null, "", TimeSpan.FromSeconds(3)).Show();
        }
    }


    private async Task EnviarCapturadosAsync()
    {
        if (_entrada)
        {
            // Transforma para lista de QRCodeEntrada
            var entradas = QRCodeCapturados.Select(qr => new QRCodeEntrada
            {
                Barcode = qr.Barcode
            }).ToList();

            var resultado = await ApiClient.EnviarEntradasAsync(entradas); // você cria esse método

            if (resultado.sucesso)
            {
                await DisplayAlert("Sucesso", $"Entradas enviadas: {resultado.inseridos}", "OK");
                QRCodeCapturados.Clear(); // limpa lista da sessão
            }
            else
            {
                await DisplayAlert("Erro", resultado.mensagem ?? resultado.erro ?? "Erro desconhecido", "Fechar");
            }
        }
        else
        {
            // Transforma para lista de QRCodeRequisicao
            var requisicoes = QRCodeCapturados.Select(qr => new QRCodeRequisicao
            {
                Barcode = qr.Barcode,
                NumeroRequisicao = _numeroRequisicao
            }).ToList();

            var resultado = await ApiClient.EnviarRequisicoesAsync(requisicoes); // já existe

            if (resultado.sucesso)
            {
                await DisplayAlert("Sucesso", $"Requisições enviadas: {resultado.inseridos}", "OK");
                QRCodeCapturados.Clear(); // limpa lista da sessão
            }
            else
            {
                await DisplayAlert("Erro", resultado.mensagem ?? resultado.erro ?? "Erro desconhecido", "Fechar");
            }
        }
    }

    private async Task AnimateFinalizarButton()
    {
        try
        {
            await FinalizarButton.ScaleTo(1.2, 100, Easing.CubicOut); // Cresce
            await FinalizarButton.ScaleTo(1.0, 100, Easing.CubicIn);  // Volta
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na animação do botão: {ex.Message}");
        }
    }

    private async Task AnimateFinalizarButtonColor()
    {
        try
        {
            var originalColor = FinalizarButton.BackgroundColor;

            // Faz o botão sumir suavemente (fade out)
            await FinalizarButton.FadeTo(0, 100, Easing.CubicOut);

            // Troca a cor para verde enquanto está "invisível"
            FinalizarButton.BackgroundColor = Colors.LimeGreen;

            // Faz o botão aparecer novamente (fade in)
            await FinalizarButton.FadeTo(1, 100, Easing.CubicIn);

            // Espera 200ms com a nova cor
            await Task.Delay(200);

            // Faz o botão sumir de novo (fade out)
            await FinalizarButton.FadeTo(0, 100, Easing.CubicOut);

            // Volta para a cor original (DodgerBlue)
            FinalizarButton.BackgroundColor = originalColor;

            // Faz o botão aparecer novamente (fade in)
            await FinalizarButton.FadeTo(1, 100, Easing.CubicIn);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na animação de cor com fade: {ex.Message}");
        }
    }

    private async void OnDeleteSwipe(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe && swipe.CommandParameter is QRCodeItem item)
        {
            bool confirmar = await DisplayAlert("Excluir", $"Remover QR \"{item.Barcode}\"?", "Sim", "Não");
            if (!confirmar) return;

            QRCodeCapturados.Remove(item);
            CapturasLabel.Text = $"Capturados: {QRCodeCapturados.Count}";

            // Remoção opcional do banco local, se desejar manter consistência
            if (_entrada)
            {
                var entradas = await Database.GetEntradas();
                var correspondente = entradas.FirstOrDefault(e => e.Barcode == item.Barcode);
                if (correspondente != null)
                    await Database.DeletarEntradaAsync(correspondente);
            }
            else
            {
                var requisicoes = await Database.GetRequisicoes();
                var correspondente = requisicoes.FirstOrDefault(r =>
                    r.Barcode == item.Barcode && r.NumeroRequisicao == _numeroRequisicao);
                if (correspondente != null)
                    await Database.DeletarRequisicaoAsync(correspondente);
            }
        }
    }



}