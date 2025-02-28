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
{//>begin namespaceMain
    namespace Main
    {//>>begin namespaceSub
        [BepInPlugin("s649_PotionBottleRecycle", "s649 Potion Bottle Recycle", "0.0.0.0")]
        public class PatchMain : BaseUnityPlugin
        {//>>>begin class:PatchExe
        //////-----Config Entry---------------------------------------------------------------------------------- 
            //CE 00 general
            private static ConfigEntry<bool> CE_AllowFunction01Use;
            private static ConfigEntry<bool> CE_AllowFunction02Blend;
            private static ConfigEntry<bool> CE_AllowFunction03Craft;
            private static ConfigEntry<bool> CE_AllowFunction04Throw;
            //CE 01 use
            private static ConfigEntry<bool> CE_PcCreatesBottlesWhenDrinking;
            private static ConfigEntry<bool> CE_AllowPcCreatesJunkBottles;
            private static ConfigEntry<bool> CE_NpcCreatesBottlesWhenDrinking;
            private static ConfigEntry<bool> CE_AllowNpcCreatesJunkBottles;
            //CE 02 Blend
            //private static ConfigEntry<bool> CE_RecycleBottlesWhenBlending;
            //CE 03 Craft
            //CE 04 Throw
            //CE GENERAL
            private static ConfigEntry<bool> CE_DebugLogging;
            private static ConfigEntry<bool> CE_Dynamic_Configuration_Loading;
        //config--------------------------------------------------------------------------------------------------------------
            // 00 general
            public static bool cf_Allow_F01_Use =>  CE_AllowFunction01Use.Value;
            public static bool cf_Allow_F02_Blend =>  CE_AllowFunction02Blend.Value;
            public static bool cf_Allow_F03_Craft =>  CE_AllowFunction03Craft.Value;
            public static bool cf_Allow_F04_Throw =>  CE_AllowFunction04Throw.Value;
            // 01 use
            public static bool cf_F01_PC_CBWD => CE_PcCreatesBottlesWhenDrinking.Value;
            public static bool cf_F02_NPC_CBWD => CE_NpcCreatesBottlesWhenDrinking.Value;
            public static bool cf_F01_a_PC_ACJB => CE_AllowPcCreatesJunkBottles.Value;
            public static bool cf_F02_a_NPC_ACJB => CE_AllowNpcCreatesJunkBottles.Value;
            //zz debug

            public static bool configDebugLogging => CE_DebugLogging.Value;
            public static bool cf_DynamicConfigLoad => CE_Dynamic_Configuration_Loading.Value;
            //method------------------------------------------------------------------------------------------------------------------------------------------------------------
            internal void LoadConfig()
            {
                ////// 00 GENERAL
                CE_AllowFunction01Use = Config.Bind("#F00-General", "ALLOW_FUNCTION_01_USE", true, "Allow control of function 01-use");
                CE_AllowFunction02Blend = Config.Bind("#F00-General", "ALLOW_FUNCTION_02_BLEND", true, "Allow control of function 02-blend");
                CE_AllowFunction03Craft = Config.Bind("#F00-General", "ALLOW_FUNCTION_03_CRAFT", true, "Allow control of function 03-craft");
                CE_AllowFunction04Throw = Config.Bind("#F00-General", "ALLOW_FUNCTION_04_THROW", true, "Allow control of function 04-throw");
                // 01 use
                CE_PcCreatesBottlesWhenDrinking = Config.Bind("#F01-Use_01", "PC_CREATES_BOTTLE_WHEN_DRINKING", true, "PC creates empty bottle when drinking");
                CE_AllowPcCreatesJunkBottles = Config.Bind("#F01-Use_01_a", "PC_ALLOW_CREATES_JUNK_BOTTLES", true, "Allow PC to generate junk bottles");
                CE_NpcCreatesBottlesWhenDrinking = Config.Bind("#F01-Use_02", "NPC_CREATES_BOTTLE_WHEN_DRINKING", true, "NonPC creates empty bottle when drinking");
                CE_AllowNpcCreatesJunkBottles = Config.Bind("#F01-Use_02_a", "NPC_ALLOW_CREATES_JUNK_BOTTLES", true, "Allow NonPC to generate junk bottles");
                // 02 Blend PC only?
                //CE_RecycleBottlesWhenBlending = Config.Bind("#F02-Blend_01", "RECYCLE_BOTTLES_WHEN_BLENDING", true, "");
                // 03 Craft PC only!
                //CE_RecycleBottlesWhenCrafting = Config.Bind("#FUNC_03-Craft_00", "RECYCLE_BOTTLES_WHEN_CRAFTING", true, "");
                // 04 Throw
                //CE_RecycleBottlesWhenThrowing = Config.Bind("#FUNC_04-Throw_00", "RECYCLE_BOTTLES_WHEN_THROWING", true, "");
                // zz debug
                CE_DebugLogging = Config.Bind("#FZZ", "DEBUG_LOGGING", false, "If true, Outputs debug info.");
                CE_Dynamic_Configuration_Loading = Config.Bind("#FZZ", "DYNAMIC_CONFIGURATION_LOADING", false, "Enable dynamic configuration loading.(Experimental)");
                if(configDebugLogging)
                {
                    string text = "[PBR]CF";
                    //text += ("[F01/" + TorF(cf_F01_NCBWD) + "]");
                    Debug.Log(text);
                }
            }
            private void Start()
            {//>>>>begin method:Start
                LoadConfig();
                var harmony = new Harmony("PatchMain");
                new Harmony("PatchMain").PatchAll();
            }//<<<<end method:Start
            private string TorF(bool b){return (b)? "T": "F";}

            internal static int TypeContainsPotionBottle(Thing t)
            {//>>>>begin method:TypeContainsPotionBottle
                //description
                    //return 1 : potion bottle
                    //return 2 : empty bucket
                    //return 0 : none
                    //return -1 : bottle junk
                    //return -2 : can junk
                    //return -3  : can not junk
                    //return -999 : fumei
                //description end
                Trait trait = t.trait; 
                string category = t.sourceCard.category;
                string unit = t.source.unit;
                    
                Debug.Log("[TCPB]t is " + trait.ToString() + "/cate:" + category + "/u:" + unit);
                if(trait is TraitSnow){return 0;}
                if(trait is TraitPotion || trait is TraitPotionRandom){return 1;}
                if(trait is TraitPerfume){return 0;}
                //if(trait is TraitDrinkMilk || trait is TraitDrinkMilkMother){return 0;}
                if(trait is TraitDrink)
                {
                    if(category == "Booze"){
                        return -1;
                    }else if(category == "_drink"){
                        if(unit == "bucket"){return 2;}
                        if(unit == "pot"){return 1;}
                        return 0;
                    }
                    return 0;
                }
                return -999;
                    /*
                    
                    if(t.trait == "Drug"){return 0;}
                    if(t.trait == "Perfume"){return 1;}
                    if(t.trait == "DrinkWater" || t.trait == "DrinkWaterDirty")
                    {
                        return 1;
                        //if(t.unit == "bucket"){return 2;} else {return 1;}
                    }
                    if(t is Potion || t is PotionLove || t is PotionRandom){return 1;}
                    if(t is DrinkMilk || t is DrinkMilkMother){return 0;}
                    if(t is Drink)
                    {
                        if(t is Booze){return -1;}
                        //if(t.name == "soft drink"){return -2;} else {return 0;}
                    }
                    return -999;
                    */
                    
                    //return 0;
            }//<<<<end method:TypeContainsPotionBottle

            internal static Thing DoRecycleBottle(Thing t)
            {
                int prodN = TypeContainsPotionBottle(t);
                    //Thing prodT = null;
                    string prod = "";
                    switch(prodN){
                        case 1 : prod = "potion_empty";
                        break;
                        case 2 : prod = "bucket_empty";
                        break;
                        case 0 : prod = "";
                        break;
                        case -1 : prod = "";//bottle
                        break;
                        case -2 : prod = "";//can
                        break;
                        case -3 : prod = "";//can not junk
                        break;
                        default : prod = "qqq";
                        break;
                    }
                    if(prod == ""|| prod == "qqq"){return null;} else {return ThingGen.Create(prod);} 
                    //return null;
            }
        }//<<<end class:Main
    }//<<end namespaceSub
}//<end namespaceMain


 /* potionList [id/Trait/CanDrink/CanBlend/ContainBottle]
                    snow	雪	TraitSnow
                    water   水  TraitDrink																									
                    water_dirty	汚水    TraitDrink
                    bucket	水の入ったバケツ    TraitDrink
                    potion	ポーション  TraitPotion/TraitPotionRandom
                    drug	薬      TraitPotion
                    perfume	香水    TraitPerfume
                    milk	乳
                    drink	飲料
                    milkcan	ミルク缶
                    +++++ thingV list ++++++++++++++++++++++++++
                    crimAle	drink	クリムエール    TraitDrink
                    48	drink	ワイン
                    49	drink	液体
                    50	drink	お酒
                    51	drink	お酒
                    52	drink	お酒
                    53	drink	クリムエール
                    54	drink	ビア
                    55	drink	お酒
                    56	drink	お酒
                    57	drink	お酒
                    58	drink	ビア
                    59	drink	ビン
                    501	drink	お酒
                    502	drink	お酒
                    503	drink	お茶
                    504	drink	缶ジュース
                    505	drink	缶ジュース
                    506	drink	お酒
                    507	drink	お酒
                    508	drink	牛乳
                    718	drink	コーヒー
                    732	drink	ワイン
                    733	drink	カクテル
                    776	drink	お酒
                    777	drink	お酒
                    778	drink	牛乳
                    789	drink	ココナッツジュース
                    840	drink	お酒
                    1081	drink	ラムネ
                    1134	drink	酒  

                    1165	potion	エーテル抗体のポーション
                    1163	potion	魔法のポーション
                    330	potion	盲目のポーション
                    331	potion	混乱のポーション
                    334	drug	睡眠剤
                    335	potion	麻痺のポーション
                    lovepotion	potion	媚薬
                    336	potion	毒薬
                    338	potion	耐酸性コーティング液
                    mercury	potion	水銀
                    blood_angel	potion	堕天使の血
                    928	potion	ホルモン薬
                    1142	potion	塩水                 
                    */
                    /* akibin List
                    potion_empty	空き瓶
                    bucket_empty	空のバケツ

                    529	junkFlat	空き缶
                    231	junkFlat	薬のビン
                    1170	junkFlat	空き缶
                    236	junkFlat	空き缶
                    726	junkFlat	空き瓶
                    727	junkFlat	空き瓶
                    728	junkFlat	空き瓶
                    */




