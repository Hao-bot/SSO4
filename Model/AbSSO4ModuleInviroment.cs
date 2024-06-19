using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.Samples;
using PlayFab.ServerModels;

public  static  class  AbSSO4ModuleInviroment
{

    // public  static FunctionExecutionContext<dynamic> context {get; set;}

    // public static PlayFabServerInstanceAPI server = new PlayFabServerInstanceAPI(SetData.Token(context),SetData.TitleToken(context));

    // protected PlayFabEconomyInstanceAPI economyApi = new PlayFabEconomyInstanceAPI(SetData.Token(context), SetData.TitleToken(context));

    // protected  string currentPlayer = context.CallerEntityProfile.Lineage.MasterPlayerAccountId;

    // protected virtual UpdateUserDataRequest updateDataRequest(Dictionary<string, string> keyValues)
    // {
    //     return new UpdateUserDataRequest{
         
    //         Data = keyValues,
    //         PlayFabId = currentPlayer
    //     };
    // }
    // protected virtual  GetUserDataRequest getUserDataRequest(List<string>keys)
    // {
    //     return new GetUserDataRequest{
    //         PlayFabId = currentPlayer,
    //         Keys = keys
    //     };
    // }
    // protected virtual GetTitleDataRequest GetTitleData(string key)
    //     {
    //         return new GetTitleDataRequest{
    //             Keys = new List<string>{key}
    //         };
    //     }
}