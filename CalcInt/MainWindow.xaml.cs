using System;
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
        //＝が押されたかを示します。
        static internal bool isEqualEntered = false;
        //演算子が押されたかを示します
        static internal bool isOperatorEntered = false;

        //四則演算キー押下時に入力値の表示位置を前回入力値の部分に変更します
        //入力値をintに変換し変数tempに代入します
        void BringInEntry()
        {
            string prevResult ="";
            if (PreviousResult.Content is not null)
            {
                prevResult = (string)PreviousResult.Content;
            }
            try
            {
                PreviousResult.Content = Result.Content;
                if (isTempEnterd && !isEqualEntered)
                {
                    ContinuousCalc();
                }
                isTempEnterd = true;
                MainWindow.temp = int.Parse((string)PreviousResult.Content);
            }
            catch (Exception ex)
            {
                WindowFunctions.ShowErrorMessage(ex);
                switch (ex) 
                {
                    case FormatException:
                        PreviousResult.Content = MainWindow.temp.ToString();
                        break;
                    case DivideByZeroException:
                    case OverflowException:
                        allReset();
                        break;
                    default:
                        break;

                }

            }
            Result.Content = "";
        }

        //メソッド実行時の前回入力値と現在入力値でボタンに応じた演算を行う
        internal void ContinuousCalc() 
        {
            int calcResult = checked(calc.Calculate((string)PreviousResult.Content));
            PreviousResult.Content = calcResult.ToString();

        }

        //cボタン押下時と再計算不可の例外に対して。
        //すべての入力を取り消すメソッドです
        //isTempEnterdも初期値に戻します
        void allReset()
        {
            Result.Content = "";
            calc = null;
            ToBinary();
            ToHex();
            PreviousResult.Content = "";
            isTempEnterd = false;
            isEqualEntered = false;
            isOperatorEntered = false;
            WindowFunctions.Logging(Environment.NewLine);
        }

        //現在入力値を2進表記に変換し2進表記部に表示
        void ToBinary()
        {
            try
            {
                BinaryResult.Content = ((string)Result.Content == "") ?
                 "bin:" : "bin:" + Convert.ToString(int.Parse((string)Result.Content), 2);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {
                    BinaryResult.Content = "bin:0";
                }
                else {
                    WindowFunctions.ShowErrorMessage(ex);
                    Result.Content = "0";
                }
            }
        }
        //現在入力値を16進表記に変換し16進表記部に表示
        void ToHex()
        {
            try
            {
                HexaDecimalResult.Content = ((string)Result.Content == "") ?
                "hex:" : "hex:" + Convert.ToString(int.Parse((string)Result.Content), 16);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {
                    MessageBox.Show("値が許容範囲を超えています" + Environment.NewLine +
                  "CEを押してリセットしていただくか" + Environment.NewLine +
                  "再び数字を入力してください");
                    HexaDecimalResult.Content = "hex:0";
                    Result.Content = "0";
                }
                else
                {
                    WindowFunctions.ShowErrorMessage(ex);
                    Result.Content = "0";
                }
            }
        }

        //ボタン(テンキー)入力した数字を表示部に反映させる
        //0のみが入力されていたら入力値で上書き
        //それ以外は右側に入力値を追記する
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

        //数字キー
        private void seven_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)seven.Content);
        }

        private void eight_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)eight.Content);
        }

        private void nine_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)nine.Content);
        }

        private void four_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)four.Content);
        }

        private void five_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)five.Content);
        }

        private void six_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)six.Content);
        }

        private void one_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)one.Content);
        }

        private void two_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)two.Content);
        }

        private void three_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)three.Content);
        }

        private void zero_Click(object sender, RoutedEventArgs e)
        {
            isOperatorEntered = false;
            ButtonInput((string)zero.Content);
        }

       //機能キー
       //+/-キーに対応するメソッドです。
        private void sign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PreviousResult.Content is null)
                {
                    WindowFunctions.Logging((string)Result.Content);
                    PreviousResult.Content = Result.Content;
                }
                var result = int.Parse((string)Result.Content);
                Result.Content = (-result).ToString();
                if (result == Int32.MinValue)
                {
                    MessageBox.Show("入力下限値です。" + Environment.NewLine
                        + "符号反転はできません" + Environment.NewLine
                        + "(減算・乗算も出来ません)");
                }
                else
                {
                    WindowFunctions.Logging("(+/-)");
                }
            }
            catch (Exception ex)
            {
                if (PreviousResult.Content is null)
                {
                    WindowFunctions.ShowErrorMessage(ex);
                }
                else 
                {
                    MainWindow.temp = -int.Parse((string)PreviousResult.Content);
                    PreviousResult.Content = MainWindow.temp.ToString();
                    WindowFunctions.Logging("(+/-)");
                }
                
            }
        }

       //CEボタンに対応するメソッドです。
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            Result.Content = "0";
        }

        //cボタンクリックに対応するメソッドです。
        //すべての入力を取り消します
        //isTempEnterdも初期値に戻します
        private void c_Click(object sender, RoutedEventArgs e)
        {
            allReset();
        }

        //四則演算キー
        internal void sum_Click(object sender, RoutedEventArgs e)
        {
            if (!isOperatorEntered)
            {
                var s = Result.Content + "+";
                WindowFunctions.Logging(s);
            }
            isOperatorEntered = true;
            BringInEntry();
            isEqualEntered = false;
            calc = new Sum();
        }

        internal void diff_Click(object sender, RoutedEventArgs e)
        {
            if (!isOperatorEntered)
            {
                var s = Result.Content + "-";
                WindowFunctions.Logging(s);
            }
            isOperatorEntered = true;
            BringInEntry();
            isEqualEntered = false;
            calc = new Diff();
        }

        internal void multip_Click(object sender, RoutedEventArgs e)
        {
            if (!isOperatorEntered)
            {
                var s = Result.Content + "×";
                WindowFunctions.Logging(s);
            }
            isOperatorEntered = true;
            BringInEntry();
            isEqualEntered = false;
            calc = new Multip();
        }

        internal void div_Click(object sender, RoutedEventArgs e)
        {
            if (!isOperatorEntered)
            {
                var s = Result.Content + "÷";
                WindowFunctions.Logging(s);
            }
            isOperatorEntered = true;
            BringInEntry();
            isEqualEntered = false;
            calc = new Div();
        }

        //＝キー
        internal void equal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isOperatorEntered = false;
                var onePrevious = (string)Result.Content;
                Result.Content = calc.Calculate(onePrevious).ToString();
                ToBinary();
                ToHex();
                if (isEqualEntered) 
                {
                    string oprt = WindowFunctions.oparatorReturn(calc);
                    var s = onePrevious +oprt + MainWindow.temp.ToString() + " = " + Result.Content + Environment.NewLine;
                    WindowFunctions.Logging(s);
                }
                else
                {
                    var s = onePrevious + " = " + Result.Content + Environment.NewLine;
                    WindowFunctions.Logging(s);
                    MainWindow.temp = int.Parse(onePrevious);
                }
                isEqualEntered = true;
                PreviousResult.Content = Result.Content;
            }
            catch (Exception ex)
            {
                WindowFunctions.ShowErrorMessage(ex);
                if (ex is not NullReferenceException)
                {
                    Result.Content = "";
                }
                if (ex is OverflowException || ex is DivideByZeroException)
                {
                    allReset();
                }
            }
        }

        //テンキー入力とアプリ内キーを連動させるメソッドです
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                    isOperatorEntered = false;
                    ButtonInput((string)zero.Content);
                    break;
                case Key.NumPad1:
                    isOperatorEntered = false;
                    ButtonInput((string)one.Content);
                    break;
                case Key.NumPad2:
                    isOperatorEntered = false;
                    ButtonInput((string)two.Content);
                    break;
                case Key.NumPad3:
                    isOperatorEntered = false;
                    ButtonInput((string)three.Content);
                    break;
                case Key.NumPad4:
                    isOperatorEntered = false;
                    ButtonInput((string)four.Content);
                    break;
                case Key.NumPad5:
                    isOperatorEntered = false;
                    ButtonInput((string)five.Content);
                    break;
                case Key.NumPad6:
                    isOperatorEntered = false;
                    ButtonInput((string)six.Content);
                    break;
                case Key.NumPad7:
                    isOperatorEntered = false;
                    ButtonInput((string)seven.Content);
                    break;
                case Key.NumPad8:
                    isOperatorEntered = false;
                    ButtonInput((string)eight.Content);
                    break;
                case Key.NumPad9:
                    isOperatorEntered = false;
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