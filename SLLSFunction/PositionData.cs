using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLLSFunction
{
    public class PositionData {
        //ここの位置変数を増やしていけば他の骨格を使用したい場合にも対応可能
        public Normalization P_RightHand;
        public Normalization P_LeftHand;
        public List<DataArray> P_SpineShoulder;
        public List<DataArray> P_Head;
    //public Normalization P_Neck;

        public PositionData() {
        }

        public PositionData(string Foldername) {
            string filename;
            List<DataArray> tmpData = new List<DataArray>();

            filename = Foldername + "\\P_HandRight.csv";
            tmpData.Clear();
            FileIO.FileRead(filename, tmpData);
            P_RightHand = new Normalization(tmpData);

            filename = Foldername + "\\P_HandLeft.csv";
            tmpData.Clear();
            FileIO.FileRead(filename, tmpData);
            P_LeftHand = new Normalization(tmpData);

            filename = Foldername + "\\P_SpineShoulder.csv";
            tmpData.Clear();
            FileIO.FileRead(filename, tmpData);
            P_SpineShoulder = tmpData;

            filename = Foldername + "\\P_Head.csv";
            tmpData.Clear();
            FileIO.FileRead(filename, tmpData);
            P_Head = tmpData;

            //filename = Foldername + "\\P_Neck.csv";
            //tmpData = new List<DataArray>();
            //FileIO.FileRead(filename, tmpData);
            //P_Neck = new Normalization(tmpData);
        }

        //public PositionData Do_SeparatePD(int start, int stop) {
        //    PositionData retData = new PositionData() {
        //        P_RightHand = P_RightHand.Do_Separate(start, stop),
        //        P_LeftHand = P_LeftHand.Do_Separate(start, stop),
        //        P_SpineShoulder = P_SpineShoulder.Do_Separate(start, stop),
        //        P_Head = P_Head.Do_Separate(start, stop),
        //    };
        //    return retData;
        //}

        public void setNormalization(List<DataArray> dataArrays, int identifier) {
            switch (identifier) {
                case 0:
                    P_RightHand = new Normalization(dataArrays);
                    break;
                case 1:
                    P_LeftHand = new Normalization(dataArrays);
                    break;
                case 2:
                    P_SpineShoulder = dataArrays;
                    break;
                case 3:
                    P_Head = dataArrays;
                    break;
                //case 4:
                //    P_Neck = dataArrays;
                //    break;
                //骨格を増やしたい場合はこちらも増やす
            }
        }
    }
}
