using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Jotunn;

namespace ItemDropLister;

public static class ItemDropPrinter
{
    private static readonly string OutputPath = Path.Combine(Paths.BepInExRootPath, "Debug");
    private const string FileSuffix = ".yaml";

    public static void WriteData(string prefabNamePrefixFilter)
    {
        IEnumerable<KeyValuePair<string, List<ItemDropModel>>> filteredData = prefabNamePrefixFilter == ""
            ? ItemDropRegistry.ItemDrops
            : ItemDropRegistry.ItemDrops
                .Where(pair => pair.Key.StartsWith(prefabNamePrefixFilter));

        foreach (KeyValuePair<string, List<ItemDropModel>> modGroup in filteredData)
        {
            if (modGroup.Value.Count > 0)
            {
                string fileNamePath = Path.Combine(OutputPath, $"{modGroup.Key}{FileSuffix}");
                Dictionary<string, List<string>> fixedOutputs = modGroup.Value
                        .GroupBy(item => item.ItemCategory)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(item => item.ItemPrefabName)
                                .Distinct()
                                .OrderBy(line => line)
                                .ToList()
                        )
                    ;
                WriteFile(fixedOutputs, fileNamePath);
            }
            else
                Logger.LogWarning($"mod '{modGroup.Key}' does not contain any prefabs having ItemDrop component");
        }
    }

    private static void WriteFile(Dictionary<string, List<string>> itemDrops, string filPath)
    {
        if (!Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);
        List<string> output = new List<string>();
        foreach (var itemDrop in itemDrops)
        {
            output.Add($"{itemDrop.Key}:");
            output.AddRange(itemDrop.Value.Select(item => $"  - {item}"));
        }
        File.WriteAllText(filPath, string.Join("\n", output));
        Logger.LogInfo($"wrote item drops to file '{filPath}'");
    }
}