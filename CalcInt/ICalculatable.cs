using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt;

/// <summary>
/// 多相性による各演算実現のためのクラス
/// </summary>
///<remarks>
/// =キーを押した際に演算種類に関わらず統一した記述ができるよう
///各演算に対応するクラスにて実装させてインスタンスの型により演算を区別するために
///インタフェースを作成した
///</remarks>
interface ICalculatable
{
    int Calculate(int x);
}
