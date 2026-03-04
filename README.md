<div align="center">

![Framework](https://img.shields.io/badge/Framework-.NET%208.0-blueviolet?style=for-the-badge)
![Language](https://img.shields.io/badge/Language-C%23-green?style=for-the-badge)
![Platform](https://img.shields.io/badge/Platform-Windows%20Forms-blue?style=for-the-badge)

# 🚀 ALE.Controls: DataGridEx

**An Advanced, Themed DataGrid for High-Performance Enterprise Applications**

</div>

---

## 🌟 Overview

**ALE.Controls** is a professional-grade UI library designed to bridge the gap between standard WinForms controls and modern dashboard requirements.

At its core is **DataGridEx**, a highly optimized, feature-rich wrapper for `DataGridView` that provides:

- Multi-level grouping  
- Complex filtering  
- Debounced searching  
- Modern themes  
- Smooth user interactions  

---

## 🛠️ Key Features

### 📂 Multi-Level Drag & Drop Grouping
Drag column headers into the grouping panel to instantly organize data into hierarchical, collapsible sections. Includes interactive chip-management for active groups.

### 🔍 Debounced Global Search & Advanced Filtering
- Built-in real-time search box  
- Prevents UI thread blocking  
- Expression builder supporting `AND`, `OR`, `NOT`  

### ⚙️ Enterprise Tooling Built-In
- One-click CSV Export  
- Dynamic Column Chooser  
- Eliminates repetitive boilerplate code  

### 🖱️ Extensible Context Menu
Built-in options:
- Copy Cell / Row  
- Export  
- Auto-size  

Plus support for injecting custom actions:

```csharp
AddContextMenuItem()
```

### ⌨️ Native Keyboard Shortcuts
- `Ctrl + F` → Search  
- `Ctrl + C` → Copy  
- `Ctrl + E` → Export  

### 🎨 Dynamic Layout & O(1) Theming
- Adjustable `RowHeight`
- Adjustable `SearchBoxWidth`
- Clean theme with Indigo/Purple accents
- O(1) rendering logic for instant theme switching

### ⚖️ Null-Safe & High Performance
- Built-in null handling  
- Uses `BindingList<object>`  
- Optimized LINQ queries  
- Maintains 60 FPS responsiveness even with 100,000+ rows  

---

## 📸 Feature Gallery

| Feature | Description |
|----------|-------------|
| Clean Theme & Dynamic Padding | Light background, Indigo selection, soft hover tracking, spacious enterprise row heights |
| Interactive Grouping | "Drag a column header here" panel with collapsible chevrons |
| Filter Builder | Nested logical groups for complex queries |
| Built-in Toolbars | Search, Column Chooser, CSV Export, Advanced Filter |

---

# 🚀 Quick Start

---

## 1️⃣ Configure the Grid & Layout

```csharp
// Layout and Theme Setup
dataGridEx1.Theme = GridTheme.Clean;
dataGridEx1.RowHeight = 36;          
dataGridEx1.SearchBoxWidth = 250;    
dataGridEx1.Font = new Font("Segoe UI", 10F);

// Define Columns
dataGridEx1.ConfigureColumns(new[] 
{
    ("Id",         "ID",         60),
    ("FullName",   "Name",       180),
    ("Department", "Department", 140),
    ("Salary",     "Salary",     120)
});
```

---

## 2️⃣ Load Data & Apply Default Groups

```csharp
// Load Data
dataGridEx1.SetData(myEmployeeList);

// Apply a default group
dataGridEx1.AddGroupProperty("Department");
```

---

## 3️⃣ Inject Custom Context Menu Actions

```csharp
// Add an 'Edit' action bound to the Enter key
dataGridEx1.AddContextMenuItem("✏️ Edit Record", (s, e) => 
{
    var selected = dataGridEx1.GetSelectedItems<Employee>().FirstOrDefault();
    if (selected != null) OpenEditForm(selected);
}, Keys.Enter);

// Add a 'Delete' action bound to the Delete key
dataGridEx1.AddContextMenuItem("🗑️ Delete Record", (s, e) => 
{
    DeleteSelectedRecord();
}, Keys.Delete);
```

---

## 📦 Installation

Install via NuGet Package Manager:

```powershell
Install-Package ALE.Controls
```

Or via .NET CLI:

```bash
dotnet add package ALE.Controls
```

---

## 📌 License

Specify your license here (MIT, Commercial, etc.).

---

## ⭐ Support

If you find this project useful, consider giving it a star ⭐
