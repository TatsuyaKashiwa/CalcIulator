using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalcInt
{

    public enum Nums
    {
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public int Eight { get; set; } = 8;

        /// <summary>
        /// 前回入力値
        /// </summary>
        /// <remarks>
        /// 演算は多相性を用いるため別クラスとして、前回入力値と引数で与えられた現在の入力値で行う
        ///前回入力値は各演算クラスで利用できる必要があるため
        ///前回入力値をstatic変数として受ける
        ///</remarks>
        internal static int temp;

        /// <summary>
        /// 演算用インスタンス
        /// </summary>
        /// <remarks>
        /// =押下時に演算キーに対応する演算がなされるようにするインスタンスを受ける変数
        /// 演算キー押下時に各演算に対応したインスタンスが代入される
        /// </remarks>
        internal Calculatable calc;

        /// <summary>
        /// 前回入力値フラグ
        /// </summary>
        /// <remarks>
        /// 前回入力値があるときには連続計算(演算子押下時に演算がなされる)可能としたい
        ///連続計算(前回入力値に対する演算)できるかを区別する必要があるため
        ///前回入力値が表示されるタイミングでtrueとなり、Cボタンや例外で前回入力値がリセットされる際にfalseになるフラグを導入した
        ///</remarks>
        private bool isTempEnterd = false;

        /// <summary>
        /// =押下フラグ
        /// </summary>
        /// <remarks>
        /// ＝連続押下と通常の演算で演算手法に差があり、＝が押される前後でオペランドの向きが変わる演算がある
        ///＝連続押下の演算であるかどうかの判別が必要となるため
        ///＝押下でtrueになり、演算子が押される(通常の演算に戻る)時falseに戻るフラグを導入した
        ///</remarks>
        internal static bool isEqualEntered = false;

        /// <summary>
        /// 演算キー押下フラグ
        /// </summary>
        /// <remarks>
        /// 演算キー過剰押下による演算がなされない演算子がログに記録されるのを防ぎたい
        ///演算キー連続押下時に演算子のログ記録を防ぐために
        ///演算キーが押されるとtrueになり、数字キー・＝キーが押されるとfalseに戻るフラグを導入した
        ///</remarks>
        private bool isOperatorEntered = false;

        /// <summary>
        /// BringInEntry()メソッドの例外の処理を行うメソッド
        /// </summary>
        /// <param name="ex">catchされた例外</param>
        /// <remarks>
        /// BringInEntry()メソッドの例外に対応する部分が煩雑となっていたため分離
        /// FormatException(入力忘れ)は前回入力値に値を収納し、
        /// 演算不可能な例外は値をリセット
        /// </remarks>
        private void EnterException(Exception ex)
        {
            WindowFunctions.ShowErrorMessage(ex);
            switch (ex)
            {
                case FormatException:
                    this.PreviousResult.Content = MainWindow.temp.ToString();
                    break;

                case DivideByZeroException:
                case OverflowException:
                    this.AllReset();
                    break;

                default:
                    break;

            }
        }

        /// <summary>
        /// 符号反転時の例外に対応するメソッド。
        /// </summary>
        /// <param name="ex">catchされた例外</param>
        /// <remarks>
        /// 符号反転時に発生しうる例外がFormatExceptionのみなので、
        /// 前回入力値があれば前回入力値に対して符号反転を行い
        /// 入力値がなければエラーメッセージを表示させるようにした。
        /// </remarks>
        private void SignException(Exception ex)
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

        /// <summary>
        /// 表示値を前回表示値に取り込むメソッド
        /// </summary>
        /// <exception cref="FormatException">値未入力時</exception>
        /// <exception cref="DivideByZeroException">ゼロ除算時(演算不可能)</exception>
        /// <exception cref="OverflowException">計算値がint32型の限界を超えた時</exception>
        /// <remarks>
        ///演算キー押下により現在の表示値の前回表示値へ移動し、現在表示値の表示をリセットする
        ///演算キー押下により呼び出されるため演算キー押下フラグはtrueになり
        ///処理後に=連続押下フラグはfalseとなる
        ///</remarks>
        void BringInEntryValue()
        {
            this.isOperatorEntered = true;
            try
            {
                this.EntryPrevious();
            }
            catch (Exception ex)
            {
                this.EnterException(ex);
            }
            isEqualEntered = false;
            this.Result.Content = "";
        }

        /// <summary>
        /// 値の取り込み・四則演算キー押下による演算
        /// </summary>
        /// <remarks>
        /// 現在入力値を前回入力値へ取り込み
        /// 前回入力値フラグがtrueであれば
        /// 演算キー押下により前回作成インスタンスに応じた演算を行う
        /// 前回入力値が取り込まれたため前回入力値フラグをtrueにして
        /// 演算用の仮置き値に前回入力値を取り込む
        /// </remarks>
        internal void EntryPrevious()
        {
            var previousResult = (string)Result.Content;
            this.PreviousResult.Content = previousResult;

            if (this.isTempEnterd)
            {
                this.PreviousResult.Content = this.calc.Calculate(previousResult).ToString();
            }

            this.isTempEnterd = true;

            MainWindow.temp = int.Parse(previousResult);
        }

        /// <summary>
        /// 全リセットメソッド
        /// </summary>
        /// <remarks>
        /// cボタン押下時と再計算不可の例外に対してはすべての入力を取り消すようにしたい
        ///共通動作なのでメソッド化を行うべきである
        ///前者はボタンの機能、後者は演算継続不可能なので値を初期値に戻す仕様にしたいため
        ///すべての入力を初期値に戻す
        ///</remarks>
        void AllReset()
        {
            this.Result.Content = "";
            this.calc = null;
            this.ShowBinary();
            this.ShowHex();
            this.PreviousResult.Content = "";
            this.isTempEnterd = false;
            isEqualEntered = false;
            this.isOperatorEntered = false;
            WindowFunctions.Logging(Environment.NewLine);
        }

        /// <summary>
        /// 2進表記部への表示メソッド
        /// </summary>
        /// <remarks>
        /// 現在入力値を2進表記に変換し2進表記部に表示
        ///下記の16進数と常にペアで運用されるため
        ///両者共通の操作(エラーメッセージ表示・現在入力値を0にする)は16進数のメソッドにまとめた。
        ///</remarks>
        void ShowBinary()
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
        /// <summary>
        /// 16進表記部への表示メソッド
        /// </summary>
        /// <remarks>
        ///現在入力値を16進表記に変換し16進表記部に表示
        ///OverflowExceptionは致命的でなく現在入力値を再入力すれば良いので
        ///通常とは別個の処理とした
        ///</remarks>
        void ShowHex()
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
                    MessageBox.Show("""
                                        値が許容範囲を超えています
                                        CEを押してリセットしていただくか
                                        再び数字を入力してください
                                    """);
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

        /// <summary>
        /// 数値表示メソッド
        /// </summary>
        /// <param name="s"></param>
        /// <remarks>
        /// ボタン(テンキー)入力した数字を表示部に反映させるメソッド
        ///int型を対象で0は0その者以外で先頭に立ってはならないので
        ///0のみが入力されていたら入力値で上書き、それ以外は左端から右へ入力値が押下ごとに表示されるようにした
        ///</remarks>
        private void InputButton(int s)
        {
            this.isOperatorEntered = false;
            int result = int.Parse((string)this.Result.Content);

            if (this.Result.Content is "0")
            {
                this.Result.Content =( s + 10 *result).ToString();
                this.ShowBinary();
                this.ShowHex();
            }
            else
            {
                this.Result.Content =( s + 10 *result).ToString();
                this.ShowBinary();
                this.ShowHex();
            }
        }

        /// <summary>
        /// 数字キー
        /// </summary>
        /// <remarks>
        ///数字キー押下に対応するメソッド
        ///数字キー押下時に各数字キーに対応する値を数値表示メソッドへ渡す
        ///</remarks>
        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)Nums.SEVEN);
        }

        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            //this.InputButton(this.btnEight.Content);
        }

        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Nine.Tag);
        }

        private void Four_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)Nums.FOUR);
        }

        private void Five_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Five.Tag);
        }

        private void Six_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Six.Tag);
        }

        private void One_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.One.Tag);
        }

        private void Two_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Two.Tag);
        }

        private void Three_Click(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Three.Tag);
        }

        private void Zero_Click(object sender, RoutedEventArgs e) 
        { 
            this.InputButton((int)this.Zero.Tag);
        }

        //機能キー
        /// <summary>
        /// +/-キーに対応するメソッド
        /// </summary>
        /// <exception cref="FormatException">現在表示値未入力の際に発生</exception>
        /// <remarks>
        /// 現在入力値を符号反転メソッドに渡すことで符号を反転
        /// 前回入力値がない状態だと押下時の現在表示値が記録されないため
        /// 現在表示値を記録して、値は前回表示値へ取り込むこととした。
        /// </remarks>
        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentResult = (string)this.Result.Content;

                if (this.PreviousResult.Content is "")
                {
                    WindowFunctions.Logging(currentResult);
                    this.PreviousResult.Content = currentResult;
                }

                this.Result.Content = WindowFunctions.TurnedResult(currentResult);
            }
            catch (Exception ex)
            {
                SignException(ex);
            }
        }



        /// <summary>
        /// CEボタンに対応するメソッド。
        /// </summary>
        /// <remarks>
        /// 表示値のみを0にして残りの値を保持した状態にするため
        ///表示値を0に変更するのみの記述とした
        /// </remarks>
        private void Clear_Click(object sender, RoutedEventArgs e) => this.Result.Content = "0";


        /// <summary>
        /// cボタンクリックに対応するメソッド。
        /// </summary>
        /// <remarks>
        /// すべての入力を取り消すため
        /// 押下に対応して
        /// allResetメソッドを呼び出す。
        /// </remarks>
        private void C_Click(object sender, RoutedEventArgs e) => this.AllReset();




        /// <summary>
        /// 四則演算キー
        /// </summary>
        /// <remarks>
        ///前回入力値がない場合は実際の演算が行われない準備段階となる
        ///演算される値の受け入れならびに演算の準備を行うため
        ///現在入力値を前回入力値として取り込み、演算に対応するインスタンス生成を行う
        ///演算キー押下フラグにより過剰に演算子文字が記録されないようにする
        ///</remarks>
        internal void Sum_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "+";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.calc = new Sum();
        }

        internal void Diff_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "-";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.calc = new Diff();
        }

        internal void Multip_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "×";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.calc = new Multip();
        }

        internal void Div_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "÷";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.calc = new Div();
        }

        /// <summary>
        /// ＝キー
        /// </summary>
        /// <exception cref="FormatException">現在表示値がないときに押下された時</exception>
        /// <exception cref="NullReferenceException">演算キー押下前に=が押下された時</exception>
        /// <exception cref="DivideByZeroException">ゼロ除算時(再計算不可)</exception>
        /// <exception cref="OverflowException">計算結果がint32型の範囲を超えた時(再計算不可)</exception>
        /// <remarks>
        /// =押下時に行う処理のメソッド
        /// 演算とその結果の表示、2進数・16進数表示の更新、ログ記録を行う
        /// 例外の処理はエラーメッセージの表示に加えて
        /// 再計算不可例外は値を全リセットして
        /// NullReferenceExceptionは表示値リセットすべきでない状況で発生するため
        /// 表示値リセットは行わない
        /// </remarks>
        internal void Equal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.isOperatorEntered = false;
                var onePrevious = (string)this.Result.Content;

                var result = this.calc.Calculate(onePrevious).ToString();
                this.Result.Content = result;

                this.ShowBinary();
                this.ShowHex();

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
                    this.AllReset();
                }
            }
        }


        /// <summary>
        /// テンキー入力とアプリ内キーを連動させるメソッド
        /// </summary>
        /// <remarks>
        ///テンキー入力とアプリ内キーが一対一で対応する必要があるため
        ///押下されたキーに対して対応するアプリ内キー押下時メソッドを呼び出すようにした
        /// </remarks>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                    this.Zero_Click(sender, e);
                    break;
                case Key.NumPad1:
                    this.One_Click(sender, e);
                    break;
                case Key.NumPad2:
                    this.Two_Click(sender, e);
                    break;
                case Key.NumPad3:
                    this.Three_Click(sender, e);
                    break;
                case Key.NumPad4:
                    this.Four_Click(sender, e);
                    break;
                case Key.NumPad5:
                    this.Five_Click(sender, e);
                    break;
                case Key.NumPad6:
                    this.Six_Click(sender, e);
                    break;
                case Key.NumPad7:
                    this.Seven_Click(sender, e);
                    break;
                case Key.NumPad8:
                    this.Eight_Click(sender, e);
                    break;
                case Key.NumPad9:
                    this.Nine_Click(sender, e);
                    break;
                case Key.Add:
                    this.Sum_Click(sender, e);
                    break;
                case Key.Subtract:
                    this.Diff_Click(sender, e);
                    break;
                case Key.Multiply:
                    this.Multip_Click(sender, e);
                    break;
                case Key.Divide:
                    this.Div_Click(sender, e);
                    break;
                case Key.Enter:
                    this.Equal_Click(sender, e);
                    break;
                default:
                    break;
            }

        }   
    }
}