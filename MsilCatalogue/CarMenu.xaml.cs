using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MsilCatalogue.Common;
using MsilCatalogue.Helpers;
using MsilCatalogue.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MsilCatalogue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CarMenu : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        string navigationMode;
        List<Car> carListFromApi = new List<Car>();
        DatabaseHelperClass _dbHelper = new DatabaseHelperClass();
        StorageFolder dataFolder;
        List<carMaster> listCarMaster = new List<carMaster>();
        DateTime DateLastSynced;
        string fileDateLastSynced = "FileLastSynced.txt";

        public CarMenu()
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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            if (e.NavigationParameter.ToString() == "Refresh")
                navigationMode = "Refresh";
            bool isSameDataApiAndDb = true;

            // Local storage folder of this app
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
            dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

            // Get the lsit of cars from the local SqLite database.
            List<Car> carListFromDbInitial = _dbHelper.ReadAllCars().ToList();

            if (navigationMode == NavigationMode.New.ToString())
            {
                // Fetch the data from API silently without prompting the user of any error message(s)
                carListFromApi = await getAllCarDataFromApi(0, 0);

                if (carListFromApi != null)
                {

                    // Compare if the two lists are equal or not
                    //if (!CheckTwoCarListsEqual(carListFromApi, carListFromDbInitial))
                    if(!CompareCarLists(carListFromApi, carListFromDbInitial))
                    {
                        isSameDataApiAndDb = false;
                    }

                    if (!isSameDataApiAndDb)
                    {
                        
                        //Delete the existing car images
                        await dataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
                        dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

                        //Delete the old data and insert new cars
                        _dbHelper.DeleteAllCars();

                        var statusBar = StatusBar.GetForCurrentView();
                        statusBar.ProgressIndicator.Text = "Please wait";
                        await statusBar.ShowAsync();
                        await statusBar.ProgressIndicator.ShowAsync();

                        foreach (var car in carListFromApi)
                        {
                            _dbHelper.InsertCar(car);
                        }

                        await statusBar.ProgressIndicator.HideAsync();
                        //await statusBar.HideAsync();

                        // This list, listCarMaster has to be populated before calling the download image function
                        listCarMaster = _dbHelper.ReadCarMaster().ToList();

                        await DownloadImages();

                        DateLastSynced = System.DateTime.Now;
                        byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(DateLastSynced.ToString());
                        
                        // Create a new file named DataFile.txt.
                        var file = await dataFolder.CreateFileAsync(fileDateLastSynced,CreationCollisionOption.OpenIfExists);
                        using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileDateLastSynced, CreationCollisionOption.ReplaceExisting))
                        {
                            stream.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                }
            }

            //if(e.NavigationMode == NavigationMode.Back)
            //{

            //}
            if (navigationMode == "Refresh")
            {

                if (Frame.CanGoBack)
                {
                    this.Frame.BackStack.Remove(this.Frame.BackStack.LastOrDefault());
                }
                // Fetch the data from API informing the user if there is HttpRequestException or Exception
                carListFromApi = await getAllCarDataFromApi(1, 1);

                if(carListFromApi != null)
                {
                    //Delete the existing car images
                    await dataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
                    dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

                    //Delete the old data and insert new cars
                    _dbHelper.DeleteAllCars();

                    foreach (var car in carListFromApi)
                    {
                        _dbHelper.InsertCar(car);
                    }

                    // This list, listCarMaster has to be populated before calling the download image function
                    listCarMaster = _dbHelper.ReadCarMaster().ToList();

                    await DownloadImages();

                    DateLastSynced = System.DateTime.Now;
                    byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(DateLastSynced.ToString());

                    // Create a new file named DataFile.txt.
                    var file = await dataFolder.CreateFileAsync(fileDateLastSynced, CreationCollisionOption.OpenIfExists);

                    using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileDateLastSynced, CreationCollisionOption.ReplaceExisting))
                    {
                        stream.Write(fileBytes, 0, fileBytes.Length);
                    }
                }
                

            }


            listCarMaster = _dbHelper.ReadCarMaster().ToList();

            if(listCarMaster == null)
            {
                MessageDialog msg = new MessageDialog("Unable to find data. Please connect to the internet and restart the app.","Error");
                await msg.ShowAsync();
            }
            else
            {
                bool isImageFileNotFoundException = false;
                foreach (var item in listCarMaster)
                {
                    try
                    {
                        StorageFile imageFile;
                        //imageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(item.carImage);
                        imageFile = await dataFolder.GetFileAsync(item.carImage);
                        Uri uri = new Uri(imageFile.Path);

                        item.image = new BitmapImage(uri);

                    }
                    catch (FileNotFoundException)
                    {
                        isImageFileNotFoundException = true;
                    }
                    catch (Exception ex) { }
                }
                if(isImageFileNotFoundException)
                {
                    MessageDialog msg = new MessageDialog("Please connect to internet and hit refresh.", "Could not load all images");
                    await msg.ShowAsync();
                }

            }

            // Get the file
            bool isDateFileNotFoundException = false;
            try
            {
                var file = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileDateLastSynced);
                // Read the data.
                using (StreamReader streamReader = new StreamReader(file))
                {
                    this.TextBlockDateLastSync.Text = streamReader.ReadToEnd();
                }
            }
            catch(FileNotFoundException)
            {
                isDateFileNotFoundException = true;
            }
            catch (Exception ex) { }
            if(isDateFileNotFoundException)
            {
                MessageDialog msg = new MessageDialog("Could not trace the date when the data was last synchronized to your phone.", "Error");
                await msg.ShowAsync();
            }
            GridViewCars.ItemsSource = listCarMaster;
        }


        private bool CompareCarLists(List<Car> l1, List<Car> l2)
        {
            //// If the two lists are of unequal length, return false. 
            //if (list1.Count != list2.Count)
            //    return false;

            //List<Car> differenceQuery = new List<Car>();
            //differenceQuery = list1.Except(list2).ToList();
            
            //if (differenceQuery.Count != 0)
            //    return false;

            //differenceQuery = list2.Except(list1).ToList();
            //if (differenceQuery.Count != 0)
            //    return false;

            //return true;
            
            if (l1.Count != l2.Count)
                return false;

            bool flag =false;
            foreach (var item1 in l2)
            {
                flag = false;
                foreach(var item2 in l1)
                {
                    if (item1.carId == item2.carId && item1.C_State == item2.C_State &&
                        item1.carImage == item2.carImage && item1.carName == item2.carName &&
                        item1.CarPrice == item2.CarPrice && item1.CityId == item2.CityId &&
                        item1.CityName == item2.CityName && item1.colours == item2.colours &&
                        item1.Metallic == item2.Metallic && item1.VariantId == item2.VariantId &&
                        item1.variantName == item2.variantName)
                    {
                        flag = true;
                        break;
                    }

                    //if (item1.Equals(item2))
                    //{ 
                    //        flag = true;
                    //        break;
                    //}
                        
                }
                if(! flag)
                {
                    return flag;
                }
            }
            return flag;
            
                       
        }

        private bool CheckTwoCarListsEqual(ICollection<Car> a, ICollection<Car> b)
        {
            // 1
            // Require that the counts are equal
            if (a.Count != b.Count)
            {
                return false;
            }
            // 2
            // Initialize new Dictionary of the type
            Dictionary<Car, int> d = new Dictionary<Car, int>();
            // 3
            // Add each key's frequency from collection A to the Dictionary
            foreach (Car item in a)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    d[item] = c + 1;
                }
                else
                {
                    d.Add(item, 1);
                }
            }
            // 4
            // Add each key's frequency from collection B to the Dictionary
            // Return early if we detect a mismatch
            foreach (Car item in b)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    if (c == 0)
                    {
                        return false;
                    }
                    else
                    {
                        d[item] = c - 1;
                    }
                }
                else
                {
                    // Not in dictionary
                    return false;
                }
            }
            // 5
            // Verify that all frequencies are zero
            foreach (int v in d.Values)
            {
                if (v != 0)
                {
                    return false;
                }
            }
            // 6
            // We know the collections are equal
            return true;

        }
        private async Task<int> DownloadImages()
        {
            try
            {
                // The name of the blob should match the name of the car in the database

                //  create Azure Storage
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=msil;AccountKey=BrT96cQpN8FXtJ59FeKMOC1a8OE+IptOdktVypDIE90owdzEU0EF2GdBmchjP/SPyN51DFSa7g1BzIvv0jVH0w==");

                //  create a blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                //  create a container 
                CloudBlobContainer container = blobClient.GetContainerReference("msilimages");

                //  create a block blob
                if (carListFromApi != null)
                {
                    //List<string> uniqueCars = new List<string>();

                    //foreach (var car in carListFromApi)
                    //{
                    //    uniqueCars.Add(car.carImage);
                    //}
                    //uniqueCars = uniqueCars.Distinct<string>().ToList();

                    StatusBar statusBar;
                    statusBar = StatusBar.GetForCurrentView();
                    //statusBar.ProgressIndicator.ProgressValue = 0;

                    await statusBar.ProgressIndicator.ShowAsync();


                    int carImagesToDownloadCount = listCarMaster.Count;
                    int carImageCurrentDownloadCount = 1;

                    foreach (var carBlob in listCarMaster)
                    {
                        //  create a block blob
                        //string uri  = carBlob.Replace(@"https://msil.blob.core.windows.net/msilimages/","");
                        string uri = carBlob.carImage;

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(uri);

                        //  create a local file
                        StorageFile file = await dataFolder.CreateFileAsync(uri, CreationCollisionOption.ReplaceExisting);


                        statusBar.ProgressIndicator.ProgressValue = (carImageCurrentDownloadCount / carImagesToDownloadCount);


                        statusBar.ProgressIndicator.Text = "Downloading images " + carImageCurrentDownloadCount + " of " + carImagesToDownloadCount;
                        //  download from Azure Storage
                        await blockBlob.DownloadToFileAsync(file);

                        carImageCurrentDownloadCount++;
                    }

                    await statusBar.ProgressIndicator.HideAsync();
                }
                //CloudBlockBlob blockBlob = container.GetBlockBlobReference("CIAZ.png");

                //  create a local file
                //StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("CIAZ.png", CreationCollisionOption.ReplaceExisting);

                //  download from Azure Storage
                // await blockBlob.DownloadToFileAsync(file);

                return 1;
            }
            catch
            {
                //  return error
                return 0;
            }
        }

        private async Task<List<Car>> getAllCarDataFromApi(int isWithNetworkErrorMessage, int isWithExceptionErrorMessage)
        {   // This function calls the GetAllCars web API, fetches the JSON, parses it and 
            // returns the list of all the cars.
            //This function either returns the list of all the cars, or returns a null list if any error(s) occurred.
            // The parameter isWithNetworkErrorMessage should be 
            //        0 : If no error message is to be shown to the user for HttpRequestException (as may be the case if the app has to
            //                silently pull the latest data without informing the user)
            //                    
            //        1 : If an error message is to be showed to the user for HttpRequestException

            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.ShowAsync();
            statusBar.ProgressIndicator.Text = "Checking for newer data";

            string apiUrl = @"http://msil.azurewebsites.net/api/ProcViewMsil/GetAllCars";
            string response = String.Empty; //To hold the JSON response from the web API

            // Create a list of Cars that will be returned by this function.
            List<Car> carList = null;

            bool isHttpRequestExceptionOccurred = false;
            bool isExceptionOccurred = false;

            try
            {
                HttpClient request = new HttpClient();
                //Call the API
                response = await request.GetStringAsync(apiUrl);
                response = JsonConvert.DeserializeObject(response).ToString();
                // Deserialize the JSOn received.
                carList = JsonConvert.DeserializeObject<List<Car>>(response);
            }
            catch (HttpRequestException)
            {
                isHttpRequestExceptionOccurred = true;
            }
            catch (Exception)
            {
                isExceptionOccurred = true;
            }

            //Show the error messgage to the user corresponding to HttpRequestException.
            if (isHttpRequestExceptionOccurred && isWithNetworkErrorMessage == 1)
            {
                MessageDialog msg = new MessageDialog("Unable to fetch latest cars data.", "No internet connectivity");
                await msg.ShowAsync();
            }
            else if (isExceptionOccurred && isWithExceptionErrorMessage == 1)
            {
                MessageDialog msg = new MessageDialog("Oops! Something has gone wrong. That's all we know right now.", "Error");
                await msg.ShowAsync();
            }

            await statusBar.ProgressIndicator.HideAsync();
            return carList;
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
            
            if (e.NavigationMode == NavigationMode.New)
                navigationMode = "New";
            else if (e.NavigationMode == NavigationMode.Refresh)
                navigationMode = "Refresh";
            else if(e.NavigationMode == NavigationMode.Back)
                navigationMode = "Back";

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void CommandBarRefresh_Click(object sender, RoutedEventArgs e)
        {
            navigationMode = "Refresh";
            Frame.Navigate(typeof(CarMenu), navigationMode);
        }

        private void GridViewCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            carMaster selCar = (carMaster)e.AddedItems[0];
            string abc = selCar.carId.ToString();

            Frame.Navigate(typeof(CarDetails), abc);
        }

        private void CommandBarAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }
    }
}