//------------template--------------------------------------------------------------------------------------------
/*
//------------namespace class--------------------------------------------------------------------------------------------
namespace NAMAE-MAIN
{//>begin namespaceMain
    namespace NAMAE-SUB
    {//>>begin namespaceSub
        //--nakami----------------------
    }//<<end namespaceSub
}//>end namespaceMain

[HarmonyPatch]
internal class PatchExe
{//>begin class:PatchExe
    //----nakami-------------------
}//<end class:PatchExe

//----method--------------------------------------------------------------

[HarmonyPrefix]
[HarmonyPatch(typeof(ClassName), "MethodName")]
internal static bool WakariyasuiName()
{   //begin:method-@@@@@@@@@@@@
   
}   //end:method-@@@@@@@@@@@@

[HarmonyPostfix]
[HarmonyPatch(typeof(ClassName), "MethodName")]
internal static void WakariyasuiName()
{
    //nakami
}

//Harmony Patch Argument list
__result
__instance


//---debug logging-------------------------------------------------------
if(PatchMain.configDebugLogging)
{
    string text = "";
}
//text += "[aaa:" + xxx.ToString() +"]"; 
//Debug.Log(text);


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
*/

//////trash box//////////////////////////////////////////////////////////////////////////////////////////////////
///
/*
    [HarmonyPatch(typeof(Zone))]
    [HarmonyPatch(nameof(Zone.Activate))]
    public class HarmonyAct
    {
        //[HarmonyPostfix]
        
        static void Postfix(Zone __instance)
        {
            Zone z = __instance;
            Zone topZo = z.GetTopZone();
            FactionBranch br = __instance.branch;
            //Lg("[LS]Fooked!");
            if (Main.propFlagEnablelLogging)
            {
                Lg("[LS]CALLED : Zone.Activate ");
                string text;

                text = ("[LS]Ref : [Z:" + z.id.ToString() + "]");
                //text += (" [id:" + z.id.ToString() + "]");
                text += (" [Dlv:" + z.DangerLv.ToString() + "]");
                text += (" [blv:" + Mathf.Abs(z.lv).ToString() + "]");
                text += (" [bDLV:" + z._dangerLv.ToString() + "]");
                text += (" [Dlfi:" + z.DangerLvFix.ToString() + "]");
                if(topZo != null && z != topZo){text += (" [tpZ:" + topZo.NameWithLevel + "]");}
                if(br != null){text += (" [br:" + br.ToString() + "]");}
                if(z.ParentZone != null && z != z.ParentZone)text += (" [PaZ: " + z.ParentZone.id.ToString() + "]") ;
                 text += (" [Pce:" + z.isPeace.ToString() + "]");
                 text += (" [Twn:" + z.IsTown.ToString() + "]");
                Lg(text);
                //text = ("[LS]Charas : " + EClass._map.charas.Count);
                //text += (" [Stsn:" + z.isPeace.ToString() + "]");
            }
            
        }
        public static void Lg(string t)
        {
            UnityEngine.Debug.Log(t);
        }
        
    }

    [HarmonyPatch(typeof(HotItem))]
    public class HotPatch {
        //[HarmonyPrefix]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(HotItem.TrySetAct))]
        static void FookPostExe(HotItem __instance){
            //Debug.Log("[LS]Fooking->" + __instance.ToString());
        }
    }  

    [HarmonyPatch]
    public class TickPatch{
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Chara),"TickConditions")]
        public static void TickExe(Chara __instance){
            Chara c = __instance;
            if(c.IsPC){
                //Debug.Log("[LS]QuestMain : " + QuestMain.Phase.ToString());
            }
        }
    }
    */
