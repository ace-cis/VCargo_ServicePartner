using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VCargo_ServicePartner.Models;
using Xamarin.Forms;

namespace VCargo_ServicePartner.Services
{
    public class MockDataStore : IDataStore<Summary>
    {
        private  List<Summary> items;
        
        public MockDataStore()
        {

            var client = new RestClient("https://161.49.173.194/api/booking?$filter=courier eq " + "'" + Application.Current.Properties["UserCode"].ToString() + "'");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);

            request.AddParameter("application/json", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful == true)
            {
                //var data = JsonConvert.DeserializeObject(response.Content)
                List<Booking> result = (List<Booking>)JsonConvert.DeserializeObject(response.Content, typeof(List<Booking>));


                items = new List<Summary>();
                foreach (Booking book in result)
                {
                    int i = 0;
                    int x = book.bookingStatus.Count;
                    var sortedItems = book.bookingStatus.OrderBy(c => c.lineNum);
                  //  var line = sortedItems.Select(g => g.Max(j => j.lineNum));
                    //var result = grouped.Where(g => ids.Contains(g.Id));


                    foreach (Booking.BookingStatus BS in sortedItems)
                    {
                        i++;
                        

                      //  var lastStatus = sortedItems.Where(t=> sortedItems.Contains(t.bookingId));




                        if (i==x)
                        {
                            items.Add(new Summary() { 
                                Id = Guid.NewGuid().ToString(), 
                                BookingId = BS.bookingId.ToString(),
                                OrderDate=book.postingDate.Substring(0,10).ToString(), 
                                OrdeConsignee = book.consignee, 
                                OrderStatus = BS.bookStatus,  
                                OrderDestination = BS.destination,
                                OrderRefNo=BS.refNo,
                                Shipper=book.cardName ,
                                hwbdate = book.docDate.Substring(0, 10).ToString(),
                                servicemode =book.serviceMode
                            });


                        }


                    }

                }





            }





        //    return await Task.FromResult(items);


            //   items = new List<Pending>()
            //   {
            //       new Pending { Id = Guid.NewGuid().ToString(), TLC_DocEntry = 1,    OrderRefNo=78991,       TLC_ToWHS="Lazada",TLC_DocDate="2021-09-01",  TLC_Filler="Cebu City" },
            //       new Pending { Id = Guid.NewGuid().ToString(), TLC_DocEntry =12,    OrderRefNo=24545,       TLC_ToWHS="Shoppee",TLC_DocDate="2021-09-11",  TLC_Filler="Tarlac City" },
            //       new Pending { Id = Guid.NewGuid().ToString(), TLC_DocEntry = 123,  OrderRefNo= 454577,     TLC_ToWHS="Alibaba",TLC_DocDate="2021-09-31",  TLC_Filler="Mandaue City" },
            //       new Pending { Id = Guid.NewGuid().ToString(), TLC_DocEntry = 1234, OrderRefNo= 909962,    TLC_ToWHS="Lalamove",TLC_DocDate="2021-09-21",  TLC_Filler="Marikina City." },
            //       new Pending { Id = Guid.NewGuid().ToString(), TLC_DocEntry = 12345,OrderRefNo= 876669,   TLC_ToWHS="Food Panda",TLC_DocDate="2021-09-11",  TLC_Filler="Old Clark City" },
            //       new Pending { Id = Guid.NewGuid().ToString(), TLC_DocEntry = 123456,OrderRefNo = 097732, TLC_ToWHS="Star Bucks",TLC_DocDate="2021-09-18",  TLC_Filler="New Clark City" }
            //   };
        }

        public async Task<bool> AddItemAsync(Summary item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Summary item)
        {
            var oldItem = items.Where((Summary arg) => arg.BookingId == item.BookingId).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Summary arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Summary>> GetBookingAsync(bool forceRefresh = false, string _cardcode = "")
        {
            var client = new RestClient("https://161.49.173.194/api/booking?$filter=cardCode eq " + "'" + _cardcode + "'");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);

            request.AddParameter("application/json", "", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful == true)
            {
                //var data = JsonConvert.DeserializeObject(response.Content)
                List<Booking> result = (List<Booking>)JsonConvert.DeserializeObject(response.Content, typeof(List<Booking>));


                items = new List<Summary>();
                foreach (Booking book in result)
                {
                    int i = 0;
                    int x = book.bookingStatus.Count;
                    foreach (Booking.BookingStatus BS in book.bookingStatus)
                    {
                        i++;

                        if (x == i)
                        {
                            items.Add(new Summary() { Id = Guid.NewGuid().ToString(), OrderRefNo = BS.bookingId.ToString(), OrdeConsignee = book.consignee, OrderStatus = BS.bookStatus ,OrderDestination = BS.destination });

                        }


                    }

                }





            }





            return await Task.FromResult(items);
        }

        public async Task<Summary> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.BookingId == id));
        }

        public async Task<IEnumerable<Summary>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}