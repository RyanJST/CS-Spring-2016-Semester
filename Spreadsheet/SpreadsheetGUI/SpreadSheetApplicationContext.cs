using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// The class that keeps track of how many instances that have been opened.  When there are no instances created, the program will close.
    /// </summary>
    class SpreadSheetApplicationContext : ApplicationContext
    {
        /// <summary>
        /// Keeps track of all windows that have been opened 
        /// </summary>
        private int windowCount = 0;

        /// <summary>
        /// Is used by all instances of the spreadhseetapplicaitoncontext, making sure that the class keeps all instances together
        /// </summary>
        private static SpreadSheetApplicationContext context;


        /// <summary>
        /// Private constructor that does nothing.
        /// </summary>
        private SpreadSheetApplicationContext()
        {
        }

        /// <summary>
        /// Method that checks to see if there is a context created, if not, creates one.  If so, then returns the context.
        /// </summary>
        /// <returns></returns>
        public static SpreadSheetApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new SpreadSheetApplicationContext();
            }

            return context;
        }

        /// <summary>
        /// Creates a new instance of the form,pairing it with a new controller
        /// When this happens, it increases the amount of windowCount.
        /// If the window is closed, it checks and see if windowCount is going to be less than or equal to zero.  If so,
        /// then it ends the thread.
        /// </summary>
        public void RunNew()
        {
          
                
                Form1 window = new Form1();
                new Controller(window);

                windowCount++;

                window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

                window.Show();
            
        }

        /// <summary>
        /// This version of RunNew takes a string of a filePath, and passes it to the controller.  Doing this will create a window that contains 
        /// the saved files data.
        /// </summary>
        /// <param name="sheet"></param>
        public void RunNew(string sheet)
        {
                Form1 window = new Form1();
                new Controller(window, sheet);

                windowCount++;

                window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

                window.Show();
            }
        }
    }

