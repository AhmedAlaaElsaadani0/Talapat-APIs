using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.IRepositories;
using Talabat.Core.IServices;
using Talabat.Core.Specifications.Orders;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepo;
        private readonly IUnitOfWork unitOfWork;

        public OrderService(
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork
            )
        {
            this.basketRepo = basketRepo;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. Get Basket From Baskets Repo
            var basket = await basketRepo.GetBasketAsync(basketId);
            
            // 2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();

            if(basket?.Items.Count > 0)
            {
                var productRepo = unitOfWork.Repository<Product>();
                if(productRepo is not null)
                {
                    foreach (var item in basket.Items)
                    {
                        var product = await productRepo.GetByIdAsync(item.Id);

                        if (product is not null)
                        {
                            var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                            var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                            orderItems.Add(orderItem);
                        }
                    }
                }
            }

            // 3. Calculate SubTota1
            var subTotal = orderItems.Sum(O => O.Price * O.Quantity);

            // 4 .Get Delivery Method From DeliveryMethods Repo
            DeliveryMethod deliveryMethod = new DeliveryMethod();
            
            var deliveryMethodsRepo = unitOfWork.Repository<DeliveryMethod>();
            
            if(deliveryMethodsRepo is not null)
                deliveryMethod = await deliveryMethodsRepo.GetByIdAsync(deliveryMethodId);
            
            // 5. Create Order
            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal);

            var orderRepo = unitOfWork.Repository<Order>();
            if(orderRepo is not null)
            {
                await orderRepo.AddAsync(order);

                // 6. Save To Database[TODO]
                var result = await unitOfWork.Complete();

                if (result > 0)
                    return order;
            }

            return null;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            return deliveryMethods;
        }

        public async Task<Order?> GetOrderByIdAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);

            var ordersRepo = unitOfWork.Repository<Order>();

            if (ordersRepo is not null)
                return await ordersRepo.GetByIdWithSpecAsync(spec);

            return null;
        }

        public async Task<IReadOnlyList<Order>?> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);

            var ordersRepo = unitOfWork.Repository<Order>();
            
            if(ordersRepo is not null)
                return await ordersRepo.GetAllWithSpecAsync(spec);

            return null;
        }
    }
}
