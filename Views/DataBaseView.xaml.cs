using AmigoSecreto.Models;
using AmigoSecreto.Services;

namespace AmigoSecreto;

public partial class DataBaseView : ContentPage
{
    private readonly DataBaseService _dataBaseService = new();

    public DataBaseView()
    {
        InitializeComponent();
        CarregarDados(GetSorteioListView());
    }

    private ListView GetSorteioListView()
    {
        return SorteioListView;
    }

    private void CarregarDados(ListView sorteioListView)
    {
        try
        {
            var participantes = _dataBaseService.ObterParticipantes();
            var sorteios = _dataBaseService.ObterSorteios();

            ParticipantesListView.ItemsSource = participantes;
            sorteioListView.ItemsSource = sorteios;
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao carregar os dados: {ex.Message}", "OK");
        }
    }

    private void OnAtualizarDadosClicked(object sender, EventArgs e)
    {
        CarregarDados(GetSorteioListView());
    }

    private void OnRealizarSorteioClicked(object sender, EventArgs e)
    {
        var participantes = _dataBaseService.ObterParticipantes();
        _dataBaseService.RealizarSorteio(participantes);
        DisplayAlert("Sorteio Realizado", "O sorteio foi realizado com sucesso!", "OK");
        CarregarDados(GetSorteioListView());
    }
}