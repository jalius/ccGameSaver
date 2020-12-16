using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ccGameSaver
{
    public partial class Form1 : Form
    {
        int user_custom_autosave_interval = 0;
        GameSaverRestorer gameSaverRestorer = new GameSaverRestorer();
        public Form1()
        {
            InitializeComponent();
        }
        private void SetAutoSaveInterval(int interval)
        {
            if (interval > 0)
            {
                timer1.Interval = 60 * 1000 * interval; //ms to minutes conversion
                timer1.Start();
            }
            else
            {
                timer1.Stop();
            }
        }
        private void StoreUserCustomAutosaveInterval(string user_input)
        {
            int user_input_number = 0;
            if (Int32.TryParse(user_input, out user_input_number))
            {
                user_custom_autosave_interval = user_input_number;
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoSaveInterval(2);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoSaveInterval(5);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoSaveInterval(10);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoSaveInterval(user_custom_autosave_interval);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            StoreUserCustomAutosaveInterval(textBox1.Text);
            if(radioButton4.Checked)
            {
                SetAutoSaveInterval(user_custom_autosave_interval);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gameSaverRestorer.SaveGame("manual");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                gameSaverRestorer.BackupThenRestoreGame(comboBox1.SelectedItem.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool isRunning = Process.GetProcessesByName("CannibalCrossing").Any();
            if (!isRunning)
            {
                label4.Visible = true;
                label4.Text = "skipped autosave, CannibalCrossing.exe not open";
                return;
            }
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string save_name = gameSaverRestorer.SaveGame("autosave");
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            double truncated = Math.Truncate(ts.TotalSeconds * 100) / 100;

            label4.Visible = true;
            label4.Text = save_name + " created in " + truncated + " s";
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval =  60 * 1000 * 5;//5 minutes
            timer1.Start();
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            var local_save_names = gameSaverRestorer.GetLocalSaveNames();
            foreach (var save in local_save_names)
            {
                comboBox1.Items.Add(save);
            }

        }

        private void StorePreserveAutosaveCount(string user_input)
        {
            int user_input_number = 0;
            if (Int32.TryParse(user_input, out user_input_number))
            {
                gameSaverRestorer.preserve_autosave_count = user_input_number;
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            StorePreserveAutosaveCount(textBox2.Text);
        }
    }
}
