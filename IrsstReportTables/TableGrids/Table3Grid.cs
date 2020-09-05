using System;
using System.Collections.Generic;
using Zygotine.WebExpo;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class Table3Grid : ReportGrid
    {
        static Table3Grid()
        {

        }

        public override string Description()
        {
            return "Exposure metrics point estimates and credible intervals for an example of Bayesian calculation for the lognormal model";
        }

        public override string[] ColumnHeadings()
        {
            return new string[] {
                "Parameter",
                "Point estimates and 90% credible interval"
            };
        }

        public override Tuple<string, ExposureMetricFunc>[] DefineContent(Dictionary<string,object> customVals)
        {
            MeasureList ml = new MeasureList(measures: new[] { 24.7, 64.1, 13.8, 43.7, 19.9, 133, 32.1, 15, 53.7 },
                                      oel: 100);
            SEGInformedVarModelParameters modelParams = SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true);
            this.OverwriteDefaults(modelParams, customVals);

            McmcParameters mcmcParams = new McmcParameters();
            this.OverwriteDefaults(mcmcParams, customVals);

            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                            new SEGInformedVarModel(measures: ml, specificParams: modelParams, mcmcParams : mcmcParams));
            this.Emes = new ExposureMetricEstimates[] { eme };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance Fraction (%)", new ExposureMetricFunc(e => e.ExceedanceFrac(true))),
                Tuple.Create("95th percentile", new ExposureMetricFunc(e => e.P95(true))),
                Tuple.Create("AIHA band probabilities in % (95th percentile)", new ExposureMetricFunc(e => e.BandProbabilities())),
                Tuple.Create("Arithmetic mean", new ExposureMetricFunc(e => e.ArithMean(true))),
                Tuple.Create("AIHA band probabilities in % (AM)", new ExposureMetricFunc(e => e.BandProbabilities(false)))
            };
        }
    }
}
