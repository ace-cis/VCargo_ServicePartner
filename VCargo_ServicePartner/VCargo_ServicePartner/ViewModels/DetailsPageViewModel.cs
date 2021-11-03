using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using VCargo_ServicePartner.Models;
using Xamarin.Forms;

namespace VCargo_ServicePartner.ViewModels
{
    [QueryProperty(nameof(Id),nameof(Id))]
 public   class DetailsPageViewModel:BaseViewModel
    {
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

        public Command ConfirmCommand { get; }
        public Command CancelCommand { get; }

        public DetailsPageViewModel()
        {

            ConfirmCommand = new Command(OnConfirmAsync);
            CancelCommand = new Command(OnCancelAsync);
        }

        private async void OnConfirmAsync(object obj)
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
                    destination = Destination,
                    bookStatus = "Pending Delivery",
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

                    await Application.Current.MainPage.DisplayAlert("Message", "UPDATED SUCCESSFULLY! " + HWBNo + "    " + DateTime.Now.ToString(), "ok");


                    Summary newItem = new Summary()
                    {
                        Id = Guid.NewGuid().ToString(),
                        BookingId = HWBNo,
                        OrderDate = hwbdate,
                        OrdeConsignee = Consignee,
                        OrderStatus = "Pending Delivery",
                        OrderDestination = Destination,
                        OrderRefNo = MWAB,
                        Shipper = "Air Asia",
                        hwbdate = HWBDate,
                        servicemode = servicemode
                    };
                    //MCA update model here
                    await DataStore.UpdateItemAsync(newItem);

                    await Shell.Current.GoToAsync("..");

                    return;

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", responsex.StatusDescription, "Ok");
                    return;
                }




            }


        }
        private async void OnCancelAsync(object obj)
            {

                if (await Application.Current.MainPage.DisplayAlert("Message", "Are you sure you want to cancel?", "Yes", "No"))
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            } finally
            {
                IsBusy = false;
            }


            IsBusy = false;
        }

    }
}
