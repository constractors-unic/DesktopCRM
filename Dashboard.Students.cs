using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace anzhela_crm
{
    public partial class Dashboard : Form
    {
        private void ShowAddStudentForm()
        {
            // Clear the content panel
            contentPanel.Controls.Clear();

            Panel centerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };
            // Centering the panel
            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

            // Title
            Label lblTitle = new Label
            {
                Text = "Add Student",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(0, 0, 0, 20)
            };

            // Wrapper for the title
            FlowLayoutPanel wrapper = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                BackColor = Color.Transparent
            };

            wrapper.Controls.Add(lblTitle);

            // Input fields
            TextBox txtFirstName = new TextBox { Width = 250 };
            TextBox txtLastName = new TextBox { Width = 250 };
            TextBox txtEmail = new TextBox { Width = 250 };
            TextBox txtParentName = new TextBox { Width = 250 };
            DateTimePicker dtpDob = new DateTimePicker { Width = 250 };
            TextBox txtPhone = new TextBox { Width = 250 };

            // Adding labels and textboxes to the layout
            layout.Controls.Add(new Label { Text = "First Name", ForeColor = Color.White, AutoSize = true }, 0, 0);
            layout.Controls.Add(txtFirstName, 1, 0);
            layout.Controls.Add(new Label { Text = "Last Name", ForeColor = Color.White, AutoSize = true }, 0, 1);
            layout.Controls.Add(txtLastName, 1, 1);
            layout.Controls.Add(new Label { Text = "Email", ForeColor = Color.White, AutoSize = true }, 0, 2);
            layout.Controls.Add(txtEmail, 1, 2);
            layout.Controls.Add(new Label { Text = "Parent Name", ForeColor = Color.White, AutoSize = true }, 0, 3);
            layout.Controls.Add(txtParentName, 1, 3);
            layout.Controls.Add(new Label { Text = "Date of Birth", ForeColor = Color.White, AutoSize = true }, 0, 4);
            layout.Controls.Add(dtpDob, 1, 4);
            layout.Controls.Add(new Label { Text = "Phone", ForeColor = Color.White, AutoSize = true }, 0, 5);
            layout.Controls.Add(txtPhone, 1, 5);

            // Save button
            Button btnSave = new Button
            {
                Text = "Save Student",
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
                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                    INSERT INTO Students (FirstName, LastName, Email, ParentName, DOB, Phone)
                    VALUES (@FirstName, @LastName, @Email, @ParentName, @DOB, @Phone)";
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@ParentName", txtParentName.Text);
                    cmd.Parameters.AddWithValue("@DOB", dtpDob.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Student saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFirstName.Clear(); txtLastName.Clear(); txtEmail.Clear(); txtParentName.Clear(); txtPhone.Clear();
            };

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
        // ShowAllStudents button click event
        private void ShowAllStudents()
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
            // Load students from the database
            void LoadStudents(string filter = "")
            {
                flow.Controls.Clear();

                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    string query = "SELECT * FROM Students";
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        query += " WHERE FirstName LIKE @q OR LastName LIKE @q";
                    }

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(filter))
                            cmd.Parameters.AddWithValue("@q", $"%{filter}%");

                        // Execute the command and read the results
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var card = new Panel
                                {
                                    Width = 760,
                                    Height = 160,
                                    Margin = new Padding(10),
                                    BackColor = Color.FromArgb(60, 60, 60)
                                };

                                card.Controls.Add(new Label
                                {
                                    Text = "Name: " + reader["FirstName"] + " " + reader["LastName"],
                                    ForeColor = Color.White,
                                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                                    AutoSize = true,
                                    Location = new Point(10, 10)
                                });

                                card.Controls.Add(new Label
                                {
                                    Text = "Email: " + reader["Email"],
                                    ForeColor = Color.White,
                                    AutoSize = true,
                                    Location = new Point(10, 40)
                                });

                                card.Controls.Add(new Label
                                {
                                    Text = "Phone: " + reader["Phone"],
                                    ForeColor = Color.White,
                                    AutoSize = true,
                                    Location = new Point(10, 60)
                                });

                                card.Controls.Add(new Label
                                {
                                    Text = "Parent: " + reader["ParentName"],
                                    ForeColor = Color.White,
                                    AutoSize = true,
                                    Location = new Point(10, 80)
                                });

                                card.Controls.Add(new Label
                                {
                                    Text = "DOB: " + reader["DOB"],
                                    ForeColor = Color.White,
                                    AutoSize = true,
                                    Location = new Point(10, 100)
                                });

                                int studentId = Convert.ToInt32(reader["Id"]);

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
                                // Delete button click event
                                btnDelete.Click += (sender, args) =>
                                {
                                    var confirm = MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (confirm == DialogResult.Yes)
                                    {
                                        using (var conn2 = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                                        {
                                            conn2.Open();
                                            var delCmd = conn2.CreateCommand();
                                            delCmd.CommandText = "DELETE FROM Students WHERE Id = @id";
                                            delCmd.Parameters.AddWithValue("@id", studentId);
                                            delCmd.ExecuteNonQuery();
                                        }
                                        LoadStudents(searchBox.Text.Trim());
                                    }
                                };

                                card.Controls.Add(btnDelete);

                                // Edit button
                                Button btnEdit = new Button
                                {
                                    Text = "Edit",
                                    Width = 80,
                                    Height = 30,
                                    Location = new Point(660, 50),
                                    BackColor = Color.FromArgb(0, 120, 215),
                                    ForeColor = Color.White,
                                    FlatStyle = FlatStyle.Flat,
                                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                                };
                                // Edit button click event
                                btnEdit.Click += (sender, args) =>
                                {
                                    ShowEditStudentForm(studentId);
                                };

                                card.Controls.Add(btnEdit);
                                flow.Controls.Add(card);
                            }
                        }
                    }
                }
            }
            // Search button click event
            searchBtn.Click += (s, e) =>
            {
                LoadStudents(searchBox.Text.Trim());
            };

            contentPanel.Resize += (s, e) =>
            {
                searchPanel.Location = new Point((contentPanel.Width - searchPanel.Width) / 2, 10);
                flow.Location = new Point((contentPanel.Width - flow.Width) / 2, 70);
            };

            LoadStudents();
        }
        // ShowEditStudentForm method to edit student details
        private void ShowEditStudentForm(int id)
        {
            contentPanel.Controls.Clear();

            TextBox txtFirstName = new TextBox { Width = 250 };
            TextBox txtLastName = new TextBox { Width = 250 };
            TextBox txtEmail = new TextBox { Width = 250 };
            TextBox txtParentName = new TextBox { Width = 250 };
            DateTimePicker dtpDob = new DateTimePicker { Width = 250 };
            TextBox txtPhone = new TextBox { Width = 250 };

            using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Students WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtFirstName.Text = reader["FirstName"].ToString();
                        txtLastName.Text = reader["LastName"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtParentName.Text = reader["ParentName"].ToString();
                        dtpDob.Value = DateTime.TryParse(reader["DOB"].ToString(), out var dob) ? dob : DateTime.Today;
                        txtPhone.Text = reader["Phone"].ToString();
                    }
                }
            }

            // Clear the content panel
            TableLayoutPanel layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(50),
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

            layout.Controls.Add(new Label { Text = "First Name", ForeColor = Color.White, AutoSize = true }, 0, 0);
            layout.Controls.Add(txtFirstName, 1, 0);
            layout.Controls.Add(new Label { Text = "Last Name", ForeColor = Color.White, AutoSize = true }, 0, 1);
            layout.Controls.Add(txtLastName, 1, 1);
            layout.Controls.Add(new Label { Text = "Email", ForeColor = Color.White, AutoSize = true }, 0, 2);
            layout.Controls.Add(txtEmail, 1, 2);
            layout.Controls.Add(new Label { Text = "Parent Name", ForeColor = Color.White, AutoSize = true }, 0, 3);
            layout.Controls.Add(txtParentName, 1, 3);
            layout.Controls.Add(new Label { Text = "Date of Birth", ForeColor = Color.White, AutoSize = true }, 0, 4);
            layout.Controls.Add(dtpDob, 1, 4);
            layout.Controls.Add(new Label { Text = "Phone", ForeColor = Color.White, AutoSize = true }, 0, 5);
            layout.Controls.Add(txtPhone, 1, 5);

            // Update button
            Button btnUpdate = new Button
            {
                Text = "Update",
                Width = 100,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 20, 0, 0)
            };
            // Update button click event
            btnUpdate.Click += (s, e) =>
            {
                using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
                {
                    conn.Open();
                    var updateCmd = conn.CreateCommand();
                    updateCmd.CommandText = @"
                        UPDATE Students
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            Email = @Email,
                            ParentName = @ParentName,
                            DOB = @DOB,
                            Phone = @Phone
                        WHERE Id = @Id";

                    updateCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    updateCmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    updateCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    updateCmd.Parameters.AddWithValue("@ParentName", txtParentName.Text);
                    updateCmd.Parameters.AddWithValue("@DOB", dtpDob.Value.ToString("yyyy-MM-dd"));
                    updateCmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    updateCmd.Parameters.AddWithValue("@Id", id);

                    updateCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Student updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowAllStudents();
            };
            // Adding the update button to the layout
            layout.Controls.Add(btnUpdate, 1, 6);
           
            Panel centerPanel = new Panel { Dock = DockStyle.Fill };
            centerPanel.Controls.Add(layout);
            layout.Location = new Point((centerPanel.Width - layout.Width) / 2, (centerPanel.Height - layout.Height) / 2);
            // Centering the panel
            centerPanel.Resize += (s, e) =>
            {
                layout.Location = new Point((centerPanel.Width - layout.Width) / 2, (centerPanel.Height - layout.Height) / 2);
            };

            contentPanel.Controls.Add(centerPanel);
        }
    }
}

    