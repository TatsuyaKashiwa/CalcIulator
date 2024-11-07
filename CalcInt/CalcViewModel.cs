using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt
{
    public class CalcViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public CalcViewModel() { }

        public int PreviousResult{get; set;}=0;

        public int Result{get; set; }=0;

        public int Zero { get; set; } = 0;
        public int One { get; set; } = 1; 
        public int Two { get; set; } = 2;
        public int Three { get; set; } = 3; 
        public int Four { get; set; } = 4;
        public int Five { get; set; } = 5;
        public int Six { get; set; } = 6;
        public int Seven { get; set; } = 7;
        public int Eight { get; set; } = 8;
        public int Nine { get; set; } = 9;
    }
}
