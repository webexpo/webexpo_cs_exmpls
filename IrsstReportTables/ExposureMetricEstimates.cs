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
        double TargetPerc { get; set; } = 95;
        double[] MuChain { get; set; }
        double[] SigmaChain { get; set; }
        double[] SigmaWithinChain { get; set; } = null;

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

                bool isBWModel = m.GetType() == typeof(BetweenWorkerModel);
                MuChain = m.Result.GetChainByName("mu" + (isBWModel ? "Overall" : "") + "Sample");
                SigmaChain = m.Result.GetChainByName(isBWModel ? "sigmaBetweenSample" : "sdOverall");
                if ( isBWModel )
                {
                    SigmaWithinChain = m.Result.GetChainByName("sigmaWithinSample");
                }
            }
        }

        public TableEntryData GeomMean()
        {
            return new PointEstimateWInterval(MuChain.Select(mu => LogNormDist ? Math.Exp(mu) : mu).ToArray<double>());
        }

        public TableEntryData GeomStanDev(bool useDefaultSigma = true)
        {
            double[] sdChain = useDefaultSigma ? SigmaChain : SigmaWithinChain;
            return new PointEstimateWInterval(sdChain.Select(sd => LogNormDist ? Math.Exp(sd) : sd).ToArray<double>());
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

        public TableEntryData Rho()
        {
            double[] rhoChain = RhoChain();
            return new PointEstimateWInterval(rhoChain);
        }

        public TableEntryData RRatio()
        {
            double[] rhoChain = RRatioChain();
            return new PointEstimateWInterval(rhoChain);
        }

        public TableEntryData IndivOverexpoP95()
        {
            double[] ioChain = IndivOverexpoP95Chain();
            return new PointEstimateWInterval(ioChain);
        }

        public TableEntryData IndivOverexpoAm()
        {
            double[] ioChain = IndivOverexpoAmChain();
            return new PointEstimateWInterval(ioChain);
        }

        public TableEntryData RhoProbGt(double p)
        {
            return new OverExposureRiskPercentage((double) OverExposureRisk(RhoChain(), p, false));
        }

        public TableEntryData RRatioProbGt(double p)
        {
            return new OverExposureRiskPercentage((double)OverExposureRisk(RRatioChain(), p, false));
        }

        public TableEntryData IndivOverexpoP95ProbGt(double p)
        {
            return new OverExposureRiskPercentage((double)OverExposureRisk(IndivOverexpoP95Chain(), p, false));
        }

        public TableEntryData IndivOverexpoAmProbGt(double p)
        {
            return new OverExposureRiskPercentage((double)OverExposureRisk(IndivOverexpoAmChain(), p, false));
        }

        public TableEntryData BandProbabilities(bool p95 = true)
        {
            return new AihaBandProbabilities(p95 ? P95Chain() : AmChain(), Oel);
        }

        double[] P95Chain()
        {
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

        double[] RhoChain()
        {
            double[] rhoChain = new double[SigmaChain.Length];
            for (int i = 0; i < SigmaChain.Length; i++)
            {
                rhoChain[i] = Math.Pow(SigmaChain[i], 2) / (Math.Pow(SigmaChain[i], 2) + Math.Pow(SigmaWithinChain[i], 2));
            }
            return rhoChain;
        }

        double[] RRatioChain(double rbRatioCoverage = 80)
        {
            double[] chain = new double[SigmaChain.Length];
            for (int i = 0; i < SigmaChain.Length; i++)
            {
                chain[i] = Math.Exp(2 * NormalDistribution.QNorm(1 - (100 - rbRatioCoverage) / 200) * SigmaChain[i]);
            }
            return chain;
        }

        double[] IndivOverexpoP95Chain()
        {
            double[] chain = new double[SigmaChain.Length];

            for (int i = 0; i < SigmaChain.Length; i++)
            {
                chain[i] = 100 * (1 - NormalDistribution.PNorm(((LogNormDist ? Math.Log((Oel)) : Oel) - MuChain[i] - NormalDistribution.QNorm(TargetPerc / 100) * SigmaWithinChain[i]) / SigmaChain[i]));
            }
            return chain;
        }

        double[] IndivOverexpoAmChain()
        {
            double[] chain = new double[SigmaChain.Length];

            for (int i = 0; i < SigmaChain.Length; i++)
            {
                chain[i] = 100 * (1 - NormalDistribution.PNorm(((LogNormDist ? (Math.Log(Oel) - 0.5 * Math.Pow(SigmaWithinChain[i], 2)) : Oel) - MuChain[i]) / SigmaChain[i]));
            }

            return chain;
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
                int count = chain.Where(p => p > limit).ToArray<double>().Length;
                return (double)100 * count / chain.Length;
            }

            return null;
        }

    }
}
