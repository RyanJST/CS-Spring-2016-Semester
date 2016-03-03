using System;

namespace SpreadsheetGUI
{
    public interface ISpreadSheet
    {
        /// <summary>
        /// Event that is used when the user decides to open up a saved file.
        /// </summary>
        event Action<string> FileChosenEvent;

        /// <summary>
        /// Event that is used when the user decides to close the current window.
        /// </summary>
        event Action CloseEvent;

        /// <summary>
        /// Event that is triggered when the user decides to open a new window.  
        /// </summary>
        event Action NewEvent;

        /// <summary>
        /// Event that is triggered when the user decides to save the current file.
        /// </summary>
        event Action<string> SaveEvent;

        /// <summary>
        /// Event that is triggered when the user changes the contents of the current cell.
        /// </summary>
        event Action<string, int, int> ChangeContents;

        /// <summary>
        /// Event that is triggered when the user changes the selection.
        /// </summary>
        event Action<int, int> ChangeSelection;

        /// <summary>
        /// Property that sets and gets the title of the current window.
        /// </summary>
        string Title { set; get; }

        /// <summary>
        /// Method that closes the current window.
        /// </summary>
        void DoClose();

        /// <summary>
        /// Method that opens up a new window.
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Property that show a message to the user.
        /// </summary>
        string Message { set; }

        /// <summary>
        /// property that gets a yes or no message for the user.
        /// </summary>
        bool MessageYesNo(string value);

        /// <summary>
        /// Property that sets the text of the cellNamebox
        /// </summary>
        string cellNameMainBox { set; }

        /// <summary>
        /// property that sets the text of the cellValuebox
        /// </summary>
        string cellValueMainBox { set; }

        /// <summary>
        /// Property that sets the text of the cellContentsBox
        /// </summary>
        string cellContentsMainBox { set; }

        /// <summary>
        /// Method that updates the value of the selected cell
        /// </summary>
        /// <param name="obj">string that is the new value of the cell to display</param>
        /// <param name="row">number that correlates to the letter of the cellname</param>
        /// <param name="col">number that correlates to the number of the cellname</param>
        void updateTable(string obj, int row, int col);

        void OpenOldNew(string obj);


    }
}