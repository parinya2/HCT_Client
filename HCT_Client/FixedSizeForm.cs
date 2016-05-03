﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HCT_Client
{
    public partial class FixedSizeForm : Form
    {
        public int SCREEN_WIDTH;
        public int SCREEN_HEIGHT;

        public FixedSizeForm() 
        {
            InitializeComponent();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
            SCREEN_WIDTH = workingRectangle.Width;
            SCREEN_HEIGHT = workingRectangle.Height;
        }
    }
}
