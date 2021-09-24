using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using SLLSFunction;

namespace SignLanguageLearningSystem4
{
    /// <summary>
    /// Page3.xaml の相互作用ロジック
    /// </summary>
    public partial class Page3 : Page
    {
        public Page3()
        {
            InitializeComponent();
        }

        NavigationService _navigation;
        private bool bStop = false;
        private bool gStop = false;
        Window window;

        public DataArray ScoreRight;
        public DataArray ScoreLeft;
        public new int Tag = 0;
        private ObservableCollection<ScoreTableData> ScoreList = new ObservableCollection<ScoreTableData>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.KeyUp += Page_KeyUp;

            _navigation = NavigationService;

            //軌跡付き動画で表示するかしないか
            string Beg_filename;
            string Mas_filename;
            Uri Beg_uri;
            Uri Mas_uri;
            if (MainWindow.Mode == 0)
            {
                //Beg_filename = MainWindow.saveDirectory + "\\Movie" + Tag +".avi";
                Beg_filename = MainWindow.saveDirectory + "\\Movie.avi";
                //Mas_filename = @"MasterModelData\" + MainWindow.Wordname + @"\Movie" + Tag + ".avi";
                Mas_filename = @"MasterModelData\" + MainWindow.Wordname + @"\Movie.avi";
            }
            else
            {
                //Beg_filename = MainWindow.saveDirectory + "\\MovieAddLine" + Tag + ".avi";
                Beg_filename = MainWindow.saveDirectory + "\\MovieAddLine.avi";
                //Mas_filename = @"MasterModelData\" + MainWindow.Wordname + @"\MovieAddLine" + Tag + ".avi";
                Mas_filename = @"MasterModelData\" + MainWindow.Wordname + @"\MovieAddLine.avi";
            }
            Beg_uri = new Uri(Beg_filename, UriKind.RelativeOrAbsolute);
            Mas_uri = new Uri(Mas_filename, UriKind.RelativeOrAbsolute);
            BeginerModel.Source = Beg_uri;
            GoodModel.Source = Mas_uri;
            BeginerModel.Play();
            GoodModel.Play();

            ////スコア表示
            if (MainWindow.Mode <= 1)
            {
                ScoreViewer.Visibility = Visibility.Hidden;
            }
            else
            {
                int i = 0;
                foreach (ScoreTableData std in MainWindow.ScoreValue)
                {
                    if (i < MainWindow.Compound_Mode)
                    {
                        std.Wordname = MainWindow.wordnameList[i];
                    }
                    if (i % 3 == 0)
                    {
                        ScoreList.Add(std);
                    }
                    if (i % 3 == 2)
                    {
                        ScoreList.Add(new ScoreTableData());
                    }
                    i++;
                }


                ScoreViewer.DataContext = ScoreList;
                ScoreViewer.FontSize = 16;
            }

            //3Dモデル
            if (MainWindow.Mode == 0 || MainWindow.Mode == 1 || MainWindow.Mode == 2)
            {
                foreach (Process pafter in Process.GetProcessesByName("DepthResult"))
                {
                    pafter.Kill();
                }
            }
        }

        public void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                TurnBack();
            }
        }

        private void TurnBack()
        {
            BeginerModel.Clock = null;
            BeginerModel.Close();
            GoodModel.Clock = null;
            GoodModel.Close();

            foreach (Process pafter in Process.GetProcessesByName("DepthResult"))
            {
                pafter.Kill();
            }
            window.KeyUp -= Page_KeyUp;

            foreach (var sc in MainWindow.ScoreValue)
            {
                if (sc.judge == "yes")
                {
                    sc.judge = "no";
                }
            }
            //MainWindow.ScoreValue.Clear();
            Page1 page1 = new Page1();
            _navigation.Navigate(page1);
        }

        private void BeginerModel_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
        private void GoodModel_MediaEnded(object sender, RoutedEventArgs e)
        {

        }
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MfpaButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
