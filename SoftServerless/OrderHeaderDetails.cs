using System;
using System.Collections.Generic;
using System.Text;

namespace SoftServerless
{
    public class OrderHeaderDetails
    {
        public string PoNumber { get; set; }

        public DateTime Date { get; set; }

        public string LocationId { get; set; }

        public string LocationName { get; set; }

        public string LocationAddress { get; set; }

        public int LocationPostcode { get; set; }

        public double TotalCost { get; set; }

        public double TotalTax { get; set; }
    }
}
