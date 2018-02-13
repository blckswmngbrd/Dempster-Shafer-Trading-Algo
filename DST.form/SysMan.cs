using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TradeMatching;
using IBApi;
using System.Diagnostics;

namespace WindowsFormsApplication3
{

    class SysMan
    {
        public Instrument _instr;
        // public Contract contract;
        // public massFusion massFusionFunc;


        public double Ask { get; private set; }
        public double Bid { get; private set; }
        public double Price { get; private set; }
        public int AskQty { get; private set; }
        public int BidQty { get; private set; }

        public int Position { get; private set; }
        //public List<TagValue> Ticklist;

        public Queue<double> BidPriList;
        public Queue<int> BidSizeList;
        public Queue<double> AskPriList;
        public Queue<int> AskSizeList;
        public Queue<double> LastPrice;



        public double MA100_new;
        public double MA100_old;
        public double MA200_new;
        public double MA200_old;
        public double MA500_new;
        public double MA500_old;
        public double k;
        public double temp_hold;
        public int maMaxQ = 50;
        public int maxQ = 4;
        public int arrayLocation;
        public int _PriceChangeCounter;
        public int orderPlaced = 0;
        //public double[] M;
        //public int x;
        //public double highestMass;
        //public double percentageChange;
        //public Fill myPosition;
        //public int InitialPosition;
        //public int RollingPosition;

        private int _Counter;
        private int NetPos;

        private bool _StartUp;

        private TradeMatcher m_Matcher;

        public int OrderId { get; private set; }
        public double CancelOrder { get; private set; }


        //public object DecisionTimer { get; private set; }


        public event RTDupdate OnPriceUpdate;
        public event OrderUpdate OnFillUpdate;

        //  public System.Threading.AutoResetEvent ReEval;
        // private int d_Counter;

        private System.Timers.Timer DecisionTimer;
        


        //public System.Threading.AutoResetEvent get_ReEval()
        //{
        //    return ReEval;
        //}


        public SysMan(Contract contract)
        {
            _Counter = 0;

            _StartUp = true;

            _instr = new Instrument(contract);

            //Ticklist = new List<TagValue>();
            BidPriList = new Queue<double>();
            BidSizeList = new Queue<int>();
            AskPriList = new Queue<double>();
            AskSizeList = new Queue<int>();
            LastPrice = new Queue<double>();

            m_Matcher = new TradeMatcher(RoundTurnMethod.LIFO, 0.01);

            _instr.PriceUpdate += OnInstrUpdate;
            _instr.FillUpdate += OnInstrumentFill;

            DecisionTimer = new System.Timers.Timer(15000);
            DecisionTimer.Elapsed += OnTimedEvent;
            DecisionTimer.AutoReset = true;
            DecisionTimer.Enabled = true;
            DecisionTimer.Start();
        }

        ~SysMan()
        {

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (_PriceChangeCounter > 52)
            {
                // this is where we make mass1, depending on the MAs calculated above
                double[] m1 = new double[12];
                double[] m2 = new double[12];
                m1 = MassFuncs.mass1(MA100_old, MA100_new, MA200_old, MA200_new, MA500_old, MA500_new);

                //    Bid m2m = new Bid();
                m2 = MassFuncs.mass2(AskPriList, BidPriList, AskSizeList, BidSizeList);

                // now we call the DST fusion. You need to insert the other mass function somewhere
                double[] M = new double[12];

                //  MassFuncs.massFusion Mass = new massFusion();
                M = MassFuncs.massFusionFunc(m1, m2);

                //Timer DSTtime = new Timer();


                // Here we figure out what are position decision will be, i.e. increase/decrease and by what percentage
                //percentageChange = percentDecision(M);
                int percentageChange = percentDecision(M);
                for (int i = 0; i < 12; i++)
                    Debug.WriteLine(M[i]);
                Debug.WriteLine("im working nooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo0000000000w" + _Counter + "  " + percentageChange);
                Debug.WriteLine(percentageChange);
                // Here we place the order
                if (percentageChange > 0)
                    _instr.SendOrder("BUY", (Math.Abs(percentageChange)), "MKT");
                if (percentageChange < 0)
                    _instr.SendOrder("SELL", (Math.Abs(percentageChange)), "MKT");
                orderPlaced++;
            }

        }

        private int percentDecision(double[] M)
        {

            double maxValue = M.Max();
            int highestIndx = M.ToList().IndexOf(maxValue);
            return highestIndx * 2 - 10;
        }

