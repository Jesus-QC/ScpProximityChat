using System.Collections.Generic;
using Hints;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerRoles.Voice;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace ScpProximityChat.Features;

public static class ScpProximityChatHandler
{
    public static readonly HashSet<ReferenceHub> ToggledPlayers = [];
    
    public static bool OnPlayerTogglingNoClip(ReferenceHub player)
    {
        if (FpcNoclip.IsPermitted(player))
            return true;
        
        if (!ScpProximityChatModule.Config.AllowedRoles.Contains(player.roleManager.CurrentRole.RoleTypeId))
            return true;
        
        if (!ToggledPlayers.Add(player))
        {
            ToggledPlayers.Remove(player);
            player.hints.Show(new TextHint(ScpProximityChatModule.Config.ProximityChatDisabledMessage, [new StringHintParameter(string.Empty)], null ,4));
            return false;
        }

        player.hints.Show(new TextHint(ScpProximityChatModule.Config.ProximityChatEnabledMessage, [new StringHintParameter(string.Empty)], null ,4));
        return false;
    }
    
    public static bool OnPlayerUsingVoiceChat(NetworkConnection connection, VoiceMessage message)
    {
        if (message.Channel != VoiceChatChannel.ScpChat)
            return true;
        
        if (!ReferenceHub.TryGetHubNetID(connection.identity.netId, out ReferenceHub player))
            return true;
        
        if (!ScpProximityChatModule.Config.AllowedRoles.Contains(player.roleManager.CurrentRole.RoleTypeId) || (ScpProximityChatModule.Config.ToggleChat && !ToggledPlayers.Contains(player)))
            return true;
        
        SendProximityMessage(message);
        return !ScpProximityChatModule.Config.ToggleChat;
    }
    
    private static void SendProximityMessage(VoiceMessage msg)
    {
        foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
        {
            if (referenceHub.roleManager.CurrentRole is SpectatorRole && !msg.Speaker.IsSpectatedBy(referenceHub))
                continue;
                
            if (referenceHub.roleManager.CurrentRole is not IVoiceRole voiceRole2)
                continue;
            
            if (Vector3.Distance(msg.Speaker.transform.position, referenceHub.transform.position) >= ScpProximityChatModule.Config.MaxProximityDistance)
                continue;

            if (voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, VoiceChatChannel.Proximity) is VoiceChatChannel.None)
                continue;
            
            msg.Channel = VoiceChatChannel.Proximity;
            referenceHub.connectionToClient.Send(msg);
        }
    }
}