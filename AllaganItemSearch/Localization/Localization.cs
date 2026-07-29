using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

using AllaganItemSearch.ItemRenderers;

namespace AllaganItemSearch.Localization;

public static class Localization
{
    private static readonly IReadOnlyDictionary<string, string> Strings = LoadStrings();

    public static string T(string key)
    {
        return Strings.TryGetValue(key, out var value) ? value : key;
    }

    public static string RendererName(IItemInfoRenderer renderer)
    {
        return T(renderer.SingularName);
    }

    public static string RendererHelp(IItemInfoRenderer renderer)
    {
        var name = RendererName(renderer);
        return renderer.RendererType == RendererType.Source
                   ? $"此物品是否可透過「{name}」取得？"
                   : $"此物品是否可用於「{name}」？";
    }

    private static IReadOnlyDictionary<string, string> LoadStrings()
    {
        using var stream = Assembly.GetExecutingAssembly()
                                   .GetManifestResourceStream("AllaganItemSearch.Localization.zh-TW.json");
        if (stream == null)
        {
            return new Dictionary<string, string>();
        }

        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? new Dictionary<string, string>();
    }
}
