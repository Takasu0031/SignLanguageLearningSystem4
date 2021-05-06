using System;
using System.Collections.Generic;
using System.Linq;

namespace SLLSFunction {
    public class DPMatchingFlow {
        //DP照合
        public static DataArray DPMatching(List<DataArray> Master, List<DataArray> Beginer) {
            int M_FrameNumber = Master.Count(); //お手本のフレーム長
            int B_FrameNumber = Beginer.Count(); //初心者手話のフレーム長
            int Sum_Framenumber = M_FrameNumber + B_FrameNumber;
            int i, j; //i:master j:beginer

            DataArray[,] Cost = new DataArray[M_FrameNumber, B_FrameNumber]; //コスト
            DataArray nowData = new DataArray();
            DataArray zeroData = new DataArray(0);
            DataArray maxData = new DataArray(float.MaxValue);
            DataArray retArray;

            Cost[0, 0] = zeroData;

            //→方向に初期値設定
            for(i = 1; i < M_FrameNumber; i++) {
                Cost[i, 0] = maxData;
            }

            //↓方向に初期値設定
            for(j = 1; j < B_FrameNumber; j++) {
                Cost[0, j] = maxData;
            }

            //コスト計算
            for(i = 1; i < M_FrameNumber; i++) {
                for(j = 1; j < B_FrameNumber; j++) {
                    nowData = DataArray.Calc_ManhattanDistance(Master[i], Beginer[j]);
                    DataArray dtemp1 = DataArray.Calc_AddDA(Cost[i - 1, j - 1], nowData.Calc_MulScalor(2));
                    DataArray dtemp2 = DataArray.Calc_AddDA(Cost[i - 1, j], nowData);
                    DataArray dtemp3 = DataArray.Calc_AddDA(Cost[i, j - 1], nowData);

                    /* 基本2
                     * 1にすると各次元それぞれ最小になるようにPM
                     * 2にすると各次元のパターン間距離の和が最小になるようにPM
                     */
                    //Cost[i, j] = Calc_MinDataArray1(dtemp1, dtemp2, dtemp3);
                    Cost[i, j] = Calc_MinDataArray2(dtemp1, dtemp2, dtemp3);
                }
            }

            retArray = new DataArray() {
                X = Cost[M_FrameNumber - 1, B_FrameNumber - 1].X / Sum_Framenumber,
                Y = Cost[M_FrameNumber - 1, B_FrameNumber - 1].Y / Sum_Framenumber,
                Z = Cost[M_FrameNumber - 1, B_FrameNumber - 1].Z / Sum_Framenumber
            };

            return retArray;
        }

        //XYZバラバラで
        private static DataArray Calc_MinDataArray1(DataArray A, DataArray B, DataArray C) {
            DataArray dataArray = new DataArray {
                X = MostMin(A.X, B.X, C.X),
                Y = MostMin(A.Y, B.Y, C.Y),
                Z = MostMin(A.Z, B.Z, C.Z),
            };
            return dataArray;
        }

        //XYZまとめて
        private static DataArray Calc_MinDataArray2(DataArray A, DataArray B, DataArray C) {
            float Asum = A.sumDataArray();
            float Bsum = B.sumDataArray();
            float Csum = C.sumDataArray();

            float mostmin = MostMin(Asum, Bsum, Csum);
            if (mostmin == Asum) {
                return A;
            } else if (mostmin == Bsum) {
                return B;
            } else if (mostmin == Csum) {
                return C;
            } else {
                Console.WriteLine(mostmin + "は存在しません");
                return new DataArray(-1);
            }
        }

        //3つのデータから最小がどれかを探す関数
        private static float MostMin(float a, float b, float c) {
            if (a <= b && a <= c) {
                return a;
            } else if (b <= c) {
                return b;
            } else {
                return c;
            }
        }
    }
}
