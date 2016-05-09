using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HCT_Client
{
    public class MediumButton : Button
    {
        public MediumButton() 
        {
            this.Width = 120;
            this.Height = 60;
            this.Font = new Font(this.Font.FontFamily, 12);
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
        }
    }
}
