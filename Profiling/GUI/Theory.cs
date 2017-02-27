using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Profiling.GUI
{
    public partial class Theory : Telerik.WinControls.UI.RadForm
    {
        public  string PathToTheory { get; set; }

        public Theory(string path)
        {
            PathToTheory = path;
            InitializeComponent();
            LoadText();
        }

        private void LoadText()
        {
            richTextBox1.LoadFile(PathToTheory);
        }
    }
}