/*
    [HarmonyPatch(typeof(TraitDoor))]
    public class PatchAct2 {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TraitDoor.TryOpen))]
        static void FookPreExe(TraitDoor __instance, Chara c, ref bool __state){
            __state = __instance.IsOpen() ? true : false;
            //if(c.IsPC){ Lg("[LS]TraitDoor.TryOpen Called! by->" + c.ToString());}
            
            
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TraitDoor.TryOpen))]
        static void FookPostExe(TraitDoor __instance, Chara c, bool __state){
            if(!__state && __instance.IsOpen()){
                if(c.IsPC){ 
                    //Lg("[LS]TraitDoor.Close->Open!" + c.ToString());
                    }
            }
           
        }
        
        
        public static void Lg(string t)
        {
            UnityEngine.Debug.Log(t);
        }
    }
   
public class Main : BaseUnityPlugin {
    private void Start() {
        var harmony = new Harmony("NerunTest");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Zone))]
[HarmonyPatch(nameof(Zone.Activate))]
class ZonePatch {
    static void Prefix() {
        Debug.Log("Harmoney Prefix");
    }
    static void Postfix(Zone __instance) {
        Debug.Log("Harmoney Postfix");
        }
}
*/

/*
        
        */
        /*
        [HarmonyPatch(typeof(Zone))]
        [HarmonyPatch(nameof(Zone.Activate))]
        class ZonePatch {
        static void Postfix(Zone __instance) {
            Lg("[LS]CALLED : Zone.Activate " + __instance.ToString());
            Lg("[LS]Zone : [DLV : " + __instance.DangerLv.ToString() + "]");
            Lg("[LS]Player : [dst : " + EClass.player.stats.deepest.ToString() + "]");
            }
        }
        */

        /*
        static void PostMoveZone(Player __instance)
        {
            Lg("[LS]Fooked!MoveZone!");
            if (Main.propFlagEnablelLogging.Value)
            {
                int dst = EClass.player.stats.deepest;
                Lg("[LS]CALLED : Player.MoveZone ");
                Lg("[LS]Player : [dst : " + dst.ToString() + "]");
            }
        }

        */


        /*
        [HarmonyPatch(typeof(Card), "AddExp")]
        
        class CardPatch
        {
            [HarmonyPrefix]
            static bool AddExpHook(Card __instance)
            {
                Lg("[LS]Fooked:AddExp!");
                
                if (Main.propFlagEnablelLogging.Value)
                {
                    if(__instance.IsPC){
                        Lg("[LS]Card : [name : " + __instance.ToString() + "]");
                    }
                    //Lg("[LS]Card : [name : " + dst.ToString() + "]");
                    //Lg("[LS]Player : [dst : " + dst.ToString() + "]");
                }
                return true;
            }
        }
        */

        /*
        [HarmonyPatch(typeof(Zone), "DangerLv", MethodType.Getter)]
        class ZonePatch {
            [HarmonyPrefix]
            static bool Prefix(Zone __instance) {
                Lg("[LS]CALLED : Zone.DangerLV ");
                //Lg("[LS]Zone : [Z.toSt : " + __instance.ToString() + "]");
                //Lg("[LS]Zone : [DLV : " + __instance.DangerLv.ToString() + "]");
                //Lg("[LS]Player : [dst : " + EClass.player.stats.deepest.ToString() + "]");
                return true;
            }
        }


        //public static void Lg(string t)
        //{
        //    UnityEngine.Debug.Log(t);
        //}
        //public static bool IsOnGlobalMap(){
        //    return (EClass.pc.currentZone.id == "ntyris") ? true : false;
        //}

        */

        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Map), "DropBlockComponent")]
        public static void Postfix(Point point,TileRow r,SourceMaterial.Row mat, bool recoverBlock, bool isPlatform, Chara c){
            string text = "[LS]DBC [";
            //text += "Map:" + __instance.ToString() + "][";
            text += "P:" + point.ToString() + "][";
            text += "r:" + r.ToString() + "][";
            text += "rid:" + r.id.ToString() + "][";
            text += "mat:" + mat.ToString() + "][";
            text += "rB:" + recoverBlock.ToString() + "][";
            text += "iP:" + isPlatform.ToString() + "][";
            //text += "c:" + c.ToString() + "][";
            text += "]";
            Debug.Log(text);
        }
        */
        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ThingGen), "CreateRawMaterial")]
        public static void Postfix(SourceMaterial.Row row){
            Debug.Log("[LS]TG->CRM : " + row.ToString());
        }*/


        /*
        [HarmonyPatch]
    public class MapExe{
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Map), "MineFloor")]
        public static bool Prefix(Map __instance, Point point, Chara c, bool recoverBlock, bool removePlatform){
            if(!Main.configFlagModInfiniteDigOnField){return true;} //#FUNC_01a Flag:falseなら何もしない
            if(Main.configFlagModDiggingChunk){return true;} //#FUNC_01Another Flag:trueなら何もしない
            
            //----debug------------------------------------------------------------------------
            string text = "[LS]MF [";
            text += "Map:" + __instance.ToString() + "][";
            text += "P:" + point.ToString() + "][";
            text += "C:" + c.ToString() + "][";
            text += "rB:" + recoverBlock.ToString() + "][";
            text += "rP:" + removePlatform.ToString() + "][";
            text += "]";
            //---debug kokomade--------------------------------------------------------------------
            if(Main.configFlagModInfiniteDigOnFieldToNothing){return false;} //#FUNC_01b　Flag:trueなら掘りつつアイテム入手をスキップ
            if(point.sourceFloor.id == 4 ){
                //Debug.Log("Floor is hatake");
                int num = UnityEngine.Random.Range(0, 99);
                Thing t = null;
                switch(num){
                    case < 25 and >= 10 : t = ThingGen.Create("stone");
                        break;
                    case < 10 and >= 2 : t = ThingGen.Create("pebble");
                        break;
                    case < 2 : t = ThingGen.Create("rock");
                        break;
                }
                if(t != null){
                    //c.Pick(t);
                    __instance.TrySmoothPick(point, t, c);
                }
                return false;
            }
            return true;
            //Debug.Log(text);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Map), "MineFloor")]
        public static void Postfix(Map __instance, Point point, Chara c){
            if(Main.configFlagModDiggingChunk && ContainsChunk(point)){
                point.SetFloor(45, 40);
            }   
        }
        private static bool ContainsChunk(Point point){
            if(point.sourceFloor.components[0].Contains("chunk@soil") || point.sourceFloor.components[0].Contains("chunk@snow") || point.sourceFloor.components[0].Contains("chunk@ice")){
                return true;
            } else {
                return false;
            }

        }

    }
[HarmonyPatch]
    public class PreExe{
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TaskChopWood), "OnCreateProgress")]
        public static bool Prefix(Progress_Custom p, TaskChopWood __instance){
            //yomikomi
            string Name = __instance.Name;
            Point pos = __instance.pos;
            Chara owner = __instance.owner;

            //vanillaの処理を拝借
            p.textHint = Name;
            p.maxProgress = Mathf.Max((15 + EClass.rnd(20)) * 100 / (100 + owner.Tool.material.hardness * 3), 2);
            p.onProgressBegin = delegate
            {
                if (owner.Tool != null)
                {
                    owner.Say("chopwood_start", owner, GetLog(pos).GetName(NameStyle.Full, 1));
                }
            };
            p.onProgress = delegate
            {
                Thing log2 = GetLog(pos);
                SourceMaterial.Row material2 = log2.material;
                log2.PlaySoundImpact();
                material2.AddBlood(pos);
                log2.PlayAnime(AnimeID.HitObj);
                material2.PlayHitEffect(pos);
                owner.renderer.NextFrame();
            };
            p.onProgressComplete = delegate
            {
                Thing log = GetLog(pos);
                SourceMaterial.Row material = log.material;
                log.PlaySoundDead();
                material.AddBlood(pos, 3 + EClass.rnd(2));
                log.material.PlayHitEffect(pos, 10);
                Thing thing = ThingGen.Create("rock", material.id).SetNum(1);//edit
                CraftUtil.MixIngredients(thing, new List<Thing> { log }, CraftUtil.MixType.General, 999);
                log.ModNum(-1);
                owner.elements.ModExp(225, 30);
                owner.stamina.Mod(-1);
                EClass._map.TrySmoothPick(pos, thing, EClass.pc);
            };
            return false;
        }
        //拝借
        public static Thing GetLog(Point pos)
        {
            return pos.FindThing((Thing t) => t.id == "log");
         }
    }



    */

    /*
    [HarmonyPatch(typeof(TaskDig))]
    public class PreExe{
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TaskDig), "GetHitResult")]
        public static bool Prefix(TaskDig __instance, HitResult __result){
            if(!Main.configFlagModDiggingChunk){return true;} //#FUNC_01Another Flag:falseなら何もしない
            
            //ここからバニラの除外処理
            if(EClass._zone.IsRegion && __instance.GetTreasureMap() != null) {
                return true;
            }
            if(__instance.mode == TaskDig.Mode.RemoveFloor){
                return true;
            }
            Point pos = __instance.pos;
            if (EClass._zone.IsSkyLevel && (pos.Installed != null || pos.Charas.Count >= 2 || (pos.HasChara && pos.FirstChara != EClass.pc))){
                return true;
            }
            if (!pos.IsInBounds || pos.IsWater || pos.HasObj || (!EClass._zone.IsPCFaction && pos.HasBlock)){
                return true;
            }
            //ここまでバニラの除外処理
            if (!pos.HasBridge && pos.sourceFloor.id == 40){
                __result = HitResult.Valid;
                return false;
            }
            return true;
        }
    }
    */
