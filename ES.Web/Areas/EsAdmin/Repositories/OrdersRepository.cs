using ES.Web.Areas.EsAdmin.Models;
using System.Data;


namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class OrdersRepository
    {
        private readonly ApplicationDbContext _context;

        public OrdersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync()
        {
            return await FetchOrdersAsync("sp_GetAllOrders");
        }

        public async Task<IEnumerable<OrderViewModel>> GetPendingOrdersAsync()
        {
            return await FetchOrdersAsync("sp_GetPendingOrders");
        }

        public async Task<IEnumerable<OrderViewModel>> GetCompletedAndCanceledOrdersAsync()
        {
            return await FetchOrdersAsync("sp_GetCompletedAndCanceledOrders");
        }

        //public async Task<OrderViewModel> GetOrderByIdAsync(int id)
        //{
        //    using var connection = _context.Database.GetDbConnection();
        //    await connection.OpenAsync();

        //    var orderDict = new Dictionary<int, OrderViewModel>();
        //    var itemDict = new Dictionary<int, OrderItemViewModel>();

        //    using var command = connection.CreateCommand();
        //    command.CommandText = "sp_GetOrderById";
        //    command.CommandType = CommandType.StoredProcedure;

        //    var orderIdParam = command.CreateParameter();
        //    orderIdParam.ParameterName = "@OrderId";
        //    orderIdParam.Value = id;
        //    command.Parameters.Add(orderIdParam);

        //    using var reader = await command.ExecuteReaderAsync();
        //    while (await reader.ReadAsync())
        //    {
        //        int orderId = reader.GetInt32("Id");

        //        if (!orderDict.ContainsKey(orderId))
        //        {
        //            var order = new OrderViewModel
        //            {
        //                Id = orderId,
        //                Name = reader.GetString("Name"),
        //                MobilePhone = reader.GetString("MobilePhone"),
        //                Address = reader.GetString("Address"),
        //                LocationLink = string.Format(CultureInfo.InvariantCulture,
        //                                "https://www.google.com/maps?q={0},{1}",
        //                                reader.GetDouble("Latitude"),
        //                                reader.GetDouble("Longitude")),
        //                PaymentMethod = reader.GetString("PaymentMethod"),
        //                EstimatedTotal = reader.GetDecimal("EstimatedTotal"),
        //                IsDelivered = reader.GetBoolean("IsDelivered"),
        //                IsCancelled = reader.GetBoolean("IsCancelled"),
        //                CreatedAt = reader.GetDateTime("CreatedAt"),
        //                Items = new List<OrderItemViewModel>()
        //            };
        //            orderDict[orderId] = order;
        //        }

        //        if (!reader.IsDBNull("OrderItemId"))
        //        {
        //            int orderItemId = reader.GetInt32("OrderItemId");

        //            if (!itemDict.ContainsKey(orderItemId))
        //            {
        //                var item = new OrderItemViewModel
        //                {
        //                    ProductTitle = reader.GetString("ProductTitle"),
        //                    CustomizationName = reader.IsDBNull("CustomizationName") ? null : reader.GetString("CustomizationName"),
        //                    Quantity = reader.GetInt32("Quantity"),
        //                    SelectedAttributes = new List<OrderItemAttributeViewModel>(),
        //                    ExcludedAttributes = new List<OrderItemAttributeViewModel>()
        //                };
        //                itemDict[orderItemId] = item;
        //                orderDict[orderId].Items.Add(item);
        //            }

        //            if (!reader.IsDBNull("SelectedAttributeValue"))
        //            {
        //                itemDict[orderItemId].SelectedAttributes.Add(new OrderItemAttributeViewModel
        //                {
        //                    Value = reader.GetString("SelectedAttributeValue")
        //                });
        //            }

        //            if (!reader.IsDBNull("ExcludedAttributeValue"))
        //            {
        //                itemDict[orderItemId].ExcludedAttributes.Add(new OrderItemAttributeViewModel
        //                {
        //                    Value = reader.GetString("ExcludedAttributeValue")
        //                });
        //            }
        //        }
        //    }

        //    return orderDict.Values.FirstOrDefault()!;
        //}

        public async Task<IEnumerable<OrderViewModel>> GetFilteredOrdersAsync(string mobile = "", string sort = "newest", string status = "pending")
        {
            IEnumerable<OrderViewModel> orders;

            switch (status.ToLower())
            {
                case "all":
                    orders = await GetAllOrdersAsync();
                    break;
                case "completed":
                    orders = await GetCompletedAndCanceledOrdersAsync();
                    orders = orders.Where(o => o.IsDelivered);
                    break;
                case "canceled":
                    orders = await GetCompletedAndCanceledOrdersAsync();
                    orders = orders.Where(o => o.IsCancelled);
                    break;
                case "pending":
                default:
                    orders = await GetPendingOrdersAsync();
                    break;
            }

            if (!string.IsNullOrEmpty(mobile))
            {
                orders = orders.Where(o => o.MobilePhone.Contains(mobile, StringComparison.OrdinalIgnoreCase));
            }

            orders = sort.ToLower() == "oldest"
                ? orders.OrderBy(o => o.CreatedAt)
                : orders.OrderByDescending(o => o.CreatedAt);

            return orders;
        }

        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "sp_CompleteOrder";
                command.CommandType = CommandType.StoredProcedure;

                var orderIdParam = command.CreateParameter();
                orderIdParam.ParameterName = "@OrderId";
                orderIdParam.Value = orderId;
                command.Parameters.Add(orderIdParam);

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error completing order: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "sp_CancelOrder";
                command.CommandType = CommandType.StoredProcedure;

                var orderIdParam = command.CreateParameter();
                orderIdParam.ParameterName = "@OrderId";
                orderIdParam.Value = orderId;
                command.Parameters.Add(orderIdParam);

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error canceling order: {ex.Message}");
                return false;
            }
        }

        private async Task<IEnumerable<OrderViewModel>> FetchOrdersAsync(string storedProcedureName)
        {
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var orders = new List<OrderViewModel>();
            var orderDict = new Dictionary<int, OrderViewModel>();
            var itemDict = new Dictionary<int, OrderItemViewModel>();

            using var command = connection.CreateCommand();
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int orderId = reader.GetInt32("Id");

                if (!orderDict.ContainsKey(orderId))
                {
                    var order = new OrderViewModel
                    {
                        Id = orderId,
                        CustomerFullName = reader.GetString("CustomerFullName"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        MobilePhone = reader.GetString("MobilePhone"),
                        StreetAddress = reader.GetString("StreetAddress"),
                        Country = reader.IsDBNull("Country") ? string.Empty : reader.GetString("Country"),
                        City = reader.IsDBNull("City") ? string.Empty : reader.GetString("City"),
                        PaymentMethod = reader.GetString("PaymentMethod"),
                        Subtotal = reader.GetDecimal("Subtotal"),
                        ShippingCost = reader.GetDecimal("ShippingCost"),
                        EstimatedTotal = reader.GetDecimal("EstimatedTotal"),
                        OrderComment = reader.IsDBNull("OrderComment") ? null : reader.GetString("OrderComment"),
                        IsDelivered = reader.GetBoolean("IsDelivered"),
                        IsCancelled = reader.GetBoolean("IsCancelled"),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        Items = new List<OrderItemViewModel>()
                    };

                    orderDict[orderId] = order;
                    orders.Add(order);
                }

                // Handle order items
                if (!reader.IsDBNull("OrderItemId"))
                {
                    int orderItemId = reader.GetInt32("OrderItemId");

                    if (!itemDict.ContainsKey(orderItemId))
                    {
                        var item = new OrderItemViewModel
                        {
                            ProductTitle = reader.GetString("ProductTitle"),
                            CustomizationName = reader.IsDBNull("CustomizationName") ? null : reader.GetString("CustomizationName"),
                            Quantity = reader.GetInt32("Quantity"),
                            SelectedAttributes = new List<OrderItemAttributeViewModel>()
                        };

                        itemDict[orderItemId] = item;
                        orderDict[orderId].Items.Add(item);
                    }

                    if (!reader.IsDBNull("SelectedAttributeValue"))
                    {
                        var value = reader.GetString("SelectedAttributeValue");
                        if (!itemDict[orderItemId].SelectedAttributes.Any(a => a.Value == value))
                        {
                            itemDict[orderItemId].SelectedAttributes.Add(new OrderItemAttributeViewModel
                            {
                                Value = value
                            });
                        }
                    }
                }
            }

            return orders;
        }

    }
}