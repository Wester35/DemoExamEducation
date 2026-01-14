using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Educat.Models
{
    public class Good
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public int IdLabel { get; set; }
        public string Label { get; set; }
        public string UnitOfMeasure { get; set; }
        public double Price { get; set; }
        public int IdSupplier { get; set; }
        public string Supplier { get; set; }
        public int IdFabric { get; set; }
        public string Fabric { get; set; }
        public int IdCategory { get; set; }
        public string Category { get; set; }
        public int Discount { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
    }
}
