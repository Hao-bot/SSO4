
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PlayFab.Samples;
using PlayFab;
using PlayFab.DataModels;
using System.Collections.Generic;
using PlayFab.ServerModels;
using System;
using PlayFab.EconomyModels;

namespace Company.Function
{
    public static class BoneReward
    {
    public static readonly string BoneGetId = "ddd7919a-ac88-40e6-806f-4ebdbf68fdbc";
        [FunctionName("GiveBone")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
{
    //SetData.Set();
    FunctionExecutionContext<dynamic> context = JsonConvert.DeserializeObject<FunctionExecutionContext<dynamic>>(await req.ReadAsStringAsync());

    string playFabId = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;
    string entityId = context.CallerEntityProfile.Entity.Id;
    string entityType = context.CallerEntityProfile.Entity.Type;

    try
    {
        var getUserDataResult = await PlayFabServerAPI.GetUserInternalDataAsync(new GetUserDataRequest
        {
            PlayFabId = playFabId,
            Keys = new List<string> { "lastReceivedTime" }
        });

        long lastReceivedTime = Convert.ToInt64(getUserDataResult.Result.Data.GetValueOrDefault("lastReceivedTime")); // Default to 0 if not found
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (currentTime - lastReceivedTime < 15000)
        {
            return new OkObjectResult(new { success = false, message = "Delay time not reached" });
        }

        var grantItemsRequest = new AddInventoryItemsRequest
        {
            Amount = 2,
            Item = new InventoryItemReference { Id = BoneGetId },
            Entity =  { Id = entityId, Type = entityType } // Set EntityKey for the player
        };
        
        var grantItemsResult = await PlayFabEconomyAPI.AddInventoryItemsAsync(grantItemsRequest);
        
        if (grantItemsResult.Error != null)
        {
            log.LogError($"Failed to grant items: {grantItemsResult.Error.GenerateErrorReport()}");
            return new BadRequestObjectResult(new { success = false, error = "Failed to grant items" });
        }

        // Update lastReceivedTime only after successful grant
        var updateUserDataRequest = new UpdateUserInternalDataRequest
        {
            PlayFabId = playFabId,
            Data = new Dictionary<string, string> { { "lastReceivedTime", currentTime.ToString() } }
        };
        await PlayFabServerAPI.UpdateUserInternalDataAsync(updateUserDataRequest);

        return new OkObjectResult(new { success = true, message = "Bone granted successfully" });

    }
    catch (PlayFabException ex)
    {
        log.LogError(ex, "PlayFab API error");
        return new BadRequestObjectResult(new { success = false, error = ex.Message });
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Unexpected error");
        return new StatusCodeResult(StatusCodes.Status500InternalServerError); 
    }
}

// ... rest of your code (GrantItemsToUser, SetDelayTime, etc.)
}


}

