using System.Collections.Generic;
using Hints;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using PluginAPI.Core;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace ScpChatExtension.Features;

public static class VoiceChatHandler
{
    private static readonly HashSet<ReferenceHub> ToggledPlayers = new();
    
    public static bool OnPlayerTogglingNoClip(ReferenceHub player)
    {
        if (FpcNoclip.IsPermitted(player))
            return true;
        
        if (!EntryPoint.Config.AllowedRoles.Contains(player.roleManager.CurrentRole.RoleTypeId))
            return true;
        
        if (ToggledPlayers.Contains(player))
        {
            ToggledPlayers.Remove(player);
            player.hints.Show(new TextHint(EntryPoint.Config.ProximityChatDisabledMessage, new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4));
            return false;
        }
        
        ToggledPlayers.Add(player);
        player.hints.Show(new TextHint(EntryPoint.Config.ProximityChatEnabledMessage, new HintParameter[] { new StringHintParameter(string.Empty) }, null ,4));
        return false;
    }
    
    public static bool OnPlayerUsingVoiceChat(NetworkConnection connection, VoiceMessage message)
    {
        if (message.Channel != VoiceChatChannel.ScpChat)
            return true;
        
        if (!ReferenceHub.TryGetHubNetID(connection.identity.netId, out ReferenceHub player))
            return true;
        
        if (!EntryPoint.Config.AllowedRoles.Contains(player.roleManager.CurrentRole.RoleTypeId) || (EntryPoint.Config.ToggleChat && !ToggledPlayers.Contains(player)))
            return true;
        
        SendProximityMessage(message);
        return false;
    }
    
    private static void SendProximityMessage(VoiceMessage msg)
    {
        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
        {
            if (referenceHub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(referenceHub))
                continue;
                
            if (referenceHub.roleManager.CurrentRole is not IVoiceRole voiceRole2)
                continue;
            
            if (Vector3.Distance(msg.Speaker.transform.position, referenceHub.transform.position) >= EntryPoint.Config.MaxProximityDistance)
                continue;

            if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) == VoiceChatChannel.None)
                continue;
            
            msg.Channel = VoiceChatChannel.Proximity;
            referenceHub.connectionToClient.Send(msg);
        }
    }
}