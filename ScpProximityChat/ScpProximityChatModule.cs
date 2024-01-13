using HarmonyLib;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using ScpProximityChat.Features;
using PluginConfig = ScpProximityChat.PluginConfig;

namespace ScpProximityChat;

public class ScpProximityChatModule
{
    public const string Version = "2.0.0.0";
    
    private static readonly Harmony HarmonyPatcher = new("scpproximitymodule.jesusqc.com");
    
    [PluginAPI.Core.Attributes.PluginConfig] public static PluginConfig Config;
    
    [PluginEntryPoint("ScpProximityChat", Version, "Makes SCPs able to talk inside the proximity chat.", "Jesus-QC")]
    private void Init()
    {
        if (!Config.IsEnabled)
            return;
        
        Log.Raw($"<color=blue>Loading ScpChatExtension {Version} by Jesus-QC</color>");
        
        HarmonyPatcher.PatchAll();
        
        EventManager.RegisterEvents(this);
    }

    [PluginEvent(ServerEventType.RoundRestart)]
    public void OnRoundRestarted()
    {
        ScpProximityChatHandler.ToggledPlayers.Clear();
    }
    
    [PluginEvent(ServerEventType.PlayerChangeRole)]
    public void OnPlayerChangingRole(Player plr, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
    {
        if (!Config.SendBroadcastOnRoleChange)
            return;
        
        if (!Config.AllowedRoles.Contains(newRole))
            return;
        
        plr.SendBroadcast(Config.BroadcastMessage, Config.BroadcastDuration);
    }
}