namespace DCMS.Client.Droid
{
    public class PrinterWriter80mm : PrinterWriter
    {
        // 纸宽80mm
        public static readonly int TYPE_80 = 80;
        public int width = 500;

        public PrinterWriter80mm()
        {
        }

        public PrinterWriter80mm(int parting) : base(parting)
        {
        }
        public PrinterWriter80mm(int parting, int width) : base(parting)
        {
            this.width = width;
        }
        protected override int GetLineWidth()
        {
            return 24;
        }
        protected override int GetLineStringWidth(int textSize)
        {
            switch (textSize)
            {
                default:
                case 0:
                    return 47;
                case 1:
                    return 23;
            }
        }
        protected override int GetDrawableMaxWidth()
        {
            return width;
        }
    }
}