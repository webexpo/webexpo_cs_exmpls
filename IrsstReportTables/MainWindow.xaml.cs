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

        public MainWindow()
        {
            InitializeComponent();
            LoadTableList();
        }

        void LoadTableList()
        {
            List<string> tableClasses = FindAllDerivedTypes<ReportGrid>().Select(type => TableClassIdToLabel(type.Name)).ToList();

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
            CurrReportGrid = (ReportGrid)Activator.CreateInstance(t);
            TableTitle.Text = CurrReportGrid.Description();
            SlowLoadWarning.Visibility = CurrReportGrid.SlowLoad ? Visibility.Visible : Visibility.Hidden;
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
            return Regex.Replace(classId, @"(Table)(\d){1,2}Grid", "$1 $2");
        }

        static string TableClassLabelToId(string classLabel)
        {
            return Regex.Replace(classLabel, @"(Table)\s(\d){1,2}", "$1$2Grid");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Spinner.Visibility = Visibility.Visible;
            TablePlaceholder.Visibility = Visibility.Hidden;

            if (TablePlaceholder.Children.Count > 0)
            {
                TablePlaceholder.Children.RemoveAt(0);
            }

            BackgroundWorker BackWorker = new BackgroundWorker();
            BackWorker.DoWork += CurrReportGrid.Load;
            BackWorker.RunWorkerCompleted += TableLoaded;
            BackWorker.RunWorkerAsync();
        }
 
        private void TableLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrReportGrid.ItemsSource = CurrReportGrid.Source;
            TablePlaceholder.Children.Add(CurrReportGrid);
            TablePlaceholder.Height = CurrReportGrid.Height + 5;
            Spinner.Visibility = Visibility.Hidden;
            TablePlaceholder.Visibility = Visibility.Visible;
        }
    }
}
