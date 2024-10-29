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

namespace CalcInt
{
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
                    MessageBox.Show("ゼロ除算です！" + Environment.NewLine +
                  "値をリセットいたします。");
                    break;
                case OverflowException:
                    Logging("OverflowException");
                    MessageBox.Show("値が許容範囲を超えています" + Environment.NewLine +
                 "値をリセットいたします。");
                    break;
                case FormatException:
                    MessageBox.Show("値の入力を忘れています" + Environment.NewLine +
                  "Cを押してリセットしていただくか" + Environment.NewLine +
                  "再び数字を入力してください");
                    break;
                case NullReferenceException:
                    MessageBox.Show("演算が選択されていません" + Environment.NewLine +
                  "演算を選択してください。");
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
        ///直前の四則演算キー押下により生成されたインスタンスに対応した演算子を記録させるため
        ///if文で各演算子に対応する分岐を書いた
        ///</remarks>
        internal static string OparatorReturn(Calculatable calc)
        {
            if (calc.GetType() == typeof(Sum))
            {
                return "+";
            }
            if (calc.GetType() == typeof(Diff))
            {
                return "-";
            }
            if (calc.GetType() == typeof(Multip))
            {
                return "×";
            }
            if (calc.GetType() == typeof(Div))
            {
                return "÷";
            }
            else
            {
                return "";
            }
        }       
    }
}
