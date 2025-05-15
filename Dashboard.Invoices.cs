
using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace anzhela_crm
{
    public partial class Dashboard : Form
    {
        // Panel sidebar, subMenuPanel, contentPanel;
        private void ShowAddInvoiceForm()
        {
            contentPanel.Controls.Clear();

            Panel centerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

            // Title label
            Label lblTitle = new Label
            {
                Text = "Add Invoice",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(0, 0, 0, 20)
            };

            FlowLayoutPanel wrapper = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent
            };

            wrapper.Controls.Add(lblTitle);

            ComboBox cmbStudents = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            TextBox txtLesson = new TextBox { Width = 250 };
            ComboBox cmbType = new ComboBox { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new string[] { "Private", "Group" });
            TextBox txtAmount = new TextBox { Width = 250 };
            ComboBox cmbPaymentMethod = new ComboBox
            {
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPaymentMethod.Items.AddRange(new string[] { "Revolut", "Bank of Cyprus", "Cash", "Cheque" });
            DateTimePicker dtpDate = new DateTimePicker { Width = 250 };

            using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Id, FirstName || ' ' || LastName AS FullName FROM Students";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbStudents.Items.Add(new
                        {
                            Id = reader["Id"],
                            Name = reader["FullName"].ToString()
                        });
                    }
                }
            }
            // Set the display and value members for the ComboBox
            cmbStudents.DisplayMember = "Name";
            cmbStudents.ValueMember = "Id";

            layout.Controls.Add(new Label { Text = "Student", ForeColor = Color.White, AutoSize = true }, 0, 0);
            layout.Controls.Add(cmbStudents, 1, 0);
            layout.Controls.Add(new Label { Text = "Lesson", ForeColor = Color.White, AutoSize = true }, 0, 1);
            layout.Controls.Add(txtLesson, 1, 1);
            layout.Controls.Add(new Label { Text = "Type", ForeColor = Color.White, AutoSize = true }, 0, 2);
            layout.Controls.Add(cmbType, 1, 2);
            layout.Controls.Add(new Label { Text = "Amount (€)", ForeColor = Color.White, AutoSize = true }, 0, 3);
            layout.Controls.Add(txtAmount, 1, 3);
            layout.Controls.Add(new Label { Text = "Payment Method", ForeColor = Color.White, AutoSize = true }, 0, 4);
            layout.Controls.Add(cmbPaymentMethod, 1, 4);
            layout.Controls.Add(new Label { Text = "Date", ForeColor = Color.White, AutoSize = true }, 0, 5);
            layout.Controls.Add(dtpDate, 1, 5);

            // Save button
            Button btnSave = new Button
            {
                Text = "Save Invoice",
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 20, 0, 0)
            };
            btnSave.FlatAppearance.BorderSize = 0;

            // Event handler for the Save button
            btnSave.Click += (s, e) =>
            {
                if (cmbStudents.SelectedItem == null || string.IsNullOrWhiteSpace(txtLesson.Text) ||
                    string.IsNullOrWhiteSpace(txtAmount.Text) || cmbPaymentMethod.SelectedItem == null || cmbType.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in all required fields.");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out var amount))
                {
                    MessageBox.Show("Amount must be a number.");
                    return;
                }

                var selected = cmbStudents.SelectedItem;
                int studentId = Convert.ToInt32(selected.GetType().GetProperty("Id").GetValue(selected));
                // Save invoice to database 
                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                INSERT INTO Invoices (StudentId, Subject, Amount, PaymentMethod, Date)
                VALUES (@StudentId, @Subject, @Amount, @PaymentMethod, @Date)";
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Subject", txtLesson.Text);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@PaymentMethod", cmbPaymentMethod.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Invoice saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLesson.Clear(); txtAmount.Clear(); cmbPaymentMethod.SelectedIndex = -1; cmbType.SelectedIndex = -1;
            };
            // Add the button to the layout
            layout.Controls.Add(btnSave, 1, 6);

            wrapper.Controls.Add(layout);
            centerPanel.Controls.Add(wrapper);

            wrapper.Location = new Point(
                (centerPanel.Width - wrapper.PreferredSize.Width) / 2,
                (centerPanel.Height - wrapper.PreferredSize.Height) / 2
            );

            centerPanel.Resize += (s, e) =>
            {
                wrapper.Location = new Point(
                    (centerPanel.Width - wrapper.PreferredSize.Width) / 2,
                    (centerPanel.Height - wrapper.PreferredSize.Height) / 2
                );
            };

            contentPanel.Controls.Add(centerPanel);
        }


        // Show all invoices
        private void ShowAllInvoices()
        {
            contentPanel.Controls.Clear();

            Panel searchPanel = new Panel
            {
                Width = 420,
                Height = 50,
                Location = new Point((contentPanel.Width - 420) / 2, 10)
            };

            TextBox searchBox = new TextBox
            {
                Width = 300,
                Font = new Font("Segoe UI", 10),
                Location = new Point(0, 10)
            };

            // Search button
            Button searchBtn = new Button
            {
                Text = "Search",
                Width = 100,
                Height = 28,
                Location = new Point(310, 10),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            searchPanel.Controls.Add(searchBox);
            searchPanel.Controls.Add(searchBtn);
            contentPanel.Controls.Add(searchPanel);

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.TopDown,
                Size = new Size(800, contentPanel.Height - 80),
                Location = new Point((contentPanel.Width - 800) / 2, 70),
                BackColor = Color.Transparent
            };

            contentPanel.Controls.Add(flow);

            contentPanel.Resize += (s, e) =>
            {
                searchPanel.Location = new Point((contentPanel.Width - searchPanel.Width) / 2, 10);
                flow.Location = new Point((contentPanel.Width - flow.Width) / 2, 70);
            };

            // Load invoices from the database
            void LoadInvoices(string filter = "")
            {
                flow.Controls.Clear();

                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();

                    string query = @"
                        SELECT I.Id, I.Subject, I.Amount, I.PaymentMethod, I.Date,
                               S.FirstName || ' ' || S.LastName AS StudentName
                        FROM Invoices I
                        INNER JOIN Students S ON S.Id = I.StudentId";

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        query += " WHERE S.FirstName LIKE @q OR S.LastName LIKE @q OR I.Subject LIKE @q";
                    }

                    var cmd = conn.CreateCommand();
                    cmd.CommandText = query;

                    if (!string.IsNullOrWhiteSpace(filter))
                        cmd.Parameters.AddWithValue("@q", $"%{filter}%");
                    // Execute the command
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Loop through the results
                        while (reader.Read())
                        {
                            var card = new Panel
                            {
                                Width = 760,
                                Height = 100,
                                Margin = new Padding(10),
                                BackColor = Color.FromArgb(60, 60, 60)
                            };

                            card.Controls.Add(new Label
                            {
                                Text = $"Student: {reader["StudentName"]}",
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                AutoSize = true,
                                Location = new Point(10, 10)
                            });

                            card.Controls.Add(new Label
                            {
                                Text = $"Subject: {reader["Subject"]}    |    Amount: €{reader["Amount"]}",
                                ForeColor = Color.White,
                                AutoSize = true,
                                Location = new Point(10, 35)
                            });

                            card.Controls.Add(new Label
                            {
                                Text = $"Payment Method: {reader["PaymentMethod"]}    |    Date: {reader["Date"]}",
                                ForeColor = Color.White,
                                AutoSize = true,
                                Location = new Point(10, 55)
                            });

                            flow.Controls.Add(card);

                            int invoiceId = Convert.ToInt32(reader["Id"]);

                            Button btnDelete = new Button
                            {
                                Text = "Delete",
                                Width = 80,
                                Height = 30,
                                Location = new Point(660, 10),
                                BackColor = Color.DarkRed,
                                ForeColor = Color.White,
                                FlatStyle = FlatStyle.Flat,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold)
                            };

                            // Event handler for the Delete button
                            btnDelete.Click += (sender, args) =>
                            {
                                var confirm = MessageBox.Show("Are you sure you want to delete this invoice?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (confirm == DialogResult.Yes)
                                {
                                    using (var conn2 = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                                    {
                                        conn2.Open();
                                        var delCmd = conn2.CreateCommand();
                                        delCmd.CommandText = "DELETE FROM Invoices WHERE Id = @id";
                                        delCmd.Parameters.AddWithValue("@id", invoiceId);
                                        delCmd.ExecuteNonQuery();
                                    }
                                    LoadInvoices(); // Refresh
                                }
                            };

                            card.Controls.Add(btnDelete);

                            // Print button
                            Button btnPrint = new Button
                            {
                                Text = "Print",
                                Width = 80,
                                Height = 30,
                                Location = new Point(660, 50),
                                BackColor = Color.FromArgb(0, 120, 215),
                                ForeColor = Color.White,
                                FlatStyle = FlatStyle.Flat,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold)
                            };

                            // Event handler for the Print button
                            btnPrint.Click += (sender, args) =>
                            {
                                var printForm = new InvoicePrintForm(invoiceId);
                                printForm.ShowDialog();
                            };

                            card.Controls.Add(btnPrint);
                        }
                    }
                }
            }

            // Event handler for the Search button
            searchBtn.Click += (s, e) =>
            {
                LoadInvoices(searchBox.Text.Trim());
            };

            LoadInvoices(); //start load
        }



    }
}
