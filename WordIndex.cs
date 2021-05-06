using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using SLLSFunction;

namespace SignLanguageLearningSystem4
{
	class WordIndex
	{
        private static WordData[] worddatas = ReadCSV();
        public static string[] WordnameList { get; private set; } = GetWordnameList();
        private struct WordData
        {
            public int index;//ID
            public string wordname;//ローマ字
            public string wordname_jp;//日本語名
            public string explanation;//説明
        }

        private static WordData[] ReadCSV()
        {
            string csvPath = @"config\WordList.csv";
            List<string> datas = FileIO.FileTextsInput(csvPath);
            var retDatas = new WordData[datas.Count];
            if (datas.Any())
            {
                int i = 0;
                foreach (var data in datas)
                {
                    string[] inData = data.Split(',');
                    WordData wd = new WordData
                    {
                        index = i,
                        wordname = inData[1],
                        wordname_jp = inData[2],
                        explanation = inData[3],

                    };
                                           
                    retDatas[i] = wd;
                    i++;
                }
            }
            return retDatas;
        }

        public static int WordnamejpToIndex(string wordname_jp)
        {
            //日本語名をIDに
            foreach (var worddata in worddatas)
            {
                if (worddata.wordname_jp == wordname_jp)
                {
                    return worddata.index;
                }
            }
            return -1;
        }

        public static string IndexToWordname(int index)
        {
            //IDをローマ字に
            foreach (var worddata in worddatas)
            {
                if (worddata.index == index)
                {
                    return worddata.wordname;
                }
            }
            return "";
        }

        public static string IndexToExplanation(int index)
        {
            //IDを説明に
            foreach (var worddata in worddatas)
            {
                if (worddata.index == index)
                {
                    return worddata.explanation;
                }
            }
            return "";
        }

        private static string[] GetWordnameList()
        {
            //日本語名のリストを返す
            string[] retData = new string[worddatas.Count()];
            int i = 0;
            foreach (var worddata in worddatas)
            {
                retData[i] = worddata.wordname_jp;
                i++;
            }
            return retData;
        }
    }
}
