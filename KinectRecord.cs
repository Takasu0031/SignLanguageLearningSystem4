using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SLLSFunction;

namespace SignLanguageLearningSystem4
{
    class KinectRecord {
        const int JPEGQUARITYLEVEL = 50; //JPEGの品質レベル

        KinectSensor kinect;
        FrameDescription colorFrameDesc;
        MultiSourceFrameReader multiReader;

        //画面表示用
        WriteableBitmap colorImage;
        WriteableBitmap cropImage;
        byte[] colorBuffer;
        int colorStride;
        Int32Rect colorRect;
        ColorImageFormat colorFormat = ColorImageFormat.Bgra;

        //骨格追跡用
        Body[] bodies;

        List<IReadOnlyDictionary<JointType, JointOrientation>> orient = new List<IReadOnlyDictionary<JointType, JointOrientation>>();
        List<IReadOnlyDictionary<JointType, Joint>> join = new List<IReadOnlyDictionary<JointType, Joint>>();
        List<Vector4> FloorClipList = new List<Vector4>();

        private bool on = false;
        private int frame = 0;

        public List<ColorSpacePoint> HandRightPoint { get; private set; } 
        public List<ColorSpacePoint> HandLeftPoint { get; private set; }

        public Image ImageColor;
        public Canvas CanvasBody;

        public KinectRecord() {
            HandRightPoint = new List<ColorSpacePoint>();
            HandLeftPoint = new List<ColorSpacePoint>();

            try {
                //初期設定
                kinect = KinectSensor.GetDefault();
                kinect.Open();
                colorFrameDesc = kinect.ColorFrameSource.CreateFrameDescription(colorFormat);
                bodies = new Body[kinect.BodyFrameSource.BodyCount];

                //フレームリーダーを開く(データの読み込み準備)
                multiReader = kinect.OpenMultiSourceFrameReader(
                    FrameSourceTypes.Color |
                    FrameSourceTypes.Body);
                multiReader.MultiSourceFrameArrived += MultiReader_MultiSourceFrameArrived;

                //画像枠作成
                colorImage = new WriteableBitmap(
                                    colorFrameDesc.Width, colorFrameDesc.Height,
                                    96, 96, PixelFormats.Bgra32, null); //外枠
                cropImage = new WriteableBitmap(
                                    colorFrameDesc.Width / 2, colorFrameDesc.Height / 2,
                                    96, 96, PixelFormats.Bgra32, null);
                colorBuffer = new byte[colorFrameDesc.LengthInPixels * colorFrameDesc.BytesPerPixel];　//[1920*1080*4]
                colorRect = new Int32Rect(0, 0, colorFrameDesc.Width, colorFrameDesc.Height);
                colorStride = colorFrameDesc.Width * (int)colorFrameDesc.BytesPerPixel; //1行の文字数(1920*4)   
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public void KinectClose() {
            if (multiReader != null) {
                multiReader.Dispose();
                multiReader = null;
            }
            if (kinect != null) {
                kinect.Close();
                colorFrameDesc = null;
            }
        }

        //複数のリーダーを同時に開く
        void MultiReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e) {
            var multiFrame = e.FrameReference.AcquireFrame();
            if (multiFrame == null) {
                return;
            }

            UpdateColorFrame(multiFrame);
            UpdateBodyFrame(multiFrame);
        }

        //カラーフレームを取得
        private void UpdateColorFrame(MultiSourceFrame e) {
            using (var colorFrame = e.ColorFrameReference.AcquireFrame()) {
                if (colorFrame == null) {
                    return;
                }
                colorFrame.CopyConvertedFrameDataToArray(colorBuffer, colorFormat);  //colorデータを取得
            }
        }

        //ボディフレームを取得
        private void UpdateBodyFrame(MultiSourceFrame e) {
            using (var bodyFrame = e.BodyFrameReference.AcquireFrame()) {
                if (bodyFrame == null) {
                    return;
                }
                DrawFrame();
                bodyFrame.GetAndRefreshBodyData(bodies);  //bodyデータを取得

                DrawBodyFrame(bodyFrame.FloorClipPlane);
            }
        }

        private void DrawFrame() {
            colorImage.WritePixels(colorRect, colorBuffer, colorStride, 0);
            cropImage = colorImage.Crop(480, 270, 960, 540); //colorImageの上下左右をトリミング
            ImageColor.Source = cropImage;
            if (on == true) {
                if (frame % MainWindow.Notepc == 0) {
                    //WriteableBitmap resizeImage;
                    //resizeImage = cropImage.Resize(IMAGEWIDTH, IMAGEHEIGHT, WriteableBitmapExtensions.Interpolation.Bilinear); //リサイズ
                    SaveImage(cropImage, @"image\image" + frame / MainWindow.Notepc + ".jpg");
                }
                frame++;
            }
        }

        //WriteableBitmapをjpgファイルとして保存
        private void SaveImage(WriteableBitmap bitmap, string fileName) {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = JPEGQUARITYLEVEL;
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
            }
        }

