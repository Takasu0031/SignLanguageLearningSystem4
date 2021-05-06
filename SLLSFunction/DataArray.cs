using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace SLLSFunction {
    /*
     * DataArray構造体の定義
     *  3次元(X,Y,Z)の値を扱いたいときに使う
     */
    public struct DataArray {
        public float X;
        public float Y;
        public float Z;       

        //DataArray型に値を代入するときに使う
        public DataArray(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public DataArray(float n) {
            X = n;
            Y = n;
            Z = n;
        }

        public DataArray(CameraSpacePoint csp) {
            X = csp.X;
            Y = csp.Y;
            Z = csp.Z;
        }

        //以下DataArray型同士の演算の定義
        public float sumDataArray() {
            return (X + Y + Z);
        }

        public static DataArray Calc_AddDA(DataArray A, DataArray B) {
            DataArray dataArray = new DataArray {
                X = A.X + B.X,
                Y = A.Y + B.Y,
                Z = A.Z + B.Z
            };
            return dataArray;
        }

        public static DataArray Calc_AddDA(DataArray A, DataArray B, DataArray C) {
            DataArray dataArray = new DataArray {
                X = A.X + B.X + C.X,
                Y = A.Y + B.Y + C.Y,
                Z = A.Z + B.Z + C.Z
            };
            return dataArray;
        }

        public static DataArray Calc_AddDA(List<DataArray> DAs) {
            DataArray retData = new DataArray(0);
            foreach(DataArray da in DAs) {
                retData.X += da.X;
                retData.Y += da.Y;
                retData.Z += da.Z;
            }
            return retData;
        }

        public static DataArray Calc_SubDA(DataArray A, DataArray B) {
            float x = A.X - B.X;
            float y = A.Y - B.Y;
            float z = A.Z - B.Z;
            return new DataArray(x, y, z);
        }

        public DataArray Calc_MulScalor(float n) {
            return new DataArray(X * n, Y * n, Z * n);
        }

        public DataArray Calc_DivScalor(float n) {
            return new DataArray(X / n, Y / n, Z / n);
        }

        public static DataArray Calc_MulDA(DataArray A, DataArray B) {
            float x = A.X * B.X;
            float y = A.Y * B.Y;
            float z = A.Z * B.Z;
            return new DataArray(x, y, z);
        }

        public static DataArray Calc_MulDA(DataArray A) {
            float x = A.X * A.X;
            float y = A.Y * A.Y;
            float z = A.Z * A.Z;
            return new DataArray(x, y, z);
        }

        public static DataArray Calc_DivDA(DataArray Dividend, DataArray Divisor) {
            float x = Dividend.X / Divisor.X;
            float y = Dividend.Y / Divisor.Y;
            float z = Dividend.Z / Divisor.Z;
            return new DataArray(x, y, z);
        }

        public DataArray Calc_SquareRoot() {
            float x = (float)Math.Pow(X, 0.5);
            float y = (float)Math.Pow(Y, 0.5);
            float z = (float)Math.Pow(Z, 0.5);
            return new DataArray(x, y, z);
        }

        public double Calc_VectorSize() {
            double size = X * X + Y * Y + Z * Z;
            return Math.Pow(size, 1.0 / 2.0);
        }

        public static DataArray Calc_ManhattanDistance(DataArray A, DataArray B) {
            float x = Math.Abs(A.X - B.X);
            float y = Math.Abs(A.Y - B.Y);
            float z = Math.Abs(A.Z - B.Z);
            return new DataArray(x, y, z);
        }

        public static double Calc_EuclideanDistance(DataArray A, DataArray B) {
            DataArray dataArray = Calc_SubDA(A, B);
            dataArray.X = dataArray.X * dataArray.X;
            dataArray.Y = dataArray.Y * dataArray.Y;
            dataArray.Z = dataArray.Z * dataArray.Z;
            return Math.Sqrt(dataArray.sumDataArray());
        }        

        public static DataArray ScoreBarChangerFlow(DataArray A) {
            DataArray tempData = A.Calc_MulScalor(100);
            if(tempData.X > 20 || tempData.X == 0) {
                tempData.X = 20;
            }
            if(tempData.Y > 20 || tempData.Y == 0) {
                tempData.Y = 20;
            }
            if(tempData.Z > 20 || tempData.Z == 0) {
                tempData.Z = 20;
            }

            tempData.X = Math.Abs(tempData.X - 20);
            tempData.Y = Math.Abs(tempData.Y - 20);
            tempData.Z = Math.Abs(tempData.Z - 20);

            return tempData;
        }

        //出力
        public void WriteDA() {
            Console.WriteLine(X + "," + Y + "," + Z);
        }
    }
}
