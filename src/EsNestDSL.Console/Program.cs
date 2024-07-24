
using EsNestDSL.Console;
using EsNestDSL.Core.Enums;
using EsNestDSL.Core.Fields;
using EsNestDSL.Core.Nest;
using EsNestDSL.Core.Extentions;
using Nest;
using Elasticsearch.Net;

var condition = new SearchCondition
{
    OrderCreateTimeStart = "2024-01-01 12:00:00",
    OrderCreateTimeEnd = "2024-06-01 12:00:00",
    OrderId = "123456",
    OrderPaystatus = new List<string> { "UnPay", "Pay" }
};

#region add query
var orderContainer = new NestSearchContainer<OrderNest, SearchCondition>();

orderContainer.AddQueryIF(string.IsNullOrWhiteSpace(condition.OrderId), o => o.OrderId)
    .AddQueryIF(condition.OrderPaystatus != null && condition.OrderPaystatus.Any(), x => x.OrderPaystatus, componentType: ComponentType.Terms);


//add nest query condition
var nestContainer = new NestSearchContainer<OrderNest, SearchCondition>(nameof(OrderNest.orderitems));
nestContainer.AddRangeQuery(RangeTypeEnum.Date, x => x.OrderCreateTimeStart, RangeOperEnum.gte, x => x.OrderCreateTimeEnd, RangeOperEnum.lte);

orderContainer.AddContainer(nestContainer);
#endregion

#region build query

var searchBuilder = new NestSearchBuilder<OrderNest, SearchCondition>();
var descriptor = searchBuilder.BuildSearchQuery(orderContainer, condition);

//add page and sort
descriptor.AddPage(condition.PageIndex, condition.PageSize)
    .AddSort(new SortField<OrderNest>(s => s.OrderCreateddate, SortTypeEnum.Desc),
        new SortField<OrderNest>(s => s.OrderId, SortTypeEnum.Desc));

#endregion


var connectionPool = new StaticConnectionPool(new Uri[] { new Uri("http://localhost:7019/elasicsearch") });
var connectionSettings = new ConnectionSettings(connectionPool)
    .DefaultIndex("myindex")
    .BasicAuthentication("user", "user12345")
    .PrettyJson()
    .DisableDirectStreaming();

var elaticClient = new ElasticClient(connectionSettings);

#if DEBUG

var json = elaticClient.ToRawRequest(descriptor);
Console.WriteLine(json);

#endif

var response = await elaticClient.SearchAsync<OrderNest>(descriptor);

