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

        internal static int temp;
        internal Calculatable calc;
        //２回目以降の演算かを示します。２回目以降の演算であればTrueとなります。
        bool isTempEnterd = false;

        //入力値の表示位置を前回入力値の部分に変更します
        //入力値をintに変換し変数tempに代入します
        void BringInEntry()
        {
            try
            {
                PreviousResult.Content = Result.Content;
                if (isTempEnterd)
                {
                    ContinuousCalc();
                }
                isTempEnterd = true;
                MainWindow.temp = int.Parse((string)PreviousResult.Content);
            }
            catch (Exception ex)
            {
                WindowFunctions.ShowErrorMessage(ex);
            }
            Result.Content = "";
        }

        //
        internal void ContinuousCalc() 
        {
            PreviousResult.Content = calc.Calculate((string)PreviousResult.Content).ToString();
        }

         void ToBinary()
        {
            BinaryResult.Content = ((string)Result.Content == "") ?
                "bin:" : "bin:" + Convert.ToString(int.Parse((string)Result.Content), 2);
        }
        void ToHex()
        {
            HexaDecimalResult.Content = ((string)Result.Content == "") ?
                "hex:" : "hex:" + Convert.ToString(int.Parse((string)Result.Content), 16);
        }

        internal void ButtonInput(string s)
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

        internal void sum_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "+";
            WindowFunctions.Logging(s);
            BringInEntry();
            calc = new Sum();
        }

        internal void diff_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "-";
            WindowFunctions.Logging(s);
            BringInEntry();
            calc = new Diff();
        }

        internal void multip_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "×";
            WindowFunctions.Logging(s);
            BringInEntry();
            calc = new Multip();
        }

        internal void div_Click(object sender, RoutedEventArgs e)
        {
            string s = Result.Content + "÷";
            WindowFunctions.Logging(s);
            BringInEntry();
            calc = new Div();
        }

        internal void equal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string onePrevious = (string)Result.Content;
                Result.Content = calc.Calculate((string)Result.Content).ToString();
                ToBinary();
                ToHex();
                string s = onePrevious + " = " + Result.Content + Environment.NewLine;
                WindowFunctions.Logging(s);
                PreviousResult.Content = Result.Content;
            }
            catch (Exception ex)
            {
                Result.Content = "";
                WindowFunctions.ShowErrorMessage(ex);
            }
        }

        //cボタンクリックに対応するメソッドです。
        //すべての入力を取り消します
        //isTempEnterdも初期値に戻します
        private void c_Click(object sender, RoutedEventArgs e)
        {
            Result.Content = "";
            ToBinary();
            ToHex();
            PreviousResult.Content = "";
            isTempEnterd = false;
        }

        //テンキー入力とアプリ内キーを連動させるメソッドです
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                    ButtonInput((string)zero.Content);
                    break;
                case Key.NumPad1:
                    ButtonInput((string)one.Content);
                    break;
                case Key.NumPad2:
                    ButtonInput((string)two.Content);
                    break;
                case Key.NumPad3:
                    ButtonInput((string)three.Content);
                    break;
                case Key.NumPad4:
                    ButtonInput((string)four.Content);
                    break;
                case Key.NumPad5:
                    ButtonInput((string)five.Content);
                    break;
                case Key.NumPad6:
                    ButtonInput((string)six.Content);
                    break;
                case Key.NumPad7:
                    ButtonInput((string)seven.Content);
                    break;
                case Key.NumPad8:
                    ButtonInput((string)eight.Content);
                    break;
                case Key.NumPad9:
                    ButtonInput((string)nine.Content);
                    break;
                case Key.Add:
                    sum_Click(sender, e);
                    break;
                case Key.Subtract:
                    diff_Click(sender, e);
                    break;
                case Key.Multiply:
                    multip_Click(sender, e);
                    break;
                case Key.Divide:
                    div_Click(sender, e);
                    break;
                case Key.Enter:
                    equal_Click(sender, e);
                    break;
                default:
                    break;
            }

        }   
    }
}