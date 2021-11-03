using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VCargo_ServicePartner.Models;
using VCargo_ServicePartner.Views;
using Xamarin.Forms;

namespace VCargo_ServicePartner.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public    class DeliveredDetailsPageViewModel:BaseViewModel
    {

        #region Declarion

        private string id;
        private string hwbdate;
        private string hwbno;
        private string shipper;
        private string consignee;
        private string destination;
        private string servicemode;
        private string description;
        private string cw;
        private decimal cbm;
        private string carrier;
        private string mwab;
        public bool isVisible =false;
        public bool isEnabled = true;

        

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                PreviewAsync(value);

            }
        }
        public string HWBDate
        {
            get => hwbdate;
            set => SetProperty(ref hwbdate, value);
        }
        public string HWBNo
        {
            get => hwbno;
            set => SetProperty(ref hwbno, value);
        }
        public string Shipper
        {
            get => shipper;
            set => SetProperty(ref shipper, value);
        }
        public string Consignee
        {
            get => consignee;
            set => SetProperty(ref consignee, value);
        }
        public string Destination
        {
            get => destination;
            set => SetProperty(ref destination, value);
        }
        public string ServiceMode
        {
            get => servicemode;
            set => SetProperty(ref servicemode, value);
        }
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public string CW
        {
            get => cw;
            set => SetProperty(ref cw, value);
        }
        public decimal CBM
        {
            get => cbm;
            set => SetProperty(ref cbm, value);
        }
        public string Carrier
        {
            get => carrier;
            set => SetProperty(ref carrier, value);
        }
        public string MWAB
        {
            get => mwab;
            set => SetProperty(ref mwab, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }

        #endregion

        #region Function

        public Command SetForDeliveryCommand { get; }
        public Command DeliverCommand { get; }
        public Command FailedCommand { get; }
        public DeliveredDetailsPageViewModel()
        {
            SetForDeliveryCommand = new Command(OnSetForDelivery);
            DeliverCommand = new Command(OnDeliver);
            FailedCommand = new Command(OnFailedToDeliver);
        }
        private async void OnSetForDelivery(object obj)
        {
            


            var client = new RestClient("https://161.49.173.194/api/booking?$filter=bookingId eq " + "" + HWBNo + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);

            request.AddParameter("application/json", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful == true)
            {
                //var data = JsonConvert.DeserializeObject(response.Content)
                List<Booking> result = (List<Booking>)JsonConvert.DeserializeObject(response.Content, typeof(List<Booking>));


                Booking.BookingStatus booker = new Booking.BookingStatus()
                {
                    bookingId = int.Parse(HWBNo),
                    refNo = MWAB,
                    destination =Destination,
                    bookStatus = "For Delivery",
                    createdBy = Application.Current.Properties["UserCode"].ToString()


                };

                foreach (var x in result)
                {
                    x.bookingStatus.Clear();

                    x.bookingStatus.Add(booker);
                }

                // result.Add(booker);



                //var bookstatus = JsonConvert.SerializeObject(booker);
                // updateBook.bookingStatus.Add(bookstatus);

                string stringjson = JsonConvert.SerializeObject(result);
                string stringjsonX = stringjson.TrimStart('[').TrimEnd(']');



                var clientx = new RestClient("http://18.139.49.140:8089/api/booking/u");
                clientx.Timeout = -1;
                var requestx = new RestRequest(Method.PATCH);
                requestx.AddHeader("Content-Type", "application/json");
                requestx.AddParameter("application/json", JObject.Parse(stringjsonX), ParameterType.RequestBody);
                IRestResponse responsex = clientx.Execute(requestx);
                if (responsex.IsSuccessful == true)
                {

                    await Application.Current.MainPage.DisplayAlert("Message", "STATUS SENT! " + HWBNo + "    " + DateTime.Now.ToString(), "ok");
                    IsVisible = true;
                    IsEnabled = false;
                    return;

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", responsex.StatusDescription, "Ok");
                    return;
                }




            }

           
        }
        private async void OnFailedToDeliver(object obj)
        {
            
           // _RefNo = value.ToString();


            var client = new RestClient("https://161.49.173.194/api/booking?$filter=bookingId eq " + "" + HWBNo + "");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);

            request.AddParameter("application/json", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful == true)
            {
                //var data = JsonConvert.DeserializeObject(response.Content)
                List<Booking> result = (List<Booking>)JsonConvert.DeserializeObject(response.Content, typeof(List<Booking>));


                Booking.BookingStatus booker = new Booking.BookingStatus()
                {
                    bookingId = int.Parse(HWBNo),
                    refNo = MWAB,
                    destination =Destination,
                    bookStatus = "Failed"


                };

                foreach (var x in result)
                {
                    x.bookingStatus.Clear();

                    x.bookingStatus.Add(booker);
                }

                // result.Add(booker);



                //var bookstatus = JsonConvert.SerializeObject(booker);
                // updateBook.bookingStatus.Add(bookstatus);

                string stringjson = JsonConvert.SerializeObject(result);
                string stringjsonX = stringjson.TrimStart('[').TrimEnd(']');



                var clientx = new RestClient("http://18.139.49.140:8089/api/booking/u");
                clientx.Timeout = -1;
                var requestx = new RestRequest(Method.PATCH);
                requestx.AddHeader("Content-Type", "application/json");
                requestx.AddParameter("application/json", JObject.Parse(stringjsonX), ParameterType.RequestBody);
                IRestResponse responsex = clientx.Execute(requestx);
                if (responsex.IsSuccessful == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Message", "Deliveries " + HWBNo + " with order reference # " + MWAB + " is now in Failed status.", "Ok");


                    Summary newItem = new Summary()
                    {
                        Id = Guid.NewGuid().ToString(),
                        BookingId = HWBNo,
                        OrderDate = hwbdate,
                        OrdeConsignee = Consignee,
                        OrderStatus = "Failed",
                        OrderDestination = Destination,
                        OrderRefNo = MWAB,
                        Shipper = "Air Asia",
                        hwbdate = HWBDate,
                        servicemode = servicemode
                    };
                    //MCA update model here
                    await DataStore.UpdateItemAsync(newItem);

                    await Shell.Current.GoToAsync("..");


                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", responsex.StatusDescription, "Ok");
                    return;
                }


            }

        }   
        private async void OnDeliver(object obj)
        {
            await Shell.Current.GoToAsync($"{nameof(ImageAndSignaturePage)}?{nameof(ImageAndSignatureViewModel.Id)}={HWBNo}");
            Application.Current.Properties["BookingId"] = HWBNo;
        }

        #endregion

        #region Load
        public async Task PreviewAsync(string id)
        {
            IsBusy = true;

            try
            {

                var itemDetails = await DataStore.GetItemAsync(id);
                HWBDate = itemDetails.OrderDate;
                HWBNo = itemDetails.BookingId;
                Shipper = itemDetails.Shipper;
                Consignee = itemDetails.OrdeConsignee;
                Destination = itemDetails.OrderDestination;
                ServiceMode = itemDetails.servicemode;
                description = "Sample " + itemDetails.BookingId;
                CW = itemDetails.cw;
                cbm = itemDetails.cbm;
                Carrier = "Air Asia";
                MWAB = itemDetails.OrderRefNo;


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }


            IsBusy = false;
        }

        #endregion


    }
}
