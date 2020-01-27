using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrsstReportTables
{
    public class AihaBandProbabilities : TableEntryData
    {
        private ExposureMetricEstimates oe;
        private double[] chain;

        public AihaBandProbabilities(double[] chain, double oel)
        {
            this.chain = chain;
            oe = new ExposureMetricEstimates(oel);
        }

        public override string ToString()
        {
            return string.Join(" / ", new string[] {
                ToSignificantDigits(100 - (double) oe.OverExposureRisk(this.chain, 0.01), 2),
                ToSignificantDigits((double) oe.OverExposureRisk(this.chain, 0.01) - (double) oe.OverExposureRisk(this.chain, 0.1), 2),
                ToSignificantDigits((double) oe.OverExposureRisk(this.chain, 0.1) - (double) oe.OverExposureRisk(this.chain, 0.5), 2),
                ToSignificantDigits((double) oe.OverExposureRisk(this.chain, 0.5) - (double) oe.OverExposureRisk(this.chain), 2),
                ToSignificantDigits((double) oe.OverExposureRisk(this.chain), 2)
            });
        }
    }
}
