using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;

namespace ScpChatExtension;

public class EntryPoint
{
    public const string Version = "1.0.0.4";

    private static readonly Harmony HarmonyPatcher = new("chatextensions.jesusqc.com");
    
    [PluginAPI.Core.Attributes.PluginConfig] public static PluginConfig Config;

    [PluginEntryPoint("ScpChatExtension", Version, "Makes SCPs able to talk inside the proximity chat.", "Jesus-QC")]
    private void Init()
    {
        if(!Config.IsEnabled)
            return;
        
        Log.Raw($"<color=blue>Loading ScpChatExtension {Version} by Jesus-QC</color>");
        HarmonyPatcher.PatchAll();
    }
    
}
