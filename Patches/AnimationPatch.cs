using System.Reflection;
using System.Reflection.Emit;

using HTCCL.Content;

namespace HTCCL.Patches;

[HarmonyPatch]
internal class AnimationPatch
{
    
    /**
     * Patch:
     * - Overrides the animation controller for custom animations if the animation is set to a custom STRIKE animation.
     * - Resets the animation controller if the animation is set to a regular animation.
     * - Runs the custom animation script if the animation is set to a custom STRIKE animation.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.BCHJKLDMDFB))]
    [HarmonyPrefix]
    public static void Player_BCHJKLDMDFB(UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        var anim = p.animator;
        if (anim == null || anim.runtimeAnimatorController == null) return;
        var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
        if (p.anim >= 1000000)
        {
            if (CustomAnimations[p.anim - 1000000].ReceiveAnim != null) return;
            var ovr2 = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            ((AnimatorOverrideController) anim.runtimeAnimatorController).GetOverrides(ovr2);
            if (!ovr2.Exists(x => x.Value != null) || controller["Assist11"].name != CustomAnimations[p.anim - 1000000].Anim.name)
            {
                controller["Assist11"] = CustomAnimations[p.anim - 1000000].Anim;
            }
            Animations.DoCustomAnimation(p, p.anim, CustomAnimations[p.anim - 1000000].ForwardSpeedMultiplier);
        }
        else
        {
            List<KeyValuePair<AnimationClip, AnimationClip>> ovr = new();
            ((AnimatorOverrideController) anim.runtimeAnimatorController).GetOverrides(ovr);
            if (p.grappler == 0 && ovr.Exists(x => x.Value != null))
            {
                controller["Assist11"] = null;
            }
        }
    }
    
    /**
     * Patch:
     * - Overrides the animation controller for custom animations if the animation is set to a custom GRAPPLE animation.
     * - Resets the animation controller if the animation is set to a regular animation.
     * - Runs the custom animation script if the animation is set to a custom GRAPPLE animation.
     * - Runs the post-grapple code if the animation is set to a custom GRAPPLE animation.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.JPNLADBMDNK))]
    [HarmonyPrefix]
    public static bool Player_JPNLADBMDNK(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        var anim = p.animator;
        if (anim == null || anim.runtimeAnimatorController == null) return true;
        var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
        if (p.anim >= 1000000)
        {

            if (CustomAnimations[p.anim - 1000000].ReceiveAnim == null) return true;
            p.fileA = 0;
            p.frameA = 0f;
            p.fileB = 0;
            p.frameB = 0f;
            if (p.sellTim > 0f)
            {
                p.sellTim = 0f;
            }
            var ovr1 = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            controller.GetOverrides(ovr1);
            if (!ovr1.Exists(x => x.Value != null) || controller["Assist11"].name != CustomAnimations[p.anim - 1000000].Anim.name)
            {
                controller["Assist11"] = CustomAnimations[p.anim - 1000000].Anim;
            }
            var opponent = p.pV;
            if (opponent?.animator.runtimeAnimatorController == null) return true;
            var oppController = (AnimatorOverrideController) opponent.animator.runtimeAnimatorController;
            List<KeyValuePair<AnimationClip, AnimationClip>> ovr2 = new();
            oppController.GetOverrides(ovr2);
            if (!ovr2.Exists(x => x.Value != null) || oppController["Assist11"].name != CustomAnimations[p.anim - 1000000].ReceiveAnim.name)
            {
                oppController["Assist11"] = CustomAnimations[p.anim - 1000000].ReceiveAnim;
            }

            Animations.DoCustomAnimation(p, p.anim, CustomAnimations[p.anim - 1000000].ForwardSpeedMultiplier);
            Animations.PerformPostGrappleCode(p);
            return false;
        }
        List<KeyValuePair<AnimationClip, AnimationClip>> ovr = new();
        ((AnimatorOverrideController) p.animator.runtimeAnimatorController).GetOverrides(ovr);
        if (p.grappler == 0 && ovr.Exists(x => x.Value != null))
        {
            controller["Assist11"] = null;
        }
        return true;
    }
    
    /**
     * Patch:
     * - Sets the name of the animation if the animation is set to a custom animation.
     */
    [HarmonyPatch(typeof(UnmappedAnims), nameof(UnmappedAnims.DDIJBPJLEBF))]
    [HarmonyPrefix]
    public static bool Anims_DDIJBPJLEBF(ref string __result, int NOLKIINBMHA)
    {
        if (NOLKIINBMHA >= 1000000)
        {
            __result = CustomAnimations[NOLKIINBMHA - 1000000].Name ?? "Custom Animation" + (NOLKIINBMHA - 1000000).ToString("00");
            return false;
        }
        return true;
    }
    
