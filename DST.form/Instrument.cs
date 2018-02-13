using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;
using TradeMatching;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApplication3
{

    public delegate void RTDupdate();
    public delegate void OrderUpdate(Fill fill);

    public class Instrument : EWrapper
    {

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        private EClientSocket _clientSocket;
        private int _nextOrderId;
        private Contract _contract;

        public event RTDupdate PriceUpdate;
        public event OrderUpdate FillUpdate;

        private static int instances = 1000;

        public Instrument(Contract c)
        {
            instances++;

            _contract = c;

            Thread _RTDthread = new Thread(new ThreadStart(Connect));
            _RTDthread.Start();
        }

        private void Connect()
        {
            _clientSocket = new EClientSocket(this);

            _clientSocket.eConnect("127.0.0.1", 7497, 0, false);

            _clientSocket.reqMarketDataType(2);
            _clientSocket.reqMktData(instances, _contract, "", false, GetFakeParameters(3));
        }
        public void ShutDown()
        {
            _clientSocket.cancelMktData(1002);

            Debug.WriteLine("Done");

            _clientSocket.eDisconnect();
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="numParams"></param>
        /// <returns></returns>

        private List<TagValue> GetFakeParameters(int numParams)
        {
            List<TagValue> fakeParams = new List<TagValue>();
            for (int i = 0; i < numParams; i++)
                fakeParams.Add(new TagValue("tag" + i, i.ToString()));
            return fakeParams;
        }
        public EClientSocket ClientSocket
        {
            get { return _clientSocket; }
            set { _clientSocket = value; }
        }
        public virtual void error(Exception e)
        {
            Debug.WriteLine("Exception thrown: " + e);
            //throw e;
        }
        public virtual void error(string str)
        {
            Debug.WriteLine("Error: " + str + "\n");
        }
        public virtual void error(int id, int errorCode, string errorMsg)
        {
            Debug.WriteLine("Error. Id: " + id + ", Code: " + errorCode + ", Msg: " + errorMsg + "\n");
        }
        public virtual void connectionClosed()
        {
            Debug.WriteLine("Connection closed.\n");
        }
        public virtual void currentTime(long time)
        {
            Debug.WriteLine("Current Time: " + time + "\n");
        }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public int BidQty { get; private set; }
        public double Bid { get; private set; }
        public int AskQty { get; private set; }
        public double Ask { get; private set; }
        public int LastQty { get; private set; }
        public double Last { get; private set; }
        

        public virtual void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            switch (field)
            {
                case 1:
                    Bid = price;
                    PriceUpdate();
                    break;
                case 2:
                    Ask = price;
                    PriceUpdate();
                    break;
                case 4:
                    Last = price;
                    PriceUpdate();
                    break;
            }
            Debug.WriteLine("Tick Price. Ticker Id:" + tickerId + ", Field: " + field + ", Price: " + price + ", CanAutoExecute: " + canAutoExecute + "\n");

            PriceUpdate();
        }

        public virtual void tickSize(int tickerId, int field, int size)
        {
            switch (field)
            {
                case 0:
                    BidQty = size;
                    //B.BidSizeList.Enqueue(BidQty);
                    PriceUpdate();
                    break;
                case 3:
                    AskQty = size;
                    //B.AskSizeList.Enqueue(AskQty);
                    PriceUpdate();
                    break;
                case 5:
                    LastQty = size;
                    break;
            }
            Debug.WriteLine("Tick Size. Ticker Id:" + tickerId + ", Field: " + field + ", Size: " + size + "\n");

            PriceUpdate();
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>


        public virtual void tickString(int tickerId, int tickType, string value)
        {
            Debug.WriteLine("Tick string. Ticker Id:" + tickerId + ", Type: " + tickType + ", Value: " + value + "\n");
        }

        public virtual void tickGeneric(int tickerId, int field, double value)
        {
            Debug.WriteLine("Tick Generic. Ticker Id:" + tickerId + ", Field: " + field + ", Value: " + value + "\n");
        }

        public virtual void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureExpiry, double dividendImpact, double dividendsToExpiry)
        {
            Debug.WriteLine("TickEFP. " + tickerId + ", Type: " + tickType + ", BasisPoints: " + basisPoints + ", FormattedBasisPoints: " + formattedBasisPoints + ", ImpliedFuture: " + impliedFuture + ", HoldDays: " + holdDays + ", FutureExpiry: " + futureExpiry + ", DividendImpact: " + dividendImpact + ", DividendsToExpiry: " + dividendsToExpiry + "\n");
        }

        public virtual void tickSnapshotEnd(int tickerId)
        {
            Debug.WriteLine("TickSnapshotEnd: " + tickerId + "\n");
        }

        public virtual void nextValidId(int orderId)
        {
            Debug.WriteLine("Next Valid Id: " + orderId + "\n");
            _nextOrderId = orderId;
        }

        public virtual void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            Debug.WriteLine("DeltaNeutralValidation. " + reqId + ", ConId: " + underComp.ConId + ", Delta: " + underComp.Delta + ", Price: " + underComp.Price + "\n");
        }

        public virtual void managedAccounts(string accountsList)
        {
            Debug.WriteLine("Account list: " + accountsList + "\n");
        }

        public virtual void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            Debug.WriteLine("TickOptionComputation. TickerId: " + tickerId + ", field: " + field + ", ImpliedVolatility: " + impliedVolatility + ", Delta: " + delta
                + ", OptionPrice: " + optPrice + ", pvDividend: " + pvDividend + ", Gamma: " + gamma + ", Vega: " + vega + ", Theta: " + theta + ", UnderlyingPrice: " + undPrice + "\n");
        }

        public virtual void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            Debug.WriteLine("Acct Summary. ReqId: " + reqId + ", Acct: " + account + ", Tag: " + tag + ", Value: " + value + ", Currency: " + currency + "\n");
        }

        public virtual void accountSummaryEnd(int reqId)
        {
            Debug.WriteLine("AccountSummaryEnd. Req Id: " + reqId + "\n");
        }

        public virtual void updateAccountValue(string key, string value, string currency, string accountName)
        {
            Debug.WriteLine("UpdateAccountValue. Key: " + key + ", Value: " + value + ", Currency: " + currency + ", AccountName: " + accountName + "\n");
        }

        public virtual void updatePortfolio(Contract contract, int position, double marketPrice, double marketValue, double averageCost, double unrealisedPNL, double realisedPNL, string accountName)
        {
            Debug.WriteLine("UpdatePortfolio. " + contract.Symbol + ", " + contract.SecType + " @ " + contract.Exchange
                + ": Position: " + position + ", MarketPrice: " + marketPrice + ", MarketValue: " + marketValue + ", AverageCost: " + averageCost
                + ", UnrealisedPNL: " + unrealisedPNL + ", RealisedPNL: " + realisedPNL + ", AccountName: " + accountName + "\n");
        }

        public virtual void updateAccountTime(string timestamp)
        {
            Debug.WriteLine("UpdateAccountTime. Time: " + timestamp + "\n");
        }

        public virtual void accountDownloadEnd(string account)
        {
            Debug.WriteLine("Account download finished: " + account + "\n");
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public int SendOrder(String BS, int qty, String type, double limitprice = 0.0)
        {
            _nextOrderId++;

            Order order = new Order();
            order.OrderId = _nextOrderId;
            order.Action = BS;
            order.OrderType = type;
            if (type == "LMT")
                order.LmtPrice = Bid + 0.01; // double
            order.TotalQuantity = qty;
            //order.Account = "DI555555";
            order.Tif = "GTD";
            order.GoodTillDate = DateTime.Now.AddSeconds(30).ToString("yyyyMMdd HH:mm:ss");

            //openOrder( _nextOrderId, _contract, order, null );
            _clientSocket.placeOrder(_nextOrderId, _contract, order);
            return _nextOrderId;
        }

        public void CancelOrder(int orderId)
        {
            _clientSocket.cancelOrder(orderId);
        }

        public virtual void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            Debug.WriteLine("Open Order. ID: " + orderId + ", " + contract.Symbol + ", " + contract.SecType + " @ " + contract.Exchange + ": " + order.Action + ", " + order.OrderType + " " + order.TotalQuantity + "\n");
            //clientSocket.reqMktData(2, contract, "", false);
            //contract.ConId = 0;
            //_clientSocket.placeOrder( orderId, contract, order );
        }

        public virtual void execDetails(int reqId, Contract contract, Execution execution)
        {
            Debug.WriteLine("ExecDetails. " + reqId + " - " + contract.Symbol + ", " + contract.SecType + ", " + contract.Currency + " - " + execution.ExecId + ", " + execution.OrderId + ", " + execution.Shares + "\n");

            Fill fill = new Fill();
            fill.TradeID = execution.OrderId.ToString();
            //fill.Status = "FILLED";
            fill.Qty = execution.Shares;
            fill.Price = execution.Price;
            if (execution.Side == "BOT")
                fill.BS = TradeType.BUY;
            else
                fill.BS = TradeType.SELL;
            fill.Symbol = contract.Symbol;

            fill.Time = DateTime.Now; //DateTime.Parse(execution.Time);

            FillUpdate(fill);
        }

        public virtual void orderStatus(int orderId, string status, int filled, int remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
            Debug.WriteLine("Order Status. Id: " + orderId + ", Status: " + status + ", Filled: " + filled + ", Remaining: " + remaining
                + ", AvgFillPrice: " + avgFillPrice + ", PermId: " + permId + ", ParentId: " + parentId + ", LastFillPrice: " + lastFillPrice + ", ClientId: " + clientId + ", WhyHeld: " + whyHeld + "\n");
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public virtual void openOrderEnd()
        {
            Debug.WriteLine("OpenOrderEnd");
        }

        public virtual void contractDetails(int reqId, ContractDetails contractDetails)
        {
            Debug.WriteLine("ContractDetails. ReqId: " + reqId + " - " + contractDetails.Summary.Symbol + ", " + contractDetails.Summary.SecType + ", ConId: " + contractDetails.Summary.ConId + " @ " + contractDetails.Summary.Exchange + "\n");
        }

        public virtual void contractDetailsEnd(int reqId)
        {
            Debug.WriteLine("ContractDetailsEnd. " + reqId + "\n");
        }

        public virtual void execDetailsEnd(int reqId)
        {
            Debug.WriteLine("ExecDetailsEnd. " + reqId + "\n");
        }

        public virtual void commissionReport(CommissionReport commissionReport)
        {
            Debug.WriteLine("CommissionReport. " + commissionReport.ExecId + " - " + commissionReport.Commission + " " + commissionReport.Currency + " RPNL " + commissionReport.RealizedPNL + "\n");
        }

        public virtual void fundamentalData(int reqId, string data)
        {
            Debug.WriteLine("FundamentalData. " + reqId + "" + data + "\n");
        }

        public virtual void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
            Debug.WriteLine("HistoricalData. " + reqId + " - Date: " + date + ", Open: " + open + ", High: " + high + ", Low: " + low + ", Close: " + close + ", Volume: " + volume + ", Count: " + count + ", WAP: " + WAP + ", HasGaps: " + hasGaps + "\n");
        }

        public virtual void marketDataType(int reqId, int marketDataType)
        {
            Debug.WriteLine("MarketDataType. " + reqId + ", Type: " + marketDataType + "\n");
        }

        public virtual void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            Debug.WriteLine("UpdateMarketDepth. " + tickerId + " - Position: " + position + ", Operation: " + operation + ", Side: " + side + ", Price: " + price + ", Size" + size + "\n");
        }

        public virtual void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            Debug.WriteLine("UpdateMarketDepthL2. " + tickerId + " - Position: " + position + ", Operation: " + operation + ", Side: " + side + ", Price: " + price + ", Size" + size + "\n");
        }

        public virtual void updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
            Debug.WriteLine("News Bulletins. " + msgId + " - Type: " + msgType + ", Message: " + message + ", Exchange of Origin: " + origExchange + "\n");
        }

        public virtual void position(string account, Contract contract, int pos, double avgCost)
        {
            Debug.WriteLine("Position. " + account + " - Symbol: " + contract.Symbol + ", SecType: " + contract.SecType + ", Currency: " + contract.Currency + ", Position: " + pos + ", Avg cost: " + avgCost + "\n");
        }

        public virtual void positionEnd()
        {
            Debug.WriteLine("PositionEnd \n");
        }

        public virtual void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            Debug.WriteLine("RealTimeBars. " + reqId + " - Time: " + time + ", Open: " + open + ", High: " + high + ", Low: " + low + ", Close: " + close + ", Volume: " + volume + ", Count: " + count + ", WAP: " + WAP + "\n");
        }

        public virtual void scannerParameters(string xml)
        {
            Debug.WriteLine("ScannerParameters. " + xml + "\n");
        }

        public virtual void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            Debug.WriteLine("ScannerData. " + reqId + " - Rank: " + rank + ", Symbol: " + contractDetails.Summary.Symbol + ", SecType: " + contractDetails.Summary.SecType + ", Currency: " + contractDetails.Summary.Currency
                + ", Distance: " + distance + ", Benchmark: " + benchmark + ", Projection: " + projection + ", Legs String: " + legsStr + "\n");
        }

        public virtual void scannerDataEnd(int reqId)
        {
            Debug.WriteLine("ScannerDataEnd. " + reqId + "\n");
        }

        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            Debug.WriteLine("Receing FA: " + faDataType + " - " + faXmlData + "\n");
        }

        public virtual void bondContractDetails(int requestId, ContractDetails contractDetails)
        {
            Debug.WriteLine("Bond. Symbol " + contractDetails.Summary.Symbol + ", " + contractDetails.Summary);
        }

        public virtual void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            Debug.WriteLine("Historical data end - " + reqId + " from " + startDate + " to " + endDate);
        }

        public virtual void verifyMessageAPI(string apiData)
        {
            Debug.WriteLine("verifyMessageAPI: " + apiData);
        }
        public virtual void verifyCompleted(bool isSuccessful, string errorText)
        {
            Debug.WriteLine("verifyCompleted. IsSuccessfule: " + isSuccessful + " - Error: " + errorText);
        }
        public virtual void displayGroupList(int reqId, string groups)
        {
            Debug.WriteLine("DisplayGroupList. Request: " + reqId + ", Groups" + groups);
        }
        public virtual void displayGroupUpdated(int reqId, string contractInfo)
        {
            Debug.WriteLine("displayGroupUpdated. Request: " + reqId + ", ContractInfo: " + contractInfo);
        }
    }
}
