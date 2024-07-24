using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsNestDSL.Console
{
    public class OrderNest
    {
        [Keyword(Name = "orderId")]
        public string OrderId { get; set; }

        [Date(Name = "orderCreateddate", Format = "yyyy-MM-dd HH:mm:ss")]
        public string OrderCreateddate { get; set; }

        [Date(Name = "orderModifieddate", Format = "yyyy-MM-dd HH:mm:ss")]
        public string OrderModifieddate { get; set; }

        [Keyword(Name = "orderPaystatus")]
        public string orderPaystatus { get; set; }

        [Nested]
        public List<OrderItemNest> orderitems { get; set; }
    }

    public class OrderItemNest
    {
        [Keyword(Name = "orderId")]
        public string orderId { get; set; }

        [Keyword(Name = "productId")]
        public string productId { get; set; }

        [Text(Name = "productName", Analyzer = "ik_max_word")]
        public string productName { get; set; }

        [Keyword(Name = "skuAmount")]
        public long skuAmount { get; set; }
    }
}
