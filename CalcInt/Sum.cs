using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt
{
    //Sumクラスのインスタンス(+押下時に作成)があるとき和の演算作用を行う
    //そのままでは例外が発生せず異常な計算値として表示されるため、
    //演算をcheckedで囲み、例外をMainWindow側でまとめて処理するためthrowした（他の演算も同様）
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

    //Diffクラスのインスタンス(-押下時に作成)があるとき差の演算の作用を行う
    //＝連続押下時は演算結果から前回入力値の差を求め、通常演算時は前回入力値から現在入力値の差を求める
    //すなわち前回入力値のオペランドの位置が＝連続押下時かどうかで変化するため
    //三項演算子でオペランドの配置を入れ替えた
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

    //multipクラスのインスタンス(×押下時に作成)があるとき積の演算作用を行う
    //そのままでは例外が発生せず異常な計算値として表示されるため、
    //演算をcheckedで囲み、例外をMainWindow側でまとめて処理するためthrowした
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

    //Divクラスのインスタンス(-押下時に作成)があるとき除算の作用を行う
    //すなわち前回入力値のオペランドの位置が＝連続押下時かどうかで変化するため
    //三項演算子でオペランドの配置を入れ替えた
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
