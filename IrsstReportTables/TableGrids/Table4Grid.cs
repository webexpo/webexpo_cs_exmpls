using System;
using System.Collections.Generic;
using Zygotine.WebExpo;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class Table4Grid : ReportGrid
    {
        static Table4Grid()
        {

        }

        public override string[] ColumnHeadings()
        {
            return new string[] {
                "Parameter",
                "Informedvar",
                "Uninformative",
                "Past.data"
            };
        }
        public override string Description()
        {
            return "Exposure metrics point estimates and credible intervals for 3 choices of prior distribution";
        }

    public override Tuple<string, ExposureMetricFunc>[] DefineContent(Dictionary<string, object> customVals)
        {
            MeasureList ml = new MeasureList(measures: new[] { 24.7, 64.1, 13.8, 43.7, 19.9, 133, 32.1, 15, 53.7 },
                                      oel: 100);

            SEGInformedVarModelParameters modelParams = SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true);
            this.OverwriteDefaults(modelParams, customVals);

            McmcParameters mcmcParams = new McmcParameters();
            this.OverwriteDefaults(mcmcParams, customVals);

            ExposureMetricEstimates emeInformed = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: modelParams,
                    mcmcParams : mcmcParams
                ));

            UninformativeModelParameters uninfModelParams = UninformativeModelParameters.GetDefaults(logNormalDstrn: true);
            this.OverwriteDefaults(modelParams, customVals);

            ExposureMetricEstimates emeUninformed = new ExposureMetricEstimates(
                new SEGUninformativeModel(
                    measures: ml,
                    specificParams: uninfModelParams,
                    mcmcParams: mcmcParams
                ));

            ExposureMetricEstimates emePdInformed = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: modelParams,
                    mcmcParams : mcmcParams,
                    pastDataSummary: new PastDataSummary(mean: Math.Log(5), sd: Math.Log(2.4), n: 5)
                ));

            this.Emes = new ExposureMetricEstimates[] { emeInformed, emeUninformed, emePdInformed };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM (90% CrI)", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD (90% CrI)", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance fraction (%)(90 % CrI)", new ExposureMetricFunc(e => e.ExceedanceFrac())),
                Tuple.Create("95th percentile (90% CrI)", new ExposureMetricFunc(e => e.P95())),
                Tuple.Create("Overexposure risk (%, P95)", new ExposureMetricFunc(e => e.OverExposureRisk())),
                Tuple.Create("AM (90% CrI)", new ExposureMetricFunc(e => e.ArithMean())),
                Tuple.Create("Overexposure risk (%, AM)", new ExposureMetricFunc(e => e.OverExposureRisk(false))),
            };
        }
    }
}
