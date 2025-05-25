using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using s649PBR.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace s649PBR
{//>begin namespaceMain
    namespace Main
    {//>>begin namespaceSub
        [BepInPlugin("s649_PotionBottleRecycle", "s649 Potion Bottle Recycle", "0.2.0.0")]
        public class PatchMain : BaseUnityPlugin
        {//>>>begin class:PatchExe
            
            //////-----Config Entry---------------------------------------------------------------------------------- 

            //CE 00 general
            private static ConfigEntry<bool> CE_AllowFunctionUse;//飲む/使う
            private static ConfigEntry<bool> CE_AllowFunctionBlend;//混ぜる
            private static ConfigEntry<bool> CE_AllowFunctionCraft;//製作・及び加工機械
            private static ConfigEntry<bool> CE_AllowFunctionThrow;//投げる

            public static bool Cf_Allow_Use => CE_AllowFunctionUse.Value;
            public static bool Cf_Allow_Blend => CE_AllowFunctionBlend.Value;
            public static bool Cf_Allow_Craft => CE_AllowFunctionCraft.Value;
            public static bool Cf_Allow_Throw => CE_AllowFunctionThrow.Value;

            //CE キャラ制御
            private static ConfigEntry<int> CE_WhichCharaCreatesWhenDrink;//Useをどのキャラに適応させるか
            private static ConfigEntry<int> CE_WhichCharaCreatesWhenBlend;//Blendをどのキャラに適応させるか
            private static ConfigEntry<int> CE_WhichCharaCreatesWhenThrow;//Throwをどのキャラに適応させるか

            //Junk bottle
            private static ConfigEntry<bool> CE_AllowCreatesJunkBottles;
            private static ConfigEntry<int> CE_WhichCharaCreatesJunkBottles;//CreateJunkBottlesをどのキャラに適応させるか

            public static bool Cf_Reg_JunkBottle => CE_AllowCreatesJunkBottles.Value;
            
            //CE debug
            private static ConfigEntry<int> CE_LogLevel;//デバッグ用のログの出力LV　-1:出力しない 0~:第二引数に応じて出力
            public static int Cf_LogLevel => CE_LogLevel.Value;

            //loading------------------------------------------------------------------------------------------------------------------------------------------------------------
            internal void LoadConfig()
            {
                ////// 00 GENERAL
                CE_AllowFunctionUse = Config.Bind("#F00-General", "ALLOW_FUNCTION_USE", true, "Allow control of function [Use]");
                CE_AllowFunctionBlend = Config.Bind("#F00-General", "ALLOW_FUNCTION_BLEND", true, "Allow control of function [blend]");
                CE_AllowFunctionCraft = Config.Bind("#F00-General", "ALLOW_FUNCTION_CRAFT", true, "Allow control of function [craft]");
                CE_AllowFunctionThrow = Config.Bind("#F00-General", "ALLOW_FUNCTION_THROW", true, "Allow control of function [throw]");

                //CE_AllowFunction10_TraitDye = Config.Bind("#F00-General", "ALLOW_FUNCTION_10_TRAIT_DYE", true, "Allow control of function 10-dye");//v0.1.1
                
                // Regulate
                CE_WhichCharaCreatesWhenDrink = Config.Bind("#01-Reg", "Which_Chara_Creates_When_Drinking", 1, "0 = None, 1 = PC, 2 = PC&PARTY, 3 = ALL");
                CE_WhichCharaCreatesWhenBlend = Config.Bind("#01-Reg", "Which_Chara_Creates_When_Blending", 1, "0 = None, 1 = PC, 2 = PC&PARTY, 3 = ALL");
                CE_WhichCharaCreatesWhenThrow = Config.Bind("#01-Reg", "Which_Chara_Creates_When_Throwing", 1, "0 = None, 1 = PC, 2 = PC&PARTY, 3 = ALL");

                // junk
                CE_AllowCreatesJunkBottles = Config.Bind("#F00-General", "ALLOW_CREATES_JUNK_Things", true, "Allow to generate junk things");
                CE_WhichCharaCreatesJunkBottles = Config.Bind("#01-Reg", "Which_Chara_Creates_Junk_Things", 1, "0 = None, 1 = PC, 2 = PC&PARTY, 3 = ALL");
                
                // zz debug
                CE_LogLevel = Config.Bind("#ZZ-Debug", "DEBUG_LOGGING", 0, "If value >= 0, Outputs debug info.");
                
                string text = "[PBR]CF:Load";
                //text += ("[F01/" + TorF(cf_F01_NCBWD) + "]");
                Log(text, 0);
                
            }
            
            private void Start()
            {//>>>>begin method:Start
                LoadConfig();
                var harmony = new Harmony("PatchMain");
                new Harmony("PatchMain").PatchAll();
            }//<<<<end method:Start


            //local----------------------------------------------------------------------------------------------------------------------------------------
            //public static PlayerType playerType;//コンフィグのプレイヤー識別用

            internal static void Log(string text, int lv = 0)
            {
                if (Cf_LogLevel >= lv)
                {
                    Debug.Log(text);
                }
            }
            
            private static int TypeCharaPlaying(Card c)
            {
                if (c == null) { return 0; }
                if (c.IsPC)
                {
                    return 1;
                } else if(!c.IsPC && c.IsPCParty)
                {
                    return 2;
                } else
                {
                    return 3;
                }
            }
            private static bool GetRegulation(Card c, int at, bool isJunk = false)
            {
                int wcc = 0;
                int tcp = TypeCharaPlaying(c);
                bool allow_Func;
                bool reg_chara;
                bool reg_junk = Cf_Reg_JunkBottle && (isJunk) ? (tcp <= CE_WhichCharaCreatesJunkBottles.Value) : true;
                switch (at)
                {
                    case ActType.None: return false;
                    case ActType.Use:
                        wcc = CE_WhichCharaCreatesWhenDrink.Value;
                        allow_Func = Cf_Allow_Use;
                        break;
                    case ActType.Blend:
                        wcc = CE_WhichCharaCreatesWhenBlend.Value;
                        allow_Func = Cf_Allow_Blend;
                        break;
                    case ActType.Throw:
                        wcc = CE_WhichCharaCreatesWhenThrow.Value;
                        allow_Func = Cf_Allow_Throw;
                        break;
                    case ActType.Craft: return Cf_Allow_Craft;
                    default: return false;
                }
                reg_chara = TypeCharaPlaying(c) <= wcc;

                bool result = (!isJunk) ? (allow_Func && reg_chara) : (allow_Func && reg_chara && reg_junk);
                string text = "[PBR:GR]";
                text += "TCP:" + tcp.ToString() + "/";
                text += "Func:" + ToTF(allow_Func) + "/";
                text += "RC:" + ToTF(reg_chara) + "/";
                text += (isJunk)? ("RJ:" + ToTF(reg_junk) + "/") : "RJ:-/";
                text += "rs:" + ToTF(result) + "/";
                //Log(text, 1);
                return result; 
            }
            
            private static string ToTF(bool b){return (b)? "T": "F";}

            
            internal static int ReturnBottleIngredient(Thing t)
            {//>>>>begin method:ReturnBottleIngredient
             //description
             //return 1 : potion bottle
             //return 2 : empty bucket
             //return 0 : none
             //return -1 : bottle junk
             //return -2 : can junk
             //return -3  : can not junk
             //return -4 : drug bin
             //return -999 : fumei
             //description end
                if (t == null) { return 0; }
                Trait trait = t.trait; 
                string category = t.sourceCard.category;
                string unit = t.source.unit;
                    
                //Log("[TCPB]t is " + trait.ToString() + "/cate:" + category + "/u:" + unit);
                if(trait is TraitSnow){return BottleIngredient.None;}
                if(trait is TraitDye){return BottleIngredient.Bottle_Empty;}
                if(trait is TraitPotion || trait is TraitPotionRandom || trait is TraitPotionEmpty)
                {
                    if(category == "drug"){return BottleIngredient.Drug;} else {return BottleIngredient.Bottle_Empty; }
                    //return 1;
                }
                if(trait is TraitPerfume){return BottleIngredient.Junk_Bottles;}
                if(trait is TraitDrinkMilk || trait is TraitDrinkMilkMother){return BottleIngredient.None; }
                if(trait is TraitDrink)
                {
                    if(category == "booze"){
                        return BottleIngredient.Junk_Bottles;
                    }else if(category == "_drink"){
                        if(unit == "bucket"){return BottleIngredient.Bucket_Empty;}
                        if(unit == "pot"){return BottleIngredient.Bottle_Empty; }
                        if(unit == "bottle"){return BottleIngredient.Junk_Bottles; }
                    }
                }
                return BottleIngredient.None;
                    
            }//<<<<end method:ReturnBottleIngredient
            private static bool IsEnableRecycleBottle(int bottleIng, Card c, int acttype)
            {
                string text = "[IERB]bi:" + bottleIng.ToString() + "/C:" + c.NameSimple + "/at:" + acttype.ToString();
               // Log("[IERB]bi:" + bottleIng.ToString() + "/" + c.NameSimple + "/" + acttype.ToString());
                //regulateを参照して実行できるかどうかを返す
                bool isJunk = (bottleIng < 0) ? true : false;
                bool result = (bottleIng != 0) ? GetRegulation(c, acttype, isJunk) : false;
                text += "/iJ:" + ToTF(isJunk) + "/rs:" + ToTF(result);
                //Log(text , 1);
                return result;
                //return false;
            }

            internal static string GetBottleIngredient(Thing t)
            {
                return GetBottleIngredient(ReturnBottleIngredient(t));
            }
            internal static string GetBottleIngredient(int bi)
            {
                string resultid = null;
                
                switch (bi)
                {
                    case BottleIngredient.Bottle_Empty:
                        resultid = "potion_empty";
                        break;
                    case BottleIngredient.Bucket_Empty:
                        resultid = "bucket_empty";
                        break;
                    case BottleIngredient.None://nothing
                        break;
                    case BottleIngredient.Junk_Bottles:
                        resultid = GetRandomJunkBottle();//bottle
                        break;
                    case BottleIngredient.Junk_Can:
                        resultid = GetRandomJunkCan();//can
                        break;
                    case BottleIngredient.Can:
                        resultid = "";//can not junk
                        break;
                    case BottleIngredient.Drug:
                        resultid = "";//(!broken) ? "231" : "";//drug bin
                        break;
                    default:
                        resultid = "";
                        break;
                }
                return resultid;
            }
            internal static Thing DoRecycleBottle(Thing t, Chara c, int acttype, bool broken = false, Point p = null)
            {
                /*
                * Thing t に使われるBottleIngをidで返す->リサイクル実行:成否をThingで返す
               */
                Thing result = null;
                int bottleIng = ReturnBottleIngredient(t);
                if (!IsEnableRecycleBottle(bottleIng, (Card)c, acttype)) { return result; }
                //int oNum = t.Num;
                //int prodN = ReturnBottleIngredient(t);
                string resultid = GetBottleIngredient(bottleIng);
                //Thing prodT = null;
                //string prod = "";
                if (broken) //破損処理
                {
                    switch (bottleIng)
                    {
                        case BottleIngredient.Bottle_Empty:
                            resultid = "fragment";
                            break;
                        case BottleIngredient.Bucket_Empty:
                            //resultid = "bucket_empty";
                            break;
                        case BottleIngredient.None://nothing
                            break;
                        case BottleIngredient.Junk_Bottles:
                            resultid = "fragment";//bottle
                            break;
                        case BottleIngredient.Junk_Can:
                            //resultid = GetRandomJunkCan();//can
                            break;
                        case BottleIngredient.Can:
                            //resultid = "";//can not junk
                            break;
                        case BottleIngredient.Drug:
                            resultid = "";
                            //resultid = "";//(!broken) ? "231" : "";//drug bin
                            break;
                        default:
                            //resultid = "";
                            break;
                    }
                }
                

                //if(prod == ""|| prod == "qqq"){return null;} else {return ThingGen.Create(prod);} 
                //Thing result;
                if (resultid != "")
                {
                    result = ThingGen.Create(resultid);
                    if (c.IsPC) { c.Pick(result); } else 
                    { 
                        if(p == null) { p = c.pos; }
                        EClass._zone.AddCard(result, p);
                    }
                    PatchMain.Log("[PBR:DRB]Create:" + result.id + "  -> " + c.NameSimple, 1);
                    //return true;
                }
                return result;
            }
            public static List<string> JunkBottleList = new List<string> { "726", "727", "728" };
            public static string GetRandomJunkBottle()
            {
                List<string> sList = JunkBottleList;
                return (sList != null) ? sList[Random.Range(0, sList.Count)] : "";
            }
            public static List<string> JunkCanList = new List<string> { "236", "529", "1170" };
            public static string GetRandomJunkCan()
            {
                List<string> sList = JunkCanList;
                return (sList != null) ? sList[Random.Range(0, sList.Count)] : "";
            }
            internal static void AddThingToList(List<RecycleThing> list, RecycleThing rt)
            {
                //bool b = false;
                foreach (RecycleThing t in list)
                {
                    if (t.IsEqualName(rt))
                    {
                        t.AddNum(rt);//listにあったので加算
                        //b = true;
                        return;
                    }
                }
                list.Add(rt);//listになかったので追加
            }

            internal static void RemoveFromList(List<RecycleThing> rlist, string rt, int rnum)
            {
                if(rt != "" && rnum > 0)
                {
                    foreach (RecycleThing rthing in rlist)
                    {
                        if (rthing.IsEqualName(rt))
                        {
                            rthing.Decrease(rnum);
                            if (rthing.num <= 0)
                            {
                                rlist.Remove(rthing);
                            }
                            return;
                        }

                    }
                }
                
            }
            internal static void ExeRecycle(List<RecycleThing> rlist, Chara c)
            {
                string text = "[recycle]";
                foreach (RecycleThing rthing in rlist)
                {
                    Thing t = ThingGen.Create(rthing.name, rthing.num);
                    EClass._zone.AddCard(t, c.pos);
                    text += "N:" + t.NameSimple + "/n:" + t.Num.ToString();
                }
                Log(text, 1);
            }

        }//<<<end class:Main
        
        public class ActType
        {//class:PlayerType
            //public int value;
            public const int None = 0;
            public const int Use = 1;
            public const int Blend = 2;
            public const int Throw = 3;
            public const int Craft = 4;
            
        }//class:PlayerType
        public class BottleIngredient
        {//class:BottleIng
            public const int None = 0;
            public const int Bottle_Empty = 1;
            public const int Bucket_Empty = 2;
            public const int Junk_Bottles = -1;
            public const int Junk_Can = -2;
            public const int Can = -3;
            public const int Drug = -4;
            public const int Other = -999;
        }//class:BottleIngredient
        public class RecycleThing
        {
            public string name { get; }
            public int num { get; set; }

            // Change the constructor's access modifier to 'public' to fix CS0122  
            public RecycleThing(string name, int num)
            {
                this.name = name;
                this.num = num;
            }
            public void AddNum(RecycleThing t)
            { this.num += t.num; }
            public bool IsEqualName(RecycleThing t)
            {
                return IsEqualName(t.name);
            }
            public bool IsEqualName(string tid)
            {
                return (tid == this.name) ? true : false;
            }

            public void Decrease(int a)
            {
                this.num -= a;
            }
        }

    }//<<end namespaceSub
}//<end namespaceMain
/*
//関連するレシピリスト・バニラ
    ・・クラフト（未対応）
    ・便利屋の机
    大窯    飲料20  予測結果20～0
    渇きの壺    水2     予測結果2
    染料窯  飲料20  予測結果20～0
    書道具　水1     予測結果1
    ・木工の机
    ベッド　※まだ
    奉納酒　※まだ
    ・金属工の机
        ※まだりすと　バーの椅子・カジノの椅子
    バスタブ　飲料20    予測結果20～0
    トイレ      耐酸2   予測結果2
    ・彫刻
    間欠泉　染料1　※まだ
    流し台  飲料4   予測結果4～0
    石のバスタブ    飲料20  予測結果20～0
    ・硝子工
    なし
    ・装飾台
    パンプキンランプ　染料1　※まだ
    ・裁縫
        ※まだりすと　　椅子・ハートのクッション・お布団
    変な枕  媚薬1   予測結果1
    ・筆記用具
    危ない本    乳1　※まだ
    ・建材
    素材染料　カーペット・水の床・タイルの床・モダンなカーペット
    ・料理
    ※まだ　乳・ミルクが含まれるもの・ムース・デラックス雪プチケーキ・プリン・クリームシチュー
    
・・加工設備
    ・石うす
    粘土    ポーション1    予測結果0～1
    空き瓶  ポーション1     予測結果0
    ・染料窯
    染料    空き瓶1素材1    予測結果1～0





*/
/* potionList [id/Trait/[CanDrink/CanBlend/ContainBottle]
                   snow	雪	TraitSnow   [F/T/F]
                   water   水  TraitDrink																									
                   water_dirty	汚水    TraitDrink
                   bucket	水の入ったバケツ    TraitDrink
                   potion	ポーション  TraitPotion/TraitPotionRandom
                   drug	薬      TraitPotion [T/T/F]
                   perfume	香水    TraitPerfume
                   milk	乳
                   drink	飲料
                   milkcan	ミルク缶
                   dye	染料   TraitDye [F/T/T]


                   +++++ thingV list ++++++++++++++++++++++++++
                   TraitDrink
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

                   TraitPotion
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

//関係ありそうなアイテム
//  potion_empty 空きポーション瓶　TraitPotionEmpty
//   dyamaker   染料窯 


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
/*
            internal static bool IsEnableRecyclelBottle(Chara c, int acttype, bool isJunk)
            {
                switch(acttype)
                {
                    case ActType.Use:
                        if (!Cf_Allow_Use) { return false; }
                        if (c.IsPC)
                        { return Cf_Reg_Use_PC && (!isJunk)? true : Cf_Reg_CJB_PC; }
                        else 
                        { return Cf_Reg_Use_NPC; }
                    case ActType.Blend :
                        if (!Cf_Allow_Blend) { return false; }
                        if (c.IsPC)
                        { return Cf_Reg_Blend_PC; }
                        else
                        { return Cf_Reg_Blend_NPC; }
                    case ActType.Throw:
                        if (!Cf_Allow_Throw) { return false; }
                        if (c.IsPC)
                        { return Cf_Reg_Throw_PC; }
                        else
                        { return Cf_Reg_Throw_NPC; }
                    case ActType.Craft:
                        return Cf_Allow_Craft;
                    default:
                        return false;

                }
            }*/
