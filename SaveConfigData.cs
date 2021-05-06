using System.Collections.Generic;
//単語の種類や動作設定などを保存しておく変数の設定

namespace SignLanguageLearningSystem4
{
    class SaveConfigData {
        public static string wordname { get; private set; } = "Aomori";
        public static void setWordname(string wordname) {
            SaveConfigData.wordname = wordname;
        }

        public static string wordname_jp { get; private set; } = "青森";
        public static void setWordname_jp(string wordname_jp) {
            SaveConfigData.wordname_jp = wordname_jp;
        }

        public static List<string> wordnameList { get; private set; } = new List<string>();
        public static void setWordnameList(List<string> wordnameList) {
            SaveConfigData.wordnameList = wordnameList;
        }
    }
}
