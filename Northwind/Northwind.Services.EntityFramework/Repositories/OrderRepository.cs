using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using Order = Northwind.Services.EntityFramework.Entities.Order;
using OrderDetail = Northwind.Services.EntityFramework.Entities.OrderDetail;
using Product = Northwind.Services.EntityFramework.Entities.Product;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;
using RepositoryProduct = Northwind.Services.Repositories.Product;
using RepositoryCustomer = Northwind.Services.Repositories.Customer;
using RepositoryCustomerCode = Northwind.Services.Repositories.CustomerCode;
using RepositoryEmployee = Northwind.Services.Repositories.Employee;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;
using RepositoryShippingAddress = Northwind.Services.Repositories.ShippingAddress;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext context;

    public OrderRepository(NorthwindContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), "Skip cannot be negative.");
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive.");
        }

        var orders = await context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Category)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Supplier)
            .OrderBy(o => o.OrderId)
            .Skip(skip)
            .Take(count)
            .ToListAsync();

        return orders.Select(order => MapToRepositoryOrder(order)).ToList();
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        var order = await context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Category)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Supplier)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
        {
            throw new OrderNotFoundException($"Order with ID {orderId} not found.");
        }

        return MapToRepositoryOrder(order);
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        try
        {
            var entityOrder = MapToEntityOrder(order);
            context.Orders.Add(entityOrder);
            await context.SaveChangesAsync();
            return entityOrder.OrderId;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to add order to repository.", ex);
        }
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        var order = await context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
        {
            throw new OrderNotFoundException($"Order with ID {orderId} not found.");
        }

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        var existingOrder = await context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == order.Id);

        if (existingOrder == null)
        {
            throw new OrderNotFoundException($"Order with ID {order.Id} not found.");
        }

        // Update order properties
        existingOrder.CustomerId = order.Customer.Code.Code;
        existingOrder.EmployeeId = (int)order.Employee.Id;
        existingOrder.OrderDate = order.OrderDate;
        existingOrder.RequiredDate = order.RequiredDate;
        existingOrder.ShippedDate = order.ShippedDate;
        existingOrder.ShipVia = (int)order.Shipper.Id;
        existingOrder.Freight = (decimal)order.Freight;
        existingOrder.ShipName = order.ShipName;
        existingOrder.ShipAddress = order.ShippingAddress.Address;
        existingOrder.ShipCity = order.ShippingAddress.City;
        existingOrder.ShipRegion = order.ShippingAddress.Region;
        existingOrder.ShipPostalCode = order.ShippingAddress.PostalCode;
        existingOrder.ShipCountry = order.ShippingAddress.Country;

        // Remove existing order details
        context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

        // Add new order details with proper validation
        foreach (var orderDetail in order.OrderDetails)
        {
            // Validate quantity
            if (orderDetail.Quantity <= 0)
            {
                throw new ArgumentException($"OrderDetail quantity must be greater than 0. Got: {orderDetail.Quantity}");
            }

            existingOrder.OrderDetails.Add(new OrderDetail
            {
                OrderId = (int)order.Id,
                ProductId = (int)orderDetail.Product.Id,
                UnitPrice = (decimal)orderDetail.UnitPrice,
                Quantity = (int)orderDetail.Quantity,
                Discount = (float)orderDetail.Discount,
            });
        }

        await context.SaveChangesAsync();
    }

    private static RepositoryOrder MapToRepositoryOrder(Order order)
    {
        var repositoryOrder = new RepositoryOrder(order.OrderId)
        {
            Customer = new RepositoryCustomer(new RepositoryCustomerCode(order.CustomerId ?? string.Empty))
            {
                CompanyName = order.Customer?.CompanyName ?? string.Empty
            },
            Employee = new RepositoryEmployee(order.EmployeeId ?? 0)
            {
                FirstName = order.Employee?.FirstName ?? string.Empty,
                LastName = order.Employee?.LastName ?? string.Empty,
                Country = order.Employee?.Country ?? string.Empty,
            },
            OrderDate = order.OrderDate ?? DateTime.MinValue,
            RequiredDate = order.RequiredDate ?? DateTime.MinValue,
            ShippedDate = order.ShippedDate,
            Shipper = new RepositoryShipper(order.ShipVia ?? 0)
            {
                CompanyName = order.Shipper?.CompanyName ?? string.Empty,
            },
            Freight = (double)(order.Freight ?? 0),
            ShipName = order.ShipName ?? string.Empty,
            ShippingAddress = new RepositoryShippingAddress(
                order.ShipAddress ?? string.Empty,
                order.ShipCity ?? string.Empty,
                order.ShipRegion,
                order.ShipPostalCode ?? string.Empty,
                order.ShipCountry ?? string.Empty),
        };

        foreach (var orderDetail in order.OrderDetails)
        {
            repositoryOrder.OrderDetails.Add(new RepositoryOrderDetail(repositoryOrder)
            {
                Product = new RepositoryProduct(orderDetail.ProductId)
                {
                    ProductName = orderDetail.Product?.ProductName ?? string.Empty,
                    CategoryId = orderDetail.Product?.CategoryId ?? 0,
                    Category = orderDetail.Product?.Category?.CategoryName ?? string.Empty,
                    SupplierId = orderDetail.Product?.SupplierId ?? 0,
                    Supplier = orderDetail.Product?.Supplier?.CompanyName ?? string.Empty
                },
                UnitPrice = (double)orderDetail.UnitPrice,
                Quantity = orderDetail.Quantity,
                Discount = orderDetail.Discount
            });
        }

        return repositoryOrder;
    }

    private static Order MapToEntityOrder(RepositoryOrder repositoryOrder)
    {
        var order = new Order
        {
            CustomerId = repositoryOrder.Customer.Code.Code,
            EmployeeId = (int)repositoryOrder.Employee.Id,
            OrderDate = repositoryOrder.OrderDate,
            RequiredDate = repositoryOrder.RequiredDate,
            ShippedDate = repositoryOrder.ShippedDate,
            ShipVia = (int)repositoryOrder.Shipper.Id,
            Freight = (decimal)repositoryOrder.Freight,
            ShipName = repositoryOrder.ShipName,
            ShipAddress = repositoryOrder.ShippingAddress.Address,
            ShipCity = repositoryOrder.ShippingAddress.City,
            ShipRegion = repositoryOrder.ShippingAddress.Region,
            ShipPostalCode = repositoryOrder.ShippingAddress.PostalCode,
            ShipCountry = repositoryOrder.ShippingAddress.Country,
        };

        foreach (var repositoryOrderDetail in repositoryOrder.OrderDetails)
        {
            // Validate quantity
            if (repositoryOrderDetail.Quantity <= 0)
            {
                throw new ArgumentException($"OrderDetail quantity must be greater than 0. Got: {repositoryOrderDetail.Quantity}");
            }

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = (int)repositoryOrderDetail.Product.Id,
                UnitPrice = (decimal)repositoryOrderDetail.UnitPrice,
                Quantity = (int)repositoryOrderDetail.Quantity,
                Discount = (float)repositoryOrderDetail.Discount,
            });
        }

        return order;
    }
}
