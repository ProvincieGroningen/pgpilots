using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Esri.FileGDB;

namespace GdbCodaInventarisatie
{
    internal static class InspectCodaFields
    {
        private const string OldCoda1 = "CODA_1E";
        private const string NewCoda2 = "CODA_2E";
        private const string OldCoda3 = "CODA_3E";
        private const string NewCoda4 = "CODA_4E";

        public static IEnumerable<LogResult> Rename(string fileName, Action<string> fileStarted)
        {
            var rootDir = new DirectoryInfo(fileName);
            foreach (var dir in rootDir.GetDirectories("*.gdb", SearchOption.TopDirectoryOnly))
            {
                LogResult result;
                try
                {
                    fileStarted(dir.Name);
                    result = ListGdb(dir.FullName);
                }
                catch (Exception ex)
                {
                    result = new LogResult(dir.FullName, ex.Message);
                }
                yield return result;
            }
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
    }

    public class LogResult
    {
        private const string Separator = ";";

        public string FileName { get; private set; }
        public string TableName { get; private set; }
        public string Fout { get; set; }
        public string Coda1Type { get; private set; }
        public string Coda2Type { get; private set; }
        public string Coda3Type { get; private set; }
        public string Coda4Type { get; private set; }

        public LogResult(string fileName, string tableName, string coda1Type, string coda2Type, string coda3Type, string coda4Type)
        {
            FileName = fileName;
            TableName = tableName;
            Coda1Type = coda1Type;
            Coda2Type = coda2Type;
            Coda3Type = coda3Type;
            Coda4Type = coda4Type;
        }

        public LogResult(string fileName, string fout)
        {
            FileName = fileName;
            Fout = fout;
        }

        public override string ToString()
        {
            return $"{FileName}{Separator}{Fout}{Separator}{TableName}{Separator}{Coda1Type}{Separator}{Coda2Type}{Separator}{Coda3Type}{Separator}{Coda4Type}";
        }
    }
}