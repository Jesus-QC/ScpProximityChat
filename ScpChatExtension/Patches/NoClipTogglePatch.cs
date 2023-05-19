using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using ScpChatExtension.Features;

namespace ScpChatExtension.Patches;

[HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
public class NoClipTogglePatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        Label ret = generator.DefineLabel();
        
        newInstructions[newInstructions.Count - 1].labels.Add(ret);

        int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + 1;
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
            new (OpCodes.Call, AccessTools.Method(typeof(VoiceChatHandler), nameof(VoiceChatHandler.OnPlayerTogglingNoClip))),
            new (OpCodes.Brfalse, ret),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}