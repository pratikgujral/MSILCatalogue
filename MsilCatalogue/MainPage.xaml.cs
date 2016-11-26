using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MsilCatalogue.Helpers;
using MsilCatalogue.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MsilCatalogue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Car> carListFromApi = new List<Car>();
        DatabaseHelperClass _dbHelper = new DatabaseHelperClass();
        StorageFolder dataFolder;
        List<carMaster> listCarMaster = new List<carMaster>();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.Navigate(typeof(CarMenu));
            //bool isSameDataApiAndDb = true;

            //// Local storage folder of this app
            //StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            //// Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
            //dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

            //// Get the lsit of cars from the local SqLite database.
            //List<Car> carListFromDbInitial = _dbHelper.ReadAllCars().ToList();
            
            //if(e.NavigationMode == NavigationMode.New)
            //{
            //    // Fetch the data from API silently without prompting the user of any error message(s)
            //    carListFromApi = await getAllCarDataFromApi(0, 0);

            //    if(carListFromApi != null)
            //    {
            //        // Compare if the two lists are equal or not
            //        if (!CheckTwoCarListsEqual(carListFromApi, carListFromDbInitial))
            //        {
            //            isSameDataApiAndDb = false;
            //        }

            //        if (!isSameDataApiAndDb)
            //        {
            //            // If the data does not match, download the images and update the data in the database

            //            //Delete the existing car images
            //            await dataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //            // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
            //            dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

            //            //Delete the old data and insert new cars
            //            _dbHelper.DeleteAllCars();

            //            foreach (var car in carListFromApi)
            //            {
            //                _dbHelper.InsertCar(car);
            //            }

            //            // This list, listCarMaster has to be populated before calling the download image function
            //            listCarMaster = _dbHelper.ReadCarMaster().ToList();

            //            await DownloadImages();
            //        }
            //    }
            //}

            ////if(e.NavigationMode == NavigationMode.Back)
            ////{

            ////}
            //if(e.NavigationMode == NavigationMode.Refresh)
            //{

            //    if(Frame.CanGoBack)
            //    {
            //        this.Frame.BackStack.Remove(this.Frame.BackStack.LastOrDefault());
            //    }
            //    // Fetch the data from API informing the user if there is HttpRequestException or Exception
            //    carListFromApi = await getAllCarDataFromApi(1, 1);

            //    //Delete the existing car images
            //    await dataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            //    // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
            //    dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

            //    //Delete the old data and insert new cars
            //    _dbHelper.DeleteAllCars();

            //    foreach (var car in carListFromApi)
            //    {
            //        _dbHelper.InsertCar(car);
            //    }

            //    // This list, listCarMaster has to be populated before calling the download image function
            //    listCarMaster = _dbHelper.ReadCarMaster().ToList();

            //    await DownloadImages();

            //}


            //listCarMaster = _dbHelper.ReadCarMaster().ToList();
            //foreach (var item in listCarMaster)
            //{
            //    try
            //    {
            //        StorageFile imageFile;
            //        //imageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(item.carImage);
            //        imageFile = await dataFolder.GetFileAsync(item.carImage);
            //        Uri uri = new Uri(imageFile.Path);

            //        item.image = new BitmapImage(uri);

            //    }
            //    catch (FileNotFoundException)
            //    {
            //    }
            //    catch (Exception ex) { }

            //}

            //GridViewCars.ItemsSource = listCarMaster;

         















            
            ////if (e.NavigationMode == NavigationMode.New)
            ////{
            ////    // Fetch the data from API
            ////    carListFromApi = await getAllCarDataFromApi(0, 0);

            ////   // List<Car> carListFromDbInitial = _dbHelper.ReadAllCars().ToList();

            ////    if(carListFromApi != null)
            ////    {
            ////        //if (carListFromApi != carListFromDbInitial)
                    
            ////        // if (!Enumerable.SequenceEqual(carListFromDbInitial, carListFromApi))
            ////        if(CheckTwoCarListsEqual(carListFromApi, carListFromDbInitial))
            ////        {
            ////            isSameDataApiAndDb = false;
                      
            ////        }
                                      
            ////    }

            ////    listCarMaster = _dbHelper.ReadCarMaster().ToList();

            ////    if(!isSameDataApiAndDb)
            ////    {
            ////        // If the data does not match, download the images and update the data in the database

            ////        //Delete the existing car images
            ////          await dataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            ////        // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
            ////        dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

            ////        //Delete the old data and insert new cars
            ////        _dbHelper.DeleteAllCars();

            ////        foreach (var car in carListFromApi)
            ////        {
            ////            _dbHelper.InsertCar(car);
            ////        }

            ////        // This list, listCarMaster has to be populated before calling the download image function
            ////        listCarMaster = _dbHelper.ReadCarMaster().ToList();

            ////        await DownloadImages();
            ////    }
                
            ////}
            //////bool isFirstTimeUser = false;
            
            
            ////// TODO: Prepare page for display here.

            ////// TODO: If your application contains multiple pages, ensure that you are
            ////// handling the hardware Back button by registering for the
            ////// Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            ////// If you are using the NavigationHelper provided by some templates,
            ////// this event is handled for you.


            
            ////if(carListFromApi==null)
            ////{
            ////    listCarMaster = _dbHelper.ReadCarMaster().ToList();
            ////}
            
            ////foreach (var item in listCarMaster)
            ////{
            ////    try
            ////    {
            ////        StorageFile imageFile;
            ////        //imageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(item.carImage);
            ////        imageFile = await dataFolder.GetFileAsync(item.carImage);
            ////        Uri uri = new Uri(imageFile.Path);

            ////        item.image = new BitmapImage(uri);

            ////    }
            ////    catch (FileNotFoundException)
            ////    {
            ////    }
            ////    catch (Exception ex) { }

            ////}

            ////GridViewCars.ItemsSource = listCarMaster;

        }

        //public async void checkLatestData(int n)
        //{ // The value of the parameter n decides if any error messages are to be shown to the user.
        //   // n = 0  => No error is shown to the user 
        //    // n = 1 => Error message is shown to the user eg. that there is no internet connectivity

        //    bool isSameDataApiAndDb = true;
        //    // Local storage folder of this app
        //    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;


        //    // Fetch the data from API
        //    carListFromApi = await getAllCarDataFromApi(1, 1);

        //    // Get the list of cars from the SQLite database in phone storage
        //    //List<Car> carListFromDbInitial = _dbHelper.ReadAllCars().ToList();

        //    if (carListFromApi != null)
        //    {
        //        //Delete the existing car images
        //        await dataFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        //        // Create a new folder by the name carImagesFolder if it does not exist. If it exists, open it.
        //        dataFolder = await local.CreateFolderAsync("carImagesFolder", CreationCollisionOption.OpenIfExists);

        //        //Delete the old data and insert new cars
        //        _dbHelper.DeleteAllCars();

        //        foreach (var car in carListFromApi)
        //        {
        //            _dbHelper.InsertCar(car);
        //        }

        //        // This list, listCarMaster has to be populated before calling the download image function
        //        listCarMaster = _dbHelper.ReadCarMaster().ToList();

        //        await DownloadImages();


        //        foreach (var item in listCarMaster)
        //        {
        //            try
        //            {
        //                StorageFile imageFile;
        //                //imageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(item.carImage);
        //                imageFile = await dataFolder.GetFileAsync(item.carImage);
        //                Uri uri = new Uri(imageFile.Path);

        //                item.image = new BitmapImage(uri);

        //            }
        //            catch (FileNotFoundException)
        //            {
        //            }
        //            catch (Exception ex) { }

        //        }
        //    }
        //}

        //private bool CheckTwoCarListsEqual(ICollection<Car> a, ICollection<Car> b)
        //{
        //    // 1
        //    // Require that the counts are equal
        //    if (a.Count != b.Count)
        //    {
        //        return false;
        //    }
        //    // 2
        //    // Initialize new Dictionary of the type
        //    Dictionary<Car, int> d = new Dictionary<Car, int>();
        //    // 3
        //    // Add each key's frequency from collection A to the Dictionary
        //    foreach (Car item in a)
        //    {
        //        int c;
        //        if (d.TryGetValue(item, out c))
        //        {
        //            d[item] = c + 1;
        //        }
        //        else
        //        {
        //            d.Add(item, 1);
        //        }
        //    }
        //    // 4
        //    // Add each key's frequency from collection B to the Dictionary
        //    // Return early if we detect a mismatch
        //    foreach (Car item in b)
        //    {
        //        int c;
        //        if (d.TryGetValue(item, out c))
        //        {
        //            if (c == 0)
        //            {
        //                return false;
        //            }
        //            else
        //            {
        //                d[item] = c - 1;
        //            }
        //        }
        //        else
        //        {
        //            // Not in dictionary
        //            return false;
        //        }
        //    }
        //    // 5
        //    // Verify that all frequencies are zero
        //    foreach (int v in d.Values)
        //    {
        //        if (v != 0)
        //        {
        //            return false;
        //        }
        //    }
        //    // 6
        //    // We know the collections are equal
        //    return true;

        //}
        //private async Task<int> DownloadImages()
        //{
        //    try
        //    {
        //        // The name of the blob should match the name of the car in the database

        //        //  create Azure Storage
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=msil;AccountKey=BrT96cQpN8FXtJ59FeKMOC1a8OE+IptOdktVypDIE90owdzEU0EF2GdBmchjP/SPyN51DFSa7g1BzIvv0jVH0w==");

        //        //  create a blob client.
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        //  create a container 
        //        CloudBlobContainer container = blobClient.GetContainerReference("msilimages");

        //        //  create a block blob
        //        if(carListFromApi != null)
        //        {
        //            //List<string> uniqueCars = new List<string>();

        //            //foreach (var car in carListFromApi)
        //            //{
        //            //    uniqueCars.Add(car.carImage);
        //            //}
        //            //uniqueCars = uniqueCars.Distinct<string>().ToList();

        //            StatusBar statusBar;
        //            statusBar = StatusBar.GetForCurrentView();
        //            //statusBar.ProgressIndicator.ProgressValue = 0;

        //            await statusBar.ProgressIndicator.ShowAsync();


        //            int carImagesToDownloadCount = listCarMaster.Count;
        //            int carImageCurrentDownloadCount = 1;

        //            foreach (var carBlob in listCarMaster)
        //            {
        //                //  create a block blob
        //                //string uri  = carBlob.Replace(@"https://msil.blob.core.windows.net/msilimages/","");
        //                string uri = carBlob.carImage;

        //                CloudBlockBlob blockBlob = container.GetBlockBlobReference(uri);

        //                //  create a local file
        //                StorageFile file = await dataFolder.CreateFileAsync(uri, CreationCollisionOption.ReplaceExisting);

     
        //                statusBar.ProgressIndicator.ProgressValue = (carImageCurrentDownloadCount / carImagesToDownloadCount);
                        
                        
        //                statusBar.ProgressIndicator.Text = "Downloading images "+ carImageCurrentDownloadCount + " of " + carImagesToDownloadCount;
        //                //  download from Azure Storage
        //                await blockBlob.DownloadToFileAsync(file);

        //                carImageCurrentDownloadCount++;
        //            }

        //            await statusBar.ProgressIndicator.HideAsync();
        //        }
        //        //CloudBlockBlob blockBlob = container.GetBlockBlobReference("CIAZ.png");

        //        //  create a local file
        //        //StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("CIAZ.png", CreationCollisionOption.ReplaceExisting);

        //        //  download from Azure Storage
        //        // await blockBlob.DownloadToFileAsync(file);

        //        return 1;
        //    }
        //    catch
        //    {
        //        //  return error
        //        return 0;
        //    }
        //}
       
        //private async Task<List<Car>> getAllCarDataFromApi(int isWithNetworkErrorMessage, int isWithExceptionErrorMessage)
        //{   // This function calls the GetAllCars web API, fetches the JSON, parses it and 
        //    // returns the list of all the cars.
        //    //This function either returns the list of all the cars, or returns a null list if any error(s) occurred.
        //    // The parameter isWithNetworkErrorMessage should be 
        //    //        0 : If no error message is to be shown to the user for HttpRequestException (as may be the case if the app has to
        //    //                silently pull the latest data without informing the user)
        //    //                    
        //    //        1 : If an error message is to be showed to the user for HttpRequestException

        //    StatusBar statusBar = StatusBar.GetForCurrentView();
        //    await statusBar.ProgressIndicator.ShowAsync();
        //    statusBar.ProgressIndicator.Text = "Checking for newer data";

        //    string apiUrl = @"http://msil.azurewebsites.net/api/ProcViewMsil/GetAllCars";
        //    string response = String.Empty; //To hold the JSON response from the web API

        //    // Create a list of Cars that will be returned by this function.
        //    List<Car> carList = null;

        //    bool isHttpRequestExceptionOccurred = false;
        //    bool isExceptionOccurred = false;

        //    try
        //    {
        //        HttpClient request = new HttpClient();
        //        //Call the API
        //        response = await request.GetStringAsync(apiUrl);
        //        response = JsonConvert.DeserializeObject(response).ToString();
        //        // Deserialize the JSOn received.
        //        carList = JsonConvert.DeserializeObject<List<Car>>(response);
        //    }
        //    catch (HttpRequestException)
        //    {
        //        isHttpRequestExceptionOccurred = true;
        //    }
        //    catch (Exception)
        //    {
        //        isExceptionOccurred = true;
        //    }

        //    //Show the error messgage to the user corresponding to HttpRequestException.
        //    if (isHttpRequestExceptionOccurred && isWithNetworkErrorMessage == 1)
        //    {
        //        MessageDialog msg = new MessageDialog("Unable to fetch latest cars data.", "No internet connectivity");
        //        await msg.ShowAsync();
        //    }
        //    else if (isExceptionOccurred && isWithExceptionErrorMessage == 1)
        //    {
        //        MessageDialog msg = new MessageDialog("Oops! Something has gone wrong. That's all we know right now.", "Error");
        //        await msg.ShowAsync();
        //    }
            
        //    await statusBar.ProgressIndicator.HideAsync();
        //    return carList;
        //}

        private void GridViewCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         //   List<Object>seletedItems = (List<Object>)(e.AddedItems);
            //string abc = ((e.AddedItems[0] as GridViewItem).FindName("TextBlockId") as TextBlock).Text;
            carMaster selCar = (carMaster)e.AddedItems[0];
            string abc = selCar.carId.ToString();

            Frame.Navigate(typeof(CarDetails), abc);
        }

        private void CommandBarRefresh_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(MainPage));
            //var page = (Window.Current.Content as Frame).Content as MainPage;
            //if (page != null)
            //{
            //    page.Refresh(); //This is a method that you implement in the page, that refreshes it
            //}
            Frame.Navigate(typeof(MainPage), "Refresh");
        }
    }
}