//internal static bool GetRegulation(Card c, int at, bool isJunk = false)
//{
//Log("[GR]" + c.ToString() + "/" + at.ToString() + isJunk.ToString());

//PlayerType pt = playerType.GetPT(c);
//return GetRegulation(c, at, isJunk);
//}
/*
        public class PlayerType
        {//class:PlayerType
            private int _value;//constructorで指定するように
            public PlayerType(int v = 0)
            {
                _value = v;
            }
            public const int None = 0;
            public const int Player = 1;
            public const int PlayerAndParty = 2;
            public const int All = 3;
            public PlayerType GetPT(Card c)
            {
                PlayerType pt;
                if(c.IsPC)
                {
                    pt = new PlayerType(PlayerType.Player);
                } else if(c.IsPCParty)
                {
                    pt = new PlayerType(PlayerType.PlayerAndParty);
                } else
                {
                    pt = new PlayerType(PlayerType.All);
                }
                return pt;
            }
            
            public bool IsContain(int wcc)
            {
                return wcc >= _value;
            }
            
        }//class:PlayerType
        */
//操作キャラごとの制御のコンフィグ

//private static bool Cf_Reg_Use_PC => Cf_Allow_Use && PlayerType.ContainPC(CE_WhichCharaCreatesWhenDrink.Value);
//private static bool Cf_Reg_Use_NPC => Cf_Allow_Use &&  PlayerType.ContainNPC(CE_WhichCharaCreatesWhenDrink.Value);
//private static bool Cf_Reg_Blend_PC => Cf_Allow_Blend && PlayerType.ContainPC(CE_WhichCharaCreatesWhenBlend.Value);
//private static bool Cf_Reg_Blend_NPC => Cf_Allow_Blend && PlayerType.ContainNPC(CE_WhichCharaCreatesWhenBlend.Value);
//private static bool Cf_Reg_Throw_PC => Cf_Allow_Throw && PlayerType.ContainPC(CE_WhichCharaCreatesWhenThrow.Value);
//private static bool Cf_Reg_Throw_NPC => Cf_Allow_Throw && PlayerType.ContainNPC(CE_WhichCharaCreatesWhenThrow.Value);
//public static bool Cf_Reg_CJB_PC => Cf_Reg_JunkBottle && PlayerType.ContainPC(CE_WhichCharaCreatesJunkBottles.Value);
// public static bool Cf_Reg_CJB_NPC => Cf_Reg_JunkBottle && PlayerType.ContainNPC(CE_WhichCharaCreatesJunkBottles.Value);

