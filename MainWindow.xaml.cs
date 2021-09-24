using System;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows.Navigation;
using SLLSFunction;

namespace SignLanguageLearningSystem4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //このウィンドウを画面の左上に表示する
            Top = 0;
            Left = 0;
            frame.Source = _uriList[0];
        }

        private NavigationService _navi;
        private List<Uri> _uriList = new List<Uri>() {
            new Uri("Page1.xaml",UriKind.Relative),
            new Uri("Page2.xaml",UriKind.Relative),
            new Uri("Page3.xaml",UriKind.Relative),
        };

        public static string Username { get; private set; }
        public static string Wordname { get; private set; }
        public static string Wordname_jp { get; private set; }
        public static int Mode { get; private set; }
        public static int CountdownNumber { get; private set; }
        public static int Notepc { get; private set; } = 1;
        public static List<string> wordnameList { get; private set; }
        public static int Compound_Mode { get; private set; }

        public static string saveDirectory { get; set; }
        public static int ScoreCount;
        public static List<ScoreTableData> ScoreValue = new List<ScoreTableData>();
        public static List<ScoreTableData> preScoreValue = new List<ScoreTableData>();


        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            Username = Properties.Settings.Default.user_Name;
            Mode = Properties.Settings.Default.Record_Mode;
            CountdownNumber = Properties.Settings.Default.CountDown_Time - 1;
            if (Properties.Settings.Default.Lightweight_Mode == true)
            {
                Notepc = 3;
            }

            Wordname = SaveConfigData.wordname;
            Wordname_jp = SaveConfigData.wordname_jp;
            wordnameList = SaveConfigData.wordnameList;
            Compound_Mode = wordnameList.Count;
            ScoreValue = new List<ScoreTableData>();
            preScoreValue = new List<ScoreTableData> {
                new ScoreTableData(),
                new ScoreTableData(),
                new ScoreTableData()
            };

            StartFlow();
            //_navi.Navigate(_uriList[0]);
        }

        private void StartFlow()
        {
            FileIO.FileOutputText(@"3DModel\Tag.txt", "0");

            //imageフォルダの中の画像の削除
            string directry = @"image";
            FileIO.DeleteDirectryFile(directry);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (Process pafter in Process.GetProcessesByName("DepthResult"))
            {
                pafter.Kill();
            }
        }

    }
}
