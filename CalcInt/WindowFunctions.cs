using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.IO;
using System.CodeDom;

namespace CalcInt;

internal class WindowFunctions
{
    /// <summary>
    /// 例外に対応するエラーメッセージダイアログを表示するメソッド
    /// </summary>
    /// <param name="e">catchされた例外</param>
    /// <remarks>
    /// 例外発生に伴うエラー表示の統一を行うべく
    ///一つの事象（例外を受け取る）に対し4個の分岐があるため
    ///switch文により処理を分岐させた
    ///</remarks>
    internal static void ShowErrorMessage(Exception e)
    {
        switch (e)
        {
            case DivideByZeroException:
                Logging("DivideByZeroException");
                MessageBox.Show("""
                                    ゼロ除算です！
                                    値をリセットいたします。
                                """);
                break;
            case OverflowException:
                Logging("OverflowException");
                MessageBox.Show("""
                                    値が許容範囲を超えています
                                    値をリセットいたします。
                                """);
                break;
            case FormatException:
                MessageBox.Show("""
                                    値の入力を忘れています
                                    Cを押してリセットしていただくか
                                    再び数字を入力してください
                                """);
                break;
            case NullReferenceException:
                MessageBox.Show("""
                                    演算が選択されていません
                                    演算を選択してください。
                                """);
                break;
            default:
                break;
        }

    }

    /*log.txtは絶対パスを記載してください*/
    /// <summary>
    /// 入力値をログファイルに記録するメソッド
    /// </summary>
    /// <param name="s">ログ記録対象の数値・演算子や例外</param>
    /// <remarks>
    /// 入力値をログとして記録するべく
    ///適切なタイミングで(演算子や＝押下時等)で記録できるよう
    ///入力値（と演算子）をまとめて引数で受け取り記録できるようにした
    ///</remarks>
    internal static void Logging(string s) => File.AppendAllText(@".\log.txt", s);


    /// <summary>
    ///インスタンスに対応する演算子文字を返すメソッド
    /// </summary>
    /// <param name="calc">現在作成されている演算インスタンス</param>
    /// <returns>演算に対応する演算子</returns>
    /// <remarks>
    /// 連続＝押下での演算時にログに演算子が記録されない事象を解決するため
    ///直前の四則演算キー押下により生成されたインスタンスに応じて
    ///分岐するswitch式で各演算子の文字を出力
    ///返すものは演算子記号のみのためswitch式で表現
    ///</remarks>
    private static string ReturnOparator(ICalculatable calc)
    {
        return calc switch 
        {
            Sum sum => "+",
            Substract diff =>"-",
            Multiple multip =>"×",
            Divide div =>"÷",
            _=>""
            
        };
    }

    /// <summary>
    /// =押下時のログ記録のメソッド
    /// </summary>
    /// <param name="onePrevious">最も直近の値</param>
    /// <param name="calc">現在の演算インスタンス</param>
    /// <param name="result">計算結果</param>
    /// <remarks>
    /// =押下時の直近の入力値・演算インスタンス・演算結果を引数として受け取り=押下による計算を記録する
    /// =連続押下による演算時は直近値(前回計算結果)と演算子と最初に=を押下した直前に入力した値を
    /// そうでない(=が最初に押された)時は、直近の入力値を
    /// それぞれ記録し、その後 = 計算結果 改行を記録する。
    /// </remarks>
    internal static void LoggingAtEqual(int onePrevious, ICalculatable calc,int result) 
    {
        if (MainWindow.isEqualEntered)
        {
            string oprt = ReturnOparator(calc);
            var s = onePrevious + oprt + MainWindow.Temp.ToString() + " = " + result + Environment.NewLine;
            Logging(s);
        }
        else
        {
            var s = onePrevious + " = " + result + Environment.NewLine;
            Logging(s);
            MainWindow.Temp = onePrevious;
        }
    }

    /// <summary>
    /// 表示値の符号を反転させるメソッド
    /// </summary>
    /// <param name="currentResult">現在の表示値</param>
    /// <returns>符号反転した表示値</returns>
    /// <remarks>
    /// <para>
    /// 現在の表示値を引数に取り、int32型へ変換し符号を反転させて
    /// string型へ変更した値を変換した値を返す
    /// 返した先で表示値プロパティに代入され、符号反転した値が表示される
    /// </para>
    /// <para>
    /// int32型最小値であれば例外にならない制限事項があるためメッセージを表示し
    /// それ以外は(+/-)ボタンが押されたことを示す(+/-)をログに記録する
    /// </para>
    /// </remarks>
    internal static int TurnedResult(int currentResult) 
    {

        if (currentResult == Int32.MinValue)
        {
            MessageBox.Show("""
                                入力下限値です。
                                符号反転はできません
                                (減算・乗算も出来ません)
                            """);
        }
        else
        {
            WindowFunctions.Logging("(+/-)");
        }

        return -currentResult;
    }

}
