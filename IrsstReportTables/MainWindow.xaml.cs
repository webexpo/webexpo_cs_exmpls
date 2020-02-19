using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Zygotine.WebExpo;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace IrsstReportTables
{
    public partial class MainWindow : Window
    {
        ReportGrid CurrReportGrid = null;
        BackgroundWorker BackWorker = null;
        Dictionary<string, ReportGrid> CreatedTables = new Dictionary<string, ReportGrid>();

        public MainWindow()
        {
            InitializeComponent();
            LoadTableList();
        }

        void LoadTableList()
        {
            string[] tableClasses = FindAllDerivedTypes<ReportGrid>().Select(type => TableClassIdToLabel(type.Name)).ToArray();
            int CompareStrings(string s1, string s2)
            {
                s1 = s1.Replace("Table ", "").Replace("C", "1");
                s2 = s2.Replace("Table ", "").Replace("C", "1");
                int i1 = Int16.Parse(s1);
                int i2 = Int16.Parse(s2);
                return i1 < i2 ? -1 : (i1 > i2 ? 1 : 0);
            }
            Array.Sort(tableClasses, CompareStrings);
            foreach (string tableId in tableClasses)
            {
                Tables4Display.Items.Add(tableId);
            }
        }

        private void Tables4Display_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object s = ((ComboBox)sender).SelectedValue;
            string classId = TableClassLabelToId((string) s);
            string qualClassId = String.Format("{0}.{1}", GetType().Namespace, classId);
            Type t = Type.GetType(qualClassId);
            if ( !CreatedTables.TryGetValue(classId, out CurrReportGrid) ) {
                CurrReportGrid = (ReportGrid)Activator.CreateInstance(t);
            }
            TableTitle.Text = CurrReportGrid.Description();
            SlowLoadWarning.Visibility = CurrReportGrid.SlowLoad && CurrReportGrid.ItemsSource == null ? Visibility.Visible : Visibility.Hidden;
            DisplayButton.IsEnabled = TablePlaceholder.Children.Count == 0 || TablePlaceholder.Children[0].GetType().Name != CurrReportGrid.GetType().Name;
        }

        public static List<Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }

        public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                .Where(t =>
                    t != derivedType &&
                    derivedType.IsAssignableFrom(t)
                    ).ToList();

        }

        static string TableClassIdToLabel(string classId)
        {
            return Regex.Replace(classId, @"(Table)([A-Z]?\d)Grid", "$1 $2");
        }

        static string TableClassLabelToId(string classLabel)
        {
            return Regex.Replace(classLabel, @"(Table)\s([A-Z]?\d)", "$1$2Grid");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TablePlaceholder.Children.Count > 0)
            {
                TablePlaceholder.Children.RemoveAt(0);
            }

            if (CurrReportGrid.ItemsSource == null)
            {
                Spinner.Visibility = Visibility.Visible;
                TablePlaceholder.Visibility = Visibility.Hidden;

                if (BackWorker != null && BackWorker.IsBusy)
                {
                    BackWorker.CancelAsync();
                }
                BackWorker = new BackgroundWorker();
                BackWorker.DoWork += CurrReportGrid.Load;
                BackWorker.RunWorkerCompleted += TableLoaded;
                BackWorker.WorkerSupportsCancellation = true;
                BackWorker.RunWorkerAsync();
            } else
            {
                ShowTable();
            }
        }
 
        private void ShowTable()
        {
            TablePlaceholder.Children.Add(CurrReportGrid);
            DisplayButton.IsEnabled = false;
        }

        private void TableLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrReportGrid.ItemsSource = CurrReportGrid.Source;
            ShowTable();
            Spinner.Visibility = Visibility.Hidden;
            SlowLoadWarning.Visibility = Visibility.Hidden;
            TablePlaceholder.Visibility = Visibility.Visible;
            string key = TableClassLabelToId((string) Tables4Display.SelectedValue);
            CreatedTables.Add(key, CurrReportGrid);
        }
    }
}
