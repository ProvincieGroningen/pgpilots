using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Esri.FileGDB;

namespace GdbCodaInventarisatie
{
    internal static class Program
    {
        private const string OldCoda1 = "CODA_1E";
        private const string NewCoda2 = "CODA_2E";
        private const string OldCoda3 = "CODA_3E";
        private const string NewCoda4 = "CODA_4E";
        private const string Separator = ";";

        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                new LogResult("Bestand", "Tabel", "Coda_1e", "Coda_2e", "Coda_3e", "Coda_4e") {Fout = "Fout"}.Show();
                var rootDir = new DirectoryInfo(args[0]);
                foreach (var dir in rootDir.GetDirectories("*.gdb", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        ListGdb(dir.FullName).Show();
                    }
                    catch (Exception ex)
                    {
                        new LogResult(dir.FullName, ex.Message).Show();
                    }
                }
            }
            else
            {
                Console.WriteLine($"Aanroep: {nameof(GdbCodaInventarisatie)} <Naam van folder met GDB's>");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Press any key to continue ... ");
            Console.Read();
        }

        private static LogResult ListGdb(string gdbFileName)
        {
            var geodatabase = Geodatabase.Open(gdbFileName);
            var tableName = geodatabase
                .GetChildDatasets("\\", "Feature Class")
                .OrderBy(l => l.Length)
                .First();
            var table = geodatabase.OpenTable(tableName);
            var definition = XDocument.Parse(table.Definition);
            var logResult = new LogResult(
                gdbFileName,
                tableName,
                FieldType(definition, OldCoda1),
                FieldType(definition, NewCoda2),
                FieldType(definition, OldCoda3),
                FieldType(definition, NewCoda4)
                );
            table.Close();
            geodatabase.Close();
            return logResult;
        }

        private static string FieldType(XContainer definition, string fieldName)
        {
            var fieldElements = definition.Descendants("Field").ToArray();
            return (string) fieldElements
                .SingleOrDefault(e => e?.Element("Name")?.Value == fieldName)
                ?.Element("Type")
                ;
        }

        private class LogResult
        {
            private readonly string fileName;
            private readonly string tableName;
            public string Fout;
            private readonly string coda1Type;
            private readonly string coda2Type;
            private readonly string coda3Type;
            private readonly string coda4Type;

            public LogResult(string fileName, string tableName, string coda1Type, string coda2Type, string coda3Type, string coda4Type)
            {
                this.fileName = fileName;
                this.tableName = tableName;
                this.coda1Type = coda1Type;
                this.coda2Type = coda2Type;
                this.coda3Type = coda3Type;
                this.coda4Type = coda4Type;
            }

            public LogResult(string fileName, string fout)
            {
                this.fileName = fileName;
                Fout = fout;
            }

            public override string ToString()
            {
                return $"{fileName}{Separator}{Fout}{Separator}{tableName}{Separator}{coda1Type}{Separator}{coda2Type}{Separator}{coda3Type}{Separator}{coda4Type}";
            }

            public void Show()
            {
                Console.WriteLine(this);
            }
        }
    }
}