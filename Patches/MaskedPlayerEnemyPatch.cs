using HarmonyLib;

namespace MaskedFixes.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    public class MaskedPlayerEnemyPatch
    {
        [HarmonyPatch("SetVisibilityOfMaskedEnemy")]
        [HarmonyPrefix]
        //original implementation of MaskedPlayerEnemy.SetVisibilityOfMaskedEnemy with minor edits to prevent masked going invisible when players die.
        private static bool MaskedInvisibilityFixPrefix(MaskedPlayerEnemy __instance) //aka InvisFix
        {
            if(!Plugin.MaskedInvisibilityFix) //if false, my patch is disabled, currently forced TRUE
            {
                return true; //someone else is fixing this fix so return true
            }
            /*Plugin.Logger.LogError("allowNoPlayer?=" + __instance.allowSpawningWithoutPlayer);
            Plugin.Logger.LogWarning("mimickedPlayer= " + (__instance.mimickingPlayer == null ? "null" : __instance.mimickingPlayer));
            Plugin.Logger.LogWarning("mimickedPlayer.deadBody= " + ((__instance.mimickingPlayer == null || __instance.mimickingPlayer.deadBody == null) ? "null" : __instance.mimickingPlayer.deadBody));
            Plugin.Logger.LogWarning("mimickedPlayer.deadBody.deactivated= " + (__instance.mimickingPlayer == null || __instance.mimickingPlayer.deadBody == null || __instance.mimickingPlayer.deadBody.deactivated == null ? "null" : __instance.mimickingPlayer.deadBody.deactivated));*/
            if (__instance.allowSpawningWithoutPlayer)
            {
                if (__instance.mimickingPlayer != null && __instance.mimickingPlayer.deadBody != null && !__instance.mimickingPlayer.deadBody.deactivated) //seems to be correct for non mimicked masked.
                {
                    if (__instance.enemyEnabled)
                    {
                        __instance.enemyEnabled = false;
                        __instance.EnableEnemyMesh(false, false);
                        Plugin.Logger.LogDebug("(InvisFix)NonMimickedMasked-MaskedMadeInvisible");
                        return false;
                    }
                }
                else if (!__instance.enemyEnabled)
                {
                    __instance.enemyEnabled = true;
                    __instance.EnableEnemyMesh(true, false);
                    Plugin.Logger.LogDebug("(InvisFix)NonMimickedMasked-MaskedMadeVisible");
                }
                return false;
            }
            if (__instance.mimickingPlayer == null /*|| (__instance.mimickingPlayer.deadBody != null && !__instance.mimickingPlayer.deadBody.deactivated)*/) //this is now correct as masked should not go invisible when a player is dead, only should be invisible if there should be a player mimicked and there isnt.
            {
                if (__instance.enemyEnabled)
                {
                    __instance.enemyEnabled = false;
                    __instance.EnableEnemyMesh(false, false);
                    Plugin.Logger.LogDebug("(InvisFix)MimickedMasked-MaskedMadeInvisible");
                }
                return false;
            }
            if (!__instance.enemyEnabled)
            {
                __instance.enemyEnabled = true;
                __instance.EnableEnemyMesh(true, false);
                Plugin.Logger.LogDebug("(InvisFix)MimickedMasked-MaskedMadeVisible");
            }
            return false;
        }
    }
}
