using MsilCatalogue.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsilCatalogue.Helpers
{
    class DatabaseHelperClass
    {
        SQLiteConnection dbConn;

        //Create Tabble 
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Car>(); // Table name is Car
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> CheckFileExists(string fileName)
        {   // Function needs the file name as string and checks the local app folder for a file with this name.
            // Function returns true if file is present, else it returns false
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Retrieve the specific car data from the database. 
        public Car ReadCar(int carId)
        { // This function needs a car id and returns the first car object that matches the in the table.
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingCar = dbConn.Query<Car>("select * from Car where carId =" + carId).FirstOrDefault();
                return existingCar;
            }
        }

        public ObservableCollection<Car> ReadCarVariantsByCarId(int carId)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Car> myCollection = dbConn.Query<Car>("SELECT * FROM Car WHERE carId =" + carId);
                ObservableCollection<Car> carList = new ObservableCollection<Car>(myCollection);
                return carList;
            }
        }

        public ObservableCollection<CarDetailsPrices> ReadCarPricesByCarIdAndCityName(int carId, string cityName)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<CarDetailsPrices> myCollection = dbConn.Query<CarDetailsPrices>("SELECT t1.carId,t1.carName,t1.variantName, t1.Carprice as NonMetallic ,t2.carprice as Metallic from Car t1 , Car t2 " +
                "where t1.carId = t2.carId and t1.VariantId = t2.VariantId  and t1.CityId = t2.CityId and t1.Metallic = 0 and t2.Metallic= 1 and t1.carId = '" + carId + "'" +" AND t1.CityName = " +"'" + cityName + "'");
                ObservableCollection<CarDetailsPrices> carList = new ObservableCollection<CarDetailsPrices>(myCollection);
                return carList;
            }
        }

        public ObservableCollection<string> ReadDistinctStates()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Car> myCollection = dbConn.Query<Car>("SELECT DISTINCT C_State from Car");
                ObservableCollection<string> stateList = new ObservableCollection<string>();

                //ObservableCollection<string> stateList = new ObservableCollection<string>();
                foreach (var car in myCollection)
                {
                    stateList.Add(car.C_State);
                }
                return stateList;
            }
        }

        public ObservableCollection<string> ReadCitiesByStateName(string stateName)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Car> myCollection = dbConn.Query<Car>("SELECT DISTINCT CityName from Car where C_State = '" + stateName + "'");
                //ObservableCollection<Car> cityList = new ObservableCollection<Car>(myCollection);

                ObservableCollection<string> cityList = new ObservableCollection<string>();
                foreach (var car in myCollection)
                {
                    cityList.Add(car.CityName);
                }
                return cityList;
            }
        }
        public ObservableCollection<Car> ReadCarPricesByCarIdAndCityName(int carId, int cityId)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Car> myCollection = dbConn.Query<Car>("SELECT * from Car where carId = " + carId + " AND cityId = " + cityId);
                ObservableCollection<Car> carList = new ObservableCollection<Car>(myCollection);
                return carList;
            }
        }

        public ObservableCollection<carMaster> ReadCarMaster()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<carMaster> myCollection = dbConn.Query<carMaster>("SELECT distinct carId, carName, carImage from Car ");
                ObservableCollection<carMaster> carList = new ObservableCollection<carMaster>(myCollection);
                return carList;
            }
        }

        // Retrieve the all contact list from the database. 
        public ObservableCollection<Car> ReadAllCars()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Car> myCollection = dbConn.Table<Car>().ToList<Car>();
                ObservableCollection<Car> CarList = new ObservableCollection<Car>(myCollection);
                return CarList;
            }
        }


        //Update existing car data
        public void UpdateCar(Car car)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingCar = dbConn.Query<Car>("select * from Car where Id =" + car.carId).FirstOrDefault();
                if (existingCar != null)
                {
                    existingCar.carName = car.carName;
                    existingCar.VariantId = car.VariantId;
                    existingCar.variantName = car.variantName;
                    existingCar.CityId = car.CityId;
                    existingCar.CityName = car.CityName;
                    existingCar.C_State = car.C_State;
                    existingCar.colours = car.colours;
                    existingCar.Metallic = car.Metallic;
                    car.CarPrice = car.CarPrice;

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingCar);
                    });
                }
            }
        }

        // Insert the new car in the Car table. 
        public void InsertCar(Car newCar)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newCar);
                });
            }
        }

        //Delete specific Car 
        public void DeleteCar(int Id)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingCar = dbConn.Query<Car>("select * from Car where carId =" + Id).FirstOrDefault();
                if (existingCar != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingCar);
                    });
                }
            }
        }

        //Delete all Cars
        public void DeleteAllCars()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                //dbConn.RunInTransaction(() => 
                //   { 
                dbConn.DropTable<Car>();
                dbConn.CreateTable<Car>();
                dbConn.Dispose();
                dbConn.Close();
                //}); 
            }
        }
    }
}
