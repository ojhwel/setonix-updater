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
    /// <summary>
    /// The form showing the user the update progress.
    /// </summary>
    internal partial class UpdateForm : Form
    {
        // To allow calling SetFileName() one without updating the progress bar.
        private bool initialUpdate = true;

        public UpdateForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Displays the update progress form.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="message">The "please wait" message.</param>
        /// <param name="tasks">The tasks to perform.</param>
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

        /// <summary>
        /// Updates the file name and increased the progress bar value.
        /// </summary>
        /// <param name="fileName">The file name to display.</param>
        public void SetFileName(string fileName)
        {
            fileLabel.Text = fileName;
            if (!initialUpdate)
                progressBar.Increment(1);
            else
                initialUpdate = false;
            // TODO I'm not sure that does the trick. I've never seen progress past 10 %
            Application.DoEvents();
        }
    }
}
