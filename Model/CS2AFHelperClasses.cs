// Copyright (C) Microsoft Corporation. All rights reserved.

namespace PlayFab.Samples
{
    using System;
    using System.Collections.Generic;
    using PlayFab.EconomyModels;
    using SSO4Module;


    // Shared models
    public static  class    SetData{
        public static readonly int DayCount = 7;
        public static readonly string DataJsonPath ="Model/Data.json";
        public static readonly string Title =   "BA398"; // Set your actual Title ID
        public static readonly string  DevKey =  "5ZG8U4KSNFC8Q6AI8ZFG5BJCWTOSDYMKZOADS5PZ4T6NGB54TT";
        public static readonly string BoneGetId = "ddd7919a-ac88-40e6-806f-4ebdbf68fdbc";
        public static readonly string TokenId = "b2b0238c-983b-4fac-b394-9e323b1f217c";
        public static readonly string BundleContentV2Id = "4eac38d0-a45e-4c2b-969e-eac5242fdc06";
        public static readonly string BundleContentV1Id = "e93efa64-9e92-4df2-900b-1524ada1fa7b";
        public static string dailyData = @"
                {
                    ""dayCurrent"": null,
                    ""dayCount"": null,
                }";
        // add reward for new daily
        public static readonly   List <CollectionData> DataDailyReward = new List<CollectionData>{
            // day 1
            new CollectionData{ Amount = 10, Id = BoneGetId},
            // day 2 
            new CollectionData{ Amount = 20, Id = BoneGetId},
            //day n....
            new CollectionData{ Amount = 1, Id = BundleContentV1Id},

            new CollectionData{ Amount = 1, Id = BundleContentV2Id},
            new CollectionData {Amount = 2, Id = BundleContentV1Id},
            new CollectionData {Amount = 2, Id = BundleContentV2Id},
            new CollectionData {Amount = 500, Id = BoneGetId}
        };
        public static PlayFabApiSettings Token(FunctionExecutionContext<dynamic> token)
        {
             return new PlayFabApiSettings()
             {
                 TitleId = token.TitleAuthenticationContext.Id,
                 DeveloperSecretKey = DevKey,
             };
        }
         public static PlayFabAuthenticationContext TitleToken(FunctionExecutionContext<dynamic> token)
        {
            return new PlayFabAuthenticationContext()
            {
                EntityToken = token.TitleAuthenticationContext.EntityToken
            };

        }
        public static  AddInventoryItemsRequest CustomDataAddItem(int amount, string id,FunctionExecutionContext<dynamic> context )
        {
            return new AddInventoryItemsRequest{
            Amount = amount,
               Item = new InventoryItemReference
               {
                Id = id,
                StackId = "Bone"
               },
               Entity = new EntityKey() { 
                     Id = context.CallerEntityProfile.Entity.Id, 
                    Type = context.CallerEntityProfile.Entity.Type 
                }
               
               
               
            };
        } 
    }
    public class TitleAuthenticationContext
    {
        public string Id { get; set; }
        public string EntityToken { get; set; }

    }

    // Models  via ExecuteFunction API
    public class FunctionExecutionContext<T>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class FunctionExecutionContext : FunctionExecutionContext<object>
    {
    }

    // Models via Player PlayStream event, entering or leaving a 
    // player segment or as part of a player segment based scheduled task.
    public class PlayerPlayStreamFunctionExecutionContext<T>
    {
        public PlayFab.CloudScriptModels.PlayerProfileModel PlayerProfile { get; set; }
        public bool PlayerProfileTruncated { get; set; }
        public PlayFab.CloudScriptModels.PlayStreamEventEnvelopeModel PlayStreamEventEnvelope { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class PlayerPlayStreamFunctionExecutionContext : PlayerPlayStreamFunctionExecutionContext<object>
    {
    }

    // Models via Scheduled task
    public class PlayStreamEventHistory
    {
        public string ParentTriggerId { get; set; }
        public string ParentEventId { get; set; }
        public bool TriggeredEvents { get; set; }
    }

    public class ScheduledTaskFunctionExecutionContext<T>
    {
        public PlayFab.CloudScriptModels.NameIdentifier ScheduledTaskNameId { get; set; }
        public Stack<PlayStreamEventHistory> EventHistory { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public T FunctionArgument { get; set; }
    }

    public class ScheduledTaskFunctionExecutionContext : ScheduledTaskFunctionExecutionContext<object>
    {
    }

    // Models via entity PlayStream event, entering or leaving an 
    // entity segment or as part of an entity segment based scheduled task.
    public class EventFullName
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
    }

    public class OriginInfo
    {
        public string Id { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class EntityPlayStreamEvent<T>
    {
        public string SchemaVersion { get; set; }
        public EventFullName FullName { get; set; }
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public PlayFab.CloudScriptModels.EntityKey Entity { get; set; }
        public PlayFab.CloudScriptModels.EntityKey Originator { get; set; }
        public OriginInfo OriginInfo { get; set; }
        public T Payload { get; set; }
        public PlayFab.ProfilesModels.EntityLineage EntityLineage { get; set; }
    }

    public class EntityPlayStreamEvent : EntityPlayStreamEvent<object>
    {
    }

    public class EntityPlayStreamFunctionExecutionContext<TPayload, TArg>
    {
        public PlayFab.ProfilesModels.EntityProfileBody CallerEntityProfile { get; set; }
        public EntityPlayStreamEvent<TPayload> PlayStreamEvent { get; set; }
        public TitleAuthenticationContext TitleAuthenticationContext { get; set; }
        public bool? GeneratePlayStreamEvent { get; set; }
        public TArg FunctionArgument { get; set; }
    }

    public class EntityPlayStreamFunctionExecutionContext : EntityPlayStreamFunctionExecutionContext<object, object>
    {
    }
}