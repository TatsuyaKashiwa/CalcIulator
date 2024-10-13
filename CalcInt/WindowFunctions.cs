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
        static bool isTempEnterd = false;
        internal static void ShowErrorMessage(Exception e) 
        {
            switch (e)
            {
                case DivideByZeroException:
                    Logging("DivideByZeroException");
                    MessageBox.Show("ゼロ除算です！" + Environment.NewLine +
                  "値をリセットいたします。" );
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
                  "演算を選択してください。" );
                    break;
                default:
                    break;
            }

        }

        /*log.txtは絶対パスを記載してください*/
        internal static void Logging(string s)
        {
            File.AppendAllText(@".\log.txt", s);
        }

        internal static string oparatorReturn(Calculatable calc)
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
                return "÷6";
            }
            else
            {
                return "";
            }
        }
        

       
    }
}
