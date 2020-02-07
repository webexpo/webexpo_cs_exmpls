using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zygotine.WebExpo;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public partial class MainWindow : Window
    {
        string secretCity { get; set; } = "Montreal";
        
        public MainWindow()
        {
            InitializeComponent();
            //Window bww = new BetweenWorkerWindow();
            //bww.Show();
            //bww.Activate();
        }

        public void Table3Loader(object sender, RoutedEventArgs ev)
        {
            var grid = sender as ReportGrid;
            ObservableCollection<TableEntry> tableData = grid.Source;

            MeasureList ml = new MeasureList(measures: new[] { 24.7, 64.1, 13.8, 43.7, 19.9, 133, 32.1, 15, 53.7 },
                                      oel: 100);
            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                            new SEGInformedVarModel(measures: ml, specificParams:
                                                SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true)));

            Tuple<string, ExposureMetricFunc>[] tuples = new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance Fraction (%)", new ExposureMetricFunc(e => e.ExceedanceFrac(true))),
                Tuple.Create("95th percentile", new ExposureMetricFunc(e => e.P95(true))),
                Tuple.Create("AIHA band probabilities in % (95th percentile)", new ExposureMetricFunc(e => e.BandProbabilities())),
                Tuple.Create("Arithmetic mean", new ExposureMetricFunc(e => e.ArithMean(true))),
                Tuple.Create("AIHA band probabilities in % (AM)", new ExposureMetricFunc(e => e.BandProbabilities(false)))
            };

            foreach (Tuple<string, ExposureMetricFunc> t in tuples)
            {
                tableData.Add(new TableEntry { Title = t.Item1 }.Add(t.Item2(eme)));
            }

            grid.ItemsSource = tableData;
        }

        public void Table4Loader(object sender, RoutedEventArgs ev)
        {
            var grid = sender as ReportGrid;
            ObservableCollection<TableEntry> tableData = grid.Source;
            MeasureList ml = new MeasureList(measures: new[] { 24.7, 64.1, 13.8, 43.7, 19.9, 133, 32.1, 15, 53.7 },
                                      oel: 100);

            ExposureMetricEstimates emeInformed = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true)
                ) );
            ExposureMetricEstimates emeUninformed = new ExposureMetricEstimates(
                new SEGUninformativeModel(
                    measures: ml,
                    specificParams: UninformativeModelParameters.GetDefaults(logNormalDstrn: true)
                ) );
            ExposureMetricEstimates emePdInformed = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true),
                    pastDataSummary: new PastDataSummary(mean: Math.Log(5), sd: Math.Log(2.4), n: 5)
                ) );

            ExposureMetricEstimates[] emes = new ExposureMetricEstimates[] { emeInformed, emeUninformed, emePdInformed };

            Tuple<string, ExposureMetricFunc>[] tuples = new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM (90% CrI)", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD (90% CrI)", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance fraction (%)(90 % CrI)", new ExposureMetricFunc(e => e.ExceedanceFrac())),
                Tuple.Create("95th percentile (90% CrI)", new ExposureMetricFunc(e => e.P95())),
                Tuple.Create("Overexposure risk (%, P95)", new ExposureMetricFunc(e => e.OverExposureRisk())),
                Tuple.Create("AM (90% CrI)", new ExposureMetricFunc(e => e.ArithMean())),
                Tuple.Create("Overexposure risk (%, AM)", new ExposureMetricFunc(e => e.OverExposureRisk(false))),
            };

            foreach (Tuple<string, ExposureMetricFunc> t in tuples)
            {
                tableData.Add(emes.Aggregate(new TableEntry { Title = t.Item1 }, (te, e) => te.Add(t.Item2(e))));
            }

            grid.ItemsSource = tableData;
        }

        public void Table5Loader(object sender, RoutedEventArgs ev)
        {
            var grid = sender as ReportGrid;
            ObservableCollection<TableEntry> tableData = grid.Source;
            double[] measures = new double[] { 96.6, 38.3, 80.8, 15.1, 34, 73.4, 14.5, 64.8, 27.4, 48.7, 43.3, 43.4, 57.8, 94.9, 44.1, 44.3, 62.9, 117, 51.6, 64.7, 50.1, 74.7, 221, 46.8, 84.3, 93.4, 126, 46.9, 29.5, 73.8, 66.9, 61.3, 30.2, 101, 22.6, 191, 29.3, 68, 114, 33.7, 52.5, 118, 49.7, 60.4, 36.6, 55.9, 31.9, 84.3, 75.8, 39.5, 28.3, 56.5, 44.2, 48, 36.6, 70, 37, 72, 48, 66.1, 72.4, 80.9, 69.1, 162, 67.3, 75.2, 40.5, 25.6, 44, 120, 56.3, 42.9, 6.63, 24.9, 40.9, 81, 97.2, 74.7, 79.6, 48.8, 75.3, 54.8, 66.5, 71.3, 28.7, 87.5, 51.9, 19.6, 60.8, 45.9, 46.9, 84.8, 120, 103, 36.7, 92.7, 32.8, 73.8, 214, 65.3 };
            double oel = 100;
            SEGInformedVarModelParameters modelParams = SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true);


            ExposureMetricEstimates emeNoErr = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: new MeasureList(measures: measures, oel: oel),
                    specificParams: modelParams
                ));

            ExposureMetricEstimates emeKnownErr = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: new MeasureList(measures: measures, oel: oel, measErr: 30),
                    specificParams: modelParams
                ));

            ExposureMetricEstimates emeUnknownErr = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: new MeasureList(measures: measures, oel: oel, measErrRange: new double[] { 15, 45 }),
                    specificParams: modelParams
                ));

            ExposureMetricEstimates[] emes = new ExposureMetricEstimates[] { emeNoErr, emeKnownErr, emeUnknownErr };

            Tuple<string, ExposureMetricFunc>[] tuples = new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM (90% CrI)", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD (90% CrI)", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance fraction (%)(90 % CrI)", new ExposureMetricFunc(e => e.ExceedanceFrac())),
                Tuple.Create("95th percentile (90% CrI)", new ExposureMetricFunc(e => e.P95())),
                Tuple.Create("Arithmetic mean (90% CrI)", new ExposureMetricFunc(e => e.ArithMean())),
            };

            foreach (Tuple<string, ExposureMetricFunc> t in tuples)
            {
                tableData.Add(emes.Aggregate(new TableEntry { Title = t.Item1 }, (te, e) => te.Add(t.Item2(e))));
            }

            grid.ItemsSource = tableData;
        }
    }
}
