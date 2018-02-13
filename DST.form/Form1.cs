using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Timers; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeMatching;
using IBApi;


namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        private SysMan _Manager;

        public Form1()
        {
            InitializeComponent();
        }



       
        /*private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1 += "Mass Function: " + 
        }*/

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //public System.Threading.AutoResetEvent ReEval;


        private void button1_Click(object sender, EventArgs e)
        {
            Contract contract = new Contract();
            contract.Symbol = ContractInput.Text;
            contract.SecType = SecTypeInput.Text;
            contract.Currency = CurrencyInput.Text;
            contract.Exchange = ExchangeInput.Text;

            SecTypeInput.Text = contract.SecType;
            CurrencyInput.Text = contract.Currency;
            ExchangeInput.Text = contract.Exchange;
            // Create a new Instrument
            _Manager = new SysMan(contract);

            // Subscribe to the PriceUpdate event for real time data.
            _Manager.OnPriceUpdate += InstrUpdate;

            // Subscribe to the FillUpdate event for fill updates.
            _Manager.OnFillUpdate += FillUpdate;

            //dataGridView1.DataSource = _Manager.Matcher.SellTable;
            //dataGridView2.DataSource = _Manager.Matcher.BuyTable;
            //dataGridView3.DataSource = _Manager.Matcher.RoundTurnTable;
            //_Manager.PriceUpdate += percentDecision;


        }
        private void InstrUpdate()
        {
            // Cross thread invoke.
            this.Invoke(new Action(() => this.GUIUpdate()));

        }

        private void GUIUpdate()
        {

            textBox5.Text = _Manager.Price.ToString() + Environment.NewLine;
            //textBox1.Text += " M1: " + _Manager.M[1]
            //    + " M2: " + _Manager.M[2].ToString()
            //    + " M3: " + _Manager.M[3].ToString()
            //    + " M4: " + _Manager.M[4].ToString()
            //    + " M5: " + _Manager.M[5].ToString()
            //    + " M6: " + _Manager.M[6].ToString()
            //    + " M7: " + _Manager.M[7].ToString()
            //    + " M8: " + _Manager.M[8].ToString()
            //    + " M9: " + _Manager.M[9].ToString()
            //    + " M10: " + _Manager.M[10].ToString()
            //    + " M11: " + _Manager.M[11].ToString()
            //    + " M12: " + _Manager.M[12].ToString() +
            //    Environment.NewLine;

            //     _instr.SendOrder("BUY", (Math.Abs(Position * percentageChange) - Position), "MKT");
            // if (_Manager. < 0)
            //     _instr.SendOrder("SELL", (Math.Abs(Position * percentageChange) - Position), "MKT");

            // Update the GUI with real time data.
            // textBox1.Text += "BidQty"+ _instr.BidQty.ToString()+
            // "Bid"+ _instr.Bid.ToString()+"Ask"+ _instr.Ask.ToString()+
            // "AskQty"+_instr.AskQty.ToString()+"Last"+ _instr.Last.ToString()+
            //"LastQty"+ _instr.LastQty.ToString();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //Stop Timer and Shutdown system
            //Timer DSTend = new Timer();
            //DSTend.DecisionTimer.Stop();
            //Instrument dst = new Instrument( );

          

        }

        private void FillUpdate(Fill fill)
        {
            // Cross thread invoke.
            this.Invoke(new Action(() => this.GUIFillUpdate(fill)));
        }

        private void GUIFillUpdate(Fill fill)
        {
            // Get the fill information and update the GUI.
            textBox4.Text += "OrderId: " + fill.TradeID +
                "   Symbol: " + fill.Symbol +
                "   BS: " + fill.BS +
                //"   Status: " + fill.Status +
                "   Filled: " + fill.Qty +
                "   Price: " + fill.Price +
                "   Time: " + fill.Time +
                Environment.NewLine;

        }



        private void retrieveContract_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.ContractInput.Text);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (_Manager != null)
            {
                _Manager.Matcher.WriteBuys("C:\\temp\\buys.csv");
                _Manager.Matcher.WriteSells("C:\\temp\\sells.csv");
                _Manager.Matcher.WriteRoundTurns("C:\\temp\\roundturns.csv");

                _Manager.OnFillUpdate -= FillUpdate;
                _Manager.OnPriceUpdate -= InstrUpdate;

                _Manager = null;
                GC.Collect();

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

}
