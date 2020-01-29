using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zygotine.Statistics;

namespace IrsstReportTables
{
    public class PointEstimateWInterval : TableEntryData
    {
        double Estimate { get; set; }
        double IntervalLowerBound { get; set; }
        double IntervalUpperBound { get; set; }
        double CredibleIntervalProb { get; set; }
        string OverExpoInfo { get; set; }

        public PointEstimateWInterval(double[] chain, double cred = 90, double? overexpo = null) : base()
        {
            Estimate = GeomMean(chain);
            CredibleIntervalProb = cred;
            OverExpoInfo = overexpo != null ? String.Format(nfi, " Overexposure risk: {0}%", ToSignificantDigits((double) overexpo, 2)) : "";
            IntervalLowerBound = new Quantile((100 - CredibleIntervalProb) / 200).Compute(chain)[0];
            IntervalUpperBound = new Quantile(1 - (100 - CredibleIntervalProb) / 200).Compute(chain)[0];
        }

        public override string ToString()
        {
            return String.Format(this.nfi, "{0} [{1} - {2}]{3}", ToSignificantDigits(Estimate), ToSignificantDigits(IntervalLowerBound), ToSignificantDigits(IntervalUpperBound), OverExpoInfo);
        }

        private double GeomMean(double[] chain)
        {
            Array.Sort(chain);
            double mean = chain[chain.Length / 2];
            return mean;
        }
    }
}