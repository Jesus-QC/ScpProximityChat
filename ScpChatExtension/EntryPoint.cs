using HarmonyLib;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace ScpChatExtension;

public class EntryPoint
{
    public const string Version = "1.0.0.6";

    private static readonly Harmony HarmonyPatcher = new("chatextensions.jesusqc.com");
    
    [PluginAPI.Core.Attributes.PluginConfig] public static PluginConfig Config;

    [PluginEntryPoint("ScpChatExtension", Version, "Makes SCPs able to talk inside the proximity chat.", "Jesus-QC")]
    private void Init()
    {
        if (!Config.IsEnabled)
            return;
        
        Log.Raw($"<color=blue>Loading ScpChatExtension {Version} by Jesus-QC</color>");
        
        HarmonyPatcher.PatchAll();
        
        if (!Config.SendBroadcastOnRoleChange)
            return;
        
        EventManager.RegisterEvents(this);
    }

    [PluginEvent(ServerEventType.PlayerChangeRole)]
    public void OnPlayerChangingRole(Player plr, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
    {
        if (!Config.AllowedRoles.Contains(newRole))
            return;
        
        plr.SendBroadcast(Config.BroadcastMessage, Config.BroadcastDuration);
    }
    
}
