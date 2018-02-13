using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;

namespace WindowsFormsApplication3
{
    class massFusion
    {
        public double[] massFusionFunc(double[] m1, double[] m2)
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
    }
}