        private void DrawBodyFrame(Vector4 Floor) {
            CanvasBody.Children.Clear();

            //bodies[0-5]の中からisTrackedのものだけを対象とする
            int bodyNumber = bodies.Where(b => b.IsTracked).Count();
            if (bodyNumber > 1) {
                Console.WriteLine(bodyNumber + "人います，上手く処理できない場合があります。");
            }
            foreach (var body in bodies.Where(b => b.IsTracked)) {
                //IReadOnlyDictionary<JointType,Joint> joint = body.Joints;
                if (on == true) {
                    FloorClipList.Add(Floor);
                    orient.Add(body.JointOrientations);
                    join.Add(body.Joints);
                }

                if (body.Joints[JointType.HandRight].TrackingState != TrackingState.NotTracked) {
                    DrawEllipse(body.Joints[JointType.HandRight], 20, System.Windows.Media.Brushes.Red);
                }
                if (body.Joints[JointType.HandLeft].TrackingState != TrackingState.NotTracked) {
                    DrawEllipse(body.Joints[JointType.HandLeft], 20, System.Windows.Media.Brushes.Blue);
                }
            }
        }

        private void DrawEllipse(Joint joint, int R, System.Windows.Media.Brush brush) {
            var ellipse = new Ellipse() {
                Width = R,
                Height = R,
                Fill = brush,
            };

            //カメラ座標系をColor座標系に変換する
            var point = kinect.CoordinateMapper.MapCameraPointToColorSpace(joint.Position);
            if (point.X < 0 || point.Y < 0 || point.X > 1440 || point.Y > 810) {
                return;
            }

            //Color座標系で骨格位置を描画する
            Canvas.SetLeft(ellipse, (point.X - 480) * 2 / 3 - (R / 2));
            Canvas.SetTop(ellipse, (point.Y - 270) * 2 / 3 - (R / 2));
            CanvasBody.Children.Add(ellipse);
        }

        //3次元座標保存
        public PositionData SavePosition() {
            PositionData BeginerData = new PositionData();
            foreach (JointType jointType in Enum.GetValues(typeof(JointType))) {
                CameraSpacePoint[] SpacePoints = new CameraSpacePoint[orient.Count];
                for (int i = 0; i < join.Count(); i++) {
                    SpacePoints[i] = join[i][jointType].Position;
                }

                List<DataArray> posdatas = new List<DataArray>();
                if (jointType == JointType.HandRight) {
                    for (int i = 0; i < SpacePoints.Count(); i++) {
                        posdatas.Add(new DataArray(SpacePoints[i]));
                        var point = kinect.CoordinateMapper.MapCameraPointToColorSpace(SpacePoints[i]);
                        point.X = point.X - 480;
                        point.Y = point.Y - 270;
                        HandRightPoint.Add(point);
                    }
                    BeginerData.setNormalization(posdatas, 0);

                } else if (jointType == JointType.HandLeft) {
                    for (int i = 0; i < SpacePoints.Count(); i++) {
                        posdatas.Add(new DataArray(SpacePoints[i]));
                        var point = kinect.CoordinateMapper.MapCameraPointToColorSpace(SpacePoints[i]);
                        point.X = point.X - 480;
                        point.Y = point.Y - 270;
                        HandLeftPoint.Add(point);
                    }
                    BeginerData.setNormalization(posdatas, 1);

                } else if (jointType == JointType.SpineShoulder) {
                    for (int i = 0; i < SpacePoints.Count(); i++) {
                        posdatas.Add(new DataArray(SpacePoints[i]));
                    }
                    BeginerData.setNormalization(posdatas, 2);

                } else if (jointType == JointType.Head) {
                    for (int i = 0; i < SpacePoints.Count(); i++) {
                        posdatas.Add(new DataArray(SpacePoints[i]));
                    }
                    BeginerData.setNormalization(posdatas, 3);
                }

                string filename = MainWindow.saveDirectory + "\\P_" + jointType.ToString() + ".csv";                
                using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default)) {
                    foreach (var SpacePoint in SpacePoints) {
                        sw.WriteLine(SpacePoint.X + "," + SpacePoint.Y + "," + SpacePoint.Z);
                    }
                }

