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
            this.Width = 290;
            this.Height = 120;
            this.Font = new Font(this.Font.FontFamily, 28);
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
        }

        public void SetLocationForBackButton()
        {
            int SCREEN_HEIGHT = SystemInformation.VirtualScreen.Height;
            this.Location = new Point(80, SCREEN_HEIGHT - this.Height - 80);
        }
    }
}
