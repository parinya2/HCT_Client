using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HCT_Client
{
    public class BaseTextLabel : Label
    {
        public BaseTextLabel() 
        {
            this.Width = 200;
            this.Height = 30;
            this.Font = UtilFonts.GetTHSarabunFont(16);
            this.ForeColor = Color.White;
        }
    }
}
