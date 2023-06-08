using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Orders
{
    public class OrderSpecifications : BaseSpecification<Order>
    {
        public OrderSpecifications(string email) : base(O => O.BuyerEmail == email)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);
            AddOrderByDesc(O => O.OrderDate);
        }

        public OrderSpecifications(string email, int id) : base(O => O.BuyerEmail == email && O.Id == id)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryMethod);
        }
    }
}
