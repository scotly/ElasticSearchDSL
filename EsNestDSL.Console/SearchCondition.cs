using System.Text.Json.Serialization;

namespace EsNestDSL.Console
{
    public class SearchCondition
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }


        [JsonPropertyName("orderCreateddate")]
        public string OrderCreateTimeStart { get; set; }

        public string OrderCreateTimeEnd { get; set; }


        [JsonPropertyName("orderPaystatus")]
        public List<string> OrderPaystatus { get; set; }


      
        [JsonPropertyName("skuAmount")]
        public long? PaymentAmountStart { get; set; }

        public long? PaymentAmountEnd { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
