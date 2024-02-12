using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public static class SD
    {
        // Roles
        public const string Role_Admin = "Admin";
        public const string Role_Company_Admin = "Company Admin";
        public const string Role_User = "User";

        public static string CreatePassword()
        {
            int lenth = 2;
            //int lenth2 = 2;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string valid2 = "abcdefghijklmnopqrstuvwxyz";
            const string valid3 = "!@#$%^&*";
            const string valid4 = "1234567890";
            //const string valid4 = "";

            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < lenth--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
                res.Append(valid2[rnd.Next(valid2.Length)]);
                res.Append(valid3[rnd.Next(valid3.Length)]);
                res.Append(valid4[rnd.Next(valid4.Length)]);
            }

            return res.ToString();
        }

        public static string DetermineActionType(string action)
        {
            if (action == "Login")
            {
                return "Login";
            }
            else if (action == "Logout")
            {
                return "Logout";
            }
            else
            {
                return string.Empty;
            }
        }

        public static DataTable ToConvertDatatable<T>(List<T> items)
        {
            DataTable dt = new DataTable(typeof(T).Name);
            PropertyInfo[] propinfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in propinfo)
            {
                dt.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[propinfo.Length];
                for (int i = 0; i < propinfo.Length; i++)
                {
                    values[i] = propinfo[i].GetValue(item, null);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            if (cell.DataType == null)
            {
                return cell.InnerText;
            }

            var cellValue = cell.CellValue.InnerXml;
            if (cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                return sharedStringTable.ChildElements[int.Parse(cellValue)].InnerText;
            }
            else
            {
                return cellValue;
            }
        }

        public static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { SD.Role_Admin, SD.Role_Company_Admin, SD.Role_User };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
