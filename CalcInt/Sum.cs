using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt
{
    public class Sum : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            return (MainWindow.temp) + (int.Parse(s));
        }
    }

    public class Diff : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            return (MainWindow.temp) - (int.Parse(s));
        }
    }

    public class Multip : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            return (MainWindow.temp) * (int.Parse(s));
        }
    }

    public class Div : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            return (MainWindow.temp) / (int.Parse(s));
        }
    }
}
