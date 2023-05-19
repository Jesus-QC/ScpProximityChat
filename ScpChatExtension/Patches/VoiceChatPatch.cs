using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using ScpChatExtension.Features;
using VoiceChat.Networking;

namespace ScpChatExtension.Patches;

[HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
public class VoiceChatPatch
{
    private static MethodInfo GetSendMethod()
    {
        foreach (MethodInfo method in typeof(NetworkConnection).GetMethods())
        {
            if (method.Name is nameof(NetworkConnection.Send) && method.GetGenericArguments().Length != 0)
                return method.MakeGenericMethod(typeof(VoiceMessage));
        }

        return null;
    }
    
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        Label ret = generator.DefineLabel();

        newInstructions[newInstructions.Count - 1].labels.Add(ret);
        
        int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_0);
        
        newInstructions.InsertRange(index, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Call, AccessTools.Method(typeof(VoiceChatHandler), nameof(VoiceChatHandler.OnPlayerUsingVoiceChat))),
            new (OpCodes.Brfalse_S, ret),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}