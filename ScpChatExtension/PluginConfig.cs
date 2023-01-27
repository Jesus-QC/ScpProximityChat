using System.Collections.Generic;
using PlayerRoles;

namespace ScpChatExtension
{
    public class PluginConfig
    {
        public bool IsEnabled { get; set; } = true;
        
        public float MaxProximityDistance { get; set; } = 7f;

        public HashSet<RoleTypeId> AllowedRoles { get; set; } = new HashSet<RoleTypeId>()
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp0492,
            RoleTypeId.Scp939,
        };
        
        public string EnableHint { get; set; } = "Proximity Chat has been enabled!";
        public string DisableHint { get; set; } = "Proximity Chat has been disabled!";
    }
}