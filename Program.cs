using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace anzhela_crm
{
    static class Program
    {
        
        [STAThread]
        static void Main()
        {
            // This is the main entry point for the application.
            Application.EnableVisualStyles();
            // This line enables visual styles for the application, allowing it to use the current Windows theme.
            Application.SetCompatibleTextRenderingDefault(false);
            // This line sets the default text rendering engine to be compatible with the current Windows theme.
            Database.Initialize();
            // This line initializes the database by calling the Initialize method from the Database class.
            Application.Run(new Dashboard());
            // This line starts the application and opens the Dashboard form.
        }
    }
}
