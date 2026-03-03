<div align="center">
  <img src="https://img.shields.io/badge/Framework-.NET%208.0-blueviolet?style=for-the-badge" alt="Framework">
  <img src="https://img.shields.io/badge/Language-C%23-green?style=for-the-badge" alt="Language">
  <img src="https://img.shields.io/badge/Platform-Windows%20Forms-blue?style=for-the-badge" alt="Platform">
  <br/>
  <h1>🚀 ALE.Controls: DataGridEx</h1>
  <p><b>An Advanced, Themed DataGrid for High-Performance Enterprise Applications</b></p>
</div>

---

### 🌟 Overview
**ALE.Controls** is a professional-grade UI library designed to bridge the gap between standard WinForms controls and modern dashboard requirements. At its core is **DataGridEx**, a highly optimized wrapper for the `DataGridView` that provides "out-of-the-box" support for complex filtering, modern themes, and smooth user interactions.

### 🛠️ Key Features

* **🔍 Advanced Filter Builder**: 
    A sophisticated dialog allowing users to build nested logical expressions (AND, OR, NOT) for deep data mining.
* **🎨 Designer-Friendly Theming**: 
    Features a primary **Clean** theme with **Indigo/Purple** accents. Selection colors are synchronized across all row types to eliminate visual glitches.
* **🖱️ Intelligent Row Hover**: 
    Custom "Hot Tracking" logic highlights rows as the mouse moves, significantly improving scannability for large datasets.
* **⚖️ Null-Safe Logic**: 
    Engineered for stability with built-in null-handling for sorting and filtering across strings, numbers, and dates.
* **⚡ High Performance**: 
    Uses `BindingList<object>` and optimized LINQ queries to maintain responsiveness even with complex active filters.

### 📸 UI Gallery
| Feature | Description |
| :--- | :--- |
| **Clean Theme** | Light background with Indigo selection and soft blue hover. |
| **Filter Builder** | Nested logical groups for complex queries. |
| **Enterprise Mode** | High-contrast layout for professional data management. |

### 🚀 Quick Start

#### 1. Configure the Grid
Set up your columns and apply your preferred theme:

```csharp
dataGridEx1.Theme = GridTheme.Clean; // Your preferred Indigo theme
dataGridEx1.ConfigureColumns(new[] 
{
    ("Id",         "ID",         60),
    ("FullName",   "Name",       180),
    ("Department", "Department", 140)
});
