using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace anzhela_crm
{
    
    public partial class Dashboard : Form
    {
        /// Panels for sidebar and content
        Panel sidebar, subMenuPanel, contentPanel;
        bool isSubmenuExpanded = false;

        /// Method to toggle the visibility of the submenu
        public Dashboard()
        {
            InitializeComponent();
            this.Shown += (s, e) => ShowHome();
            
        }

        /// Method to create a menu button
        private void InitializeComponent()
        {
            this.Text = "Anzhela CRM";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Sidebar
            sidebar = new Panel();
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 200;
            sidebar.BackColor = Color.FromArgb(24, 24, 24);
            this.Controls.Add(sidebar);

            // Content Panel
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.FromArgb(40, 40, 40);
            contentPanel.AutoScroll = true;
            this.Controls.Add(contentPanel);

            // Exit Button
            Button exitBtn = CreateMenuButton("Exit", (s, e) => this.Close());
            exitBtn.Dock = DockStyle.Bottom;
            sidebar.Controls.Add(exitBtn);

            // Menu Buttons
            sidebar.Controls.Add(CreateMenuButton("Reports", (s, e) => ShowReports()));


            sidebar.Controls.Add(CreateMenuButton("Add Outvoice", (s, e) => ShowAddOutvoiceForm()));
            sidebar.Controls.Add(CreateMenuButton("Outvoice", (s, e) => ShowAllOutvoices()));

            sidebar.Controls.Add(CreateMenuButton("Add Invoice", (s, e) => ShowAddInvoiceForm()));
            sidebar.Controls.Add(CreateMenuButton("Invoices", (s, e) => ShowAllInvoices()));

            // Submenu for Students
            subMenuPanel = new Panel { Dock = DockStyle.Top, Height = 0, BackColor = Color.FromArgb(36, 36, 36) };
            sidebar.Controls.Add(subMenuPanel);

            Button addStudentBtn = CreateSubMenuButton("Add Student", (s, e) => ShowAddStudentForm());
            subMenuPanel.Controls.Add(addStudentBtn);

            Button studentsBtn = CreateMenuButton("Students", ToggleStudentSubmenu);
            sidebar.Controls.Add(studentsBtn);

            Button homeBtn = CreateMenuButton("Home", (s, e) => ShowHome());
            sidebar.Controls.Add(homeBtn);

            
            ShowHome();
        }


    }
}
