using System.Data.SQLite;

namespace anzhela_crm
{
    public static class Database
    {
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=crm.db;Version=3;");
        }
        // This method initializes the database and creates the necessary tables if they do not exist.
        public static void Initialize()
        {
            // Create the database file if it doesn't exist
            using (var conn = GetConnection())
            {
                conn.Open();

                var cmd1 = conn.CreateCommand();
                cmd1.CommandText = @"
        CREATE TABLE IF NOT EXISTS Students (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT,
            LastName TEXT,
            Email TEXT,
            ParentName TEXT,
            DOB TEXT,
            Phone TEXT
        );";
                cmd1.ExecuteNonQuery();

                var cmd2 = conn.CreateCommand();
                cmd2.CommandText = @"
         CREATE TABLE IF NOT EXISTS Invoices (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            StudentId INTEGER,
            Subject TEXT,
            Amount REAL,
            PaymentMethod TEXT,
            Date TEXT,
            FOREIGN KEY(StudentId) REFERENCES Students(Id)
        );";
                cmd2.ExecuteNonQuery();
                var cmd3 = conn.CreateCommand();

                string createOutvoicesTable = @"
CREATE TABLE IF NOT EXISTS Outvoices (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Description TEXT NOT NULL,
    Amount REAL NOT NULL,
    PaymentMethod TEXT NOT NULL,
    Date TEXT NOT NULL
)";
                cmd3.CommandText = createOutvoicesTable;
                cmd3.ExecuteNonQuery();

            }

        }
    }
}
