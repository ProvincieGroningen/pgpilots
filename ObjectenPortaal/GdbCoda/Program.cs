﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Esri.FileGDB;

namespace GdbCoda
{
    internal static class Program
    {
        private const string OldCoda1 = "CODA_1E";
        private const string NewCoda2 = "CODA_2E";
        private const string OldCoda3 = "CODA_3E";
        private const string NewCoda4 = "CODA_4E";

        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var rootDir = new DirectoryInfo(args[0]);
                foreach (var dir in rootDir.GetDirectories("*.gdb", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        ModifyGdb(dir.FullName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fout: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Aanroep: {nameof(GdbCoda)} <Naam van folder met GDB's>");
            }
        }

        private static void ModifyGdb(string gdbFileName)
        {
            Console.WriteLine($"Modifying {gdbFileName}");
            var geodatabase = Geodatabase.Open(gdbFileName);
            var tableName = geodatabase
                .GetChildDatasets("\\", "Feature Class")
                .OrderBy(l => l.Length)
                .First();
            var table = geodatabase.OpenTable(tableName);
            if (FieldsExist(table, new[] {OldCoda1, OldCoda3}))
            {
                EnsureField(table, NewCoda2);
                EnsureField(table, NewCoda4);
                UpdateFields(table);
                EnsureFieldIsRemoved(table, OldCoda1);
                EnsureFieldIsRemoved(table, OldCoda3);
            }

            table.Close();
            geodatabase.Close();
        }

        private static void UpdateFields(Table table)
        {
            foreach (var r in table.Search("*", "", RowInstance.Unique))
            {
                if (!r.IsNull(OldCoda1))
                {
                    r.SetInteger(NewCoda2, GetCoda2Value(r.GetInteger(OldCoda1)));
                    table.Update(r);
                }
                if (!r.IsNull(OldCoda3))
                {
                    try
                    {
                        r.SetInteger(NewCoda4, GetCoda4Value(r.GetInteger(OldCoda3)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving {OldCoda3}: {ex.Message}");
                    }
                }
            }
        }

        private static int GetCoda2Value(int coda1Value)
        {
            return -1*coda1Value;
        }

        private static int GetCoda4Value(int coda3Value)
        {
            return coda3Value;
        }

        private static void EnsureField(Table table, string fieldName)
        {
            try
            {
                table.AddField(CodaFieldDef(fieldName));
            }
            catch
            {
                Console.WriteLine($"Field allready added {fieldName}");
            }
        }

        private static void EnsureFieldIsRemoved(Table table, string fieldName)
        {
            try
            {
                table.DeleteField(fieldName);
            }
            catch
            {
                Console.WriteLine($"Field allready removed {fieldName}");
            }
        }

        private static bool FieldsExist(Table table, string[] fieldNames)
        {
            var definition = XDocument.Parse(table.Definition);
            var fieldElements = definition.Descendants("Field").ToArray();
            foreach (var fieldName in fieldNames)
            {
                if (fieldElements.All(e => e?.Element("Name")?.Value != fieldName))
                {
                    Console.WriteLine($"Veld {fieldName} niet gevonden");
                    return false;
                }
            }
            return true;
        }

        private static FieldDef CodaFieldDef(string fieldName) =>
            new FieldDef {Name = fieldName, Alias = fieldName, IsNullable = true, Type = FieldType.Integer};
    }
}