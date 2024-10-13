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
            try
            {
                return checked((MainWindow.temp) + (int.Parse(s)));
            }
            catch(Exception) 
            {
                throw;
            }
        }
    }

    public class Diff : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            try
            {
                return checked(MainWindow.isEqualEntered ?
                (int.Parse(s)) - (MainWindow.temp) : (MainWindow.temp) - (int.Parse(s)));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class Multip : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            try
            {
                return checked((MainWindow.temp) * (int.Parse(s)));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class Div : Calculatable
    {
        int Calculatable.Calculate(string s)
        {
            try
            {
                return checked(MainWindow.isEqualEntered ?
                (int.Parse(s)) / (MainWindow.temp) : (MainWindow.temp) / (int.Parse(s)));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
