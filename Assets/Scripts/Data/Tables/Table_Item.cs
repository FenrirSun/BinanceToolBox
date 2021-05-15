using System.Collections.Generic;

public class Table_Item : TableBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    
    public static List<Table_Item> tableList;
    public static Dictionary<int, Table_Item> nameTableDic;

    public static Table_Item GetTableByIndex(int index)
    {
        if (index >= 0 && index < tableList.Count)
            return tableList[index];
        return null;
    }

    public static Table_Item GetTableById(int id)
    {
        if (nameTableDic.ContainsKey(id))
            return nameTableDic[id];
        return null;
    }

    public static void InitTable<T>(List<object> oriList)
    {
        tableList = new List<Table_Item>();
        nameTableDic = new Dictionary<int, Table_Item>();
        foreach (var table in oriList)
        {
            if (table is Table_Item item)
            {
                tableList.Add(item);
                nameTableDic.Add(item.Id, item);
            }
        }
    }
}