                switch (MainWindow.ScoreCount%5) {
                    case 0:
                        string filenameR5 = @"C: \Users\Takasu Yu\Desktop\BeginerModelData\A\" + MainWindow.Wordname + "_5"  + "\\P_" + jointType.ToString() + ".csv";
                        using (StreamWriter sw = new StreamWriter(filenameR5, false, Encoding.Default)) {
                            foreach (var SpacePoint in SpacePoints) {
                                sw.WriteLine(SpacePoint.X + "," + SpacePoint.Y + "," + SpacePoint.Z);
                            }
                        }
                        break;
                    case 1:
                        string filenameR1= @"C: \Users\Takasu Yu\Desktop\BeginerModelData\A\" + MainWindow.Wordname + "_1" + "\\P_" + jointType.ToString() + ".csv";
                        using (StreamWriter sw = new StreamWriter(filenameR1, false, Encoding.Default)) {
                            foreach (var SpacePoint in SpacePoints) {
                                sw.WriteLine(SpacePoint.X + "," + SpacePoint.Y + "," + SpacePoint.Z);
                            }
                        }
                        break;
                    case 2:
                        string filenameR2 = @"C: \Users\Takasu Yu\Desktop\BeginerModelData\A\" + MainWindow.Wordname + "_2" + "\\P_" + jointType.ToString() + ".csv";
                        using (StreamWriter sw = new StreamWriter(filenameR2, false, Encoding.Default)) {
                            foreach (var SpacePoint in SpacePoints) {
                                sw.WriteLine(SpacePoint.X + "," + SpacePoint.Y + "," + SpacePoint.Z);
                            }
                        }
                        break;
                    case 3:
                        string filenameR3 = @"C: \Users\Takasu Yu\Desktop\BeginerModelData\A\" + MainWindow.Wordname + "_3" + "\\P_" + jointType.ToString() + ".csv";
                        using (StreamWriter sw = new StreamWriter(filenameR3, false, Encoding.Default)) {
                            foreach (var SpacePoint in SpacePoints) {
                                sw.WriteLine(SpacePoint.X + "," + SpacePoint.Y + "," + SpacePoint.Z);
                            }
                        }
                        break;
                    case 4:
                        string filenameR4 = @"C: \Users\Takasu Yu\Desktop\BeginerModelData\A\" + MainWindow.Wordname + "_4" + "\\P_" + jointType.ToString() + ".csv";
                        using (StreamWriter sw = new StreamWriter(filenameR4, false, Encoding.Default)) {
                            foreach (var SpacePoint in SpacePoints) {
                                sw.WriteLine(SpacePoint.X + "," + SpacePoint.Y + "," + SpacePoint.Z);
                            }
                        }
                        break;

                }

            }
            return BeginerData;
        }

        //4次元座標保存
        public void SaveQuaternion() {
            //quaternionが設定されていない骨格
            JointType[] Qut_NODATA = new JointType[] {
                JointType.FootLeft,
                JointType.FootRight,
                JointType.HandTipLeft,
                JointType.HandTipRight,
                JointType.ThumbLeft,
                JointType.ThumbRight,
                JointType.Head
            };

        string filename;
            //床の傾き
            filename = MainWindow.saveDirectory + "\\Q_Floor.csv";
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default)) {
                for (int i = 0; i < FloorClipList.Count; i++) {
                    sw.WriteLine(FloorClipList[i].W + "," + FloorClipList[i].X + "," + FloorClipList[i].Y + "," + FloorClipList[i].Z);
                }
            }
            //骨格クォータニオン
            foreach (JointType jointType in Enum.GetValues(typeof(JointType))) {
                Vector4[] quaternions = new Vector4[orient.Count];
                for (int i = 0; i < quaternions.Count(); i++) {
                    quaternions[i] = orient[i][jointType].Orientation;
                }
                if (!Qut_NODATA.Contains(jointType)) {
                    filename = MainWindow.saveDirectory + "\\Q_" + jointType.ToString() + ".csv";
                    using (StreamWriter sw = new StreamWriter(filename, false, Encoding.Default)) {
                        foreach (var quaternion in quaternions) {
                            sw.WriteLine(quaternion.W + "," + quaternion.X + "," + quaternion.Y + "," + quaternion.Z);
                        }
                    }
                }
            }
        }

        public void RecordStart() {
            on = true;
        }

        public void RecordStop() {
            on = false;
        }
    }
}
