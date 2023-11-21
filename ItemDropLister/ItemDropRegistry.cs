using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Jotunn.Utils;

namespace ItemDropLister;

public class ItemDropModel
{
    [UsedImplicitly]
    public string ItemPrefabName;
    [UsedImplicitly]
    public string ItemCategory;

    public static ItemDropModel FromItemDrop(ItemDrop original)
    {
        return new ItemDropModel
        {
            ItemPrefabName = original.name,
            ItemCategory = original.m_itemData.m_shared.m_itemType.ToString()
        };
    }
}

public static class ItemDropRegistry
{
    public static readonly Dictionary<string, List<ItemDropModel>> ItemDrops = new();

    public static void Initialize()
    {
        Dictionary<string, List<ItemDropModel>> result = ModQuery.GetPrefabs()
            .GroupBy(pair => pair.SourceMod.Name)
            .ToDictionary(
                group => group.Key,
                group => group.Select(prefab =>
                    {
                        if (prefab.Prefab.TryGetComponent(out ItemDrop itemDrop))
                            return ItemDropModel.FromItemDrop(itemDrop);
                        return null;
                    }
                ).Where(prefab => prefab != null).ToList()
            );
        foreach (KeyValuePair<string, List<ItemDropModel>> pair in result)
        {
            ItemDrops.Add(pair.Key, pair.Value);
        }
    }
}