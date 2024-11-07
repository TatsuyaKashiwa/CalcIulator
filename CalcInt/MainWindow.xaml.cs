using System.Windows;
using System.Windows.Input;

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


        /// <summary>
        /// 前回入力値
        /// </summary>
        /// <remarks>
        /// 演算は多相性を用いるため別クラスとして、前回入力値と引数で与えられた現在の入力値で行う
        ///前回入力値は各演算クラスで利用できる必要があるため
        ///前回入力値をstatic変数として受ける
        ///</remarks>
        internal static int Temp;

        /// <summary>
        /// 演算用インスタンス
        /// </summary>
        /// <remarks>
        /// =押下時に演算キーに対応する演算がなされるようにするインスタンスを受ける変数
        /// 演算キー押下時に各演算に対応したインスタンスが代入される
        /// </remarks>
        internal ICalculatable Calc;

        /// <summary>
        /// 前回入力値フラグ
        /// </summary>
        /// <remarks>
        /// 前回入力値があるときには連続計算(演算子押下時に演算がなされる)可能としたい
        ///連続計算(前回入力値に対する演算)できるかを区別する必要があるため
        ///前回入力値が表示されるタイミングでtrueとなり、Cボタンや例外で前回入力値がリセットされる際にfalseになるフラグを導入した
        ///</remarks>
        private bool isTempEntered = false;

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
                    this.PreviousResult.Content = MainWindow.Temp.ToString();
                    break;

                case DivideByZeroException:
                case OverflowException:
                    this.ResetAll();
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
            if (this.PreviousResult.Content is null)
            {
                WindowFunctions.ShowErrorMessage(ex);
            }
            else
            {
                MainWindow.Temp = -(int)this.PreviousResult.Content;
                this.PreviousResult.Content = MainWindow.Temp;
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
                this.EntryPreviousValue();
            }
            catch (Exception ex)
            {
                this.EnterException(ex);
            }
            isEqualEntered = false;
            this.Result.Content = 0;
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
        internal void EntryPreviousValue()
        {
            var previousResult = (int)Result.Content;
            this.PreviousResult.Content = previousResult;

            if (this.isTempEntered)
            {
                this.PreviousResult.Content = this.Calc.Calculate(previousResult);
            }

            this.isTempEntered = true;

            MainWindow.Temp = previousResult;
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
        void ResetAll()
        {
            this.Result.Content = 0;
            this.Calc = null;
            this.ShowBinaryNotation();
            this.ShowHexadecimalNotation();
            this.PreviousResult.Content = 0;
            this.isTempEntered = false;
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
        void ShowBinaryNotation()
        {
            try
            {
                this.BinaryResult.Content = ((int)this.Result.Content == 0) ?
                 "bin:" : "bin:" + Convert.ToString((int)this.Result.Content, 2);
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
        void ShowHexadecimalNotation()
        {
            try
            {
                this.HexaDecimalResult.Content = ((int)this.Result.Content == 0) ?
                "hex:" : "hex:" + Convert.ToString((int)this.Result.Content, 16);
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
                    this.Result.Content = 0;
                }
                else
                {
                    WindowFunctions.ShowErrorMessage(ex);
                    this.Result.Content = 0;
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
            int result = (int)this.Result.Content;

            try
            {
                this.Result.Content = checked(s + 10 * result);
                this.ShowBinaryNotation();
                this.ShowHexadecimalNotation();
            }
            catch (Exception ex) 
            {
                WindowFunctions.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// 数字キー
        /// </summary>
        /// <remarks>
        ///数字キー押下に対応するメソッド
        ///数字キー押下時に各数字キーに対応する値を数値表示メソッドへ渡す
        ///</remarks>
        private void Seven_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Seven.Content);
        }

        private void Eight_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Eight.Content);
        }

        private void Nine_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Nine.Content);
        }

        private void Four_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Four.Content);
        }

        private void Five_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Five.Content);
        }

        private void Six_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Six.Content);
        }

        private void One_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.One.Content);
        }

        private void Two_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Two.Content);
        }

        private void Three_OnClick(object sender, RoutedEventArgs e)
        {
            this.InputButton((int)this.Three.Content);
        }

        private void Zero_OnClick(object sender, RoutedEventArgs e) 
        {
            this.InputButton((int)this.Zero.Content);
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
        private void Sign_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentResult = (int)this.Result.Content;

                if (this.PreviousResult.Content is 0)
                {
                    WindowFunctions.Logging(currentResult.ToString());
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
        private void CE_OnClick(object sender, RoutedEventArgs e) => this.Result.Content = 0;


        /// <summary>
        /// cボタンクリックに対応するメソッド。
        /// </summary>
        /// <remarks>
        /// すべての入力を取り消すため
        /// 押下に対応して
        /// allResetメソッドを呼び出す。
        /// </remarks>
        private void C_OnClick(object sender, RoutedEventArgs e) => this.ResetAll();




        /// <summary>
        /// 四則演算キー
        /// </summary>
        /// <remarks>
        ///前回入力値がない場合は実際の演算が行われない準備段階となる
        ///演算される値の受け入れならびに演算の準備を行うため
        ///現在入力値を前回入力値として取り込み、演算に対応するインスタンス生成を行う
        ///演算キー押下フラグにより過剰に演算子文字が記録されないようにする
        ///</remarks>
        internal void Sum_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "+";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.Calc = new Sum();
        }

        internal void Subtract_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "-";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.Calc = new Diff();
        }

        internal void Multiple_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "×";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.Calc = new Multip();
        }

        internal void Divide_OnClick(object sender, RoutedEventArgs e)
        {
            if (!this.isOperatorEntered)
            {
                var s = this.Result.Content + "÷";
                WindowFunctions.Logging(s);
            }

            this.BringInEntryValue();

            this.Calc = new Div();
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
        internal void Equal_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.isOperatorEntered = false;
                var onePrevious = (int)this.Result.Content;

                var result = this.Calc.Calculate(onePrevious);
                this.Result.Content = result;

                this.ShowBinaryNotation();
                this.ShowHexadecimalNotation();

                WindowFunctions.LoggingAtEqual(onePrevious, Calc, result);

                isEqualEntered = true;

                this.PreviousResult.Content = result;
            }
            catch (Exception ex)
            {
                WindowFunctions.ShowErrorMessage(ex);

                if (ex is not NullReferenceException)
                {
                    this.Result.Content = 0;
                }
                if (ex is OverflowException || ex is DivideByZeroException)
                {
                    this.ResetAll();
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
        private void Window_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                    this.Zero_OnClick(sender, e);
                    break;
                case Key.NumPad1:
                    this.One_OnClick(sender, e);
                    break;
                case Key.NumPad2:
                    this.Two_OnClick(sender, e);
                    break;
                case Key.NumPad3:
                    this.Three_OnClick(sender, e);
                    break;
                case Key.NumPad4:
                    this.Four_OnClick(sender, e);
                    break;
                case Key.NumPad5:
                    this.Five_OnClick(sender, e);
                    break;
                case Key.NumPad6:
                    this.Six_OnClick(sender, e);
                    break;
                case Key.NumPad7:
                    this.Seven_OnClick(sender, e);
                    break;
                case Key.NumPad8:
                    this.Eight_OnClick(sender, e);
                    break;
                case Key.NumPad9:
                    this.Nine_OnClick(sender, e);
                    break;
                case Key.Add:
                    this.Sum_OnClick(sender, e);
                    break;
                case Key.Subtract:
                    this.Subtract_OnClick(sender, e);
                    break;
                case Key.Multiply:
                    this.Multiple_OnClick(sender, e);
                    break;
                case Key.Divide:
                    this.Divide_OnClick(sender, e);
                    break;
                case Key.Enter:
                    this.Equal_OnClick(sender, e);
                    break;
                default:
                    break;
            }

        }   
    }
}