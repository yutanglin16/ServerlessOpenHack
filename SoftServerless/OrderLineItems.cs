using System;
using System.Collections.Generic;
using System.Text;

namespace SoftServerless
{
    public class OrderLineItems
    {
        public string PoNumber { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public double UnitCost { get; set; }

        public double TotalCost { get; set; }

        public double TotalTax { get; set; }

        public ProductInformation Product { get; set; }
    }
}
