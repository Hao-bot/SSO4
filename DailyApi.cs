using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.Samples;
using PlayFab.EconomyModels;
using EntityKey = PlayFab.EconomyModels.EntityKey;
using Microsoft.Extensions.Configuration;
using PlayFab.ServerModels;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using SSO4Module;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Net.Http;
using System.Diagnostics;

namespace Company.Function
{
    public static class DailyApi
    {
        // [FunctionName("HttpTrigger1")]
        // public static async Task<dynamic> Run(
        //     [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        //     HttpRequest req,
        //     ILogger log) 
        // {
        //     FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        //     var apiSetting = SetData.Token(context); 
        //     var titleContex = SetData.TitleToken(context);
        // //   var serverApi = new PlayFabServerInstanceAPI(apiSetting, titleContex);string requsetBody = await new StreamReader(req.Body).ReadToEndAsync();
        //     var economyApi = new PlayFabEconomyInstanceAPI(apiSetting, titleContex);
        //     var request = new AddInventoryItemsRequest()
        //     {
        //         Amount = 1,
        //         //  IdempotencyId = idempotencyId,
        //         Item = new InventoryItemReference() { Id = "e1f48366-d087-49a5-9c27-d82270aab9f3" },
        //         Entity = new EntityKey() { 
        //             Id = context.CallerEntityProfile.Entity.Id, 
        //             Type = context.CallerEntityProfile.Entity.Type 
        //         }
        //     };
        //     return await economyApi.AddInventoryItemsAsync(request);
        // }
        // [FunctionName("GetDaylyRewards")]
        // public static async Task<IActionResult> Rnun(
        // [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,ILogger log)
        // {
        //     log.LogInformation("GetDaylyRewards function processed a request.");
        //     FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

        //     var apiSetting = SetData.Token(context);
        //     var titleContex = SetData.TitleToken(context);
        //     var serverApi = new PlayFabServerInstanceAPI(apiSetting, titleContex);
        //     var data = await serverApi.GetTitleDataAsync(GetTitleData("dailyItems"));
        //     return new OkObjectResult(data.Result.Data);
        // }
        
        public static GetTitleDataRequest GetTitleData(string key)
        {
            return new GetTitleDataRequest{
                Keys = new List<string>{key}
            };
        }
    [FunctionName("DataDailyForUserV2")]
   public static async Task<IActionResult> AddTitleByServer(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
{
    // Deserialize context and initialize PlayFab Server API
    FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
    var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));

    var titleDataResult = await server.GetTitleDataAsync(new GetTitleDataRequest { Keys = new List<string> { "dailyItems" } });
    var dailyItems = JsonConvert.DeserializeObject<List<CollectionDailyV2>>(titleDataResult.Result.Data["dailyItems"]);
    // Prepare data for user
    var dailyCollect = Root.CreateCollectionDailyList(dailyItems);
    var dailyData = Root.DailyObj(); // Assuming you have this method defined elsewhere
    // Update user data
    var updateResult = await server.UpdateUserReadOnlyDataAsync(new UpdateUserDataRequest
    {
        Data = new Dictionary<string, string>
        {
            {"dailyColection", JsonConvert.SerializeObject(dailyCollect, Formatting.None)},
            {"dailyData", JsonConvert.SerializeObject(dailyData, Formatting.None)}
        },
        PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
    });
    return new OkObjectResult(updateResult);
}


