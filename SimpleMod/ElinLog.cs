
using Debug = UnityEngine.Debug;
using s649PBR.BIClass;
using System.Collections.Generic;
namespace s649ElinLog
{//begin namespaceMain
    public class LogTier
    {
        public const int Fatal = -1;//未使用
        public const int Error = 0;//引数不正やtrycatchで投げられる
        public const int Info = 1;//実行結果出力など
        public const int Deep = 2;//引数の詳細
        public const int Other = 3;//動作の確認
        public const int Tweet = 4;//末端のメソッドの呼び出し確認など
    }
    public class ElinLog 
    {
        public static string modNS { private get; set; }
        public static int Level_Log { private get; set; }
        private static List<string> stackLog = new List<string> { };
        public static void SetConfig(int lv, string ns)
        {
            Level_Log = lv;
            modNS = ns;
        }
        public static void LogDeepTry(bool b)
        {
            string text = b ? "Success!" : "Failed...";
            LogDeep(text);
        }
        public static void LogTweet(string s)
        {
            //string text = b ? "Success!" : "Failed...";
            //LogDeepTry(text);
            Log(s, LogTier.Tweet);
        }
        public static void LogOther(string s)
        {
            //string text = b ? "Success!" : "Failed...";
            //LogDeepTry(text);
            Log(s, LogTier.Other);
        }
        public static void LogDeep(string s)
        {
            //string text = b ? "Success!" : "Failed...";
            //LogDeepTry(text);
            Log(s, LogTier.Deep);
        }
        public static void LogInfo(string s)
        {
            //string text = b ? "Success!" : "Failed...";
            //LogDeepTry(text);
            Log(s, LogTier.Info);
        }
        public static void LogError(string argText)
        {
            Log(argText, 0);
        }
        public static void Log(string arg, int lv)
        {
            //string text = b ? "Success!" : "Failed...";
            //LogDeepTry(text);
            string logHeader = "";
            switch (lv)
            {
                case < 0:
                    return;
                case 0:
                    logHeader = "[Error]";
                    break;
                case 1:
                    logHeader = "[Info ]";
                    break;
                case 2:
                    logHeader = "[Deep ]";
                    break;
                case 3:
                    logHeader = "[Other]";
                    break;
                default:
                    logHeader = "[Tweet]";
                    break;

            }
            if (Level_Log >= lv)
            {
                Debug.Log(modNS + logHeader + string.Join("", stackLog) + arg);
            }
            //Debug.Log(s);
        }
        internal static void LogStack(string argString)
        {
            //メソッドの先頭で呼び出し、ログ用のヘッダーを追加する
            //メソッドの終点でLogStackDumpを呼び出す必要がある
            //※混乱を招くため処理の途中に追加してはいけない
            //末端のメソッドなら呼び出す必要はない
            //ログ処理だけして中身は別メソッド処理に任せるやり方も後の混乱を呼ぶのでだめ。中継メソッドはできるだけ簡素に
            //stackLogLast = stackLog;
            //stackLog += argString;
            stackLog.Add(argString);
        }
        internal static void LogStackDump()
        {
            if (stackLog.Count > 1) { stackLog.RemoveAt(stackLog.Count - 1); }
            //stackLog = stackLogLast;
            //stackLog += argString;
        }
        
        public static void ClearLogStack()
        {   //初期化。HarmonyPatchのpreかpostで、メソッドの頭で必ず呼び出す。
            //preとpostで重複呼び出しはしない事。
            stackLog = new List<string> { };
        }
        public static string StrConv(object input)
        {
            switch (input) 
            {
                case bool:
                    return ToTF((bool)input);
                case int:
                    return ((int)input).ToString();
                case string:
                    return (string)input;
                case Card:
                    return ((Card)input).NameSimple;
                case Recipe:
                    return ((Recipe)input).GetName();
                default:
                    return input?.ToString() ?? "";
            }
        }

        private static string ToTF(bool b) { return b ? "T" : "F"; }
        /*
        public static string GetStr(bool b)
        {
            return ToTF(b);
        }
        public static string GetStr(int arg)
        {
            return (arg != 0) ? arg.ToString() : "0";
        }
        public static string GetStr(string s)
        {
            return s;
        }
        public static string GetStr(Point arg)
        {
            return (arg != null) ? arg.ToString() : "-";
        }
        public static string GetStr(Trait arg)
        {
            return (arg != null) ? arg.ToString() : "-";
        }
        public static string GetStr(Card arg)
        {
            return (arg != null) ? arg.NameSimple : "-";
        }*/
    }
}//end namespaceMain