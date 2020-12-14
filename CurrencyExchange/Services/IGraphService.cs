using CurrencyExchange.Models;
using System.Collections.Generic;

namespace CurrencyExchange.Services
{
    public interface IGraphService
    {
        IDictionary<string, IList<Edge>> CreateGraph(IDictionary<(string, string), decimal> data);
    }
}