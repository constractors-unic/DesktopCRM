using System;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace anzhela_crm
{
    public class InvoicePrintForm : Form
    {
        private string receiptText;

        
        public InvoicePrintForm(int invoiceId)
        {
            this.Text = "Invoice Preview";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var lblTitle = new Label
            {
                Text = "Anzhela Tverdokhlib-Aravi Swimming ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            var lblDetails = new Label
            {
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.LightGray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(0, 10, 0, 10),
                BackColor = Color.FromArgb(24, 24, 24),
                WrapContents = false,
                AutoSize = false
            };

            var btnPrint = new Button
            {
                Text = "Print",
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var btnPdf = new Button
            {
                Text = "Save as PDF",
                Width = 150,
                Height = 40,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnPrint.FlatAppearance.BorderSize = 0;
            btnPdf.FlatAppearance.BorderSize = 0;

            btnPanel.Controls.Add(btnPrint);
            btnPanel.Controls.Add(btnPdf);

            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblDetails);
            mainPanel.Controls.Add(btnPanel);
            this.Controls.Add(mainPanel);

            // print document
            btnPrint.Click += (s, e) =>
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (sender, args) =>
                {
                    args.Graphics.DrawString(receiptText, new Font("Segoe UI", 12), Brushes.Black, new RectangleF(50, 50, 700, 1000));
                };

                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = pd;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    pd.Print();
                }
            };

            // pdf save 
            btnPdf.Click += (s, e) =>
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (sender, args) =>
                {
                    args.Graphics.DrawString(receiptText, new Font("Segoe UI", 12), Brushes.Black, new RectangleF(50, 50, 700, 1000));
                };

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = "invoice.pdf"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                    pd.PrinterSettings.PrintToFile = true;
                    pd.PrinterSettings.PrintFileName = saveDialog.FileName;
                    pd.Print();
                    MessageBox.Show("PDF saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            // load invoice details
            using (var conn = new SQLiteConnection("Data Source=crm.db;Version=3;"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT I.Subject, I.Amount, I.PaymentMethod, I.Date,
                           S.FirstName || ' ' || S.LastName AS StudentName
                    FROM Invoices I
                    INNER JOIN Students S ON S.Id = I.StudentId
                    WHERE I.Id = @id";
                cmd.Parameters.AddWithValue("@id", invoiceId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        receiptText =
                            $" Invoice Receipt\n\n" +
                            $" Student: {reader["StudentName"]}\n" +
                            $" Lesson: {reader["Subject"]}\n" +
                            $" Amount: €{reader["Amount"]}\n" +
                            $" Payment Method: {reader["PaymentMethod"]}\n" +
                            $" Date: {reader["Date"]}";

                        lblDetails.Text = receiptText.Replace("\n", Environment.NewLine);
                    }
                }
            }
        }
    }
}
