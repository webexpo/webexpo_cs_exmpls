using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrsstReportTables
{
    class OverExposureRiskPercentage : TableEntryData
    {
        double Risk;

        public OverExposureRiskPercentage(double risk)
        {
            Risk = risk;
        }

        public override string ToString()
        {
            return string.Format("{0}%", ToSignificantDigits(Risk, 2));
        }
    }
}
