using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace anzhela_crm
{
    public partial class Dashboard : Form
    {
        
        private void ShowAllOutvoices()
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

            // LoadOutvoices method to load outvoices from the database
            void LoadOutvoices(string filter = "")
            {
                flow.Controls.Clear();

                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();

                    string query = "SELECT * FROM Outvoices";
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        query += " WHERE Description LIKE @q OR PaymentMethod LIKE @q";
                    }

                    var cmd = conn.CreateCommand();
                    cmd.CommandText = query;

                    if (!string.IsNullOrWhiteSpace(filter))
                        cmd.Parameters.AddWithValue("@q", $"%{filter}%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int outvoiceId = Convert.ToInt32(reader["Id"]);

                            var card = new Panel
                            {
                                Width = 760,
                                Height = 100,
                                Margin = new Padding(10),
                                BackColor = Color.FromArgb(60, 60, 60)
                            };

                            card.Controls.Add(new Label
                            {
                                Text = $"Description: {reader["Description"]}",
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                                AutoSize = true,
                                Location = new Point(10, 10)
                            });

                            card.Controls.Add(new Label
                            {
                                Text = $"Amount: €{reader["Amount"]}    |    Payment Method: {reader["PaymentMethod"]}",
                                ForeColor = Color.White,
                                AutoSize = true,
                                Location = new Point(10, 35)
                            });

                            card.Controls.Add(new Label
                            {
                                Text = $"Date: {reader["Date"]}",
                                ForeColor = Color.White,
                                AutoSize = true,
                                Location = new Point(10, 55)
                            });

                            // Delete button
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
                            // Set the button's flat appearance
                            btnDelete.Click += (sender, args) =>
                            {
                                var confirm = MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (confirm == DialogResult.Yes)
                                {
                                    using (var conn2 = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                                    {
                                        conn2.Open();
                                        var delCmd = conn2.CreateCommand();
                                        delCmd.CommandText = "DELETE FROM Outvoices WHERE Id = @id";
                                        delCmd.Parameters.AddWithValue("@id", outvoiceId);
                                        delCmd.ExecuteNonQuery();
                                    }
                                    LoadOutvoices(searchBox.Text.Trim());
                                }
                            };

                            card.Controls.Add(btnDelete);
                            flow.Controls.Add(card);
                        }
                    }
                }
            }
            // Add button to add new outvoice
            searchBtn.Click += (s, e) =>
            {
                LoadOutvoices(searchBox.Text.Trim());
            };

            LoadOutvoices(); // start load
        }
        // Add button to add new outvoice
        private void ShowAddOutvoiceForm()
        {
            contentPanel.Controls.Clear();

            Panel centerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

            
            Label lblTitle = new Label
            {
                Text = "Add Expense",
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

            // Input fields
            TextBox txtDescription = new TextBox { Width = 250 };
            TextBox txtAmount = new TextBox { Width = 250 };

            ComboBox cmbPaymentMethod = new ComboBox
            {
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPaymentMethod.Items.AddRange(new string[] { "Revolut", "Bank of Cyprus", "Cash", "Cheque" });

            DateTimePicker dtpDate = new DateTimePicker { Width = 250 };

            layout.Controls.Add(new Label { Text = "Description", ForeColor = Color.White, AutoSize = true }, 0, 0);
            layout.Controls.Add(txtDescription, 1, 0);
            layout.Controls.Add(new Label { Text = "Amount (€)", ForeColor = Color.White, AutoSize = true }, 0, 1);
            layout.Controls.Add(txtAmount, 1, 1);
            layout.Controls.Add(new Label { Text = "Payment Method", ForeColor = Color.White, AutoSize = true }, 0, 2);
            layout.Controls.Add(cmbPaymentMethod, 1, 2);
            layout.Controls.Add(new Label { Text = "Date", ForeColor = Color.White, AutoSize = true }, 0, 3);
            layout.Controls.Add(dtpDate, 1, 3);

            // Save Button
            Button btnSave = new Button
            {
                Text = "Save Expense",
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 20, 0, 0)
            };
            btnSave.FlatAppearance.BorderSize = 0;

            // Save button click event
            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtAmount.Text) || cmbPaymentMethod.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, out var amount))
                {
                    MessageBox.Show("Amount must be numeric.");
                    return;
                }

                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                INSERT INTO Outvoices (Description, Amount, PaymentMethod, Date)
                VALUES (@Description, @Amount, @PaymentMethod, @Date)";
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@PaymentMethod", cmbPaymentMethod.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Expense saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDescription.Clear(); txtAmount.Clear(); cmbPaymentMethod.SelectedIndex = -1;
            };

            layout.Controls.Add(btnSave, 1, 4);

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



    }
}
