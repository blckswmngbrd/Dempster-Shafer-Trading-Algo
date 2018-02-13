using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using IBApi;
using System.Diagnostics;

namespace WindowsFormsApplication3
{
    class MassFuncs
    {
        private static double[] massAssignFunc(double alpha, double beta, double mu, double flip)
        {

            double[] m1 = new double[12];
            double sum = 0;
            for (int i = 0; i < 11; i++)
            {
                double b = 2.0 * i - 9.0;
                double a = 2.0 * i - 11.0;
                if (a <= mu && b > mu)
                    a = mu + .00001;
                else if (b <= mu)
                {
                    a = mu + .00001;
                    b = mu + .00001;
                }
                    
                m1[i] = Math.Exp(-Math.Pow((b - mu) / beta, -alpha)) - Math.Exp(-Math.Pow((a - mu) / beta, -alpha));
                sum += m1[i];
            }
            m1[11] = 1.0 - sum;
            if(flip == 1)
                Array.Reverse(m1,0,11);
            return m1;
        }
        public static double[] massFusionFunc(double[] m1, double[] m2)
        {
            double kappa = 0;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (i != j)
                    {
                        kappa += m1[i] * m2[j];
                    }
                }
            }
            double K = 1.0 / (1.0 - kappa);
            // this part does the fusion
            double[] M = new double[12];
            for (int i = 1; i < 11; i++)
            {
                M[i] = K * m1[i] * m2[i] + m1[i] * m2[11] + m1[11] * m2[i];
            }
            M[11] = K * m1[11] * m2[11];
            return M;
        }

        public static double[] mass2( Queue< double > AskPriList, Queue< double > BidPriList, Queue< int > AskSizeList, Queue< int > BidSizeList )
        {

            int Bull = 0;
            int Bear = 0;
            double[] m2 = new double[12];

            //Checking for Depth and Thickness of Bid and Ask Queue

            //BID Depth and Thickness
            if (BidSizeList.ElementAt(0) <= BidSizeList.ElementAt(1))
            {
                Bear++;
            }
            else if (BidSizeList.ElementAt(0) >= BidSizeList.ElementAt(1))
            {
                Bull++;
            }

            if (BidSizeList.ElementAt(1) <= BidSizeList.ElementAt(2))
            {
                Bear++;
            }
            else if (BidSizeList.ElementAt(1) >= BidSizeList.ElementAt(2))
            {
                Bull++;
            }


            //ASK Depth and Thickness
            if ( AskSizeList.ElementAt(0) <= AskSizeList.ElementAt(1))
            {
                Bull++;
            }
            else if (AskSizeList.ElementAt(0) >= AskSizeList.ElementAt(1))
            {
                Bear++;
            }

            if (AskSizeList.ElementAt(1) <= AskSizeList.ElementAt(2))
            {
                Bull++;
            }
            else if (AskSizeList.ElementAt(1) >= AskSizeList.ElementAt(2))
            {
                Bear++;
            }

            double AvgAsk = AskPriList.Average();
            double AvgBid = BidPriList.Average();

            double AvgSpread = AvgBid / AvgAsk;
            double BestBid = BidPriList.ElementAt(0);
            double BestAsk = AskPriList.ElementAt(0);
            double Spread = BestAsk - BestBid;

            //Evalaution of the Bid Ask Spread 

            if (Spread > AvgSpread)
            {
                Bear++;
            }
            else //if (Spread < AvgSpread)
            {
                Bull++;
            }

            //Check for Percentage of Price improvement on Both Sides 

            double BidPriceImprove;
            double BidPriceImprove1;
            

            if (BidPriList.ElementAt(1) == 0)
            {
                BidPriceImprove = Math.Sign(BestBid - BidPriList.ElementAt(1));

            }
            else
            {
                BidPriceImprove = Math.Sign((BestBid / BidPriList.ElementAt(1)) - 1);
            }

            if (BidPriList.ElementAt(2) == 0)
            {
                BidPriceImprove1 = Math.Sign((BidPriList.ElementAt(1) - BidPriList.ElementAt(2)));
            }
            else
            {
                BidPriceImprove1 = Math.Sign((BidPriList.ElementAt(1) / BidPriList.ElementAt(2)) - 1);
            }

            double AvgBidPriceImprove = (BidPriceImprove1 + BidPriceImprove) / 2;


            if (BidPriceImprove > AvgBidPriceImprove)
            {
                Bull++;
            }
            else
            {
                Bear++;
            }

            double AskPriceImprove;
            double AskPriceImprove1;

            if (AskPriList.ElementAt(1) == 0)
            {
                AskPriceImprove = Math.Sign(BestAsk - AskPriList.ElementAt(1));

            }
            else
            {
                AskPriceImprove = Math.Sign((BestAsk / AskPriList.ElementAt(1)) - 1);
            }

            if (AskPriList.ElementAt(2) == 0)
            {
                AskPriceImprove1 = Math.Sign((AskPriList.ElementAt(1) - AskPriList.ElementAt(2)));
            }
            else
            {
                AskPriceImprove1 = Math.Sign((AskPriList.ElementAt(1) / AskPriList.ElementAt(2)) - 1);
            }

            double AvgAskPriceImprove = (AskPriceImprove1 + AskPriceImprove) / 2;

            if (Math.Abs(AskPriceImprove) > Math.Abs(AvgAskPriceImprove))
            {
                Bear++;
            }
            else
            {
                Bull++;
            }


            //Check for degree of increase in size 
            double BestBidSize = BidSizeList.ElementAt(0);
            double BidSizeImprove;
            double BidSizeImprove1;

            if (BidSizeList.ElementAt(1)==0)
            {
                BidSizeImprove = Math.Sign(BestBidSize - BidSizeList.ElementAt(1));
            }else
            {
                BidSizeImprove = Math.Sign((BestBidSize / BidSizeList.ElementAt(1)) - 1);
            }

            if(BidSizeList.ElementAt(2)==0)
            {
               BidSizeImprove1 = Math.Sign(BidSizeList.ElementAt(1)-BidSizeList.ElementAt(2));
            } else
            {
               BidSizeImprove1 = Math.Sign((BidSizeList.ElementAt(1) / BidSizeList.ElementAt(2)) - 1);
            }
            
            double AvgBidSizeImprove = (BidSizeImprove + BidSizeImprove1) / 2;

            if (BidSizeImprove > AvgBidSizeImprove)
            {
                Bull++;
            }
            else
            {
                Bear++;
            }

            double BestAskSize = AskSizeList.ElementAt(0);
            double AskSizeImprove;
            double AskSizeImprove1;

            if (AskSizeList.ElementAt(1) == 0)
            {
                AskSizeImprove =Math.Sign(BestAskSize - AskSizeList.ElementAt(1));
                
            }else
            {
                AskSizeImprove =Math.Sign((BestAskSize / AskSizeList.ElementAt(1)) - 1);
            }

            if(AskSizeList.ElementAt(2)==0)
            {
                AskSizeImprove1 = Math.Sign((AskSizeList.ElementAt(1) - AskSizeList.ElementAt(2)));
            } else
            {
                AskSizeImprove1 =Math.Sign((AskSizeList.ElementAt(1)/AskSizeList.ElementAt(2)) - 1);
            }           
             
            double AvgAskSizeImprove = (AskSizeImprove1 + AskSizeImprove) / 2;

            if (AskSizeImprove > AvgAskSizeImprove)
            {
                Bear++;
            }
            else
            {
                Bull++;
            }

            if (Bull == 1 && Bear == 0)
            {
                m2 = massAssignFunc(1.11537, 4, -2.8, 0);
            }
            else if (Bull == 2 && Bear == 0)
            {
                m2 = massAssignFunc(1.2393, 4, -2.8, 0);
            }
            else if (Bull == 3 && Bear == 0)
            {
                m2 = massAssignFunc(1.377, 4, -2.8, 0);
            }
            else if (Bull == 4 && Bear == 0)
            {
                m2 = massAssignFunc(1.53, 4, -2.8, 0);
            }
            else if (Bull == 5 && Bear == 0)
            {
                m2 = massAssignFunc(1.7, 4, -2.8, 0);
            }
            else if (Bull == 6 && Bear == 0)
            {
                m2 = massAssignFunc(1.87, 4, -2.8, 0);
            }
            else if (Bull == 7 && Bear == 0)
            {
                m2 = massAssignFunc(2.057, 4, -2.8, 0);
            }
            else if (Bull == 8 && Bear == 0)
            {
                m2 = massAssignFunc(2.2627, 4, -2.8, 0);
            }
            else if (Bull == 9 && Bear == 0)
            {
                m2 = massAssignFunc(2.48897, 4, -2.8, 0);
            }
            else if (Bull == 0 && Bear == 1)
            {
                m2 = massAssignFunc(1.11537, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 2)
            {
                m2 = massAssignFunc(1.2393, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 3)
            {
                m2 = massAssignFunc(1.377, 4, -2.8, -1);
            }
            else if (Bull == 0 && Bear == 4)
            {
                m2 = massAssignFunc(1.53, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 5)
            {
                m2 = massAssignFunc(1.7, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 6)
            {
                m2 = massAssignFunc(1.87, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 7)
            {
                m2 = massAssignFunc(2.057, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 8)
            {
                m2 = massAssignFunc(2.2627, 4, -2.8, 1);
            }
            else if (Bull == 0 && Bear == 9)
            {
                m2 = massAssignFunc(2.48897, 4, -2.8, 1);
            }
            else if (Bull == 4 && Bear == 5)
            {
                m2 = massAssignFunc(1.4, 3.5, -2.1, 1);
            }
            else if (Bull == 3 && Bear == 6)
            {
                m2 = massAssignFunc(1.7, 3, -1.9, 1);
            }
            else if (Bull == 2 && Bear == 7)
            {
                m2 = massAssignFunc(2, 2.5, -1.6, 1);
            }
            else if (Bull == 1 && Bear == 8)
            {
                m2 = massAssignFunc(2.3, 2, -1.3, 1);
            }
            else if (Bull == 5 && Bear == 4)
            {
                m2 = massAssignFunc(1.4, 3.5, -2.1, 0);
            }
            else if (Bull == 6 && Bear == 3)
            {
                m2 = massAssignFunc(1.7, 3, -1.9, 0);
            }
            else if (Bull == 7 && Bear == 2)
            {
                m2 = massAssignFunc(2, 2.5, -1.6, 0);
            }
            else if (Bull == 8 && Bear == 1)
            {
                m2 = massAssignFunc(2.3, 2, -1.3, 0);
            }

            Debug.WriteLine("m2_bull: " + Bull + " m2_bear: " + Bear);
             return m2;
        }
        public static double[] mass1(double MA100_old, double MA100_new, double MA200_old, double MA200_new, double MA500_old, double MA500_new)
        {
            double[] EMAs = new double[6];
            EMAs[0] = MA100_old;
            EMAs[1] = MA100_new;
            EMAs[2] = MA200_old;
            EMAs[3] = MA200_new;
            EMAs[4] = MA500_old;
            EMAs[5] = MA500_new;

            double[] m1 = new double[12];
            int cross_bull = 0;
            int cross_bear = 0;
            // checking for crossover types
            double cross_100_200_prev = Math.Sign(EMAs[0] - EMAs[2]);
            double cross_100_200_now = Math.Sign(EMAs[1] - EMAs[3]);
            double cross_200_500_prev = Math.Sign(EMAs[2] - EMAs[4]);
            double cross_200_500_now = Math.Sign(EMAs[3] - EMAs[5]);
            double cross_100_500_prev = Math.Sign(EMAs[0] - EMAs[4]);
            double cross_100_500_now = Math.Sign(EMAs[1] - EMAs[5]);

            // 100-200 crossovers

            if (cross_100_200_prev < cross_100_200_now)
                cross_bull++;
            else if(cross_100_200_prev > cross_100_200_now)
                cross_bear++;
            // 200-500 crossovers;
            if (cross_200_500_prev < cross_200_500_now)
                cross_bull++;
            else if(cross_200_500_prev > cross_200_500_now)
                cross_bear++;
            // 100-500 crossovers;
            if (cross_100_500_prev < cross_100_500_now)
                cross_bull++;
            else if(cross_100_500_prev > cross_100_500_now)
                cross_bear++;

            // now we check the possible scenarios
            if (cross_bull == 0 && cross_bear == 0)
            {
                double diff_now_100_200 = EMAs[1] - EMAs[3];
                double diff_prev_100_200 = EMAs[0] - EMAs[2];
                double diff_now_200_500 = EMAs[3] - EMAs[5];
                double diff_prev_200_500 = EMAs[2] - EMAs[4];
                double diff_now_100_500 = EMAs[1] - EMAs[5];
                double diff_prev_100_500 = EMAs[0] - EMAs[4];

                // looking at the interval lenghts
                double shrinks = Math.Sign(diff_prev_100_200 - diff_now_100_200)
                                + Math.Sign(diff_prev_200_500 - diff_now_200_500)
                                + Math.Sign(diff_prev_100_500 - diff_now_100_500);
                if (diff_now_100_200 > 0)
                {
                    if (shrinks == 3)
                        m1 = MassFuncs.massAssignFunc(3, 7.5, -7, 0);
                    else if (shrinks == 2)
                        m1 = MassFuncs.massAssignFunc(3, 5, -5, 0);
                    else if (shrinks == 1)
                        m1 = MassFuncs.massAssignFunc(3, 5, -4, 0);
                    else
                        m1 = MassFuncs.massAssignFunc(3, 3, -2, 0);
                }
                else if (diff_now_100_200 < 0)
                {
                    if (shrinks == 3)
                        m1 = MassFuncs.massAssignFunc(3, 7.5, -7, 1);
                    else if (shrinks == 2)
                        m1 = MassFuncs.massAssignFunc(3, 5, -5, 1);
                    else if (shrinks == 1)
                        m1 = MassFuncs.massAssignFunc(3, 5, -4, 1);
                    else
                        m1 = MassFuncs.massAssignFunc(3, 3, -2, 1);
                }
            }
            else if (cross_bull == 1 && cross_bear == 0) // one bullish
                m1 = MassFuncs.massAssignFunc(3, 5, -4.5, 0);
            else if (cross_bull == 2 && cross_bear == 0) // two bullish
                m1 = MassFuncs.massAssignFunc(3, 2, -2.5, 0);
            else if (cross_bull == 3 && cross_bear == 0) // three bullish, as good as life gets
                m1 = MassFuncs.massAssignFunc(2, 4, -2, 0);
            else if (cross_bull == 0 && cross_bear == 1) // one bear
                m1 = MassFuncs.massAssignFunc(3, 5, -4.5, 1);
            else if (cross_bull == 0 && cross_bear == 2) // two bear
                m1 = MassFuncs.massAssignFunc(3, 2, -2.5, 1);
            else if (cross_bull == 0 && cross_bear == 3) // three bear, the worst!
                m1 = MassFuncs.massAssignFunc(2, 4, -2, 1);
            else if (cross_bull == 1 && cross_bear == 1)
                m1 = MassFuncs.massAssignFunc(8,18,-18,0);
            else if (cross_bull == 2 && cross_bear == 1)
                m1 = MassFuncs.massAssignFunc(1,1,-2,0);
            else if (cross_bull == 1 && cross_bear == 2)
                m1 = MassFuncs.massAssignFunc(1, 1, -2, 1);

            Debug.WriteLine("m1 bull: " + cross_bull + "m1 bear: " + cross_bear);
            return m1;
            
        }
    }
}
