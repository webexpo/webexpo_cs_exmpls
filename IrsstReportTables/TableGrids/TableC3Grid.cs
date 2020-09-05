using System;
using Zygotine.WebExpo;
using System.Collections.Generic;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class TableC3Grid : ReportGrid
    {
        static TableC3Grid()
        {

        }

        public TableC3Grid() : base()
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
            return "Exposure metrics point estimates and credible intervals for an example of Bayesian calculation for the normal model (between-worker difference analyses)";
        }

        public override Tuple<string, ExposureMetricFunc>[] DefineContent(Dictionary<string, object> customVals)
        {
            Dictionary<string, double[]> realisticMeasures = new Dictionary<string, double[]>
            {
                { "worker-1", new double[]{ 76.2, 82.3, 81.7, 73.7, 79.4, 79.1, 80.2, 71, 86.9, 75.6 } },
                { "worker-2", new double[]{ 70.6, 78.7, 77.6, 76.9, 79.5, 84.8, 77.6, 65.5, 74.1, 69.9 } },
                { "worker-3", new double[]{ 79.2, 77.7, 73.5, 78.9, 81.6, 83.1, 85.1, 84.2, 79.8, 84.1 } },
                { "worker-4", new double[]{ 79.1, 77.6, 81.2, 82.6, 81.6, 82.4, 76.9, 87.6, 80.4, 79.7 } } ,
                { "worker-5", new double[]{ 85.3, 92.2, 75.8, 84.1, 76.1, 84.6, 78.9, 75.8, 89, 87.1 } },
                { "worker-6", new double[]{ 77.8, 89, 81.9, 80.4, 88.5, 87, 85, 88.1, 81.3, 90.6 } },
                { "worker-7", new double[]{ 79.1, 80.7, 85.8, 84.8, 88.5, 82.6, 78.6, 90.1, 82.9, 83 } },
                { "worker-8", new double[]{ 80, 76.6, 84.6, 77.1, 81.5, 77.4, 73.5, 82.2, 74.4, 77.6 } },
                { "worker-9", new double[]{ 80, 81.2, 73.8, 80.7, 76.9, 77.5, 74.6, 70.6, 82.3, 66.4 } },
                { "worker-10", new double[]{ 89.1, 85.4, 81.8, 88.1, 86.4, 81.6, 86.8, 81.4, 86.7, 83.6 } }
            };

            BWModelParameters bwParams = BWModelParameters.GetDefaults(false);
            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                                new BetweenWorkerModel(measures: new MeasureList(workerMeasures: realisticMeasures, oel: 85),
                                                                       specificParams: bwParams));

            this.Emes = new ExposureMetricEstimates[] { eme };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("Arithmetic mean (90% CrI)", new ExposureMetricFunc(e => e.ArithMean())),
                Tuple.Create("Between-worker arithmetic standard deviation (90% CrI)", new ExposureMetricFunc(e => e.ArithStanDev())),
                Tuple.Create("Within-worker arithmetic standard deviation (90% CrI)", new ExposureMetricFunc(e => e.ArithStanDev(false))),
                Tuple.Create("Within-worker correlation (rho) (90% CrI)", new ExposureMetricFunc(e => e.Rho())),
                Tuple.Create("Probability that rho>0.2", new ExposureMetricFunc(e => e.RhoProbGt(0.2))),
                Tuple.Create("R.difference (90% CrI)", new ExposureMetricFunc(e => e.RDiff())),
                Tuple.Create("Probability of individual overexposure (95th percentile) in % (90% CrI)", new ExposureMetricFunc(e => e.IndivOverexpoP95())),
                Tuple.Create("Chances that the above probability is >20%", new ExposureMetricFunc(e => e.IndivOverexpoP95ProbGt(20))),
                Tuple.Create("Probability of individual overexposure (arithmetic mean) in % (90% CrI)", new ExposureMetricFunc(e => e.IndivOverexpoAm())),
                Tuple.Create("Chances that the above probability is >20%", new ExposureMetricFunc(e => e.IndivOverexpoAmProbGt(20))),
            };
        }
    }
}