using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeClock.Maui.Models
{
    internal struct PunchTypeToDecimalConverterParam
    {
        public PunchTypeToDecimalConverterParam(double valueIfBarCode, double defaultValue)
        {
            this.DefaultValue = defaultValue;
            this.ValueIfBarCode = valueIfBarCode;
        }
        public double ValueIfBarCode { get; init; }
        public double DefaultValue { get; init; }
    }
}
