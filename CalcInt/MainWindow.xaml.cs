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
using System.IO;

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

        internal static int temp;
        Calculatable calc;
        bool isTempEntred = false;

        void ToBinary()
        {
            BinaryResult.Content = ((string)Result.Content == "") ? 
                "bin:" : "bin:" + Convert.ToString(int.Parse((string)Result.Content),2);
        }
        void ToHex()
        {
            HexaDecimalResult.Content = ((string)Result.Content == "") ?
                "hex:" : "hex:" + Convert.ToString(int.Parse((string)Result.Content), 16);
        }

        void ButtonInput(string s) 
        {
            if (Result.Content is "0")
            {
                Result.Content = s;
                ToBinary();
                ToHex();
            }
            else
            {
                Result.Content += s;
                ToBinary();
                ToHex();
            }
        }

        //入力値の表示位置を前回入力値の部分に変更
        //入力値をintに変換し変数tempに代入
        void BringInEntry()
        {
            try
            {
                PreviousResult.Content = Result.Content;
                ContinuousCalc();
                isTempEntred = true;
                temp = int.Parse((string)PreviousResult.Content);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("値の入力を忘れています" + Environment.NewLine +
                    "Cを押してリセットしていただくか" + Environment.NewLine +
                    "再び数字を入力してください");
            }
            Result.Content = "";
        }

        void ContinuousCalc() 
        {
            if (isTempEntred) {
                PreviousResult.Content = calc.Calculate((string)PreviousResult.Content).ToString();
            }
        }

        /*log.txtは絶対パスを記載してください*/
        void Logging(string s) 
        {
            File.AppendAllText("log.txt", s);
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
            string s = Result.Content + "+";
            Logging(s);
            calc = new Sum();
            BringInEntry();
        }

        private void diff_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "-";
            Logging(s);
            calc = new Diff();
            BringInEntry();
        }

        private void multip_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "×";
            Logging(s);
            calc = new Multip();
            BringInEntry();
        }

        private void div_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "÷";
            Logging(s);
            calc = new Div();
            BringInEntry();
        }

        private void equal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Result.Content = calc.Calculate((string)Result.Content).ToString();
                ToBinary();
                ToHex();
                string s = PreviousResult.Content + " = " + Result.Content + Environment.NewLine;
                Logging(s);
                PreviousResult.Content = Result.Content;
            }
            catch (System.DivideByZeroException)
            {
                Result.Content = "";
                MessageBox.Show("ゼロ除算です！"+ Environment.NewLine +
                    "Cを押してリセットしていただくか" + Environment.NewLine +
                    "再び数字を入力してください");
            }
            catch (System.OverflowException)
            {
                Result.Content = "";
                MessageBox.Show("値が許容範囲を超えています" + Environment.NewLine +
                    "Cを押してリセットしていただくか" + Environment.NewLine +
                    "再び数字を入力してください");
            }
            catch (System.FormatException)
            {
                Result.Content = "";
                MessageBox.Show("値の入力を忘れています" + Environment.NewLine +
                    "Cを押してリセットしていただくか" + Environment.NewLine +
                    "再び数字を入力してください");
            }
        }

        private void c_Click(object sender, RoutedEventArgs e)
        {
            Result.Content = "";
            ToBinary();
            ToHex();
            PreviousResult.Content = "";
            isTempEntred = false;
        }
    }
}