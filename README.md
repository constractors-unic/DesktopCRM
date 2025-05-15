# Anzhela CRM - README

## Overview
Anzhela CRM is a Windows Forms-based CRM system tailored for educational centers and instructors. It allows the management of students, income (invoices), expenses (outvoices), reporting, and receipt printing.

## Main Features

### 1. Student Management
- Add, edit, and delete student records.
- Track details such as First Name, Last Name, Email, Phone, Date of Birth, and Parent Name.

### 2. Invoices (Income)
- Create invoices for each student.
- Enter subject, amount, payment method (e.g., Revolut, Bank, Cash), and date.
- View all invoices with search functionality, deletion, and **print/PDF** generation.

### 3. Outvoices (Expenses)
- Record business-related expenses including description, amount, payment method, and date.
- View and search all outvoices; delete when needed.

### 4. Reports
- **Monthly financial reports** showing total income, expenses, and balance.
- **Student-specific payments** with real-time search and total amount paid summary.

### 5. Dashboard
- Overview panels showing:
  - Total number of students
  - Total invoices
  - Total revenue 
  - Total expenses 

## Technologies Used
- **C#** (.NET Framework, WinForms)
- **SQLite** for local data storage
- **System.Drawing** for UI styling
- **System.Windows.Forms** for interface layout

## How to Run

1. Open the solution in **Visual Studio**.
2. Build the project to restore dependencies.
3. Run the project:
   ```csharp
   static void Main()
   {
       Database.Initialize();
       Application.Run(new Dashboard());
   }
   ```
4. The database (`crm.db`) is automatically created and initialized if it does not exist.

## Project Structure

| File                      | Purpose                                        |
|---------------------------|------------------------------------------------|
| `Dashboard.cs`            | Main application UI and navigation             |
| `Dashboard.Helpers.cs`    | UI components (menu buttons, dashboard panels) |
| `Dashboard.Students.cs`   | Student management (add, edit, list)           |
| `Dashboard.Invoices.cs`   | Invoice management (add, list, print)          |
| `Dashboard.Outvoices.cs`  | Expense management                             |
| `Dashboard.Reports.cs`    | Financial and student reports                  |
| `InvoicePrintForm.cs`     | Print and export invoice to PDF                |
| `Database.cs`             | SQLite database initialization                 |
| `Program.cs`              | Application entry point                        |

## Notes
- All data is stored locally in a SQLite file (`crm.db`).
- Printing uses the default Windows printer or "Microsoft Print to PDF".
- Clean and modern dark-themed UI.