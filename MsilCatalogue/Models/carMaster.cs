using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MsilCatalogue.Models
{
    public class carMaster
    {
        public int carId { get; set; }
        public string carName { get; set; }
        public string carImage { get; set; }
        public BitmapImage image { get; set; }
    }
}
