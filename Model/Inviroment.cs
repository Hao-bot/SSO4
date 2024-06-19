using System.Collections.Generic;

public class DailyRewards
{
    public Dictionary<string, List<Item>> dailyItems { get; set; }
}

public class Item
{
    public string name { get; set; }
    public int Amount { get; set; }
    public int quantity { get; set; } // Thêm thuộc tính quantity để xử lý trường hợp có hoặc không có
}
