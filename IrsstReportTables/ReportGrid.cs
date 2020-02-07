using System;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Zygotine.WebExpo;


namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public abstract class ReportGrid : DataGrid
    {
        public ObservableCollection<TableEntry> Source { get; set; } = new ObservableCollection<TableEntry>();
        public ExposureMetricEstimates[] Emes { get; set; }
       
        
        static ReportGrid()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportGrid), new FrameworkPropertyMetadata(typeof(ReportGrid)));  
        }

        public ReportGrid(bool skipLoad = false) : base()
        {
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.IsReadOnly = true;
            this.AutoGenerateColumns = false;

            void Loader(object sender, RoutedEventArgs ev)
            {
                foreach ( Tuple<string, ExposureMetricFunc> t in DefineContent() )
                {
                    Source.Add(Emes.Aggregate(new TableEntry { Title = t.Item1 }, (te, e) => te.Add(t.Item2(e))));
                }

                this.ItemsSource = Source;
            }

            if (!skipLoad)
            {
                this.Loaded += Loader;
            }
        }

        public abstract Tuple<string, ExposureMetricFunc>[] DefineContent();
    }
}
