using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;

namespace HCT_Client
{
    public class UtilFonts
    {
        private static FontFamily THSarabunFontFamily;

        private static FontFamily GetTHSarabunFontFamily()
        {
            if (THSarabunFontFamily == null)
            {
                PrivateFontCollection privateFontCollection = new PrivateFontCollection();
                privateFontCollection.AddFontFile(Util.GetFontsFolderPath() + "/THSarabunNewBold.ttf");
                THSarabunFontFamily = privateFontCollection.Families[0];
            }
            return THSarabunFontFamily;
        }

        public static Font GetTHSarabunFont(int fontSize)
        {
            fontSize += 5;
            Font font = new Font(GetTHSarabunFontFamily(), fontSize, FontStyle.Bold);
            return font;
        }
    }
}
