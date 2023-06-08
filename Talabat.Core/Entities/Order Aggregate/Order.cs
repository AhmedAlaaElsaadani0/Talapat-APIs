using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        public Order()
        {
            // Empty Parameterless Constructor is needed for EF Core
        }

        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }       // Composite Attribute
        public DeliveryMethod DeliveryMethod { get; set; } // Navvigational Properity ONE
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>(); // Navvigational Properity Many
        public decimal SubTotal { get; set; }
        
        //[NotMapped]
        //public decimal Total => SubTotal + DeliveryMethod.Cost; // Derived Attribute
        public decimal GetTotal() 
            => SubTotal + DeliveryMethod.Cost;

        public string? PaymentIntentId { get; set; }
    }
}