        private void OnInstrUpdate()
        {
            _Counter++;
            Bid = _instr.Bid;
            BidQty = _instr.BidQty;
            Ask = _instr.Ask;
            AskQty = _instr.AskQty;
            Price = _instr.Last;
            Debug.WriteLine("counter =" + _Counter + "Bid = " + Bid + "bidqty = " + BidQty + "ask= " + Ask + "askqty=" + AskQty + "last = " + Price);

            //Manage Queue size/limit


            if (maxQ < BidPriList.Count())
            {
                BidPriList.Dequeue();
            }

            if (maxQ < BidSizeList.Count())
            {
                BidSizeList.Dequeue();
            }
            if (maxQ < AskPriList.Count())
            {
                AskPriList.Dequeue();
            }

            if (maxQ < AskSizeList.Count())
            {
                AskSizeList.Dequeue();
            }

            if (maMaxQ < LastPrice.Count())
            {
                LastPrice.Dequeue();
            }


            UpdateQueues();

            /*BidPriList.Enqueue(Bid);
            BidSizeList.Enqueue(BidQty);
            AskPriList.Enqueue(Ask);
            AskSizeList.Enqueue(AskQty);
            LastPrice.Enqueue(Price);*/


            //Function to establish intial position 
            if (_Counter > 10)
                initialSetup();
            // This part checks if we have enough events to build an MA. if we dont have
            // enough events it wont start trading

            if (Price != 0)
            {
                _PriceChangeCounter++;
                arrayLocation = Math.Min(_PriceChangeCounter, 50);
                LastPrice.Enqueue(Price);

                if (_PriceChangeCounter < 10)//100
                {
                    MA100_new += LastPrice.ElementAt(arrayLocation - 1);
                }
                else if (_PriceChangeCounter == 10) //100
                {
                    MA100_new += LastPrice.ElementAt(arrayLocation - 1);
                    MA100_new /= 10;
                }
                else
                {
                    MA100_old = MA100_new;
                    temp_hold = MA100_new;
                    k = 2.0 / 11.0;//101
                    MA100_new = temp_hold * (1.0 - k) + k * LastPrice.ElementAt(arrayLocation - 1);
                }

                if (_PriceChangeCounter < 20) //200
                    MA200_new += LastPrice.ElementAt(arrayLocation - 1);
                else if (_PriceChangeCounter == 20)
                {
                    MA200_new += LastPrice.ElementAt(arrayLocation - 1);
                    MA200_new /= 20;
                }
                else
                {
                    MA200_old = MA200_new;
                    temp_hold = MA200_new;
                    k = 2.0 / 21.0;
                    MA200_new = temp_hold * (1.0 - k) + k * LastPrice.ElementAt(arrayLocation - 1);
                }

                if (_PriceChangeCounter < 50)
                    MA500_new += LastPrice.ElementAt(arrayLocation - 1);
                else if (_PriceChangeCounter == 50)
                {
                    MA500_new += LastPrice.ElementAt(arrayLocation - 1);
                    MA500_new /= 50;
                }
                else
                {
                    MA500_old = MA500_new;
                    k = 2.0 / 51.0;
                    temp_hold = MA500_new;
                    MA500_new = temp_hold * (1.0 - k) + k * LastPrice.ElementAt(arrayLocation - 1);
                }

                Debug.WriteLine("Last price update was: " + LastPrice.ElementAt(arrayLocation - 1));
                Debug.WriteLine("counter of nonzeros at: " + _PriceChangeCounter);
                Debug.WriteLine(MA100_old + " " + MA100_new);
                Debug.WriteLine(MA200_old + " " + MA200_new);
                Debug.WriteLine(MA500_old + " " + MA500_new);

            }
            OnPriceUpdate();
        }

        /*private void OnFillUpdate(Fill f)
        {
            if (f.BS == "B")
                Position += f.Qty;
            else
                Position -= f.Qty;

        }*/

        private void UpdateQueues()
        {
            BidPriList.Enqueue(Bid);
            BidSizeList.Enqueue(BidQty);
            AskPriList.Enqueue(Ask);
            AskSizeList.Enqueue(AskQty);
        }


        private void initialSetup()
        {
            if (_StartUp == true)
            {
                _StartUp = false;
                //_instr.SendOrder("BUY", 1, "MKT");
                //_instr.SendOrder("SELL", 1, "MKT");

                double[] m2IS = new double[12];
                m2IS = MassFuncs.mass2(AskPriList, BidPriList, AskSizeList, BidSizeList);

                int percentageChange = Math.Abs(percentDecision(m2IS));

                // Here we place the order

                if (percentageChange > 0)
                    _instr.SendOrder("BUY", 1, "MKT"); //Using Oddlots to preserve

                if (percentageChange < 0)
                    _instr.SendOrder("SELL", 1, "MKT");
                else if (percentageChange == 0)
                    _instr.SendOrder("BUY", 1, "MKT");

            }

        }

            // build mass function 2
            // place an initial position order based on mass 2
            // keep building EMAs until we reach 500 events

        private void OnInstrumentFill(Fill pFill)
        {
            // Update position.
            if (pFill.BS == TradeType.BUY)
            {
                Position += pFill.Qty;
            }
            else
            {
                Position -= pFill.Qty;
            }

            // Send the data to the TradeMatcher.

            //Need to convert IBApi.Order to int to 
            //IBApi.Order order = new Order();
            //key = order.OrderId; 

            //if (BS == "B")
            //    m_Fill.BS = TradeType.BUY;
            //else
            //    m_Fill.BS = TradeType.SELL;

            //m_Fill.Price = Convert.ToDouble(px);
            //m_Fill.TradeID = key.ToString();
            //m_Fill.Qty = qty;
            m_Matcher.Fill_Received(pFill);
            NetPos = m_Matcher.NetPos;
            OnFillUpdate(pFill);
        }

        /* public void ShutDown()
         {
             m_Go = false;
             Instrument.ShutDown();
             Instrument.PriceUpdate -= new InstrUpdateEventHandler(OnInstrumentUpdate);
             Instrument = null;
         }*/

        public TradeMatcher Matcher
        {
            get { return m_Matcher; }
        }


    }

 }
   
    


//(Math.Abs(100 * percentageChange))
//(Math.Abs(100 * percentageChange))
