using System;
using Exiled.API.Features;
using ScpProximityChatExiled.Features;

namespace ScpProximityChatExiled;

public class ScpProximityChatModule : Plugin<PluginConfig>
{
    public const string PluginVersion = "2.0.0.0";

    public override string Author { get; } = "Jesus-QC";
    public override string Name { get; } = "ScpProximityChat";
    public override string Prefix { get; } = "ScpProximityChat";
    public override Version Version { get; } = new (PluginVersion);
    public override Version RequiredExiledVersion { get; } = new (8, 7, 0);
    public override bool IgnoreRequiredVersionCheck { get; } = true;

    public static PluginConfig PluginConfig;
    
    public override void OnEnabled()
    {
        PluginConfig = Config;
        ScpProximityChatHandler.RegisterEvents();
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        ScpProximityChatHandler.UnregisterEvents();
        base.OnDisabled();
    }
}