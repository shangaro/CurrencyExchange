using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public interface IDataService
    {
        (string, string) CreateEquation(string name);
        Task<dynamic> GetDynamicData(string date, CancellationToken cancellationToken);
        Task<IDictionary<(string, string), decimal>> GetForexRateFromBankOfCanada(string date, CancellationToken cancellationToken = default);
    }
}