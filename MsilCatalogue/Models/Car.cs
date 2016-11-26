using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsilCatalogue.Models
{
    public class Car
    {
        
        public int carId { get; set; }
        public string carName { get; set; }
        public string carImage { get; set; }
        public int VariantId { get; set; }
        public string variantName { get; set; }
        public string colours { get; set; }
        public bool Metallic { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string C_State { get; set; }
        public double CarPrice { get; set; }

        public Car()
        {   //Empty constructor
        }

        public Car(int id, string carName, string imageUrl, int variantId, string variantName, 
        string colors, bool isMetallic, int cityId, string cityName, string state, double price)
        {
            this.carId = id;
            this.carName = carName;
            this.carImage = imageUrl;
            this.VariantId = variantId;
            this.variantName = variantName;
            this.colours = colors;
            this.Metallic = isMetallic;
            this.CityId = cityId;
            this.CityName = cityName;
            this.C_State = state;
            this.CarPrice = price;
        }
    }

}
