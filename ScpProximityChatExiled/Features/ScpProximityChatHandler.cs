using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace ScpProximityChatExiled.Features;

public static class ScpProximityChatHandler
{
    public static void RegisterEvents()
    {
        Exiled.Events.Handlers.Player.VoiceChatting += OnPlayerUsingVoiceChat;
        Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestarted;
        
        if (ScpProximityChatModule.PluginConfig.ToggleChat)
            Exiled.Events.Handlers.Player.TogglingNoClip += OnPlayerTogglingNoClip;
        
        if (!ScpProximityChatModule.PluginConfig.SendBroadcastOnRoleChange)
            return;
        
        Exiled.Events.Handlers.Player.ChangingRole += OnPlayerChangingRole;
    }
    
    public static void UnregisterEvents()
    {
        Exiled.Events.Handlers.Player.VoiceChatting -= OnPlayerUsingVoiceChat;
        Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestarted;
        
        if (ScpProximityChatModule.PluginConfig.ToggleChat)
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnPlayerTogglingNoClip;
        
        if (!ScpProximityChatModule.PluginConfig.SendBroadcastOnRoleChange)
            return;

        Exiled.Events.Handlers.Player.ChangingRole -= OnPlayerChangingRole;
    }
    
    
    private static readonly HashSet<Player> ToggledPlayers = [];

    private static void OnRoundRestarted()
    {
        ToggledPlayers.Clear();
    }
    
    private static void OnPlayerChangingRole(ChangingRoleEventArgs args)
    {
        if (!ScpProximityChatModule.PluginConfig.AllowedRoles.Contains(args.NewRole))
            return;
        
        args.Player.Broadcast(ScpProximityChatModule.PluginConfig.BroadcastDuration, ScpProximityChatModule.PluginConfig.BroadcastMessage);
    }
    
    public static void OnPlayerTogglingNoClip(TogglingNoClipEventArgs args)
    {
        if (FpcNoclip.IsPermitted(args.Player.ReferenceHub))
            return;
        
        if (!ScpProximityChatModule.PluginConfig.AllowedRoles.Contains(args.Player.Role.Type))
            return;
        
        if (!ToggledPlayers.Add(args.Player))
        {
            ToggledPlayers.Remove(args.Player);
            args.Player.ShowHint(ScpProximityChatModule.PluginConfig.ProximityChatDisabledMessage, 4f);
            args.IsAllowed = false;
            return;
        }

        args.Player.ShowHint(ScpProximityChatModule.PluginConfig.ProximityChatEnabledMessage, 4f);
        args.IsAllowed = false;
    }
    
    public static void OnPlayerUsingVoiceChat(VoiceChattingEventArgs args)
    {
        if (args.VoiceMessage.Channel != VoiceChatChannel.ScpChat)
            return;
        
        if (!ScpProximityChatModule.PluginConfig.AllowedRoles.Contains(args.Player.Role.Type) || (ScpProximityChatModule.PluginConfig.ToggleChat && !ToggledPlayers.Contains(args.Player)))
            return;
            
        SendProximityMessage(args.VoiceMessage);
        
        if (!ScpProximityChatModule.PluginConfig.ToggleChat)
            return;
        
        args.IsAllowed = false;
    }
    
    private static void SendProximityMessage(VoiceMessage msg)
    {
        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
        {
            if (referenceHub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(referenceHub))
                continue;
                
            if (referenceHub.roleManager.CurrentRole is not IVoiceRole voiceRole2)
                continue;
            
            if (Vector3.Distance(msg.Speaker.transform.position, referenceHub.transform.position) >= ScpProximityChatModule.PluginConfig.MaxProximityDistance)
                continue;

            if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) is VoiceChatChannel.None)
                continue;
            
            msg.Channel = VoiceChatChannel.Proximity;
            referenceHub.connectionToClient.Send(msg);
        }
    }
}