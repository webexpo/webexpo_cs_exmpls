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
        public MainWindow()
        {
            InitializeComponent();
            Window bww = new BetweenWorkerWindow();
            bww.Show();
            bww.Activate();
        }
    }
}
