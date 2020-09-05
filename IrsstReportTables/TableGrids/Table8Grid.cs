using System;
using Zygotine.WebExpo;
using System.Collections.Generic;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class Table8Grid : ReportGrid
    {
        static Table8Grid()
        {

        }

        public Table8Grid() : base()
        {

        }

        public override string[] ColumnHeadings()
        {
            return new string[] {
                "Parameter",
                "Point estimates and 90% credible interval"
            };
        }
        public override string Description()
        {
            return "Exposure metrics point estimates and credible intervals for an example of Bayesian calculation for the lognormal model (between-worker difference analyses) with realistic sample size";
        }

        public override Tuple<string, ExposureMetricFunc>[] DefineContent(Dictionary<string, double> customVals)
        {
            Dictionary<string, double[]> realisticMeasures = new Dictionary<string, double[]>
            {
                {"worker-01", new double[]{ 31, 60.1, 133, 27.1 } },
                {"worker-02", new double[]{ 61.1, 5.27, 30.4, 31.7 } },
                {"worker-03", new double[]{ 20.5, 16.5, 15.5, 71.5 } }
            };

            BWModelParameters bwParams = BWModelParameters.GetDefaults(true);
            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                                new BetweenWorkerModel(measures: new MeasureList(workerMeasures: realisticMeasures, oel: 150),
                                                                       specificParams: bwParams));

            this.Emes = new ExposureMetricEstimates[] { eme };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("Group GM (90% CrI)", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("Between-worker GSD (90% CrI)", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Within-worker GSD (90% CrI)", new ExposureMetricFunc(e => e.GeomStanDev(false))),
                Tuple.Create("Within-worker correlation (rho) (90% CrI)", new ExposureMetricFunc(e => e.Rho())),
                Tuple.Create("Probability that rho>0.2", new ExposureMetricFunc(e => e.RhoProbGt(0.2))),
                Tuple.Create("R.ratio (+90% CrI)", new ExposureMetricFunc(e => e.RRatio())),
                Tuple.Create("Probability that R>2", new ExposureMetricFunc(e => e.RRatioProbGt(2))),
                Tuple.Create("Probability that R>10", new ExposureMetricFunc(e => e.RRatioProbGt(10))),
                Tuple.Create("Probability of individual overexposure (95th percentile) in % (90% CrI)", new ExposureMetricFunc(e => e.IndivOverexpoP95())),
                Tuple.Create("Chances that the above probability is >20%", new ExposureMetricFunc(e => e.IndivOverexpoP95ProbGt(20))),
                Tuple.Create("Probability of individual overexposure (arithmetic mean) in % (90% CrI)", new ExposureMetricFunc(e => e.IndivOverexpoAm())),
                Tuple.Create("Chances that the above probability is >20%", new ExposureMetricFunc(e => e.IndivOverexpoAmProbGt(20))),
            };
        }
    }
}