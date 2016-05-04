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
    public partial class FormFadeView : FixedSizeForm
    {
        public FormFadeView()
        {
            InitializeComponent();

            this.Opacity = 0.8;
            this.BackColor = Color.Black;
        }
    }
}
