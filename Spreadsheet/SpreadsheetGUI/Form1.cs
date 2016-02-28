using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form, ISpreadSheet
    {
        public string Title
        {
            set
            {
                Text = value; 
            }
        }

        public string Message
        {
            set
            {
                MessageBox.Show(value);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        public event Action<string> FileChosenEvent;
        public event Action CloseEvent;
        public event Action NewEvent;
        public event Action<string> SaveEvent;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void DoClose()
        {
            Close();
        }


        public void OpenNew()
        {
            throw new NotImplementedException();
        }

        private void openItem_Click_1(object sender, EventArgs e)
        {
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (FileChosenEvent != null)
                {
                    FileChosenEvent(fileDialog.FileName);
                }
            }
        }

        private void newItem_Click_1(object sender, EventArgs e)
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        private void closeItem_Click_1(object sender, EventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (SaveEvent != null)
                {
                    SaveEvent(fileDialog.FileName);
                }
            }
        }
    }
}
