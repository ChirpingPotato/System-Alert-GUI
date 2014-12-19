using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace System_Alert_GUI
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens specified URL when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string target = "google.com";
            var startInfo = new ProcessStartInfo("chrome.exe", target);
            Process.Start(startInfo);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string target = "rameyllc.web44.net";
            var startInfo = new ProcessStartInfo("chrome.exe", target);
            Process.Start(startInfo);
        }
    }
}
