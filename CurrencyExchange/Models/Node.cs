namespace CurrencyExchange.Models
{
    public class Node
    {
        public string curr { get; set; }
        public decimal minValue { get; set; }
        public Node(string mappedTo, decimal minValue)
        {
            this.curr = mappedTo;
            this.minValue = minValue;
        }
    }
}