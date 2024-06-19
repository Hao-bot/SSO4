using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using PlayFab.Samples;

namespace SSO4Module
{

    // Define the Collection class
    public struct CollectionData
    {
        public string Id;
        public int Amount;
    }

    // Define the Daily class
    public struct CollectionDaily
    {
        public int DayCount;
        public CollectionData CollectionDatas;
        public bool IsGetToday;
    }
    public struct Daily
    {
        public int DayPeek;
        public long DayNext;
        public int RecyclingCount;
        public long TimeBegin;

    }
    // Define the Recycling class

    // Define the Root class
    public static class Root
    {
        public static List<CollectionDaily> CollecDailyObj(int count)
        {
            List<CollectionDaily> data = new List<CollectionDaily>();
            for (int i = 0;i < count; i++)
            {
                data.Add(new CollectionDaily{
                    DayCount = i+1,
                    IsGetToday = false,
                    // the data fill in  -> SetData.DataDailyReward
                    // don't forget count must be == SetData.DataDailyReward.Count
                    CollectionDatas =SetData.DataDailyReward[i]});                
            }
            return data;
        }
        public static List<CollectionDaily> CollecDailyObjV2(int count, CollectionDailyV2List items)
        {
            List<CollectionDaily> data = new List<CollectionDaily>();
             for (int i = 0;i < count; i++)
            {
                data.Add(new CollectionDaily{
                    DayCount = items.dailyItems[i].DayCount,
                    IsGetToday = false,
                    // the data fill in  -> SetData.DataDailyReward
                    // don't forget count must be == SetData.DataDailyReward.Count
                    CollectionDatas = new CollectionData{
                        Id = items.dailyItems[i].Id,
                        Amount = items.dailyItems[i].Amount
                    }});
            }                    
            return data;

        }
     public static List<CollectionDaily> CreateCollectionDailyList(List<CollectionDailyV2> items)
    {
        return items.Select((item, index) => new CollectionDaily
        {
            DayCount = index + 1, // DayCount starts at 1
            IsGetToday = false,
            CollectionDatas = new CollectionData
            {
                Id = item.Id,
                Amount = item.Amount
            }
        }).ToList();
    }
        public static Daily DailyObj()
        {
            DateTimeOffset time = Timming.GetUtcNow();
            long timeUnix = Timming.GetUnixTimeForNextDay1AM(time);
            return new Daily{
                DayPeek = 1,
                DayNext = timeUnix,
                RecyclingCount = 0,
                TimeBegin = timeUnix - 86400000
            };
        }
    }
    public class CollectionDailyV2
        {
            public string Id; 
            public int Amount;
            public int DayCount;
        }

        public class CollectionDailyV2List
        {
            public List<CollectionDailyV2> dailyItems;
        }

    



}

             

    // Define the static Data class

