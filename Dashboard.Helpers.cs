using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace anzhela_crm
{
    public partial class Dashboard : Form
    {
        private Button CreateMenuButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(24, 24, 24),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            btn.Click += onClick;
            return btn;
        }

        private Button CreateSubMenuButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(36, 36, 36),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 }
            };
            btn.Click += onClick;
            return btn;
        }

        private void ToggleStudentSubmenu(object sender, EventArgs e)
        {
            isSubmenuExpanded = !isSubmenuExpanded;
            subMenuPanel.Height = isSubmenuExpanded ? 40 : 0;
            if (!isSubmenuExpanded) ShowAllStudents();
        }

       
       private void ShowHome()
        {
            contentPanel.Controls.Clear();

            TableLayoutPanel grid = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Dock = DockStyle.None,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            grid.Controls.Add(CreateStatCard("👥 Total Students", GetCount("Students")), 0, 0);
            grid.Controls.Add(CreateStatCard("📄 Total Invoices", GetCount("Invoices")), 1, 0);
            grid.Controls.Add(CreateStatCard("💰 Revenue (€)", GetSum("Invoices", "Amount")), 0, 1);
            grid.Controls.Add(CreateStatCard("💸 Expenses (€)", GetSum("Outvoices", "Amount")), 1, 1);

            Panel centerPanel = new Panel { Dock = DockStyle.Fill };
            centerPanel.Controls.Add(grid);

            
            grid.Location = new Point(
                (centerPanel.Width - grid.PreferredSize.Width) / 2,
                (centerPanel.Height - grid.PreferredSize.Height) / 2
            );
            centerPanel.Resize += (s, e) =>
            {
                grid.Location = new Point(
                    (centerPanel.Width - grid.PreferredSize.Width) / 2,
                    (centerPanel.Height - grid.PreferredSize.Height) / 2
                );
            };

            contentPanel.Controls.Add(centerPanel);
        }


        private Panel CreateStatCard(string title, string value)
        {
            Panel card = new Panel
            {
                Width = 200,
                Height = 120,
                BackColor = Color.FromArgb(45, 45, 48),
                Margin = new Padding(20),
                Padding = new Padding(10)
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.LightBlue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(lblValue);
            card.Controls.Add(lblTitle);
            return card;
        }

        private string GetCount(string table)
        {
            using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM {table}";
                return cmd.ExecuteScalar().ToString();
            }
        }

        private string GetSum(string table, string column)
        {
            using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT SUM({column}) FROM {table}";
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value ? $"{Convert.ToDecimal(result):F2}" : "0.00";
            }
        }

    }
}
