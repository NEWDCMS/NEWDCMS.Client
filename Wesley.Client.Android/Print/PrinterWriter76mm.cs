namespace Wesley.Client.Droid
{
    public class PrinterWriter76mm : PrinterWriter
    {
        // 纸宽76mm
        public static readonly int TYPE_76 = 76;
        public int width = 420;

        public PrinterWriter76mm()
        {
        }

        public PrinterWriter76mm(int parting) : base(parting)
        {
        }

        public PrinterWriter76mm(int parting, int width) : base(parting)
        {
            this.width = width;
        }


        protected override int GetLineWidth()
        {
            return 20;
        }


        protected override int GetLineStringWidth(int textSize)
        {
            switch (textSize)
            {
                default:
                case 0:
                    return 35;
                case 1:
                    return 18;
            }
        }

        protected override int GetDrawableMaxWidth()
        {
            return width;
        }
    }
}