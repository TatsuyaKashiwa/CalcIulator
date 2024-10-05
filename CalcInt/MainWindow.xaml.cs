using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalcInt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int _temp;

        void ButtonInput(string s) 
        {
            if (Result.Content is "0")
            {
                Result.Content = s;
            }
            else
            {
                Result.Content += s;
            }
        }

        void BringInEntry()
        {
            try
            {
                PreviousResult.Content = Result.Content;
                _temp = int.Parse((string)PreviousResult.Content);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("値の入力を忘れています");
            }
            Result.Content = "";
        }

        private void seven_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)seven.Content);
        }

        private void eight_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)eight.Content);
        }

        private void nine_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)nine.Content);
        }

        private void four_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)four.Content);
        }

        private void five_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)five.Content);
        }

        private void six_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)six.Content);
        }

        private void one_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)one.Content);
        }

        private void two_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)two.Content);
        }

        private void three_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)three.Content);
        }

        private void sign_Click(object sender, RoutedEventArgs e)
        {
            int result = int.Parse((string)Result.Content);
            Result.Content = (-result).ToString();
        }

        private void zero_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput((string)zero.Content);
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            Result.Content = "";
        }

        private void sum_Click(object sender, RoutedEventArgs e)
        {

        }

        private void diff_Click(object sender, RoutedEventArgs e)
        {

        }

        private void multip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void div_Click(object sender, RoutedEventArgs e)
        {

        }

        private void equal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}