using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zygotine.WebExpo;
using System.Globalization;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public partial class MainWindow : Window
    {
        MeasureList ml;
        bool logNormDist;
        NumberFormatInfo nfi;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            this.ml = new MeasureList(measures: new[] { 24.7, 64.1, 13.8, 43.7, 19.9, 133, 32.1, 15, 53.7 },
                                      measErrRange: null, //new[] { 28.0, 30.0 },
                                      oel: 100);
            this.logNormDist = true;
            this.nfi = new NumberFormatInfo();
            this.nfi.NumberDecimalSeparator = ".";

            Table3.ItemsSource = LoadTable3Data();
            Table4.ItemsSource = LoadTable4Data();
        }

        private List<TableEntry> LoadTable3Data()
        {
            List<TableEntry> tableData = new List<TableEntry>();

            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                            new SEGInformedVarModel(measures: ml, specificParams:
                                                SEGInformedVarModelParameters.GetDefaults(this.logNormDist)));

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

            return tableData;
        }

        private List<TableEntry> LoadTable4Data()
        {
            List<TableEntry> tableData = new List<TableEntry>();

            ExposureMetricEstimates emeInformed = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: SEGInformedVarModelParameters.GetDefaults(this.logNormDist)
                ) );
            ExposureMetricEstimates emeUninformed = new ExposureMetricEstimates(
                new SEGUninformativeModel(
                    measures: ml,
                    specificParams: UninformativeModelParameters.GetDefaults(this.logNormDist)
                ) );
            ExposureMetricEstimates emePdInformed = new ExposureMetricEstimates(
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: SEGInformedVarModelParameters.GetDefaults(this.logNormDist),
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

            return tableData;
        }
    }
}