    [FunctionName("DataDailyForUser")]
    public static async Task<IActionResult> AddTitelData(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
    var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
    List<CollectionDaily> dailyCollect = Root.CollecDailyObj(SetData.DayCount);
    string dailyCollectObj = JsonConvert.SerializeObject(dailyCollect, Formatting.None);
    Daily dailyData = Root.DailyObj();
    string dailyDataObj = JsonConvert.SerializeObject(dailyData, Formatting.None);
            var result = await server.UpdateUserReadOnlyDataAsync(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>{
                {"dailyColection",dailyCollectObj},
                {"dailyData", dailyDataObj} },
                PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                Permission = UserDataPermission.Public
            });
        return new OkObjectResult(result);       
    }
     public static async Task<IActionResult> AddTitelDataUse(FunctionExecutionContext<dynamic> context,PlayFabServerInstanceAPI server)
    {
    List<CollectionDaily> dailyCollect = Root.CollecDailyObj(SetData.DayCount);
    string dailyCollectObj = JsonConvert.SerializeObject(dailyCollect, Formatting.None);
    Daily dailyData = Root.DailyObj();
    string dailyDataObj = JsonConvert.SerializeObject(dailyData, Formatting.None);
            var result = await server.UpdateUserReadOnlyDataAsync(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>{
                {"dailyColection",dailyCollectObj},
                {"dailyData", dailyDataObj} },
                PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                Permission = UserDataPermission.Public
            });
        return new OkObjectResult(result);       
    }



    [FunctionName("SetPeekDay")]
    public static async Task<IActionResult> SetPeekDay(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        var context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));

        var result = await server.GetUserReadOnlyDataAsync(
            new GetUserDataRequest
            {
                PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                Keys = new List<string> { "dailyData" }
            });

        var dailyData = JsonConvert.DeserializeObject<Daily>(result.Result.Data["dailyData"].Value);
        long currentTime =  Timming.GetUnixTimeNow();
        if (currentTime < dailyData.DayNext)
            {return new OkObjectResult("not enough time");}

        dailyData.DayNext = Timming.GetUnixTimeForNextDay1AM(Timming.GetUtcNow());
        int daysPassed = (int)((currentTime - dailyData.TimeBegin) / 86400000);
        dailyData.DayPeek += daysPassed;
    
        if (dailyData.DayPeek > SetData.DayCount)
        {
            await AddTitelDataUse(context, server);
            return new OkObjectResult(new { UpdatedDayPeek = dailyData.DayPeek, UpdatedDayNext = dailyData.DayNext, Status = "Cycle completed" });  // Return with a specific status
        }
        await server.UpdateUserReadOnlyDataAsync(new UpdateUserDataRequest
        {
            PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
            Data = new Dictionary<string, string>
            {
                { "dailyData", JsonConvert.SerializeObject(dailyData) }
            }
        });
   
        return new OkObjectResult(new { UpdatedDayPeek = dailyData.DayPeek, UpdatedDayNext = dailyData.DayNext });

    }

    [FunctionName("GetToDay")]
    public static async Task<IActionResult> GetToDay(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        // 1. Setup and Input Retrieval (unchanged)
        var context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
        var economyApi = new PlayFabEconomyInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
    
        var result = await server.GetUserReadOnlyDataAsync(new GetUserDataRequest
        {
            PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
            Keys = new List<string> {"dailyColection", "dailyData" },
            
        });
            var dailyData = JsonConvert.DeserializeObject<Daily>(result.Result.Data["dailyData"].Value);
            // 4. Reward Logic (optimized)s
            var collectionDaily = JsonConvert.DeserializeObject<List<CollectionDaily>>(result.Result.Data["dailyColection"].Value);

            if(collectionDaily[dailyData.DayPeek-1].IsGetToday == true)
            {
                 return new OkObjectResult( new { Success = false, Error = "You get to day"});
            }
            var rewardItem = collectionDaily[dailyData.DayPeek - 1].CollectionDatas;
         
            // if (isGetRecycling)
            //     dailyData.RecyclingCount--;
            collectionDaily[dailyData.DayPeek - 1] = new CollectionDaily{CollectionDatas =collectionDaily[dailyData.DayPeek - 1].CollectionDatas, DayCount = dailyData.DayPeek, IsGetToday = true };
            // // 5. Update User Data and Response (unchanged)
            var updateRequest = new UpdateUserDataRequest
            {
                PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                Data = new Dictionary<string, string>
                {
                    //{ "dailyData", JsonConvert.SerializeObject(dailyData) },
                    { "dailyColection", JsonConvert.SerializeObject(collectionDaily) }
                },
                Permission = UserDataPermission.Public
            };
            await server.UpdateUserReadOnlyDataAsync(updateRequest);
            await economyApi.AddInventoryItemsAsync(SetData.CustomDataAddItem(rewardItem.Amount, rewardItem.Id, context));
            return new OkObjectResult(new { Success = true });
    }
    [FunctionName("GetRecycling")]
    public static async Task<IActionResult> GetRecycling(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        // 1. Setup and Input Retrieval (unchanged)
        var context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
        var economyApi = new PlayFabEconomyInstanceAPI(SetData.Token(context), SetData.TitleToken(context));

        var result = await server.GetUserReadOnlyDataAsync(new GetUserDataRequest
        {
            PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
            Keys = new List<string> {"dailyColection", "dailyData" }
        });
        int dayGet = context.FunctionArgument["dayGet"] ?? 0;
        var dailyData = JsonConvert.DeserializeObject<Daily>(result.Result.Data["dailyData"].Value);
        // 4. Reward Logic (optimized)s
        var collectionDaily = JsonConvert.DeserializeObject<List<CollectionDaily>>(result.Result.Data["dailyColection"].Value);
        if(dayGet >=  dailyData.DayPeek || dayGet == 0 ||collectionDaily[dayGet - 1].IsGetToday == true)
        {
            return new OkObjectResult(new  { Success = false, Error = "Something wrong" });
        }
        var rewardItem = collectionDaily[dayGet - 1].CollectionDatas;
        await economyApi.AddInventoryItemsAsync(SetData.CustomDataAddItem(rewardItem.Amount, rewardItem.Id,context));
        dailyData.RecyclingCount--;
        collectionDaily[dayGet - 1] = new CollectionDaily {IsGetToday = true, DayCount = dayGet,CollectionDatas = collectionDaily[dayGet- 1].CollectionDatas};
        // // 5. Update User Data and Response (unchanged)
        var updateRequest = new UpdateUserDataRequest
        {
            PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
            Data = new Dictionary<string, string>
            {
                { "dailyData", JsonConvert.SerializeObject(dailyData) },
                { "dailyColection", JsonConvert.SerializeObject(collectionDaily) }
            },
            
        };
        await server.UpdateUserReadOnlyDataAsync(updateRequest);
        return new OkObjectResult(new { Success = true });
        
    }


        // [FunctionName("CheckAllDay")]
        // public static async Task<dynamic> CheckAllDay(
        // [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        // {
        //     // Deserialize the function execution context from the request body
        //     FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        //     // Create a PlayFab Server API instance
        //     var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
        //     var result =  await server.GetUserReadOnlyDataAsync(
        //         new GetUserDataRequest {
        //             PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
        //             Keys = new List<string> {"daily"},
        //         }
        //     );
        //     return new OkObjectResult(result);
        // }
    // [FunctionName("GetPeekDay")]
    // public static async Task<dynamic> GetPeekDay(
    // [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    // {
    //     // Deserialize the function execution context from the request body
    //     FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
    //     // Create a PlayFab Server API instance
    //     var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
    //         var result = await  server.GetUserReadOnlyDataAsync(
    //         new GetUserDataRequest {
    //             PlayFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId,
    //             Keys = new List<string> {"daily"},
    //         }
    //     );
    //     JObject dailyObject = JObject.Parse(result.Result.Data["daily"].Value);
    //     long dayPeek =  (long)dailyObject.GetValue("DayPeek");
    //     JArray dailyArray = (JArray)dailyObject["Daily"];
    //     JObject currentDayData = default;
    //     foreach (var day in dailyArray)
    //     {
    //         // Compare "dayPeek" with the value of "Timing" in the current day object.
    //         if (dayPeek == (long)day["Timing"])
    //         {
    //             if((bool)day["IsGetToday"]== true)
    //             {
    //                 return new OkObjectResult("you get to day");
    //             }
    //             else
    //             {
    //                 currentDayData  = (JObject)day;
    //                 break;
    //             }                
    //         }
    //     }
    //     // new 
    //     var economyApi = new PlayFabEconomyInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
    //     var resultEco  = await economyApi.AddInventoryItemsAsync(
    //             SetData.CustomDataAddItem((int)currentDayData.GetValue("Amount"),(string)currentDayData.GetValue("Id")));
    //     var updateData = await server.UpdateUserReadOnlyDataAsync(
    //         new UpdateUserDataRequest{
    //             Data = new Dictionary<string, string>{},
                

    //         }

    //     )

    //     // (3) Find the relevant Daily element
    //     return new OkObjectResult(dayPeek);
            

    
    [FunctionName("GetMissDay")]
    public static async Task<dynamic> GetMissDay(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        // Deserialize the function execution context from the request body
        FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        // Create a PlayFab Server API instance
        var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
    


        return null;
    }
    [FunctionName("GetAllMissDay")]
    public static async Task<dynamic> GetAllMissDay(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        // Deserialize the function execution context from the request body
        FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        // Create a PlayFab Server API instance
        var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
        return null;
    }
    [FunctionName("ResetNewDaily")]
    public static async Task<dynamic> ResetNewDaily(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        // Deserialize the function execution context from the request body
        FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());
        // Create a PlayFab Server API instance
        var server = new PlayFabServerInstanceAPI(SetData.Token(context), SetData.TitleToken(context));
        return null;
    } 
 
 
}
 
}

