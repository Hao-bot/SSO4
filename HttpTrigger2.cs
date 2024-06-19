using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class HttpTrigger2
    {
        [FunctionName("HttpTrigger2")]
        public static async Task<dynamic> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            var datas = new { Name = "Alice", Age = 30, City = "New York" }; 

            // Serialize the data object into a JSON string
            string json = JsonConvert.SerializeObject(data, Formatting.Indented); // Indent for readability

            // Create a ContentResult and set the content type to application/json
            var contentResult = new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200 // OK status code
            };

            return contentResult;        }
    }
}
