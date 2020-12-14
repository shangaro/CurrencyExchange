using CurrencyExchange.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    public interface IForexRateService
    {
        bool TryFindForexRateUsingRecursion(string start, string end, ref decimal product, IDictionary<string, IList<Edge>> graph);
        decimal TryFindForexRateUsingDijkstra(string start, string end, IDictionary<string, IList<Edge>> graph);
    }
}