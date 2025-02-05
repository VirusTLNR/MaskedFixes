using HarmonyLib;
using UnityEngine;

namespace MaskedInvisFix.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    public class MaskedPlayerEnemyPatch
    {
        //static string oldDebugLine = "";
        static GameObject tragedyRagdoll = StartOfRound.Instance.playerRagdolls[4]; //ragdoll when killed by Tragedy Mask/Masked.
        static GameObject comedyRagdoll = StartOfRound.Instance.playerRagdolls[5]; //ragdoll when killed by Comedy Mask/Masked.

        [HarmonyPatch("SetVisibilityOfMaskedEnemy")]
        [HarmonyPrefix]
        //original implementation of MaskedPlayerEnemy.SetVisibilityOfMaskedEnemy with minor edits to prevent masked going invisible when players die.
        private static bool MaskedInvisibilityFixPrefix(MaskedPlayerEnemy __instance) //aka InvisFix
        {
            if(!Plugin.MaskedInvisibilityFix) //if false, my patch is disabled, currently forced TRUE
            {
                return true; //someone else is fixing this fix so return true
            }
            //Plugin.Logger.LogError("tragedy = " + tragedyRagdoll.name); //just for checking i have the right ragdolls.
            //Plugin.Logger.LogError("comedy = " + comedyRagdoll.name); //just for checking i have the right ragdolls.
            string debugLine = 
                "allowNoPlayer ? = " + __instance.allowSpawningWithoutPlayer +
                " | enemyIsEnabled? = " + __instance.enemyEnabled +
                " | mimickedPlayer = " + (__instance.mimickingPlayer == null ? "null" : __instance.mimickingPlayer) +
                " | mimickedPlayer.IsPlayerDead = " + (__instance.mimickingPlayer == null ? "null" : __instance.mimickingPlayer.isPlayerDead) +
                " | mimickedPlayer.deadBody= " + ((__instance.mimickingPlayer == null || __instance.mimickingPlayer.deadBody == null) ? "null" : __instance.mimickingPlayer.deadBody.name) + 
                " | mimickedPlayer.deadBody.deactivated= " + (__instance.mimickingPlayer == null || __instance.mimickingPlayer.deadBody == null || __instance.mimickingPlayer.deadBody.deactivated == null ? "null" : __instance.mimickingPlayer.deadBody.deactivated);
            /*if (oldDebugLine != debugLine)
            {
                oldDebugLine = debugLine; //to prevent log spam
                Plugin.Logger.LogError(debugLine);
            }*/
            if (__instance.allowSpawningWithoutPlayer)
            {
                if (__instance.mimickingPlayer != null && __instance.mimickingPlayer.deadBody != null && !__instance.mimickingPlayer.deadBody.deactivated) //seems to be correct for non mimicked masked.
                {
                    if (__instance.enemyEnabled)
                    {
                        __instance.enemyEnabled = false;
                        __instance.EnableEnemyMesh(false, false);
                        Plugin.Logger.LogDebug(debugLine);
                        Plugin.Logger.LogDebug("NonMimickedMasked-MaskedMadeInvisible");
                        return false;
                    }
                }
                else if (!__instance.enemyEnabled)
                {
                    __instance.enemyEnabled = true;
                    __instance.EnableEnemyMesh(true, false);
                    Plugin.Logger.LogDebug(debugLine);
                    Plugin.Logger.LogDebug("NonMimickedMasked-MaskedMadeVisible");
                }
                return false;
            }
            if (__instance.mimickingPlayer == null || ((__instance.mimickingPlayer.deadBody == tragedyRagdoll || __instance.mimickingPlayer.deadBody == comedyRagdoll) && !__instance.mimickingPlayer.deadBody.deactivated)) //changed to prevent a graphical glitch for a few frames when a mask/masked kill a player.
            {
                if (__instance.enemyEnabled)
                {
                    __instance.enemyEnabled = false;
                    __instance.EnableEnemyMesh(false, false);
                    Plugin.Logger.LogDebug(debugLine);
                    Plugin.Logger.LogDebug("MimickedMasked-MaskedMadeInvisible");
                }
                return false;
            }
            if (!__instance.enemyEnabled)
            {
                __instance.enemyEnabled = true;
                __instance.EnableEnemyMesh(true, false);
                Plugin.Logger.LogDebug(debugLine);
                Plugin.Logger.LogDebug("MimickedMasked-MaskedMadeVisible");
            }
            return false;
        }
    }
}
