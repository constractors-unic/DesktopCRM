using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace anzhela_crm
{
    public partial class Dashboard : Form
    {
        
        private void ShowReports()
        {
            contentPanel.Controls.Clear();
            Panel centerPanel = new Panel { Dock = DockStyle.Fill };

            // monthly report
            ComboBox cmbMonth = new ComboBox
            {
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            for (int i = 0; i < 12; i++)
            {
                var date = DateTime.Now.AddMonths(-i);
                cmbMonth.Items.Add(date.ToString("MMMM yyyy"));
            }

            cmbMonth.SelectedIndex = 0;
            
            
            Button btnCalculate = new Button
            {
                Text = "Show Monthly Report",
                Width = 180,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Label lblIncome = new Label
            {
                Text = "Income: €0",
                ForeColor = Color.Lime,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };

            Label lblExpense = new Label
            {
                Text = "Expenses: €0",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };

            Label lblBalance = new Label
            {
                Text = "Balance: €0",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true
            };

            // Calculate button event
            btnCalculate.Click += (s, e) =>
            {
                if (cmbMonth.SelectedItem == null) return;
                DateTime selectedDate = DateTime.ParseExact(cmbMonth.SelectedItem.ToString(), "MMMM yyyy", null);
                string monthStart = new DateTime(selectedDate.Year, selectedDate.Month, 1).ToString("yyyy-MM-01");
                string monthEnd = new DateTime(selectedDate.Year, selectedDate.Month, DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month)).ToString("yyyy-MM-dd");

                decimal income = 0, expense = 0;

                // Database connection and query
                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();

                    var cmd1 = conn.CreateCommand();
                    cmd1.CommandText = "SELECT SUM(Amount) FROM Invoices WHERE Date BETWEEN @start AND @end";
                    cmd1.Parameters.AddWithValue("@start", monthStart);
                    cmd1.Parameters.AddWithValue("@end", monthEnd);
                    var result1 = cmd1.ExecuteScalar();
                    income = result1 != DBNull.Value ? Convert.ToDecimal(result1) : 0;

                    var cmd2 = conn.CreateCommand();
                    cmd2.CommandText = "SELECT SUM(Amount) FROM Outvoices WHERE Date BETWEEN @start AND @end";
                    cmd2.Parameters.AddWithValue("@start", monthStart);
                    cmd2.Parameters.AddWithValue("@end", monthEnd);
                    var result2 = cmd2.ExecuteScalar();
                    expense = result2 != DBNull.Value ? Convert.ToDecimal(result2) : 0;
                }

                decimal balance = income - expense;
                lblIncome.Text = $"Income: €{income:F2}";
                lblExpense.Text = $"Expenses: €{expense:F2}";
                lblBalance.Text = $"Balance: €{balance:F2}";
            };

            // Payments of students
            TextBox txtSearch = new TextBox { Width = 250};
            ComboBox cmbStudents = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            FlowLayoutPanel flowInvoices = new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 700,
                Height = 250
            };
            Label lblTotalStudent = new Label
            {
                Text = "Total Paid: €0",
                ForeColor = Color.LightGreen,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            // Load students into the ComboBox
            txtSearch.TextChanged += (s, e) =>
            {
                cmbStudents.Items.Clear();
                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT Id, FirstName || ' ' || LastName AS Name FROM Students WHERE FirstName LIKE @q OR LastName LIKE @q";
                    cmd.Parameters.AddWithValue("@q", $"%{txtSearch.Text}%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbStudents.Items.Add(new { Id = reader["Id"], Name = reader["Name"].ToString() });
                        }
                    }
                }
                cmbStudents.DisplayMember = "Name";
                cmbStudents.ValueMember = "Id";
            };

            // Load invoices for the selected student
            cmbStudents.SelectedIndexChanged += (s, e) =>
            {
                if (cmbStudents.SelectedItem == null) return;
                dynamic selected = cmbStudents.SelectedItem;
                int studentId = Convert.ToInt32(selected.Id);
                decimal total = 0;
                flowInvoices.Controls.Clear();

                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT Subject, Amount, PaymentMethod, Date FROM Invoices WHERE StudentId = @id ORDER BY Date DESC";
                    cmd.Parameters.AddWithValue("@id", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string line = $"📅 {reader["Date"]} | {reader["Subject"]} | 💶 €{reader["Amount"]} | 🏦 {reader["PaymentMethod"]}";
                            var lbl = new Label
                            {
                                Text = line,
                                ForeColor = Color.White,
                                AutoSize = true
                            };
                            flowInvoices.Controls.Add(lbl);
                            total += Convert.ToDecimal(reader["Amount"]);
                        }
                    }
                }

                lblTotalStudent.Text = $"Total Paid: €{total:F2}";
            };

            // LAYOUT
            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(50)
            };

            layout.Controls.Add(new Label
            {
                Text = "Monthly Financial Report",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            });
            layout.Controls.Add(cmbMonth);
            layout.Controls.Add(btnCalculate);
            layout.Controls.Add(lblIncome);
            layout.Controls.Add(lblExpense);
            layout.Controls.Add(lblBalance);

            //space of empty
            layout.Controls.Add(new Label { Text = "", Height = 20 });

            layout.Controls.Add(new Label
            {
                Text = "Search Student Payments",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            });
            layout.Controls.Add(txtSearch);
            layout.Controls.Add(cmbStudents);
            layout.Controls.Add(flowInvoices);
            layout.Controls.Add(lblTotalStudent);

            centerPanel.Controls.Add(layout);
            contentPanel.Controls.Add(centerPanel);

            layout.Location = new Point((centerPanel.Width - layout.Width) / 2, 20);
            centerPanel.Resize += (s, e) =>
            {
                layout.Location = new Point((centerPanel.Width - layout.Width) / 2, 20);
            };
        }




    }
}
