using System;
using Zygotine.WebExpo;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class TableC2Grid : ReportGrid
    {
        static TableC2Grid()
        {

        }

        public override string Description()
        {
            return "Exposure metrics point estimates and credible intervals for an example of Bayesian calculation for the normal model";
        }

        public override string[] ColumnHeadings()
        {
            return new string[] {
                "Parameter",
                "Point estimates and 90% credible interval"
            };
        }

        public override Tuple<string, ExposureMetricFunc>[] DefineContent()
        {
            MeasureList ml = new MeasureList(measures: new[] { 81, 79.5, 80.7, 78.1, 80.1, 74.8, 74.8, 79.8, 79.8 },
                                      oel: 85);
            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                            new SEGUninformativeModel(measures: ml, specificParams:
                                                UninformativeModelParameters.GetDefaults(logNormalDstrn: false)));
            this.Emes = new ExposureMetricEstimates[] { eme };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("Arithmetic mean", new ExposureMetricFunc(e => e.ArithMean())),
                Tuple.Create("Arithmetic standard deviation", new ExposureMetricFunc(e => e.ArithStanDev())),
                Tuple.Create("Exceedance Fraction (%)", new ExposureMetricFunc(e => e.ExceedanceFrac())),
                Tuple.Create("95th percentile", new ExposureMetricFunc(e => e.P95()))
            };
        }
    }
}
