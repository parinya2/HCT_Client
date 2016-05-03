using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HCT_Client
{
    public class BasePhotoLabel : Label
    {
        public BasePhotoLabel() 
        {
            this.Width = 100;
            this.Height = 100;
       
            this.BackColor = Color.Green;
        }
    }
}
