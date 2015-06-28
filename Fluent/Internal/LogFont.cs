namespace Fluent.Internal
{
    using System.Runtime.InteropServices;

    // A "logical font" used by old-school windows
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct LogFont
    {
        private const int LF_FACESIZE = 32;

        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;

        /// <summary>
        /// <see cref="UnmanagedType.ByValTStr"/> means that the string
        /// should be marshalled as an array of TCHAR embedded in the 
        /// structure.  This implies that the font names can be no larger
        /// than <see cref="LF_FACESIZE"/> including the terminating '\0'.
        /// That works out to 31 characters.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
        public string lfFaceName;

        // to shut it up about the warnings
        public LogFont(string lfFaceName)
        {
            this.lfFaceName = lfFaceName;
            lfHeight = lfWidth = lfEscapement = lfOrientation = lfWeight = 0;
            lfItalic = lfUnderline = lfStrikeOut = lfCharSet = lfOutPrecision
            = lfClipPrecision = lfQuality = lfPitchAndFamily = 0;
        }
    }
}