    /**
     * Patch:
     * - Skips AdaptAttack if the animation is set to a custom animation.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.OFJBOCGFCBC))]
    [HarmonyPrefix]
    public static bool Player_OFJBOCGFCBC(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        return p.anim < 1000000;
    }
    
    /**
     * Patch:
     * - Assigns the override controller to the player when the model is loaded.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.DDKAGOBJGBC))]
    [HarmonyPostfix]
    public static void Player_DDKAGOBJGBC(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        var overrideController = new AnimatorOverrideController(p.animator.runtimeAnimatorController);
        p.animator.runtimeAnimatorController = overrideController;
        p.InstantTransition(p.animFile[0], p.frame[0], 0);
    }
    
    /*
     * Patch:
     * - Changes the animation length and timing for custom animations.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.FEACEIIIAHK))]
    [HarmonyPrefix]
    public static void Player_FEACEIIIAHK(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        var assist11 = ((AnimatorOverrideController)p.animator.runtimeAnimatorController)["Assist11"];
        MappedAnims.length[43] = Mathf.RoundToInt(assist11.length * assist11.frameRate);
        MappedAnims.timing[43] = 1f / MappedAnims.length[43];
    }
    
    /*
     * Patch:
     * - First part lets custom grapple animations play in the editor.
     * - Second part sets all custom moves as executable in the editor.
     */
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Scene_Editor_Update(IEnumerable<CodeInstruction> instructions)
    {
        int flag = 0;
        int flag2 = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldloc_S)
            {
                var operand = instruction.operand;
                PropertyInfo property = operand.GetType().GetProperty("LocalIndex");
                if (property != null && (int)property.GetValue(operand) == 29)
                {
                    if (++flag == 2)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Animations), nameof(Animations.IsGrappleMove), new[] {typeof(int)}));
                    }
                }
            }
            if (instruction.opcode == OpCodes.Ldsfld && (FieldInfo)instruction.operand == AccessTools.Field(typeof(NJBJIIIACEP), nameof(NJBJIIIACEP.OAAMGFLINOB)))
            {
                if (flag2 < 4)
                {
                    flag2++;
                }
            }

            if (flag is 2 or 3)
            {
                if (instruction.opcode == OpCodes.Ldloc_0)
                {
                    flag = 4;
                    yield return instruction;
                }
                else if (instruction.opcode == OpCodes.Bge_S || instruction.opcode == OpCodes.Bge)
                {
                    yield return new CodeInstruction(OpCodes.Brfalse, instruction.operand);
                }
            } else if (flag2 is 4 or 5) {
                if (flag2 == 4)
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_0)
                    {
                        flag2 = 5;
                        yield return new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Animations), nameof(Animations.IsRegularMove), new[] {typeof(int)}));
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
                else
                {
                    if (instruction.opcode == OpCodes.Bge_S || instruction.opcode == OpCodes.Bge)
                    {
                        yield return new CodeInstruction(OpCodes.Brfalse, instruction.operand);
                        flag2 = 6;
                    }
                }
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
