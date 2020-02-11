using System;
using Zygotine.WebExpo;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class Table7Grid : ReportGrid
    {
        static Table7Grid()
        {

        }

        public Table7Grid() : base()
        {

        }

        public override string[] ColumnHeadings()
        {
            return new string[] {
                "Parameter",
                "Low within-worker correlation (rho=0.06)",
                "High within-worker correlation (rho=0.66)"
            };
        }
        public override string Description()
        {
            return "Worker specific exposure metrics point estimates and credible intervals for the least and most exposed workers in two samples with low and high within-worker correlation";
        }

        public override Tuple<string, ExposureMetricFunc>[] DefineContent()
        {
            Dictionary<string, double[]> lowWWCorrMeas = new Dictionary<string, double[]>
            {
                {"worker-01", new double[]{ 185, 34.8, 16.7, 12.4, 18.6, 47.4, 52.6, 15.3, 27.6, 26.3 } },
                {"worker-02", new double[]{ 4.79, 23, 7.54, 62.3, 8.55, 9.28, 43.6, 94.2, 44.6, 66.6 } },
                {"worker-03", new double[]{ 8.85, 31.7, 15.8, 89.6, 164, 40.5, 47.6, 75.5, 10.7, 62.3 } },
                {"worker-04", new double[]{ 16.4, 6.91, 87.4, 20, 16.8, 7.12, 6.99, 16.4, 12.6, 63.9 } },
                {"worker-05", new double[]{ 14.7, 59.6, 15, 21.8, 20.6, 96.1, 16.8, 15.8, 8.02, 26.7 } },
                {"worker-06", new double[]{ 37.9, 96.9, 40.8, 106, 21.7, 25.8, 51.3, 23, 18.9, 20.2 } },
                {"worker-07", new double[]{ 22, 44.8, 37.5, 16.6, 30.7, 7.07, 7.18, 80.9, 44.5, 135 } },
                {"worker-08", new double[]{ 69.9, 30.5, 33.4, 53, 70.7, 78.3, 18, 45.2, 51.4, 33.7 } },
                {"worker-09", new double[]{ 28.1, 7.49, 16, 23, 99.9, 12, 11.8, 57.4, 8.79, 24 } },
                {"worker-10", new double[]{ 113, 7.68, 85.6, 196, 35, 17.6, 60.7, 15.5, 34.3, 12.1 } },
            };

            Dictionary<string, double[]> highWWCorrMeas = new Dictionary<string, double[]>
            {
                {"worker-01", new double[]{ 66.8, 46, 61.1, 54.6, 31.7, 74.3, 60.9, 53.4, 38.9, 27.5 } },
                {"worker-02", new double[]{ 14.2, 53.9, 21.8, 47.8, 48.8, 76.5, 41.3, 20.4, 31.9, 31.1 } },
                {"worker-03", new double[]{ 186, 84.6, 94.4, 218, 189, 130, 107, 80.6, 288, 173 } },
                {"worker-04", new double[]{ 23.5, 16.2, 40.2, 130, 42.2, 25.7, 35.4, 40.8, 109, 40.9 } },
                {"worker-05", new double[]{ 43.8, 31.1, 13.1, 24.1, 27.7, 23.9, 40.2, 60.3, 29.8, 37.2 } },
                {"worker-06", new double[]{ 41, 11.4, 4.44, 12.9, 22.7, 20.5, 12.6, 8.35, 13.6, 28.1 } },
                {"worker-07", new double[]{ 6.56, 9.5, 6.97, 5.92, 2.42, 14, 12.3, 3.07, 7.01, 6.49 } },
                {"worker-08", new double[]{ 9.21, 9.42, 28.7, 72.9, 35.6, 17.2, 20.2, 13.4, 10.5, 26.3 } },
                {"worker-09", new double[]{ 19.6, 14.3, 22.8, 35.1, 28.9, 36.9, 13, 13.3, 13.6, 37 } },
                {"worker-10", new double[]{ 78.7, 28.2, 41.3, 14.4, 72.9, 10.2, 16.2, 15.8, 42.2, 61 } },
            };

            BWModelParameters bwParams = BWModelParameters.GetDefaults(true);
            ExposureMetricEstimates emeLowWWCorr = new ExposureMetricEstimates(
                                            new BetweenWorkerModel(measures: new MeasureList(workerMeasures: lowWWCorrMeas, oel: 150),
                                                                   specificParams: bwParams));
            ExposureMetricEstimates emeHighWWCorr = new ExposureMetricEstimates(
                                            new BetweenWorkerModel(measures: new MeasureList(workerMeasures: highWWCorrMeas, oel: 150),
                                                                   specificParams: bwParams));

            double lowWWCorrLowestGm, lowWWCorrHighestGm, highWWCorrLowestGm, highWWCorrHighestGm;

            string widLowWWCorrLeastExpo = emeLowWWCorr.FindExposedWorker(findMostExposed: false, gm: out lowWWCorrLowestGm);
            string widLowWWCorrMostExpo = emeLowWWCorr.FindExposedWorker(findMostExposed: true, gm: out lowWWCorrHighestGm);
            ExposureMetricEstimates emeLowLeast = emeLowWWCorr.GetWorkerEstimates(widLowWWCorrLeastExpo);
            ExposureMetricEstimates emeLowMost = emeLowWWCorr.GetWorkerEstimates(widLowWWCorrMostExpo);

            string widHighWWCorrLeastExpo = emeHighWWCorr.FindExposedWorker(findMostExposed: false, gm: out highWWCorrLowestGm);
            string widHighWWCorrMostExpo = emeHighWWCorr.FindExposedWorker(findMostExposed: true, gm: out highWWCorrHighestGm);
            ExposureMetricEstimates emeHighLeast = emeHighWWCorr.GetWorkerEstimates(widHighWWCorrLeastExpo);
            ExposureMetricEstimates emeHighMost = emeHighWWCorr.GetWorkerEstimates(widHighWWCorrMostExpo);

            this.Emes = new ExposureMetricEstimates[] { emeLowLeast, emeLowMost, emeHighLeast, emeHighMost };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance fraction (%)", new ExposureMetricFunc(e => e.ExceedanceFrac())),
                Tuple.Create("95th percentile", new ExposureMetricFunc(e => e.P95())),
                Tuple.Create("Arithmetic mean", new ExposureMetricFunc(e => e.ArithMean()))
            };
        }
    }
}
