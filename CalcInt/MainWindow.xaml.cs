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

        /// <summary>
        /// BringInEntry()メソッドの例外の処理を行うメソッド
        /// </summary>
        /// <param name="ex">catchされた例外</param>
        /// <remarks>
        /// BringInEntry()メソッドの例外に対応する部分が煩雑となっていたため分離
        /// FormatException(入力忘れ)は前回入力値に値を収納し、
        /// 演算不可能な例外は値をリセット
        /// </remarks>
        private void CorrespondExceptionWhenEnter(Exception ex) 
        {
            WindowFunctions.ShowErrorMessage(ex);
            switch (ex)
            {
                case FormatException:
                    this.PreviousResult.Content = MainWindow.temp.ToString();
                    break;
                case DivideByZeroException:
                case OverflowException:
                    this.allReset();
                    break;
                default:
                    break;

            }
        }

        //四則演算キー押下時に入力値を前回入力値表示へと移動し、計算される値を受け入れる状態とする必要がある
        //値を前回入力値に取り込んでから表示をリセットする必要があるため
        //前回入力値に現在値を代入→temp変数へその値を代入→現在値表示をリセットの順で処理を行う
        void BringInEntry()
        {
            this.isOperatorEntered = true;
            try
            {
                 this.ContinuousCalc();
            }
            catch (Exception ex)
            {
                this.CorrespondExceptionWhenEnter(ex);
            }
            isEqualEntered = false;
            this.Result.Content = "";
        }

        //連続演算時は＝押下時でなくて演算子押下のタイミングで演算を行いたい
        //演算関連の処理を分離するため
        //前回作成インスタンスに応じた演算を行い結果を前回表示値に表示する部分をメソッド化
        internal void ContinuousCalc()
        {
            var previousResult = (string)Result.Content;
            this.PreviousResult.Content= previousResult;
            if (this.isTempEnterd && !isEqualEntered)
            {
                this.PreviousResult.Content = this.calc.Calculate((string)this.PreviousResult.Content).ToString();
            }
                this.isTempEnterd = true;
                MainWindow.temp = int.Parse(previousResult);
        }

        //cボタン押下時と再計算不可の例外に対してはすべての入力を取り消すようにしたい
        //共通動作なのでメソッド化を行うべきである
        //前者はボタンの機能、後者は演算継続不可能なので値を初期値に戻す仕様にしたいため
        //すべての入力を初期値に戻す
        void allReset()
        {
            this.Result.Content = "";
            this.calc = null;
            this.ToBinary();
            this.ToHex();
            this.PreviousResult.Content = "";
            this.isTempEnterd = false;
            isEqualEntered = false;
            this.isOperatorEntered = false;
            WindowFunctions.Logging(Environment.NewLine);
        }

        //現在入力値を2進表記に変換し2進表記部に表示
        //下記の16進数と常にペアで運用されるため
        //両者共通の操作(エラーメッセージ表示・現在入力値を0にする)は16進数のメソッドにまとめた。
        void ToBinary()
        {
            try
            {
                this.BinaryResult.Content = ((string)this.Result.Content == "") ?
                 "bin:" : "bin:" + Convert.ToString(int.Parse((string)this.Result.Content), 2);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {
                    this.BinaryResult.Content = "bin:0";
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
                this.HexaDecimalResult.Content = ((string)this.Result.Content == "") ?
                "hex:" : "hex:" + Convert.ToString(int.Parse((string)this.Result.Content), 16);
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {
                    MessageBox.Show(@"値が許容範囲を超えています
CEを押してリセットしていただくか
再び数字を入力してください");
                    this.HexaDecimalResult.Content = "hex:0";
                    this.Result.Content = "0";
                }
                else
                {
                    WindowFunctions.ShowErrorMessage(ex);
                    this.Result.Content = "0";
                }
            }
        }

        //ボタン(テンキー)入力した数字を表示部に反映させるメソッド
        //int型を対象で0は0その者以外で先頭に立ってはならないので
        //0のみが入力されていたら入力値で上書き、それ以外は左端から右へ入力値が押下ごとに表示されるようにした
        internal void ButtonInput(string s)
        {
            this.isOperatorEntered = false;
            if (this.Result.Content is "0")
            {
                this.Result.Content = s;
                this.ToBinary();
                this.ToHex();
            }
            else
            {
                this.Result.Content += s;
                this.ToBinary();
                this.ToHex();
            }
        }

        //数字キー
        //数値が入力される時点で演算子過剰押下防止のフラグを倒す必要があるため
        //演算子押下のフラグをfalseに戻して、押下した数字と表示値を対応させる
        private void seven_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.seven.Content);
        }

        private void eight_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.eight.Content);
        }

        private void nine_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.nine.Content);
        }

        private void four_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.four.Content);
        }

        private void five_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.five.Content);
        }

        private void six_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.six.Content);
        }

        private void one_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.one.Content);
        }

        private void two_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.two.Content);
        }

        private void three_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.three.Content);
        }

        private void zero_Click(object sender, RoutedEventArgs e)
        {
            //this.isOperatorEntered = false;
            this.ButtonInput((string)this.zero.Content);
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
                if (this.PreviousResult.Content is null)
                {
                    WindowFunctions.Logging((string)this.Result.Content);
                    this.PreviousResult.Content = this.Result.Content;
                }
                //符号反転のためint32型として表示値を扱いたい
                //現在表示値のint型への変換はキャストとPerseを伴い記述量が多く、さらに後記の最小値の処理の関係で
                //仮置変数(result変数)を導入した
                var result = int.Parse((string)this.Result.Content);
                this.Result.Content = (-result).ToString();
                //int32型最小値はint32型最大値に符号を付けたものに比べて1小さい
                //しかし上記符号反転では例外が発生せず何も動作が行われないため
                //そのことを示すメッセージボックスを表示させるようにした。
                if (result == Int32.MinValue)
                {
                    MessageBox.Show(@"入力下限値です。
符号反転はできません
(減算・乗算も出来ません)");
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
                if (this.PreviousResult.Content is "")
                {
                    WindowFunctions.ShowErrorMessage(ex);
                }
                else 
                {
                    MainWindow.temp = -int.Parse((string)this.PreviousResult.Content);
                    this.PreviousResult.Content = MainWindow.temp.ToString();
                    WindowFunctions.Logging("(+/-)");
                }
                
            }
        }

        //CEボタンに対応するメソッド。
        //表示値のみを0にして残りの値を保持した状態にするため
        //表示値を0に変更するのみの記述とした
        private void clear_Click(object sender, RoutedEventArgs e) => this.Result.Content = "0";
        

        //cボタンクリックに対応するメソッド。
        //すべての入力を取り消すため
        //allResetメソッドを呼び出す。
        private void c_Click(object sender, RoutedEventArgs e) => this.allReset();
        

        

        //四則演算キー 前回入力値がない場合は実際の演算が行われない準備段階となる
        //演算される値の受け入れならびに演算の準備を行うため
        //現在入力値を前回入力値として取り込み、演算に対応するインスタンス生成を行う
        internal void sum_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "+";
                WindowFunctions.Logging(s);
            }
            this.BringInEntry();
            this.calc = new Sum();
        }

        internal void diff_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "-";
                WindowFunctions.Logging(s);
            }
            this.BringInEntry();
            this.calc = new Diff();
        }

        internal void multip_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "×";
                WindowFunctions.Logging(s);
            }
            this.BringInEntry();
            this.calc = new Multip();
        }

        internal void div_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "÷";
                WindowFunctions.Logging(s);
            }
            this.BringInEntry();
            this.calc = new Div();
        }

        //＝キー
        internal void equal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.isOperatorEntered = false;
                var onePrevious = (string)this.Result.Content;
                var result = this.calc.Calculate(onePrevious).ToString();
                this.Result.Content = result;
                this.ToBinary();
                this.ToHex();
                WindowFunctions.LoggingAtEqual(onePrevious, calc, result);
                isEqualEntered = true;
                this.PreviousResult.Content = result;
            }
            catch (Exception ex)
            {
                WindowFunctions.ShowErrorMessage(ex);
                if (ex is not NullReferenceException)
                {
                    this.Result.Content = "";
                }
                if (ex is OverflowException || ex is DivideByZeroException)
                {
                    this.allReset();
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
                    this.zero_Click(sender, e);
                    break;
                case Key.NumPad1:
                    this.one_Click(sender, e);
                    break;
                case Key.NumPad2:
                    this.two_Click(sender, e);
                    break;
                case Key.NumPad3:
                    this.three_Click(sender, e);
                    break;
                case Key.NumPad4:
                    this.four_Click(sender, e);
                    break;
                case Key.NumPad5:
                    this.five_Click(sender, e);
                    break;
                case Key.NumPad6:
                    this.six_Click(sender, e);
                    break;
                case Key.NumPad7:
                    this.seven_Click(sender, e);
                    break;
                case Key.NumPad8:
                    this.eight_Click(sender, e);
                    break;
                case Key.NumPad9:
                    this.nine_Click(sender, e);
                    break;
                case Key.Add:
                    this.sum_Click(sender, e);
                    break;
                case Key.Subtract:
                    this.diff_Click(sender, e);
                    break;
                case Key.Multiply:
                    this.multip_Click(sender, e);
                    break;
                case Key.Divide:
                    this.div_Click(sender, e);
                    break;
                case Key.Enter:
                    this.equal_Click(sender, e);
                    break;
                default:
                    break;
            }

        }   
    }
}