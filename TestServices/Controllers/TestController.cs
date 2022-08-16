using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestCommon;
using System.Linq;
using Newtonsoft.Json;

using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json.Converters;

namespace TestServices.Controllers
{
    //  [Route("api/[controller]")]
    //[ApiController]
    public class TestController : ControllerBase
    {

        [Route("api/test/applications")]

        public ActionResult<List<Application>> GetApplications()
        {

           
            string url = "https://rpnszaidmg.execute-api.eu-west-1.amazonaws.com/Prod/applications";
            string data = Helper.GetAwsApi(url);

            IList<Application> validApplictions;
            validApplictions = Helper.DeserializeToList<Application>(data);

            return Ok(validApplictions);

        }




        public List<Card> GetCards(int appId)
        {
            string data;
            string url = "https://rpnszaidmg.execute-api.eu-west-1.amazonaws.com/Prod/cards/" + appId;
            data = Helper.GetAwsApi(url);
            
            IList<Card> validCards;
            validCards = Helper.DeserializeToList<Card>(data);

            return validCards.ToList<Card>();


        }

        [Route("api/test/trans/{appId}")]

        public ActionResult<List<Application>> GetTrans(int appId)
        {

            string url = "https://rpnszaidmg.execute-api.eu-west-1.amazonaws.com/Prod/trans/" + appId;
              string data = Helper.GetAwsApi(url);
            

            IList<Trans> validTrans;
            validTrans = Helper.DeserializeToList<Trans>(data);
            List<Card> cardList = GetCards(appId);


            var query = from c in cardList
                        join t in validTrans on c.id equals t.cardId
                        select new Trans { id = t.id,
                            amount = t.amount,
                            cardId = t.cardId,
                            transType = t.transType,
                            cardDetails = c
                        };
            return Ok(query.ToList());

        }

    }

    public class Helper   {



        public static List<string> InvalidJsonElements;
    

        public static IList<T> DeserializeToList<T>(string jsonString)
        {
            InvalidJsonElements = null;
            var array = JArray.Parse(jsonString);
            IList<T> objectsList = new List<T>();

            foreach (var item in array)
            {
                try
                {
                    // CorrectElements  
                    objectsList.Add(item.ToObject<T>());
                }
                catch (Exception ex)
                {
                    InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                    InvalidJsonElements.Add(item.ToString());
                }
            }

            return objectsList;
        }

        public static string GetAwsApi(string url)

        {

            try
            {




                //// Create request object

                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("Authorization", "9874654654987654658");
                request.ContentType = "application/json";
                HttpWebResponse response;
                response = request.GetResponse() as HttpWebResponse;
                var dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

                return responseFromServer;

            }
            catch (Exception e)
            {

                //Write to log
                throw new Exception("Api error:" + e.Message);
            }
        }
    }
}
