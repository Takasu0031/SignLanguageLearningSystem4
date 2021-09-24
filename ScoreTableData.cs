using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using SLLSFunction;

namespace SignLanguageLearningSystem4
{
    public class ScoreTableData {
        public string Wordname { get; set; }
        public string Count { get; set; }
        public double RX_d { get; set; } 
        public double RY_d { get; set; } 
        public double RZ_d { get; set; } 
        public double LX_d { get; set; } 
        public double LY_d { get; set; }
        public double LZ_d { get; set; }
        public string RX { get; set; }
        public string RY { get; set; }
        public string RZ { get; set; }
        public string LX { get; set; }
        public string LY { get; set; }
        public string LZ { get; set; }
        public string judge { get; set; }

        public ScoreTableData() {
            judge = "nodata";
        }

        public ScoreTableData(DataArray[] score, ScoreTableData preData) {
            if(preData.judge == "nodata") {
                preData = this;
            }
            Count = MainWindow.ScoreCount.ToString();
            RX_d = Math.Round(score[0].X * 100, 1, MidpointRounding.AwayFromZero);
            RY_d = Math.Round(score[0].Y * 100, 1, MidpointRounding.AwayFromZero);
            RZ_d = Math.Round(score[0].Z * 100, 1, MidpointRounding.AwayFromZero);
            LX_d = Math.Round(score[1].X * 100, 1, MidpointRounding.AwayFromZero);
            LY_d = Math.Round(score[1].Y * 100, 1, MidpointRounding.AwayFromZero);
            LZ_d = Math.Round(score[1].Z * 100, 1, MidpointRounding.AwayFromZero);
            RX = ScoreToString(RX_d, preData.RX_d);
            RY = ScoreToString(RY_d, preData.RY_d);
            RZ = ScoreToString(RZ_d, preData.RZ_d);
            LX = ScoreToString(LX_d, preData.LX_d);
            LY = ScoreToString(LY_d, preData.LY_d);
            LZ = ScoreToString(LZ_d, preData.LZ_d);
            judge = "no";
        }

        private string ScoreToString(double now, double pre) {
            string retStr;
            retStr = now.ToString();
            double diff = Math.Round((now - pre), 1, MidpointRounding.AwayFromZero);
            if (diff < 0) {
                retStr += " (" + diff + ")";
            } else if (diff > 0) {
                retStr += " (" + "+" + diff + ")";
            } 
            return retStr;
        } 
    }
}
