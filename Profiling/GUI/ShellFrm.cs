using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Profiling.GUI;

namespace Profiling.GUI
{
    public partial class ShellFrm : Telerik.WinControls.UI.RadForm
    {
        public ShellFrm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pathToTheory = Environment.CurrentDirectory + "\\MethodFirst.rtf";
            Theory theory = new Theory(pathToTheory);
            theory.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var pathToTheory = Environment.CurrentDirectory + "\\MethodSecond.rtf";
            Theory theory = new Theory(pathToTheory);
            theory.Show();
        }
    }
}
