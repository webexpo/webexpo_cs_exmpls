using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrsstReportTables
{
    public class TableEntry
    {
        public string Title { get; set; }
        public TableEntryData Datum { get; set; }
        public TableEntryData Datum2 { get; set; }
        public TableEntryData Datum3 { get; set; }
        public TableEntryData Datum4 { get; set; }
        public string Extra { get; set; }
    }
}
