using System.Web;
using CurvesForAge.Data;
using CurvesForAge.Models;

namespace CurvesForAge.ViewModels;

public class ResultViewModel : ViewModelBase
{
    private readonly DataForAgesRequest _request;
    private readonly DatabaseContext _context;

    public ResultViewModel(DataForAgesRequest request)
    {
        _request = request;
        _context = new DatabaseContext();
    }
    public async Task LoadDataAsync()
    {
        var Sex = _request.Sex;

        var result = await _context.GetFileteredAsync<DataForAge>(x => x.sex == Sex && x.type == "BMIForAge");

        var dataForAges = result.ToList();
        if (dataForAges.Any())
        {
            Application.Current?.MainPage?.DisplayAlert("Alerta", $"Hay {dataForAges.Count} registros para del sexo {Sex}", "Ok");
        }
        else
        {
            Application.Current?.MainPage?.DisplayAlert("Alerta", $"No hay datos para esta búsqueda para del sexo {Sex}", "Ok");
        }
    }
}