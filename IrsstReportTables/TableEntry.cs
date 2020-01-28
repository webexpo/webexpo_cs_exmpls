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
        public TableEntryData Datum { get; set; } = null;
        public TableEntryData Datum2 { get; set; } = null;
        public TableEntryData Datum3 { get; set; } = null;
        public TableEntryData Datum4 { get; set; } = null;
        public string Extra { get; set; }

        public TableEntry Add(TableEntryData ted)
        {
            if ( Datum == null )
            {
                Datum = ted;
            }
            else
            if ( Datum2 == null )
            {
                Datum2 = ted;
            }
            else
            if (Datum3 == null)
            {
                Datum3 = ted;
            }
            else
            {
                Datum4 = ted;
            }

            return this;
        }
    }
}
