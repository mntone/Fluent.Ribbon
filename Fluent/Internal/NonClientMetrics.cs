namespace Fluent.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal struct NonClientMetrics
    {
        public int cbSize;
        public int iBorderWidth;
        public int iScrollWidth;
        public int iScrollHeight;
        public int iCaptionWidth;
        public int iCaptionHeight;
        public LogFont lfCaptionFont;
        public int iSMCaptionWidth;
        public int iSMCaptionHeight;
        public LogFont lfSMCaptionFont;
        public int iMenuWidth;
        public int iMenuHeight;
        public LogFont lfMenuFont;
        public LogFont lfStatusFont;
        public LogFont lfMessageFont;
        public int iPaddedBorderWidth;
    }
}