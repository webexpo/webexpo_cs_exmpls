using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zygotine.WebExpo;
using Zygotine.Statistics.Distribution;


namespace IrsstReportTables
{
    public class ExposureMetricEstimates : ICloneable
    {
        double Oel { get; set; }
        bool LogNormDist { get; set; } = true;
        double TargetPerc { get; set; } = 95;
        double[] MuChain { get; set; }
        double[] SigmaChain { get; set; }
        double[] SigmaWithinChain { get; set; } = null;
        public BetweenWorkerModel BWModel { get; set; } = null;
        string[] WorkerIds = null;

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
                string muMidStr = "";
                if (isBWModel)
                {
                    BWModel = (BetweenWorkerModel) m;
                    WorkerIds = m.Measures.WorkerTags;
                    SigmaWithinChain = m.Result.GetChainByName("sigmaWithinSample");
                    muMidStr = "Overall";
                }
                MuChain = m.Result.GetChainByName("mu" + muMidStr + "Sample");
                SigmaChain = m.Result.GetChainByName(isBWModel ? "sigmaBetweenSample" : "sdOverall");
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
                double qn = NormalDistribution.QNorm(TargetPerc / 100);
                chain[i] = 100 * (1 - NormalDistribution.PNorm(((LogNormDist ? Math.Log(Oel) : Oel) - MuChain[i] - qn * SigmaWithinChain[i]) / SigmaChain[i]));
            }
            return chain;
        }

        double[] IndivOverexpoAmChain()
        {
            double[] chain = new double[SigmaChain.Length];

            for (int i = 0; i < SigmaChain.Length; i++)
            {
                chain[i] = 100 * (1 - NormalDistribution.PNorm(((LogNormDist ? Math.Log(Oel) : Oel) - MuChain[i] - 0.5 * Math.Pow(SigmaWithinChain[i], 2)) / SigmaChain[i]));
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


        Tuple<string, double> FindExposed(bool findMostExposed)
        {
            double currGM = findMostExposed ? Double.NegativeInfinity : Double.PositiveInfinity;
            string exposedWid = null;
            foreach ( string wId in WorkerIds )
            {
                double gMean = PointEstimateWInterval.GeomMean(BWModel.Result.GetChainByName(string.Format("mu_{0}Sample", wId)));
                if ( findMostExposed ? gMean > currGM : gMean < currGM )
                {
                    currGM = gMean;
                    exposedWid = wId;
                }
            }

            return Tuple.Create(exposedWid, currGM);
        }

        public string FindExposedWorker(bool findMostExposed = true)
        {
            return FindExposed(findMostExposed).Item1;
        }

        public string FindExposedWorker(bool findMostExposed, out double gm)
        {
            Tuple<string,double> t = FindExposed(findMostExposed);
            gm = t.Item2;
            return t.Item1;
        }

        public ExposureMetricEstimates GetWorkerEstimates(string workerId)
        {
            ExposureMetricEstimates workerEst = (ExposureMetricEstimates) this.Clone();
            workerEst.MuChain = BWModel.Result.GetChainByName(string.Format("mu_{0}Sample", workerId));
            return workerEst;
        }

        public Object Clone()
        {
            ExposureMetricEstimates clone = new ExposureMetricEstimates(Oel);
            clone.LogNormDist = LogNormDist;
            clone.TargetPerc = TargetPerc;
            clone.MuChain = new double[MuChain.Length];
            MuChain.CopyTo(clone.MuChain, 0);
            clone.SigmaChain = new double[SigmaChain.Length];
            SigmaChain.CopyTo(clone.SigmaChain, 0);
            if ( SigmaWithinChain != null )
            {
                clone.SigmaWithinChain = new double[SigmaWithinChain.Length];
                SigmaWithinChain.CopyTo(clone.SigmaWithinChain, 0);
            }
            
            clone.BWModel = BWModel;
            if ( WorkerIds != null )
            {
                clone.WorkerIds = new string[WorkerIds.Length];
                WorkerIds.CopyTo(clone.WorkerIds, 0);
            }
            return clone;
        }
    }
}
