using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public partial class Form1 : FixedSizeForm
    {
        public Form1()
        {
            InitializeComponent();
            RenderUI();
        }

        public void RenderUI()
        {
            LargeButton startButton = new LargeButton();
            
            startButton.Location = new Point((SCREEN_WIDTH - startButton.Width) / 2 ,
                                             (SCREEN_HEIGHT - startButton.Height) / 2);
            startButton.Text = "เริ่มทำข้อสอบ";
            this.Controls.Add(startButton);
        }

    }
}
