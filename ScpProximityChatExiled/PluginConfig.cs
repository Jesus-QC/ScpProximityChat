using System.Collections.Generic;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace ScpProximityChatExiled;

public class PluginConfig : IConfig
{
    public bool IsEnabled { get; set; } = true;
    
    public bool Debug { get; set; } = false;

    public bool ToggleChat { get; set; } = true;

    public string ProximityChatEnabledMessage { get; set; } = "<i><b>proximity chat <color=#42f57b>enabled</color></b></i>";
    public string ProximityChatDisabledMessage { get; set; } = "<i><b>proximity chat <color=red>disabled</color></b></i>";
    
    public float MaxProximityDistance { get; set; } = 7f;

    public HashSet<RoleTypeId> AllowedRoles { get; set; } =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp0492,
        RoleTypeId.Scp939
    ];
    
    public bool SendBroadcastOnRoleChange { get; set; } = false;
    public ushort BroadcastDuration { get; set; } = 5;
    public string BroadcastMessage { get; set; } = "<b>Proximity Chat can be toggled with the <color=#42f57b>[ALT]</color> key</b>.";
}