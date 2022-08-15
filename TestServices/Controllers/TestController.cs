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

            string data;
            string url = "https://rpnszaidmg.execute-api.eu-west-1.amazonaws.com/Prod/applications";
           // string data = Helper.GetAwsApi(url);

            //todo:remark

            data = @" [{""id"": 1,
        ""firstName"": ""Charlee"",
        ""lastName"": ""Davenport""
    },
    {
                ""id"": 2,
        ""firstName"": ""Summer"",
        ""lastName"": ""Mccann""
    } ]";



            IList<Application> validApplictions;
            validApplictions = Helper.DeserializeToList<Application>(data);

            return Ok(validApplictions);

        }




        public List<Card> GetCards(int appId)
        {
            string data;
            string url = "https://rpnszaidmg.execute-api.eu-west-1.amazonaws.com/Prod/cards/" + appId;
          //   data = Helper.GetAwsApi(url);
            //todo:remark
            data = @" [{
        ""id"": 1,
        ""cardNo"": 533715687715708,
        ""issuer"": ""Mastercard""
    },
    {
        ""id"": 2,
        ""cardNo"": 422651798124325,
        ""issuer"": ""Visa""
    },
    {
        ""id"": 3,
        ""cardNo"": 650072579646228,
        ""issuer"": ""Mastercard""
    } ]";



            IList<Card> validCards;
            validCards = Helper.DeserializeToList<Card>(data);

            return validCards.ToList<Card>();


        }

        [Route("api/test/trans/{appId}")]

        public ActionResult<List<Application>> GetTrans(int appId)
        {

            string url = "https://rpnszaidmg.execute-api.eu-west-1.amazonaws.com/Prod/trans/" + appId;
            //  string data = Helper.GetAwsApi(url);
            //todo:remark
            string data;
            if (appId==1)
            data = @" [ {
        ""id"": 1,
        ""transType"": 1,
        ""amount"": 336,
        ""cardId"": 2
    },
    {
        ""id"": 2,
        ""transType"": 3,
        ""amount"": 972,
        ""cardId"": 2
    },
    {
       ""id"": 3,
        ""transType"": 2,
        ""amount"": 778,
        ""cardId"": 1
    },
    {
        ""id"": 4,
        ""transType"": 3,
        ""amount"": 321,
        ""cardId"": 2
    } ]";

            else

                data = @" [ 
    {
       ""id"": 3,
        ""transType"": 2,
        ""amount"": 778,
        ""cardId"": 1
    },
    {
        ""id"": 4,
        ""transType"": 3,
        ""amount"": 321,
        ""cardId"": 2
    } ]";


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
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("Authorization", "9874654654987654658");
                request.ContentType = "application/json";
                HttpWebResponse response;
                response = request.GetResponse() as HttpWebResponse;


                return response.ToString();

            }
            catch (Exception e)
            {

                //Write to log
                throw new Exception("Api error:" + e.Message);
            }
        }
    }
}
