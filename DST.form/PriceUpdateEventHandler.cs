using System;

namespace WindowsFormsApplication3
{
    internal class PriceUpdateEventHandler
    {
        private Action instrUpdate;

        public PriceUpdateEventHandler(Action instrUpdate)
        {
            this.instrUpdate = instrUpdate;
        }
    }
}