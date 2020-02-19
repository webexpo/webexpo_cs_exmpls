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
        public bool SlowLoad { get; set; } = false;

        static ReportGrid()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportGrid), new FrameworkPropertyMetadata(typeof(ReportGrid)));  
        }

        public ReportGrid() : base()
        {
            this.IsReadOnly = true;
            this.AutoGenerateColumns = false;
            this.HorizontalAlignment = HorizontalAlignment.Center;

            int i = 0;
            foreach (var heading in ColumnHeadings() )
            {
                string idx = String.Format("Datum{0}", i++);
                var binding = new Binding(idx);
                this.Columns.Add(new DataGridTextColumn() { Header = heading, Binding = binding });
            }
        }

        public abstract string[] ColumnHeadings();
        public abstract Tuple<string, ExposureMetricFunc>[] DefineContent();
        public abstract string Description();

        public void Load(object sender, DoWorkEventArgs ev)
        {
            if ( Source.Count == 0 )
            {
                foreach (Tuple<string, ExposureMetricFunc> t in DefineContent())
                {
                    Source.Add(Emes.Aggregate(new TableEntry { Datum0 = t.Item1 }, (te, e) => te.Add(t.Item2(e))));
                }
            }
        }
    }
}
