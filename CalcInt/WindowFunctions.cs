using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace CalcInt
{
    public partial class WindowFunctions : Window
    {
        internal static void ShowErrorMessage(string s) 
        {
            MessageBox.Show(s + Environment.NewLine +
             "Cを押してリセットしていただくか" + Environment.NewLine +
             "再び数字を入力してください");
        
        }
        
    }
}
