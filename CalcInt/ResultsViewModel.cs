using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt
{
    internal class ResultsViewModel : INotifyPropertyChanged
    {
        private ResultModel rM;
        public ResultsViewModel() 
        {
            ResultModel rM = new ResultModel();
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string BinaryResultContent 
        { get {return rM.BinaryResultContent; }
            set
            {
                if (rM.BinaryResultContent != value) 
                { 
                    rM.BinaryResultContent = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string HexadecimalResultContent
        {
            get { return rM.HexadecimalResultContent; }
            set
            {
                if (rM.HexadecimalResultContent != value)
                {
                    rM.HexadecimalResultContent = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string PreviousResultContent
        {
            get { return rM.PreviousResultContent; }
            set
            {
                if (rM.PreviousResultContent != value)
                {
                    rM.PreviousResultContent = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ResultContent
        {
            get { return rM.ResultContent; }
            set
            {
                if (rM.ResultContent != value)
                {
                    rM.ResultContent = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
