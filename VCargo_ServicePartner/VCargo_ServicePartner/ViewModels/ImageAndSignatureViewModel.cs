using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using RestSharp;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VCargo_ServicePartner.Models;
using Xamarin.Forms;

namespace VCargo_ServicePartner.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class ImageAndSignatureViewModel:BaseViewModel
    {

        

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

        private SignaturePadView _sign;
        public SignaturePadView Sign
        {
            get { return _sign; }
            set { SetProperty(ref _sign, value); }
        }

        #region Declaration

        private ImageSource source;
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
        byte[] imageArray = null;
        public string HWBDate
        {
            get => hwbdate;
            set => SetProperty(ref hwbdate, value);
        }
        public string receivername;

        public string Receivername
        {
            get => receivername;
            set => SetProperty(ref receivername, value);
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

        public ImageSource Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }

        #endregion 

        public Command SubmitCommand { get; }
        public Command AddImage { get; }
        public ImageAndSignatureViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
            AddImage = new Command(OnAddImage);
            Sign = new SignaturePadView();
        }
        private async void OnAddImage(object obj)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
             await  Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "VCargo",
                Name = HWBNo +".jpg"
            });

            if (file == null)
                return;

            await Application.Current.MainPage.DisplayAlert("Message", file.Path, "OK");

            using (MemoryStream memory = new MemoryStream())
            {

                Stream stream = file.GetStream();
                stream.CopyTo(memory);
                imageArray = memory.ToArray();
            }

            Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });


        }
        private void OnSubmit(object obj)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

             //   Stream SignatureImage = await Sign.GetImageStreamAsync(SignatureImageFormat.Jpeg);
             //   var mstream = (MemoryStream)SignatureImage;

                // mstream = new MemoryStream(mstream.ToArray());
             //   using (MemoryStream memory = new MemoryStream())
             //   {

             //       Stream stream = SignatureImage.GetStream();
             //       stream.CopyTo(memory);
             //       Singdata = memory.ToArray();
              //  }
               


                if (await Application.Current.MainPage.DisplayAlert("Message", "Are you sure you want to submit?", "Yes", "No"))
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
                            bookStatus = "Success",
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

                            var ImageString= new ImageModel()
                             {
                                 bookingId = int.Parse(HWBNo),
                                 imgType = "Attachement",
                                 img = imageArray,
                                 createdBy = int.Parse(Application.Current.Properties["UserCode"].ToString())
                             };

                            //Attachment
                            //  List<ImageModel> ImageString = new List<ImageModel>();

                            // ImageString.Add(new ImageModel()
                            // {
                            //     bookingId = int.Parse(HWBNo),
                            //     imgType = "Attachement",
                            //     img = imageArray,
                            //     createdBy = int.Parse(Application.Current.Properties["UserCode"].ToString())
                            // });
                            //Signature
                            // ImageString.Add(new ImageModel()
                            // {
                            //     bookingId = int.Parse(HWBNo),
                            //    imgType = "Signature",
                            //   img = imageArray,
                            //   createdBy = int.Parse(Application.Current.Properties["UserCode"].ToString())
                            // });


                            string imagejson = JsonConvert.SerializeObject(ImageString);

                            var clientV = new RestClient("https://161.49.173.194/api/bookingimg");
                            clientV.Timeout = -1;
                            var requestY = new RestRequest(Method.POST);

                            requestY.AddParameter("application/json", JObject.Parse(imagejson), ParameterType.RequestBody);
                            IRestResponse responseY = client.Execute(requestY);

                            if (responseY.IsSuccessful == true)
                            {
                                await Application.Current.MainPage.DisplayAlert("Message", "Deliveries " + HWBNo + " with order reference # " + MWAB + " is now in Success status.", "Ok");

                                Summary newItem = new Summary()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    BookingId = HWBNo,
                                    OrderDate = hwbdate,
                                    OrdeConsignee = Consignee,
                                    OrderStatus = "Success",
                                    OrderDestination = Destination,
                                    OrderRefNo = MWAB,
                                    Shipper = "Air Asia",
                                    hwbdate = HWBDate,
                                    servicemode = servicemode
                                };
                                //MCA update model here
                                await DataStore.UpdateItemAsync(newItem);

                                Application.Current.MainPage = new AppShell();
                            }

                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", responsex.StatusDescription, "Ok");
                            return;
                        }


                    }


                }
            });
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

    }
}
