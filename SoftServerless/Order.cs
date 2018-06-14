using System;
using System.Collections.Generic;
using System.Text;

namespace SoftServerless
{
    public class Order
    {
        public string id { get; set; }

        public OrderHeaderDetails Header { get; set; }

        public IEnumerable<OrderLineItems> LineItems { get; set; }


    }
}
