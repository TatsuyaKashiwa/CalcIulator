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
        //演算は多相性を用いるため別クラスとして、前回入力値と引数で与えられた現在の入力値で行う
        //前回入力値は各演算クラスで利用できる必要があるため
        //前回入力値をstatic変数として受ける
        internal static int temp;
        
        internal Calculatable calc;
        
        //前回入力値があるときには連続計算(演算子押下時に演算がなされる)可能としたい
        //連続計算(前回入力値に対する演算)できるかを区別する必要があるため
        //前回入力値が表示されるタイミングでtrueとなり、Cボタンや例外で前回入力値がリセットされる際にfalseになるフラグを導入した
        bool isTempEnterd = false;
        
        //＝連続押下と通常の演算で演算手法に差があり、＝が押される前後でオペランドの向きが変わる演算がある
        //＝連続押下の演算であるかどうかの判別が必要となるため
        //＝押下でtrueになり、演算子が押される(通常の演算に戻る)時falseに戻るフラグを導入した
        static internal bool isEqualEntered = false;
        
        //演算キー過剰押下による演算がなされない演算子がログに記録されるのを防ぎたい
        //演算キー連続押下時に演算子のログ記録を防ぐために
        //演算キーが押されるとtrueになり、数字キー・＝キーが押されるとfalseに戻るフラグを導入した
        bool isOperatorEntered = false;
        

        //四則演算キー押下時に入力値を前回入力値表示へと移動し、計算される値を受け入れる状態とする必要がある
        //値を前回入力値に取り込んでから表示をリセットする必要があるため
        //前回入力値に現在値を代入→temp変数へその値を代入→現在値表示をリセットの順で処理を行う
        void BringInEntry()
        {
            try
            {
                PreviousResult.Content = Result.Content;
                //
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

        //連続演算時は＝押下時でなくて演算子押下のタイミングで演算を行いたい
        //演算関連の処理を分離するため
        //前回作成インスタンスに応じた演算を行い結果を前回表示値に表示する部分をメソッド化
        internal void ContinuousCalc() 
        {
            int calcResult = calc.Calculate((string)PreviousResult.Content);
            PreviousResult.Content = calcResult.ToString();

        }

        //cボタン押下時と再計算不可の例外に対してはすべての入力を取り消すようにしたい
        //共通動作なのでメソッド化を行うべきである
        //前者はボタンの機能、後者は演算継続不可能なので値を初期値に戻す仕様にしたいため
        //すべての入力を初期値に戻す
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
        //下記の16進数と常にペアで運用されるため
        //両者共通の操作(エラーメッセージ表示・現在入力値を0にする)は16進数のメソッドにまとめた。
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
                }
            }
        }
        //現在入力値を16進表記に変換し16進表記部に表示
        //OverflowExceptionは致命的でなく現在入力値を再入力すれば良いので
        //通常とは別個の処理とした
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

        //ボタン(テンキー)入力した数字を表示部に反映させるメソッド
        //int型を対象で0は0その者以外で先頭に立ってはならないので
        //0のみが入力されていたら入力値で上書き、それ以外は左端から右へ入力値が押下ごとに表示されるようにした
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
        //数値が入力される時点で演算子過剰押下防止のフラグを倒す必要があるため
        //演算子押下のフラグをfalseに戻して、押下した数字と表示値を対応させる
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
                //演算キーが初期状態から一回も押されていない状態では入力値は記録されない
                //当該条件時かつ+/-ボタン押下時に数値を記録するため
                //この条件分岐を導入した
                if (PreviousResult.Content is null)
                {
                    WindowFunctions.Logging((string)Result.Content);
                    PreviousResult.Content = Result.Content;
                }
                //符号反転のためint32型として表示値を扱いたい
                //現在表示値のint型への変換はキャストとPerseを伴い記述量が多く、さらに後記の最小値の処理の関係で
                //仮置変数(result変数)を導入した
                var result = int.Parse((string)Result.Content);
                Result.Content = (-result).ToString();
                //int32型最小値はint32型最大値に符号を付けたものに比べて1小さい
                //しかし上記符号反転では例外が発生せず何も動作が行われないため
                //そのことを示すメッセージボックスを表示させるようにした。
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
            //例外に対する捕捉を行う
            //発生しうる例外がFormatExceptionのみで、前回入力値があれば前回入力値に対して符号反転させたいので
            //例外の種類ではなく前回入力値があるかどうかで条件分けを行った
            catch (Exception ex)
            {
                if (PreviousResult.Content is "")
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

        //CEボタンに対応するメソッド。
        //表示値のみを0にして残りの値を保持した状態にするため
        //表示値を0に変更するのみの記述とした
        private void clear_Click(object sender, RoutedEventArgs e) => Result.Content = "0";
        

        //cボタンクリックに対応するメソッド。
        //すべての入力を取り消すため
        //allResetメソッドを呼び出す。
        private void c_Click(object sender, RoutedEventArgs e) => allReset();
        

        

        //四則演算キー 前回入力値がない場合は実際の演算が行われない準備段階となる
        //演算される値の受け入れならびに演算の準備を行うため
        //現在入力値を前回入力値として取り込み、演算に対応するインスタンス生成を行う
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
                //PreviousResult.Content = Result.Content;
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


        //テンキー入力とアプリ内キーを連動させるメソッド
        //テンキー入力とアプリ内キーが一対一で対応する必要があるため
        //押下されたキーに対して対応するアプリ内キー押下時メソッドを呼び出すようにした
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                    zero_Click(sender, e);
                    break;
                case Key.NumPad1:
                    one_Click(sender, e);
                    break;
                case Key.NumPad2:
                    two_Click(sender, e);
                    break;
                case Key.NumPad3:
                    three_Click(sender, e);
                    break;
                case Key.NumPad4:
                    four_Click(sender, e);
                    break;
                case Key.NumPad5:
                    five_Click(sender, e);
                    break;
                case Key.NumPad6:
                    six_Click(sender, e);
                    break;
                case Key.NumPad7:
                    seven_Click(sender, e);
                    break;
                case Key.NumPad8:
                    eight_Click(sender, e);
                    break;
                case Key.NumPad9:
                    nine_Click(sender, e);
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