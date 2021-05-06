using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLLSFunction {
    public class Normalization {
        //元データ
        public List<DataArray> OriginalData;
        //正規化したデータ
        public List<DataArray> NormarizationData;
        //速さ(フレーム間距離)
        public List<DataArray> Speed;
        //速度のノルム
        public List<double> SpeedNorm;
        //元データの統計データ
        public int count;
        public DataArray average;
        public DataArray stdevp;
        public DataArray max;
        public DataArray min;

        public Normalization(List<DataArray> dataArrays) {
            OriginalData = dataArrays;

            Calc_Statistics();

            NormarizationData = new List<DataArray>();
            //DataNormalize();
            Smoothing();

            //Speed = new List<DataArray>();
            //SpeedNorm = new List<double>();
            //Calc_SpeedNorm();

            //Console.WriteLine("正規化前");
            //foreach (double s in SpeedNorm) {
            //    Console.WriteLine(s.ToString());
            //}

            //SpeedNormSmoothing();

            //Console.WriteLine("正規化後");
            //foreach (double s in SpeedNorm) {
            //    Console.WriteLine(s.ToString());
            //}
        }

        private void Calc_Statistics() {
            count = OriginalData.Count;
            Calc_average();
            Calc_stdevp();
            Calc_max();
        }

        private void Calc_average() {
            DataArray sum = DataArray.Calc_AddDA(OriginalData);
            average = sum.Calc_DivScalor(count);
        }

        private void Calc_stdevp() {
            DataArray average_square = DataArray.Calc_MulDA(average);
            DataArray sum = new DataArray(0);
            foreach(DataArray da in OriginalData) {
                sum = DataArray.Calc_AddDA(sum, DataArray.Calc_MulDA(da));
            }
            stdevp = sum.Calc_DivScalor(count);
            stdevp = DataArray.Calc_SubDA(stdevp, average_square);
            stdevp = stdevp.Calc_SquareRoot();         
        }

        private void Calc_max() {
            max = new DataArray(float.MinValue);
            foreach(DataArray da in OriginalData) {
                if(da.X > max.X) {
                    max.X = da.X;
                }
                if (da.Y > max.Y) {
                    max.Y = da.Y;
                }
                if (da.Z > max.Z) {
                    max.Z = da.Z;
                }
            }
        }

        private void Calc_min() {
            min = new DataArray(float.MaxValue);
            foreach (DataArray da in OriginalData) {
                if (da.X < min.X) {
                    min.X = da.X;
                }
                if (da.Y < min.Y) {
                    min.Y = da.Y;
                }
                if (da.Z < min.Z) {
                    min.Z = da.Z;
                }
            }
        }

        private void DataNormalize() {
            foreach (DataArray da in OriginalData) {
                DataArray addData;

                addData = DataArray.Calc_SubDA(da, average);
                addData = DataArray.Calc_DivDA(addData, stdevp);

                NormarizationData.Add(addData);
            }
        }

        private void Smoothing() {
            MovingAverage_5(3);
            StartPointTranslation();
        }

        private void Calc_SpeedNorm() {
            for (int i = 1; i < OriginalData.Count; i++) {
                DataArray speed = DataArray.Calc_SubDA(OriginalData[i], OriginalData[i - 1]);
                Speed.Add(speed);
                SpeedNorm.Add(speed.Calc_VectorSize());
            }
        }

        private void SpeedNormSmoothing() {
            int Pre_WCount = 0;
            int WCount = 0;

            do {
                Pre_WCount = WCount;
                WCount = 0;
                double start = SpeedNorm[0];

                for (int i = 1; i < SpeedNorm.Count - 1; i++) {
                    //=IF(OR(AND(AO1-AO2>0,AO3-AO2>0),AND(AO2-AO1>0,AO2-AO3>0)),1,0)
                    double temp;

                    if (SetExPos(start, SpeedNorm[i], SpeedNorm[i + 1])) {
                        temp = (SpeedNorm[i-1] + SpeedNorm[i + 1]) / 2;
                        WCount++;
                    } else {
                        temp = SpeedNorm[i];
                    }

                    start = SpeedNorm[i];
                    SpeedNorm[i] = temp;
                }
            } while (Pre_WCount != WCount);
        }


        private static bool SetExPos(double q0, double q1, double q2) {
            return ((q1 - q0 > 0) && (q1 - q2 > 0)) || ((q0 - q1 > 0) && (q2 - q1 > 0));
        }

        public Normalization Do_Separate(int start, int stop) {
            List<DataArray> dataArrays = new List<DataArray>();
            for (int i = start; i < stop; i++) {
                dataArrays.Add(OriginalData[i]);
            }
            return new Normalization(dataArrays);
        }

        public void MovingAverage_5(int AddNum) {
            int HalfAddNum = AddNum / 2 + 1;
            for (int n = 0; n < OriginalData.Count - HalfAddNum; n++) {
                DataArray da = OriginalData[n];
                for (int i = 1; i < AddNum; i++) {
                    da = DataArray.Calc_AddDA(da, OriginalData[n + i]);
                }
                NormarizationData.Add(da.Calc_DivScalor(AddNum));
            }
        }

        public void StartPointTranslation() {
            DataArray StartPoint = NormarizationData[0].Calc_MulScalor(-1);
            for (int i = 0; i < NormarizationData.Count(); i++) {
                NormarizationData[i] = DataArray.Calc_AddDA(NormarizationData[i], StartPoint);
            }
        }
    }
}
