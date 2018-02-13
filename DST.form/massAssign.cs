using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using IBApi;

namespace WindowsFormsApplication3
{
    class massAssign
    {
        public double[] massAssignFunc(double stability, double skew, double scale, double location)
        {
            double[] m1 = new double[12];
            double sum = 0;
            for (int i = 0; i < 11; i++)
            {
                m1[i] = Stable.PDF(stability, skew, scale, location, 2 * i - 9) -
                    Stable.PDF(stability, skew, scale, location, 2 * i - 11);
                sum += m1[i];
            }
            m1[11] = 1.0 - sum;
            return m1;
        }
    }
}
