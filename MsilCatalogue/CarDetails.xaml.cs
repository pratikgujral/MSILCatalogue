using MsilCatalogue.Common;
using MsilCatalogue.Helpers;
using MsilCatalogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MsilCatalogue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CarDetails : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        int carId;
        DatabaseHelperClass _dbHelper = new DatabaseHelperClass();

        public CarDetails()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            carId = Convert.ToInt16(e.NavigationParameter.ToString());
            Car selcar = _dbHelper.ReadCar(carId);
            TextBlockPageTitle.Text = selcar.carName;
            List<string> stateList = _dbHelper.ReadDistinctStates().ToList();

            ComboBoxState.DataContext = stateList;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ComboBoxState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxCity.IsEnabled = false;

            string selectedState = ComboBoxState.SelectedItem.ToString();

            List<string> CityList = _dbHelper.ReadCitiesByStateName(selectedState).ToList();
            ComboBoxCity.DataContext = CityList;
            ComboBoxCity.IsEnabled = true;
        }

        private void ComboBoxCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCity = ComboBoxCity.SelectedItem.ToString();

            List<CarDetailsPrices> carList = _dbHelper.ReadCarPricesByCarIdAndCityName(carId, selectedCity).ToList();

            foreach (var car in carList)
            {
                TextBlockCarDetails.Inlines.Add(new Run() { Text = "\n\n" + car.variantName, FontWeight = FontWeights.ExtraBold, FontSize = 30 });
                TextBlockCarDetails.Inlines.Add(new Run() { Text = "\nNon Metallic: " + car.NonMetallic, FontWeight = FontWeights.SemiBold, FontSize = 20 });
                TextBlockCarDetails.Inlines.Add(new Run() { Text = "\nMetallic " + car.Metallic, FontWeight = FontWeights.SemiBold, FontSize = 20 });
            }
            //List<Car> carList = _dbHelper.ReadCarPricesByCarIdAndCityName(carId, selectedCity).ToList();

            //string presentCarVariant = String.Empty;
            //foreach (var entry in carList)
            //{
            //    if (!presentCarVariant.Equals(entry.variantName, StringComparison.OrdinalIgnoreCase))
            //    {
            //        presentCarVariant = entry.variantName;
            //        TextBlockCarDetails.Inlines.Add(new Run() { Text = "\n\n" + entry.variantName + "\n", FontWeight = FontWeights.ExtraBold, FontSize = 30 });
            //    }
            //    if (entry.Metallic)
            //    {
            //        TextBlockCarDetails.Inlines.Add(new Run() { Text = "Metallic : ", FontWeight = FontWeights.Bold, FontSize = 20 });
            //        TextBlockCarDetails.Inlines.Add(new Run() { Text = entry.CarPrice + " INR \n", FontWeight = FontWeights.Normal, FontSize = 20 });

            //    }
            //    else if (!entry.Metallic)
            //    {
            //        TextBlockCarDetails.Inlines.Add(new Run() { Text = "Non Metallic : ", FontWeight = FontWeights.Bold, FontSize = 20 });
            //        TextBlockCarDetails.Inlines.Add(new Run() { Text = entry.CarPrice + " INR \n", FontWeight = FontWeights.Normal, FontSize = 20 });
            //    }
            //}
            
        }
    }
}
