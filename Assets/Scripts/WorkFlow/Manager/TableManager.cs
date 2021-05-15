using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;

public class TableManager : Singleton<TableManager>
{
    private CsvConfiguration config;
    public void LoadAllTables()
    {
        config = new CsvConfiguration(CultureInfo.InvariantCulture) {
            HasHeaderRecord = false,
        };
        
        Type parentType = typeof(TableBase);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
        foreach (var tableClass in subclasses)
        {
            Load(tableClass);
        }
        
        // var level = Table_Level.GetTableByIndex(0);
        // Debug.Log(level.Desc);
        //
        // var item = Table_Item.GetTableByIndex(0);
        // Debug.Log(item.Name);
    }
    
    public void Load(Type t)
    {
        string filePath = $"{Application.dataPath}/Resources/Tables/{t.Name}.csv";
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            var tableRecords = new List<dynamic>();
            csv.Read(); // 第一行字段名
            csv.Read(); // 第二行字段类型
            csv.Read(); // 第三行字段说明
            while (csv.Read())
            {
                tableRecords.Add(csv.GetRecord(t));
            }
            MethodInfo method = t.GetMethod("InitTable");
            MethodInfo generic = method.MakeGenericMethod(t);
            // generic.Invoke(tableRecords[0], new object[]{tableRecords});
        }
    }
}