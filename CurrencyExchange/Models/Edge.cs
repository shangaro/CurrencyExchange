using System;

namespace CurrencyExchange.Models
{
    // edge can be written in the form of triples(u,v,w) where u,v are nodes and w is the cost
    public class Edge
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal value { get; set; }
        public Edge(string From, string To, decimal value)
        {
            this.From = From;
            this.To = To;
            this.value = value;
        }
        public Edge(Tuple<string, string> equation, decimal value)
        {
            this.From = equation.Item1;
            this.To = equation.Item2;
            this.value = value;
        }

    }
}
