using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ページ左側単語リストの作成
            WordList.Items.Clear();
           foreach (string wordname_jp in WordIndex.WordnameList)
            {
                WordList.Items.Add(wordname_jp);
            }
        }

        private void ResetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //"回数のリセット"
        }

        private void NameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //"名前変更"
        }

        private void StatisticsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //"統計分析"
        }

        private void WordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //"単語選択"
            //選択した単語名とindexを保存
            string wordname_jp = Convert.ToString(WordList.SelectedItem);
            int index = WordIndex.WordnamejpToIndex(wordname_jp);

            //動画の設定
            string moviefilename = "MasterModelData\\" + WordIndex.IndexToWordname(index) + "\\movie.avi";
            var uri = new Uri(moviefilename, UriKind.RelativeOrAbsolute);
            MasterMovie.Source = uri;
            MasterMovie.Play();

            //説明部分の設定
            Wordname_XAML.Text = index.ToString() + ". " + wordname_jp;
            Explanation_XAML.Text = "";
            string exp = WordIndex.IndexToExplanation(index);
            Explanation_XAML.Text += exp;


            //学習時に使うデータとして保存
            SaveConfigData.setWordname_jp(wordname_jp);
            SaveConfigData.setWordname(WordIndex.IndexToWordname(index));
            //SaveConfigData.setWordnameList(WordIndex.IndexToWordnameList(index));
        }

        private void MasterMovie_MediaEnded(object sender, RoutedEventArgs e)
        {
            //"Master動画再生終了"
            MasterMovie.Position = TimeSpan.FromMilliseconds(1);
            MasterMovie.Play();
        }

        private void LearnButton_Click(object sender, RoutedEventArgs e)
        {
            //"学習開始"
        }
    }
}
