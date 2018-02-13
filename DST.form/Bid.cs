using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;

namespace WindowsFormsApplication3
{
    class Bid:massAssign
    {
        private Queue<double> BidPrilist;
        private Queue<int> BidSizelist;
        private Queue<double> AskPrilist;
        private Queue<int> AskSizelist;

        public double[] mass2(Queue<double> AskPrilist, Queue<double> BidPriList, Queue<int> AskSizeList, Queue<int> BidSizelist)
        {

            int Bull = 0;
            int Bear = 0;
            double[] m2 = new double[12];

            //Checking for Depth and Thickness of Bid and Ask Queue

            //BID Depth and Thickness
            if (BidSizelist.ElementAt(0) <= BidSizelist.ElementAt(1))
            {
                Bear++;
            }
            else if (BidSizelist.ElementAt(0) >= BidSizelist.ElementAt(1))
            {
                Bull++;
            }

            if (BidSizelist.ElementAt(1) <= BidSizelist.ElementAt(2))
            {
                Bear++;
            }
            else if (BidSizelist.ElementAt(1) >= BidSizelist.ElementAt(2))
            {
                Bull++;
            }


            //ASK Depth and Thickness
            if (AskSizelist.ElementAt(0) <= AskSizelist.ElementAt(1))
            {
                Bull++;
            }
            else if (AskSizelist.ElementAt(0) >= AskSizelist.ElementAt(1))
            {
                Bear++;
            }

            if (AskSizelist.ElementAt(1) <= AskSizelist.ElementAt(2))
            {
                Bull++;
            }
            else if (AskSizelist.ElementAt(1) >= AskSizelist.ElementAt(2))
            {
                Bear++;
            }

            double AvgAsk = AskPrilist.Average();
            double AvgBid = BidPrilist.Average();

            double AvgSpread = AvgBid / AvgAsk;
            double BestBid = BidPrilist.ElementAt(0);
            double BestAsk = AskPrilist.ElementAt(0);
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

            double BidPriceImprove = (BestBid / BidPrilist.ElementAt(1)) - 1;
            double BidPriceImprove1 = (BidPrilist.ElementAt(1) / BidPrilist.ElementAt(2)) - 1;
            double AvgBidPriceImprove = (BidPriceImprove + BidPriceImprove1) / 2;

            if (BidPriceImprove > AvgBidPriceImprove)
            {
                Bull++;
            }
            else
            {
                Bear++;
            }

            double AskPriceImprove = (BestAsk / AskPrilist.ElementAt(1)) - 1;
            double AskPriceImprove1 = (AskPrilist.ElementAt(1) / AskPrilist.ElementAt(2)) - 1;
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
            double BestBidSize = BidSizelist.ElementAt(0);
            double BidSizeImprove = (BestBidSize / BidSizelist.ElementAt(1)) - 1;
            double BidSizeImprove1 = (BidSizelist.ElementAt(1) / BidSizelist.ElementAt(2)) - 1;
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
            double AskSizeImprove = (BestAskSize / AskSizelist.ElementAt(1)) - 1;
            double AskSizeImprove1 = (AskSizelist.ElementAt(1) / AskSizelist.ElementAt(2)) - 1;
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
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 2 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 3 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 4 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 5 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 6 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 7 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 8 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 9 && Bear == 0)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 1)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 2)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 3)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 4)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 5)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 6)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 7)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 8)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 0 && Bear == 9)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 4 && Bear == 5)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 3 && Bear == 6)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 2 && Bear == 7)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 1 && Bear == 8)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 5 && Bear == 4)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 6 && Bear == 3)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 7 && Bear == 2)
            {
                m2 = massAssignFunc(, , , );
            }
            else if (Bull == 8 && Bear == 1)
            {
                m2 = massAssignFunc(, , , );
            }
            return m2;
      }
    }
}
