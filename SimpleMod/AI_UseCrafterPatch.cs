using System;
using System.IO;
using System.Diagnostics;
using BepInEx;
using HarmonyLib;

using UnityEngine;
using BepInEx.Configuration;

using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using s649PBR.Main;

namespace s649PBR
{//>begin NamespaceMain
    namespace AI_UseCrafterPatchMain
    {//>>begin NamespaceSub
        [HarmonyPatch]
        internal class PatchExe
        {//>>>begin class:PatchExe
            [HarmonyPostfix]
            [HarmonyPatch(typeof(AI_UseCrafter), "OnSuccess")]
            private static void PostPatch(AI_UseCrafter __instance)
            {//>>>>begin method:PostPatch
                List<Thing> ings = __instance.ings;
                if(PatchMain.configDebugLogging)
                {
                    string text = "[PBR]";
                    text += "[ingL:" + ings.ToString() +"]";
                    Debug.Log(text);
                } 
            }//<<<<end method:PostPatch
        }//<<<end class:PatchExe
    }//<<end namespaceSub
}//<end namespaceMain

///trashbox//////////////////////////////////////////////////////////////////
/// 
/*
/////////////////////////////////////////////////

////////////////////////////////////////////////
*/