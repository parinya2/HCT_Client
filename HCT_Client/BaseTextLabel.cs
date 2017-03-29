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
            this.Height = 70;
            this.Font = UtilFonts.GetTHSarabunFont(33);
            this.ForeColor = Color.White;
        }
    }

    public class BaseTextLabel2 : Label
    {
        public BaseTextLabel2()
        {
            this.Width = 200;
            this.Height = 40;
            this.Font = UtilFonts.GetTHSarabunFont(22);
            this.ForeColor = Color.White;
        }
    }
}
