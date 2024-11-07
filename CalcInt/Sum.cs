using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt;

/// <summary>
/// 加算と加算に対応するインスタンス生成のためクラス
/// </summary>
///<remarks>
///Sumクラスのインスタンス(+押下時に作成)があるとき和の演算作用を行う
///そのままでは例外が発生せず異常な計算値として表示されるため、
///演算をcheckedで囲み、例外をMainWindow側でまとめて処理するためthrowした（他の演算も同様）
///</remarks>
public class Sum : ICalculatable
{
    int ICalculatable.Calculate(int x)
    {
        try
        {
            return checked(MainWindow.Temp + x);
        }
        catch(Exception) 
        {
            throw;
        }
    }
}

/// <summary>
/// 減算と減算に対応するインスタンス生成のためのクラス
/// </summary>
/// <remarks>
/// Diffクラスのインスタンス(-押下時に作成)があるとき差の演算の作用を行う
///＝連続押下時は演算結果から前回入力値の差を求め、通常演算時は前回入力値から現在入力値の差を求める
///すなわち前回入力値のオペランドの位置が＝連続押下時かどうかで変化するため
///三項演算子でオペランドの配置を入れ替えた
///</remarks>
public class Substract : ICalculatable
{
    int ICalculatable.Calculate(int x)
    {
        try
        {
            return checked(MainWindow.isEqualEntered 
                            ? x - MainWindow.Temp : MainWindow.Temp - x);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

/// <summary>
///積算と積算に対応するインスタンス生成のためのクラス
/// </summary>
/// <remarks>
/// multipクラスのインスタンス(×押下時に作成)があるとき積の演算作用を行う
/// 演算をcheckedで囲み、例外をMainWindow側でまとめて処理するためthrowした
/// </remarks>
public class Multiple : ICalculatable
{
    int ICalculatable.Calculate(int x)
    {
        try
        {
            return checked(MainWindow.Temp * x);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

/// <summary>
/// 除算と除算に対応するインスタンス生成のためのクラス
/// </summary>
/// <remarks>
/// Divクラスのインスタンス(-押下時に作成)があるとき除算の作用を行う
///すなわち前回入力値のオペランドの位置が＝連続押下時かどうかで変化するため
///三項演算子でオペランドの配置を入れ替えた
///</remarks>
public class Divide : ICalculatable
{
    int ICalculatable.Calculate(int x)
    {
        try
        {
            return checked(MainWindow.isEqualEntered 
                            ? x / MainWindow.Temp : MainWindow.Temp / x);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
