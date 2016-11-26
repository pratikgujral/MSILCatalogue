using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsilCatalogue.Models
{
    public class CarDetailsPrices
    {
        public int carId { get; set; }
        public string carName { get; set; }
        public string variantName { get; set; }
        public double Metallic { get; set; }
        public double NonMetallic { get; set; }
    }
}
