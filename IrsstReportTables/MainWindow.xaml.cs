﻿using System;
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
                                            this.ml.OEL,
                                            new SEGInformedVarModel(measures: ml, specificParams:
                                                SEGInformedVarModelParameters.GetDefaults(this.logNormDist)));

            tableData.Add(new TableEntry()
            {
                Title = "GM",
                Datum = eme.GeomMean()
            });

            tableData.Add(new TableEntry()
            {
                Title = "GSD",
                Datum = eme.GeomStanDev()
            });

            tableData.Add(new TableEntry()
            {
                Title = "Exceedance Fraction (%)",
                Datum = eme.ExceedanceFrac(true)
            });

            tableData.Add(new TableEntry()
            {
                Title = "95th percentile",
                Datum = eme.P95(true)
            });

            tableData.Add(new TableEntry()
            {
                Title = "AIHA band probabilities in % (95th percentile)",
                Datum = eme.BandProbabilities()
            });

            tableData.Add(new TableEntry()
            {
                Title = "Arithmetic mean",
                Datum = eme.ArithMean(true)
            });

            tableData.Add(new TableEntry()
            {
                Title = "AIHA band probabilities in % (AM)",
                Datum = eme.BandProbabilities(false)
            });
            return tableData;
        }

        private List<TableEntry> LoadTable4Data()
        {
            List<TableEntry> tableData = new List<TableEntry>();

            ExposureMetricEstimates emeInformed = new ExposureMetricEstimates(
                this.ml.OEL,
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: SEGInformedVarModelParameters.GetDefaults(this.logNormDist)
                ) );
            ExposureMetricEstimates emeUninformed = new ExposureMetricEstimates(
                this.ml.OEL,
                new SEGUninformativeModel(
                    measures: ml,
                    specificParams: UninformativeModelParameters.GetDefaults(this.logNormDist)
                ) );
            ExposureMetricEstimates emePdInformed = new ExposureMetricEstimates(
                this.ml.OEL,
                new SEGInformedVarModel(
                    measures: ml,
                    specificParams: SEGInformedVarModelParameters.GetDefaults(this.logNormDist),
                    pastDataSummary: new PastDataSummary(mean: 4, sd: 2.4, n: 5)
                ) );

            tableData.Add(new TableEntry()
            {
                Title = "GM (90% CrI)",
                Datum = emeInformed.GeomMean(),
                Datum2 = emeUninformed.GeomMean(),
                Datum3 = emePdInformed.GeomMean()
            });

            tableData.Add(new TableEntry()
            {
                Title = "GSD (90% CrI)",
                Datum = emeInformed.GeomStanDev(),
                Datum2 = emeUninformed.GeomStanDev(),
                Datum3 = emePdInformed.GeomStanDev()
            });

            tableData.Add(new TableEntry()
            {
                Title = "Exceedance fraction (%)(90 % CrI)",
                Datum = emeInformed.ExceedanceFrac(),
                Datum2 = emeUninformed.ExceedanceFrac(),
                Datum3 = emePdInformed.ExceedanceFrac()
            });

            tableData.Add(new TableEntry()
            {
                Title = "95th percentile (90% CrI)",
                Datum = emeInformed.P95(),
                Datum2 = emeUninformed.P95(),
                Datum3 = emePdInformed.P95()
            });

            tableData.Add(new TableEntry()
            {
                Title = "Overexposure risk (%, P95)",
                Datum = emeInformed.OverExposureRisk(),
                Datum2 = emeUninformed.OverExposureRisk(),
                Datum3 = emePdInformed.OverExposureRisk()
            });

            tableData.Add(new TableEntry()
            {
                Title = "AM (90% CrI)",
                Datum = emeInformed.ArithMean(),
                Datum2 = emeUninformed.ArithMean(),
                Datum3 = emePdInformed.ArithMean()
            });

            tableData.Add(new TableEntry()
            {
                Title = "Overexposure risk (%, AM)",
                Datum = emeInformed.OverExposureRisk(false),
                Datum2 = emeUninformed.OverExposureRisk(false),
                Datum3 = emePdInformed.OverExposureRisk(false)
            });
            return tableData;
        }
    }
}
