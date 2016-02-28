using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    class SpreadSheetApplicationContext : ApplicationContext
    {
        private int windowCount = 0;

        private static SpreadSheetApplicationContext context;

        private SpreadSheetApplicationContext()
        {
        }

        public static SpreadSheetApplicationContext GetContext()
        {
            if (context == null)
            {
                context = new SpreadSheetApplicationContext();
            }

            return context;
        }

        public void RunNew()
        {
            Form1 window = new Form1();
            new Controller(window);

            windowCount++;

            window.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread();  };

            window.Show();
        }
    }
}
