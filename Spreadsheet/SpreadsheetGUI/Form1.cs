using System;
using SSGui;
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
    /// <summary>
    /// This is the form class for the Spreadsheet GUI.  This is our View in the Model-View-Controller(MVC) design pattern.  All functions that the 
    /// controller needs to alter the view is implemented with the ISpreadSheet interface.  
    /// </summary>
    public partial class Form1 : Form, ISpreadSheet
    {
        
        /// <summary>
        /// Title of the current spreadsheet window.  Default is Spreadsheet, while if you load a file, or save the current file,
        /// the title will change to the saved file's path name.
        /// </summary>
        public string Title
        {
            set
            {
                Text = value; 
            }

            get
            {
                return Text;
            }
        }

        
        /// <summary>
        /// Creates a message box with the value passed to the property as the message of the box.
        /// </summary>
        public string Message
        {
            set
            {
                MessageBox.Show(value);
            }
        }

        /// <summary>
        /// this Property is primarily used to pop up a message box asking if the user wishes to save.  If so, it will call the saveEvent in the 
        /// controller through the event handler SaveEvent
        /// </summary>
        public bool MessageYesNo(string obj)
        {
            if (MessageBox.Show(obj, "Save Current File", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                return true;
            }
                return false;

        }

        /// <summary>
        /// Sets the text of the cell name box to the value passed.
        /// </summary>
        public string cellNameMainBox
        {
            set
            {
                cellNameBox.Text = value;
            }
        }

        /// <summary>
        /// Sets the text of the cell value box to the value passed.
        /// </summary>
        public string cellValueMainBox
        {
            set
            {
                CellValueBox.Text = value;
            }
        }

        /// <summary>
        /// Sets the text of the cell Contents box to the value passed.  Also returns the Text from the cell contents box
        /// </summary>
        public string cellContentsMainBox
        {
            set
            {
                cellContentsBox.Text = value;
            }

            get
            {
                return cellContentsBox.Text;
            }
        }

        /// <summary>
        /// The constructor for the GUI form.  Initializes the components of the form, and also adds the event Handler for when
        /// the user selects a new cell, through the SeletionChanged.  If this event happens, then the DisplaySelection method is called.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += displaySelection;

        }

        
        /// <summary>
        /// Event that is called when the user clicks the open file button
        /// </summary>
        public event Action<string> FileChosenEvent;

        /// <summary>
        /// Event that is called when the user clicks the close button
        /// </summary>
        public event Action CloseEvent;

        /// <summary>
        /// Event that is called when the user clicks the new button
        /// </summary>
        public event Action NewEvent;

        /// <summary>
        /// Event that is called when the user clicks the save button
        /// </summary>
        public event Action<string> SaveEvent;

        /// <summary>
        /// Event that is called when the user presses the enter key after the text of the cell contents box has been changed.
        /// </summary>
        public event Action<string, int, int> ChangeContents;

        /// <summary>
        /// Event that occurs when the user changes the currently selected cell
        /// </summary>
        public event Action<int, int> ChangeSelection;


        public event Action SaveClose;

        /// <summary>
        /// occurs when the form is loaded, selects the cell A1 and makes it the currently selected cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            if (ChangeSelection != null)
            {
                ChangeSelection(0, 0);
            }
        }

        /// <summary>
        /// Closes the current window of the application
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        /// <summary>
        /// Using the SpreadSheetApplicationContext file, creates a new window of the spreadsheet
        /// </summary>
        public void OpenNew()
        {
            SpreadSheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Using the spreadsheetApplicationContext file, create a new window that is loaded with a saved file's contents
        /// </summary>
        /// <param name="sheet">the filepath that leads to the saved file.</param>
        public void OpenOldNew(string sheet)
        {
            SpreadSheetApplicationContext.GetContext().RunNew(sheet);
        }

        /// <summary>
        /// Occurs when the user clicks on the open button in the file menu.  If the user decides on a file and clicks yes in the fild dialog box,
        /// calls the FileChosenEvent.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// When the new button in the file menu is clicked, calls the newEvent event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newItem_Click_1(object sender, EventArgs e)
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        /// <summary>
        /// Occurs when the user clicks the close button in the file menu.  calls the CloseEvent event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeItem_Click_1(object sender, EventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        /// <summary>
        /// Occurs when the user clicks the save button in the file menu.  When this happens, the method pops up a savefiledialog box.
        /// If the user selects a file that exists, and is not the same file that they are currently working on, then it asks for a 
        /// confirmation that if you want to overwrite the chosen file.  If so, or the file does not exist, or the file is the same as the one
        /// they are currently working on, it will call the SaveEvent event, passing it the filepath to save the file to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog test = saveFileDialog1;
            DialogResult result = saveFileDialog1.ShowDialog();
            
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                if (SaveEvent != null)
                {
                    SaveEvent(saveFileDialog1.FileName);
                }
            }
        }

        /// <summary>
        /// This method is called everytime the text of the cell contents is changed.  However
        /// the only time anything is executed is when the user presses the Enter key. 
        /// When this happens, it gets the current location of the currently selected cell, and calls the 
        /// ChangeContents event, passing it the location of the currently selected cell and the text of the contents box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellContentsBox_TextChanged(object sender, KeyEventArgs e)
        {
            int column;
            int row;


            if (e.KeyCode == Keys.Enter)
            {
                if (ChangeContents != null)
                {
                    spreadsheetPanel1.GetSelection(out column, out row);
                    ChangeContents(cellContentsBox.Text, column, row);
                }
            }
        }

        /// <summary>
        /// Occurs when the user clicks on a cell.  Calls the CangeSelection event, passing it the location of the currently selected cell.
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);
            

            if(ChangeSelection != null)
            {
                ChangeSelection(col, row);
            }
        }

        /// <summary>
        /// This method sets the value of a cell, allowing them to be updated and display the correct value in the GUI for the right cell
        /// </summary>
        /// <param name="obj">The value that the cell needs to display</param>
        /// <param name="col">The number that correlates to the same letter</param>
        /// <param name="row">The number tthat correlates to the correct row</param>
        public void updateTable(string obj, int col, int row)
        {
            spreadsheetPanel1.SetValue(col, row, obj);
        }

        /// <summary>
        /// Shows the help menu when the user clicks on the show help in the help menu.  Shows a message box that contains the instructions to use the spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void seeHelpContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To change the selection of a current cell, click on the new cell that you wish to select." + "\n" + "\n"
                + "To change the contents of the currently cell, click in the cell contents box on top of the spreadsheet, and type in the new contents." +
                   "\n" + "Hit Enter to confirm the new contents and The Spreadsheet will update automatically." + "\n"
                   + "NOTE:  You must press enter or the new contents will not be saved.");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(saveClose != null)
            {
                saveClose();
            }
        }
    }

}
