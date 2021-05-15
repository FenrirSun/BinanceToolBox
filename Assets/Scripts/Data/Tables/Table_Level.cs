using System.Collections.Generic;

public class Table_Level : TableBase
{
    public int Id { get; set; }
    public int Level { get; set; }
    public int Difficulty { get; set; }
    public int Coin { get; set; }
    public string Desc { get; set; }
    
    public static List<Table_Level> tableList;
    public static Dictionary<int, Table_Level> nameTableDic;

    public static Table_Level GetTableByIndex(int index)
    {
        if (index >= 0 && index < tableList.Count)
            return tableList[index];
        return null;
    }

    public static Table_Level GetTableById(int id)
    {
        if (nameTableDic.ContainsKey(id))
            return nameTableDic[id];
        return null;
    }

    public static void InitTable<T>(List<object> oriList)
    {
        tableList = new List<Table_Level>();
        nameTableDic = new Dictionary<int, Table_Level>();
        foreach (var table in oriList)
        {
            if (table is Table_Level item)
            {
                tableList.Add(item);
                nameTableDic.Add(item.Id, item);
            }
        }
    }
}