﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HCT_Client
{
    public class LargeButton : Button
    {
        public LargeButton() 
        {
            this.Width = 380;
            this.Height = 150;
            this.Font = new Font(this.Font.FontFamily, 32);
        }
    }
}
