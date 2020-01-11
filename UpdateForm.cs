using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetonixUpdater
{
    // TODO Comment
    internal partial class UpdateForm : Form
    {
        private bool initialUpdate = true;

        public UpdateForm()
        {
            InitializeComponent();
        }

        public void Show(string title, string message, int tasks)
        {
            Text = title;
            waitLabel.Text = message;
            fileLabel.Text = "";
            progressBar.Maximum = tasks;
            progressBar.Value = 0;
            Application.DoEvents();
            Show();
        }

        public void SetFileName(string fileName)
        {
            fileLabel.Text = fileName;
            if (!initialUpdate)
                progressBar.Increment(1);
            else
                initialUpdate = false;
            Application.DoEvents();
        }
    }
}
