using System.Collections.Generic;
using UnityEngine;

public static class IListExtensions
{
    public static T GetRandomItem<T>(this IList<T> items)
    {
        return items[Random.Range(0, items.Count)];
    }
}