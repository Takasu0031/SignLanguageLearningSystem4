using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLLSFunction;

namespace SignLanguageLearningSystem4
{
    class ExcelDataUse
    {
        public int ID { get; private set; }
        public bool LeftHandCheck { get; private set; }
        public double SeparatePos { get; private set; }
        public int ScoreCount { get; set; }

        private readonly string excelFileName = @"config\Data.xlsx";

        //単語データベースから情報を取り出す
        public ExcelDataUse()
        {
            int n = -1;
            var workbook = new XLWorkbook(excelFileName);
            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed().RowNumber();
            string wordname = MainWindow.Wordname;
            for (int i = 1; i <= lastRow; i++)
            {
                string Excelword = worksheet.Cell(i, 2).GetString();
                if (string.Equals(Excelword, wordname))
                {
                    n = i;
                    break;
                    Console.WriteLine("Hi");
                }
            }

            ID = (int)worksheet.Cell(n, 1).GetDouble();
            LeftHandCheck = worksheet.Cell(n, 3).GetBoolean();
            //}
            SeparatePos = worksheet.Cell(n, 4).GetDouble();
            ScoreCount = worksheet.Cell(n, 5).GetValue<int>() + 1;

            workbook.Dispose();
        }

        public ExcelDataUse(string wordname)
        {
            int n = -1;
            var workbook = new XLWorkbook(excelFileName);
            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed().RowNumber();
            for (int i = 1; i <= lastRow; i++)
            {
                string Excelword = worksheet.Cell(i, 2).GetString();
                if (string.Equals(Excelword, wordname))
                {
                    n = i;
                    break;
                }
            }

            ID = (int)worksheet.Cell(n, 1).GetDouble();
            LeftHandCheck = worksheet.Cell(n, 3).GetBoolean();
            SeparatePos = worksheet.Cell(n, 4).GetDouble();
            ScoreCount = worksheet.Cell(n, 5).GetValue<int>() + 1;

            workbook.Dispose();
        }

        public void NoChangeScoreCount()
        {
            ScoreCount = 1;
        }

        public void SaveScoreCount()
        {
            int n = -1;
            var workbook = new XLWorkbook(excelFileName);
            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed().RowNumber();
            string wordname = MainWindow.Wordname;
            for (int i = 1; i <= lastRow; i++)
            {
                string Excelword = worksheet.Cell(i, 2).GetString();
                if (string.Equals(Excelword, wordname))
                {
                    //errorCheck = false;
                    n = i;
                    break;
                }
            }

            worksheet.Cell(n, 5).Value = ScoreCount;
            workbook.Save();
            workbook.Dispose();
        }

        public static void ResetScoreCount()
        {
            string excelFN = @"config\Data.xlsx";
            var workbook = new XLWorkbook(excelFN);
            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed().RowNumber();
            for (int i = 1; i <= lastRow; i++)
            {
                worksheet.Cell(i, 5).Value = 0;

            }
            workbook.Save();
            workbook.Dispose();
        }

        //データベースの作成(現状エクセル)
        public void SaveExcelData(DataArray[] resultData)
        {
            string filename = @"config\score.xlsx";
            var ScoreWorkbook = new XLWorkbook(filename);
            var ScoreWorksheet = ScoreWorkbook.Worksheet(1);
            int n = ScoreWorksheet.Cell(1, 1).GetValue<int>() + 1;
            ScoreWorksheet.Cell(n, 2).Value = ID;
            ScoreWorksheet.Cell(n, 3).Value = MainWindow.Wordname_jp;
            //ScoreWorksheet.Cell(n, 4).Value =MainWindow.Mode;
            //列がずれる影響を考慮してコメント
            ScoreWorksheet.Cell(n, 5).Value = ScoreCount;
            ScoreWorksheet.Cell(n, 6).Value = resultData[0].X;
            ScoreWorksheet.Cell(n, 7).Value = resultData[0].Y;
            ScoreWorksheet.Cell(n, 8).Value = resultData[0].Z;
            ScoreWorksheet.Cell(n, 9).Value = resultData[0].sumDataArray();
            ScoreWorksheet.Cell(n, 10).Value = resultData[1].X;
            ScoreWorksheet.Cell(n, 11).Value = resultData[1].Y;
            ScoreWorksheet.Cell(n, 12).Value = resultData[1].Z;
            ScoreWorksheet.Cell(n, 13).Value = resultData[1].sumDataArray();
            ScoreWorksheet.Cell(1, 1).Value = n;

            ScoreWorkbook.Save();
            ScoreWorkbook.Dispose();
        }
    }
}

