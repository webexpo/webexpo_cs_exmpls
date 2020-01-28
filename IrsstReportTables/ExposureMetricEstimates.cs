using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zygotine.WebExpo;
using Zygotine.Statistics.Distribution;

namespace IrsstReportTables
{
    public class ExposureMetricEstimates
    {
        double Oel { get; set; }
        bool LogNormDist { get; set; } = true;
        double[] MuChain { get; set; }
        double[] SigmaChain { get; set; }

        public ExposureMetricEstimates(double oel)
        {
            Oel = oel;
        }
        public ExposureMetricEstimates(Model m = null)
        {
            Oel = m.Measures.OEL;

            if (m != null)
            {
                LogNormDist = m.OutcomeIsLogNormallyDistributed;

                m.Compute();
                MuChain = m.Result.GetChainByName("muSample");
                SigmaChain = m.Result.GetChainByName("sdSample");
            }
        }

        public TableEntryData GeomMean()
        {
            return new PointEstimateWInterval(MuChain.Select(mu => LogNormDist ? Math.Exp(mu) : mu).ToArray<double>());
        }

        public TableEntryData GeomStanDev()
        {
            return new PointEstimateWInterval(SigmaChain.Select(sd => LogNormDist ? Math.Exp(sd) : sd).ToArray<double>());
        }

        public TableEntryData ExceedanceFrac(bool addOverExpo = false)
        {
            double exc = 5;
            double[] fracChain = new double[MuChain.Length];
            for (int i = 0; i < fracChain.Length; i++)
            {
                fracChain[i] = 100 * (1 - NormalDistribution.PNorm(((LogNormDist ? Math.Log(Oel) : Oel) - MuChain[i]) / SigmaChain[i]));
            }

            return new PointEstimateWInterval(chain: fracChain, overexpo: OverExposureRisk(addOverExpo ? fracChain : null, exc, false));
        }

        public TableEntryData P95(bool addOverExpo = false)
        {
            double[] p95 = P95Chain();

            return new PointEstimateWInterval(chain: p95, overexpo: OverExposureRisk(addOverExpo ? p95 : null));
        }

        public TableEntryData ArithMean(bool addOverExpo = false)
        {
            double[] amc = AmChain();
            return new PointEstimateWInterval(chain: amc, overexpo: OverExposureRisk(addOverExpo ? amc : null));
        }

        public TableEntryData BandProbabilities(bool p95 = true)
        {
            return new AihaBandProbabilities(p95 ? P95Chain() : AmChain(), Oel);
        }

        double[] P95Chain()
        {
            double TargetPerc = 95;
            double[] p95Chain = new double[MuChain.Length];
            for (int i = 0; i < p95Chain.Length; i++)
            {
                double k = MuChain[i] + NormalDistribution.QNorm(TargetPerc / 100) * SigmaChain[i];
                p95Chain[i] = LogNormDist ? Math.Exp(k) : k;
            }

            return p95Chain;
        }

        double[] AmChain()
        {
            double[] amc = new double[MuChain.Length];
            for (int i = 0; i < amc.Length; i++)
            {
                amc[i] = LogNormDist ? Math.Exp(MuChain[i] + 0.5 * Math.Pow(SigmaChain[i], 2)) : MuChain[i];
            }

            return amc;
        }

        public TableEntryData OverExposureRisk(bool p95 = true)
        {
            double[] chain = p95 ? P95Chain() : AmChain();
            double risk = (double)OverExposureRisk(chain);
            return new OverExposureRiskPercentage(risk);
        }

        public double? OverExposureRisk(double[] chain, double limit = 1, bool limitIsOelFactor = true)
        {
            if (chain != null)
            {
                if (limitIsOelFactor)
                {
                    limit *= Oel;
                }
                return (double)100 * chain.Where(p => p > limit).ToArray<double>().Length / chain.Length;
            }

            return null;
        }

    }
}
