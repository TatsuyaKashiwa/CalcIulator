using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt;

public class CalcViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ReactiveCommandSlim AddNum { get; }
    public ReactiveProperty<int> PreviousResult { get; set; } = new ReactiveProperty<int>(0);

    public ReactiveProperty<int> Result { get; set; } = new ReactiveProperty<int>(0);

    public ReactiveProperty<int> Zero { get; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> One { get; } = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> Two { get; } = new ReactiveProperty<int>(2);
    public ReactiveProperty<int> Three { get; } = new ReactiveProperty<int>(3);
    public ReactiveProperty<int> Four { get; } = new ReactiveProperty<int>(4);
    public ReactiveProperty<int> Five { get; } = new ReactiveProperty<int>(5);
    public ReactiveProperty<int> Six { get; } = new ReactiveProperty<int>(6);
    public ReactiveProperty<int> Seven { get; } = new ReactiveProperty<int>(7);
    public ReactiveProperty<int> Eight { get; } = new ReactiveProperty<int>(8);
    public ReactiveProperty<int> Nine { get; } = new ReactiveProperty<int>(9);

    public ReactivePropertySlim<bool> CanEnter { get; } = new ReactivePropertySlim<bool>(true);

    //TODO 引数が欲しい… => RSC<T>を使おう！
    public CalcViewModel()
    {
        AddNum = CanEnter.ToReactiveCommandSlim().WithSubscribe(() =>
        {
            this.Result.Value = this.Result.Value * 10 + 1;
        });
    }

}
