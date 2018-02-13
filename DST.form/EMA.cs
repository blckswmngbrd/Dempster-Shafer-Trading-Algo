using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication3
{
    class EMA : massAssign
    {
        public double[] mass1(double MA100_old, double MA100_new, double MA200_old,
              double MA200_new, double MA500_old, double MA500_new)
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
            else
                cross_bear++;
            // 200-500 crossovers;
            if (cross_200_500_prev < cross_200_500_now)
                cross_bull++;
            else
                cross_bear++;
            // 100-500 crossovers;
            if (cross_100_500_prev < cross_100_500_now)
                cross_bull++;
            else
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
                        m1 = massAssignFunc(1, 0.9, 1.5, 0.2);
                    else if (shrinks == 2)
                        m1 = massAssignFunc(1, 1, 1.5, 0.5);
                    else if (shrinks == 1)
                        m1 = massAssignFunc(1, 1, 1.5, 1.5);
                    else
                        m1 = massAssignFunc(1, 1, 1.5, 2.2);
                }
                else if (diff_now_100_200 < 0)
                {
                    if (shrinks == 3)
                        m1 = massAssignFunc(1, -0.9, 1.5, -0.2);
                    else if (shrinks == 2)
                        m1 = massAssignFunc(1, -1, 1.5, -0.5);
                    else if (shrinks == 1)
                        m1 = massAssignFunc(1, -1, 1.5, -1.5);
                    else
                        m1 = massAssignFunc(1, -1, 1.5, -2.2);
                }
            }
            else if (cross_bull == 1 && cross_bear == 0) // one bullish
                m1 = massAssignFunc(.9, 1, 1, -6.5);
            else if (cross_bull == 2 && cross_bear == 0) // two bullish
                m1 = massAssignFunc(.9, 1, 1, -5.5);
            else if (cross_bull == 3 && cross_bear == 0) // three bullish, as good as life gets
                m1 = massAssignFunc(1.2, .9, 1.5, 7);
            else if (cross_bull == 0 && cross_bear == 1) // one bear
                m1 = massAssignFunc(.9, -1, 1, 6.5);
            else if (cross_bull == 0 && cross_bear == 2) // two bear
                m1 = massAssignFunc(.9, -1, 1, 5.5);
            else if (cross_bull == 0 && cross_bear == 3) // three bear, the worst!
                m1 = massAssignFunc(1.2, -.9, 1.5, -7);
            else if (cross_bull == 1 && cross_bear == 1)
                m1 = massAssignFunc(2, 0, 1.5, 0);
            else if (cross_bull == 2 && cross_bear == 1)
                m1 = massAssignFunc(1, 1, 1, 0);
            else if (cross_bull == 1 && cross_bear == 2)
                m1 = massAssignFunc(1, -1, 1, 0);
            return m1;
        }
    }
}
