using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Axie.SanShare.Controllers
{
    public class ScholarController : Controller
    {
        Persistence.DB.AxieScholarDBEntities _axiedB;
        public ScholarController()
        {
            _axiedB = new Persistence.DB.AxieScholarDBEntities();
        }
        // GET: Scholar
        public async System.Threading.Tasks.Task<ActionResult> Index()

        {

           
            foreach (var ronin in _axiedB.ScholarProfiles.Where(x=> x.IsEnabled == true))
            {
                var roninAddress = ronin.RoninAddress;

                //https://axie-scho-tracker-server.herokuapp.com/api/account/ronin:f42092cd6bac4058d8c7a5bc822ea7040e5dc7fc


                //string url = string.Format("https://game-api.axie.technology/api/v1/{0}", roninAddress);

                string url = string.Format("https://axie-scho-tracker-server.herokuapp.com/api/account/{0}", roninAddress);

                HttpClient client = new HttpClient();

                Console.WriteLine(roninAddress);

                try
                {
                    string response = await client.GetStringAsync(url);

                    var data = JsonConvert.DeserializeObject<Models.TrackerModel>(response);


                    var batchCode = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now.AddHours(-8)));

                    var record = _axiedB
                        .RoninSLPs
                        .Where(x => x.RoninID == roninAddress && x.DateBatchCode == batchCode)
                        .FirstOrDefault();

                    var lastClaimDate = ConvertFromUnixTimestamp(data.slpData.lastClaim);

                    if (record != null)
                    {

                        record.LastUpdated = DateTime.Now;
                        record.Name = ronin.ScholarName;
                        record.MMR = data.leaderboardData.elo;
                        record.WalletSLP = data.slpData.roninSlp;
                        record.InGameSLP = data.slpData.gameSlp;
                        record.TotalSLP = data.slpData.totalSlp;
                        record.LastUpdated = DateTime.Now;
                        record.LastClaimDate = lastClaimDate;
                        record.RawSLPDailyYesterday = data.databaseData.slpDailyYesterday;
                        record.RawSLPTotalYesterday = data.databaseData.slpTotalYesterday;
                    }
                    else
                    {
                        _axiedB.RoninSLPs.Add(new Persistence.DB.RoninSLP
                        {
                            DateBatchCode = batchCode
                         ,
                            RoninID = roninAddress
                         ,
                            Name = ronin.ScholarName
                         ,
                            MMR = data.leaderboardData.elo
                         ,
                            WalletSLP = data.slpData.roninSlp
                          ,
                            InGameSLP = data.slpData.gameSlp
                          ,
                            LastClaimDate = lastClaimDate
                          ,
                            NextClaimDate = DateTime.Now
                          ,
                            TotalSLP = data.slpData.totalSlp
                          ,
                            DateCreated = DateTime.Now
                          ,
                            CreatedBy = "Job"
                          ,
                            LastUpdated = DateTime.Now
                            ,
                            RawSLPDailyYesterday = data.databaseData.slpDailyYesterday
                            ,
                            RawSLPTotalYesterday = 0

                        });
                    }

                    Console.WriteLine("Done: " + roninAddress);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

            }

            _axiedB.SaveChanges();

            return View();
        }


        
        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        private DateTime LongDateToDateTime(long longdate, string type)
        {
            DateTime date = DateTime.ParseExact(longdate.ToString(), type, System.Globalization.CultureInfo.InvariantCulture);
            return date;
        }


        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dateTime;
        }


        public DateTime UnixTimestampToDateTime111(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }

        public double DateTimeToUnixTimestampxxxx(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

    }
}