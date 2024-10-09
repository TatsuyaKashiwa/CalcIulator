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

namespace CalcInt
{
    internal class WindowFunctions
    {
        static bool isTempEnterd = false;
        internal static void ShowErrorMessage(Exception e) 
        {
            if (e is DivideByZeroException)
            {
                MessageBox.Show("ゼロ除算です！" + Environment.NewLine +
                 "Cを押してリセットしていただくか" + Environment.NewLine +
                 "再び数字を入力してください");
            }
            if (e is OverflowException)
            {
                MessageBox.Show("値が許容範囲を超えています" + Environment.NewLine +
                 "Cを押してリセットしていただくか" + Environment.NewLine +
                 "再び数字を入力してください");
            }
            if (e is FormatException)
            {
                MessageBox.Show("値の入力を忘れています" + Environment.NewLine +
                 "Cを押してリセットしていただくか" + Environment.NewLine +
                 "再び数字を入力してください");
            }

        }

        /*log.txtは絶対パスを記載してください*/
        internal static void Logging(string s)
        {
            File.AppendAllText(@".\log.txt", s);
        }

        

       
    }
}
