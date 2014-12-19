using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace System_Alert_GUI
{
    public partial class Form1 : Form
    {

        private static SpeechSynthesizer synth = new SpeechSynthesizer();
        private static bool _isMonitoring;
        public static bool _sysUp;
        public static bool _cpuWarning;
        public static bool _memWarning;
        public static int _cpuStart;
        public static int _memStart;
        public static string _cpuBox;
        public static string _memBox;
        public Form1()
        {
            InitializeComponent();
            
            //Thread Starts
            Thread welcomeSpeech = new Thread(new ThreadStart(WelcomeSpeech));
            Thread uptimeRefresh = new Thread(new ThreadStart(writeUp));

            //Disable Stop Button and RTB
            button3.Enabled = false;
            richTextBox1.Enabled = false;
            richTextBox2.Enabled = false;
            //Start and Stop Thread to greet user
            Thread.Sleep(1);
            welcomeSpeech.Start();
            Thread.Sleep(1000);
            welcomeSpeech.Abort();
            //System Uptime Refresh Loop (on by default)
            uptimeRefresh.Start();
            _sysUp = true;
            //Voice Warning is on by Default
            _cpuWarning = true;
            _memWarning = true;
            //Value used to determine level when warning is given
            _memStart = Convert.ToInt32(memInput.Text);
            _memStart = int.Parse(memInput.Text);
            _cpuStart = Convert.ToInt32(cpuInput.Text);
            _cpuStart = int.Parse(cpuInput.Text);
            //Sets strings to work with multiple threads
            _memBox = memInput.Text;
            _cpuBox = cpuInput.Text;
        }

        /// <summary>
        /// Exit button in tool strip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Thread watcher = new Thread(new ThreadStart(Watcher));
            watcher.Abort();
        }

        /// <summary>
        /// Start System Alert when this button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            _memStart = Convert.ToInt32(memInput.Text);
            _memStart = int.Parse(memInput.Text);
            _cpuStart = Convert.ToInt32(cpuInput.Text);
            _cpuStart = int.Parse(cpuInput.Text);
            button1.Enabled = false;
            button3.Enabled = true;
            richTextBox2.Clear();
            richTextBox2.AppendText("System Alert has been started! You may now minimize this tab!\r\n");
            _isMonitoring = true;
            Thread watcher = new Thread(new ThreadStart(Watcher));
            watcher.Start();
            string cpuBase = string.Format("You have selected a verbal warning to be activated at {0}% CPU usage\r\n", _cpuStart);
            richTextBox2.AppendText(cpuBase);
            string memBase = string.Format("You have selected a verbal warning to be activated when there is {0}MB of available memory\r\n", _memStart);
            richTextBox2.AppendText(memBase);
        }

        /// <summary>
        /// Clear Log when this button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
        }

        /// <summary>
        /// Stop System Alert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            richTextBox2.AppendText("System Alert has been stopped! Resources no long being monitored.\r\n");
            _isMonitoring = false;
            Thread watcher = new Thread(new ThreadStart(Watcher));
            watcher.Abort();
            button1.Enabled = true;
        }
        public static void Speak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }

        public static void Speak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            Speak(message, voiceGender);
        }
        public static void WelcomeSpeech()
        {
            Speak("Welcome To System Alert two point O by ChirpingPotato.", VoiceGender.Male, 2);
        }
        
        /// <summary>
        /// Loop for CPU and Mem monitors, starts when _isMonitoring is set to true by button click
        /// </summary>
        public static void Watcher()
        {
            _memStart = Convert.ToInt32(_memBox);
            _memStart = int.Parse(_memBox);
            _cpuStart = Convert.ToInt32(_cpuBox);
            _cpuStart = int.Parse(_cpuBox);
            //Pulls CPU load in Percent
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();

            //Pulls current avilable memory in Megabytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            int currentCpuPercentage = (int)perfCpuCount.NextValue();
            int currentAvailableMemory = (int)perfMemCount.NextValue();

            while (_isMonitoring)
            {
                if (currentCpuPercentage > _cpuStart & _cpuWarning)
                {
                    if (currentCpuPercentage > 100)
                    {
                        string cpuLoadVocalMessage = ("The current CPU load is 100 percent");
                        Speak(cpuLoadVocalMessage, VoiceGender.Female, 2);
                    }
                    else
                    {
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent", currentCpuPercentage);
                        Speak(cpuLoadVocalMessage, VoiceGender.Female, 2);
                    }
                }
                if (currentAvailableMemory < _memStart & _memWarning)
                {
                    string memAvailableLoadVocalMessage = String.Format("You currently have {0} megabytes of memory available", currentAvailableMemory);
                    Speak(memAvailableLoadVocalMessage, VoiceGender.Female, 2);
                }
            }
        }

        /// <summary>
        /// Allows for writing to RTB 1 from different threads
        /// </summary>
        /// <param name="value"></param>
        public void AppendTextBox1(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox1), new object[] { value });
                return;
            }
            richTextBox1.Text += value;
        }
        /// <summary>
        /// Allows for writing to RTB 2 from different threads
        /// </summary>
        /// <param name="value"></param>
        public void AppendTextBox2(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox2), new object[] { value });
                return;
            }
            richTextBox2.Text += value;
        }
        /// <summary>
        /// Opens CPU Burn-In on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void placeholderToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            var startupPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetEntryAssembly().Location);
            var programPath = System.IO.Path.Combine(startupPath, "cpuburn.exe");
            System.Diagnostics.Process.Start(programPath);
        }

        /// <summary>
        /// Opens Option Form on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        /// <summary>
        /// Opens About Form on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.ShowDialog();
        }
        /// <summary>
        /// Writes uptime to richTextBox2
        /// </summary>
        private void writeUp()
        { 
            while(_sysUp)
            {
                //Gets current uptime
                PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
                perfUptimeCount.NextValue();

                TimeSpan uptimeSpan1 = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
                string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds\r\n",
               (int)uptimeSpan1.TotalDays,
               (int)uptimeSpan1.Hours,
               (int)uptimeSpan1.Minutes,
               (int)uptimeSpan1.Seconds
               );

                AppendTextBox1(systemUptimeMessage);
                Thread.Sleep(1000);
                ClearTextBox1();
            } 
        }
        public void ClearTextBox1()
        {
            richTextBox1.Clear();
        }
        public void ClearTextBox2()
        {
            richTextBox2.Clear();
        }
        /// <summary>
        /// Toggles CPU warning when checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _cpuWarning = !_cpuWarning;
        }
        /// <summary>
        /// Toggles available memory warning when checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            _memWarning = !_memWarning;
        }
    }
}
