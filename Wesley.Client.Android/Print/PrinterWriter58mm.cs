namespace Wesley.Client.Droid
{
    public class PrinterWriter58mm : PrinterWriter
    {
        //默认纸宽58mm
        public static readonly int TYPE_58 = 58;
        public int width = 380;

        public PrinterWriter58mm()
        {
        }

        public PrinterWriter58mm(int parting) : base(parting)
        {
        }

        public PrinterWriter58mm(int parting, int width) : base(parting)
        {
            this.width = width;
        }


        protected override int GetLineWidth()
        {
            return 16;
        }


        protected override int GetLineStringWidth(int textSize)
        {
            switch (textSize)
            {
                default:
                case 0:
                    return 31;
                case 1:
                    return 15;
            }
        }

        protected override int GetDrawableMaxWidth()
        {
            return width;
        }
    }
}