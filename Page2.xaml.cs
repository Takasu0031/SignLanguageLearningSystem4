
using AForge.Video.VFW;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SLLSFunction;

namespace SignLanguageLearningSystem4
{
    /// <summary>
    /// Page2.xaml の相互作用ロジック
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
        }

        //固定値
        const int IMAGEWIDTH = 960; //画像縦サイズ
        const int IMAGEHEIGHT = 540; //画像横サイズ
        const int JPEGQUARITYLEVEL = 80; //JPEGの品質レベル
        const int JOINTNUMBER = 25;

        System.Drawing.Color RightHandLineColor = System.Drawing.Color.Red;
        System.Drawing.Color LeftHandLineColor = System.Drawing.Color.Blue;

        //ページ遷移用
        NavigationService _navigation;

        KinectRecord kinectRecord;

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);


        //その他のグローバル変数
        List<int> SeparateNumber = new List<int>();
        List<int> SeparateNumberMas = new List<int>();
        bool DoRecord = false;
        bool PlayNow = false;   //動画再生中か
        bool ModelOpen = false;
        bool Rsystem = false;
        Window window;
        ExcelDataUse dataSave = new ExcelDataUse();
        Page3 p3 = new Page3();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string filename;
            _navigation = NavigationService; //画面遷移用変数初期化   

            //キーボード入力に対応できるように
            window = Window.GetWindow(this);
            window.KeyUp += Page_KeyUp;

            Wordname.Text = "学習単語\n「" + MainWindow.Wordname_jp + "」";

            //保存ディレクトリの作成
            MainWindow.ScoreCount = dataSave.ScoreCount;
            MainWindow.saveDirectory = @"BeginerModelData\" + MainWindow.Username + @"\" + MainWindow.Wordname + "_" + dataSave.ScoreCount;
            if (!Directory.Exists(MainWindow.saveDirectory))
            {
                Directory.CreateDirectory(MainWindow.saveDirectory);
            }
            else
            {
                Console.WriteLine(MainWindow.saveDirectory + "は存在します");
                if (Directory.EnumerateFileSystemEntries(MainWindow.saveDirectory).Any())
                {
                    Console.WriteLine("既にデータが入っています。リセットします");
                    return;
                }
                else
                {
                    Console.WriteLine("空ディレクトリです");
                }
            }

            //kinectを開いて記録準備
            kinectRecord = new KinectRecord
            {
                ImageColor = ImageColor,
                CanvasBody = CanvasBody
            };

            //3Dモデル
            if (MainWindow.Mode != 3)
            {
                foreach (Process pafter in Process.GetProcessesByName("DepthResult"))
                {
                    pafter.Kill();
                }
            }

        }

        private void StartFlow()
        {
            if (DoRecord == false)
            {
                foreach (Process pafter in Process.GetProcessesByName("DepthResult"))
                {
                    pafter.Kill();
                }
                //GoodModel.Stop();/////////////////////////////////////////////////////////////これどうする？
                DoRecord = true;
                SleepAsync();

            }
            else if (StartButton.Content.ToString() == "Start")
            {
                SoundPlayer StartSound = new SoundPlayer(@"config\sound51.wav");
                StartSound.Play();
                StartSound.Dispose();
                StartButton.Content = "Stop";
                kinectRecord.RecordStop();
                SaveFunction();
            }
        }

        //ボタン入力後の処理(メインタスク)
        private async void SleepAsync()
        {
            //録画開始
            SoundPlayer CountDownSound = new SoundPlayer(@"config\sound71.wav");
            SoundPlayer StartSound = new SoundPlayer(@"config\sound51.wav");
            for (int i = MainWindow.CountdownNumber; i > 0; i--)
            {
                StartButton.Content = i;
                CountDownSound.Play();
                await Task.Delay(1000);
            }
            StartButton.Content = "Start";
            StartSound.Play();
            CountDownSound.Dispose();
            StartSound.Dispose();
            kinectRecord.RecordStart();

        }

        private void SaveFunction()
        {
            //ファイルに保存      
            PositionData BeginerData = kinectRecord.SavePosition();
            kinectRecord.SaveQuaternion();

            AviChanger();

            if (MainWindow.Mode >= 0)
            {
                //お手本
                string MasterFoldername = @"MasterModelData\" + MainWindow.Wordname;
                PositionData MasterData = new PositionData(MasterFoldername);

                int TotalFrame_Mas = MasterData.P_RightHand.OriginalData.Count;
                SeparateNumberMas.Add(0);
                SeparateNumberMas.Add((int)(TotalFrame_Mas * dataSave.SeparatePos));
                SeparateNumberMas.Add(TotalFrame_Mas);

                //学習者
                int TotalFrame = BeginerData.P_RightHand.OriginalData.Count;
                SeparateNumber.Add(0);
                SeparateNumber.Add((int)(TotalFrame * dataSave.SeparatePos));
                SeparateNumber.Add(TotalFrame);

                //DP照合
                DPMatchingFunction(BeginerData, MasterData);
                for (int i = MainWindow.Compound_Mode - 1; i > 0; i--)
                {
                    int[] SepNum = {
                        SeparateNumber[i - 1] - i + 1,
                        SeparateNumber[i] - i,
                        SeparateNumberMas[i - 1] - i + 1,
                        SeparateNumberMas[i] - i
                    };
                    DPMatchingFunction(BeginerData, MasterData, SepNum[0], SepNum[1], SepNum[2], SepNum[3], i);
                }

                MainWindow.ScoreValue[0].judge = "yes";

                for (int i = 1; i < MainWindow.Compound_Mode; i++)
                {
                    AviChanger_word(i, SeparateNumber[i - 1], SeparateNumber[i]);
                }
            }

            //3Dモデル再現時に必要なデータの保存
            Model3dDataSave();

            //Rシステム呼び出し
            if (Rsystem == true)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("test.bat", " / k dir");
                Process ps = Process.Start(processStartInfo);
                //string st = ps.StandardOutput.ReadToEnd();
                ps.WaitForExit();
                // string rt = ps.ExitCode.ToString();
                // Console.WriteLine("test : " + rt);
            }

            ClosePage2();

            p3.Wordname.Text = MainWindow.Wordname_jp;
            p3.Wordname.Text += " (" + MainWindow.ScoreCount + "回目)";
            _navigation.Navigate(p3);
        }

        private void Model3dDataSave()
        {
            //分割した3Dモデル再現時に必要なデータの保存(最初の3つはいらないかもしれない)
            string CommonData = MainWindow.Username + "," +
                MainWindow.Wordname + "," +
                MainWindow.ScoreCount + "," +
                Convert.ToInt32(dataSave.LeftHandCheck);

            string Beginer3dModelData = CommonData;
            foreach (int sepnum in SeparateNumber)
            {
                Beginer3dModelData += "," + sepnum;
            }
            FileIO.FileOutputText(@"3DModel\Wordname_B.txt", Beginer3dModelData);

            string Master3dModelData = CommonData;
            foreach (int sep in SeparateNumberMas)
            {
                Master3dModelData += "," + sep;
            }
            FileIO.FileOutputText(@"3DModel\Wordname_M.txt", Master3dModelData);

            //tag(0:全て, n(n>0):n番目の単語) 現状は2単語分割のみ想定
            FileIO.FileOutputText(@"3DModel\Tag.txt", "0");
        }

        private void DPMatchingFunction(PositionData BeginerData, PositionData MasterData)
        {
            float[] Distance = new float[2]; //R,L
            List<DataArray>[] MasDP = new List<DataArray>[2];
            List<DataArray>[] BegDP = new List<DataArray>[2];
            DataArray[] resultData = new DataArray[2];

            //データの正規化等の変更はPositionDataでやらせて，そのpublic変数を取ってくる形にしている

            //DP照合時に必要なデータに処理(お手本)
            MasDP[0] = MasterData.P_RightHand.NormarizationData;
            MasDP[1] = MasterData.P_LeftHand.NormarizationData;

            //DP照合時に必要なデータに処理(学習者)
            BegDP[0] = BeginerData.P_RightHand.NormarizationData;
            BegDP[1] = BeginerData.P_LeftHand.NormarizationData;

            //DP照合と結果
            resultData[0] = DPMatchingFlow.DPMatching(MasDP[0], BegDP[0]);
            Distance[0] = resultData[0].sumDataArray();
            p3.ScoreRight = DataArray.ScoreBarChangerFlow(resultData[0]);

            resultData[1] = DPMatchingFlow.DPMatching(MasDP[1], BegDP[1]);
            Distance[1] = resultData[1].sumDataArray();
            p3.ScoreLeft = DataArray.ScoreBarChangerFlow(resultData[1]);

            dataSave.SaveExcelData(resultData);
            MainWindow.ScoreValue.Insert(0, new ScoreTableData(resultData, MainWindow.preScoreValue[0]));
            MainWindow.preScoreValue[0] = MainWindow.ScoreValue[0];
        }

        private void DPMatchingFunction(PositionData BeginerData, PositionData MasterData, int startB, int stopB, int startM, int stopM, int i)
        {
            float[] Distance = new float[2]; //R,L
            List<DataArray>[] MasDP = new List<DataArray>[2];
            List<DataArray>[] BegDP = new List<DataArray>[2];
            DataArray[] resultData = new DataArray[2];

            //データの正規化等の変更はPositionDataでやらせて，そのpublic変数を取ってくる形にしている
            //DP照合時に必要なデータに処理(お手本)
            MasDP[0] = MasterData.P_RightHand.NormarizationData.GetRange(startM, stopM - startM);
            MasDP[1] = MasterData.P_LeftHand.NormarizationData.GetRange(startM, stopM - startM);

            //DP照合時に必要なデータに処理(学習者)
            BegDP[0] = BeginerData.P_RightHand.NormarizationData.GetRange(startB, stopB - startB);
            BegDP[1] = BeginerData.P_LeftHand.NormarizationData.GetRange(startB, stopB - startB);

            //DP照合と結果
            resultData[0] = DPMatchingFlow.DPMatching(MasDP[0], BegDP[0]);
            Distance[0] = resultData[0].sumDataArray();
            p3.ScoreRight = DataArray.ScoreBarChangerFlow(resultData[0]);

            resultData[1] = DPMatchingFlow.DPMatching(MasDP[1], BegDP[1]);
            Distance[1] = resultData[1].sumDataArray();
            p3.ScoreLeft = DataArray.ScoreBarChangerFlow(resultData[1]);

            dataSave.SaveExcelData(resultData);
            MainWindow.ScoreValue.Insert(1, new ScoreTableData(resultData, MainWindow.preScoreValue[i]));
            MainWindow.preScoreValue[i] = MainWindow.ScoreValue[1];
        }

        private void ClosePage2()
        {
            //動画の停止
            //GoodModel.Stop();
            //GoodModel.Close();

            //imageフォルダの中の画像の削除
            string directry = @"image";
            FileIO.DeleteDirectryFile(directry);

            //回数の保存
            dataSave.SaveScoreCount();

            //Kinectリソースの開放
            kinectRecord.KinectClose();

            //f3録画開始状態の停止
            window.KeyUp -= Page_KeyUp;
        }

        //複数のbmpファイルをつなげてavi形式で動画化
        private void AviChanger()
        {
            int count = 0;
            string bmpfilename = @"image\image" + count + ".jpg";
            string avifilename = MainWindow.saveDirectory + "\\Movie.avi";
            string avifilename_Line = MainWindow.saveDirectory + "\\MovieAddLine.avi";
            AVIWriter aviwriter = new AVIWriter();
            AVIWriter aviwriter_Line = new AVIWriter();
            System.Drawing.Pen pr = new System.Drawing.Pen(RightHandLineColor, 5);
            System.Drawing.Pen pl = new System.Drawing.Pen(LeftHandLineColor, 5);
            List<ColorSpacePoint> HandRightPoint = kinectRecord.HandRightPoint;
            List<ColorSpacePoint> HandLeftPoint = kinectRecord.HandLeftPoint;

            if (File.Exists(avifilename))
            {
                File.Delete(avifilename);
            }
            if (File.Exists(avifilename_Line))
            {
                File.Delete(avifilename_Line);
            }

            if (!Directory.Exists(MainWindow.saveDirectory + "\\image"))
            {
                Directory.CreateDirectory(MainWindow.saveDirectory + "\\image");
            }
            if (!Directory.Exists(MainWindow.saveDirectory + "\\image_Line"))
            {
                Directory.CreateDirectory(MainWindow.saveDirectory + "\\image_Line");
            }

            if (HandRightPoint.Count > 1)
            {
                aviwriter.Open(avifilename, IMAGEWIDTH, IMAGEHEIGHT);
                aviwriter_Line.Open(avifilename_Line, IMAGEWIDTH, IMAGEHEIGHT);
                while (File.Exists(bmpfilename))
                {
                    Bitmap bmp = new Bitmap(bmpfilename);
                    aviwriter.AddFrame(bmp);
                    bmp.Save(MainWindow.saveDirectory + "\\image\\image" + count + ".jpg", ImageFormat.Jpeg);

                    Graphics g = Graphics.FromImage(bmp);

                    for (int i = 0; i < count - 1; i++)
                    {
                        if (HandRightPoint[i].X >= 0 && HandRightPoint[i + 1].X >= 0)
                        {
                            g.DrawLine(pr, HandRightPoint[i].X, HandRightPoint[i].Y, HandRightPoint[i + 1].X, HandRightPoint[i + 1].Y);
                        }
                    }

                    for (int i = 0; i < count - 1; i++)
                    {
                        if (HandLeftPoint[i].X >= 0 && HandLeftPoint[i + 1].X >= 0)
                        {
                            g.DrawLine(pl, HandLeftPoint[i].X, HandLeftPoint[i].Y, HandLeftPoint[i + 1].X, HandLeftPoint[i + 1].Y);
                        }
                    }
                    g.Dispose();
                    aviwriter_Line.AddFrame(bmp);
                    bmp.Save(MainWindow.saveDirectory + "\\image_Line\\image" + count + ".jpg", ImageFormat.Jpeg);
                    bmp.Dispose();
                    count++;
                    bmpfilename = @"image\image" + count + ".jpg";
                }
            }
            aviwriter.Close();
            aviwriter_Line.Close();
        }

        private void AviChanger_word(int tag, int start, int stop)
        {
            string bmpfilename;
            string bmpfilename_Line;
            string avifilename = MainWindow.saveDirectory + "\\Movie" + tag + ".avi";
            string avifilename_Line = MainWindow.saveDirectory + "\\MovieAddLine" + tag + ".avi";
            AVIWriter aviwriter = new AVIWriter();
            AVIWriter aviwriter_Line = new AVIWriter();
            List<ColorSpacePoint> HandRightPoint = kinectRecord.HandRightPoint;
            List<ColorSpacePoint> HandLeftPoint = kinectRecord.HandLeftPoint;
            System.Drawing.Pen pr = new System.Drawing.Pen(RightHandLineColor, 5);
            System.Drawing.Pen pl = new System.Drawing.Pen(LeftHandLineColor, 5);
            Bitmap bmp;

            if (File.Exists(avifilename))
            {
                File.Delete(avifilename);
            }
            if (File.Exists(avifilename_Line))
            {
                File.Delete(avifilename_Line);
            }

            aviwriter.Open(avifilename, IMAGEWIDTH, IMAGEHEIGHT);
            aviwriter_Line.Open(avifilename_Line, IMAGEWIDTH, IMAGEHEIGHT);
            if (tag == 1)
            {
                for (int i = start; i < stop; i++)
                {
                    bmpfilename = MainWindow.saveDirectory + @"\image\image" + i + ".jpg";
                    bmp = new Bitmap(bmpfilename);
                    aviwriter.AddFrame(bmp);

                    bmpfilename_Line = MainWindow.saveDirectory + @"\image_Line\image" + i + ".jpg";
                    bmp = new Bitmap(bmpfilename_Line);
                    aviwriter_Line.AddFrame(bmp);
                    bmp.Dispose();
                }
            }
            else
            {
                int count = start;
                for (int i = start; i < stop; i++)
                {
                    bmpfilename = MainWindow.saveDirectory + @"\image\image" + i + ".jpg";
                    bmp = new Bitmap(bmpfilename);
                    aviwriter.AddFrame(bmp);

                    Graphics g = Graphics.FromImage(bmp);
                    for (int j = start; j < count - 1; j++)
                    {
                        if (HandRightPoint[j].X >= 0 && HandRightPoint[j + 1].X >= 0)
                        {
                            g.DrawLine(pr, HandRightPoint[j].X, HandRightPoint[j].Y, HandRightPoint[j + 1].X, HandRightPoint[j + 1].Y);
                        }
                    }

                    for (int j = start; j < count - 1; j++)
                    {
                        if (HandLeftPoint[j].X >= 0 && HandLeftPoint[j + 1].X >= 0)
                        {
                            g.DrawLine(pl, HandLeftPoint[j].X, HandLeftPoint[j].Y, HandLeftPoint[j + 1].X, HandLeftPoint[j + 1].Y);
                        }
                    }
                    g.Dispose();
                    count++;
                    aviwriter_Line.AddFrame(bmp);
                    bmp.Dispose();
                }
            }
            aviwriter.Close();
            aviwriter_Line.Close();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartFlow();
        }

        public void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                if (ModelOpen == true)
                {
                    ModelOpen = false;
                }
                else
                {
                    StartFlow();
                }
            }
        }
    }
}
