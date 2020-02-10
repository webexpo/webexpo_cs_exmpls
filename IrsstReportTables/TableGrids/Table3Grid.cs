﻿using System;
using Zygotine.WebExpo;

namespace IrsstReportTables
{
    using ExposureMetricFunc = Func<ExposureMetricEstimates, TableEntryData>;

    public class Table3Grid : ReportGrid
    {
        static Table3Grid()
        {

        }

        public override Tuple<string, ExposureMetricFunc>[] DefineContent()
        {
            MeasureList ml = new MeasureList(measures: new[] { 24.7, 64.1, 13.8, 43.7, 19.9, 133, 32.1, 15, 53.7 },
                                      oel: 100);
            ExposureMetricEstimates eme = new ExposureMetricEstimates(
                                            new SEGInformedVarModel(measures: ml, specificParams:
                                                SEGInformedVarModelParameters.GetDefaults(logNormalDstrn: true)));
            this.Emes = new ExposureMetricEstimates[] { eme };

            return new Tuple<string, ExposureMetricFunc>[] {
                Tuple.Create("GM", new ExposureMetricFunc(e => e.GeomMean())),
                Tuple.Create("GSD", new ExposureMetricFunc(e => e.GeomStanDev())),
                Tuple.Create("Exceedance Fraction (%)", new ExposureMetricFunc(e => e.ExceedanceFrac(true))),
                Tuple.Create("95th percentile", new ExposureMetricFunc(e => e.P95(true))),
                Tuple.Create("AIHA band probabilities in % (95th percentile)", new ExposureMetricFunc(e => e.BandProbabilities())),
                Tuple.Create("Arithmetic mean", new ExposureMetricFunc(e => e.ArithMean(true))),
                Tuple.Create("AIHA band probabilities in % (AM)", new ExposureMetricFunc(e => e.BandProbabilities(false)))
            };
        }
    }
}