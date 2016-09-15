using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Hackcom.Utility
{
    public class ImportCsv
    {
        public static List<List<string>> Read(string filePath, bool includeHeader = true)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception(string.Format("[ImportCsv.Read] Source file ({0}) does not exist", filePath));
            }
            var lines = ReadAllLines(filePath);

            var rows = new List<List<string>>();
            for (var i = 0; i < lines.Count(); ++i)
            {
                if (includeHeader && i == 0)
                {
                    continue;
                }

                var l = lines[i];
                var columns = l.Contains("\"") ? ParseLine(l) : l.Split(',').ToList();
                columns = columns.Select(c => { c = c.Trim(); return c; }).ToList();
                rows.Add(columns);
            }
            return rows;
        }

        private static string[] ReadAllLines(string filePath)
        {
            var lines = File.ReadAllLines(filePath, System.Text.Encoding.Default);

            var isOpen = false;
            var prevLine = string.Empty;
            var result = new List<string>();
            foreach (var l in lines)
            {
                var quateCount = l.Count(ch => ch == '"');
                if (quateCount%2 == 0)
                {
                    if (isOpen)
                    {
                        prevLine += l;
                    }
                    else
                    {
                        result.Add(l);
                    }
                }
                else
                {
                    if (isOpen)
                    {
                        result.Add(prevLine + l);
                        prevLine = string.Empty;
                    }
                    else
                    {
                        prevLine = prevLine + l;
                    }

                    isOpen = !isOpen;
                }
            }

            if (!string.IsNullOrEmpty(prevLine))
            {
                result.Add(prevLine);
            }

            return result.ToArray();
        }

        private static List<string> ParseLine(string line)
        {
            var columns = new List<string>();
            var isOpen = false;
            var startIndex = 0;
            var i = 0;
            for (i = 0; i  < line.Length; ++i)
            {
                var c = line[i];

                if (c == '"')
                {
                    isOpen = !isOpen;
                }
                else if (c == ',' && !isOpen)
                {
                    columns.Add(Substring(line, startIndex, i));
                    startIndex = i + 1;
                }
            }

            columns.Add(Substring(line, startIndex, i));

            return columns;
        }

        private static string Substring(string str, int startIndex, int endIndex)
        {
            return str.Substring(startIndex, endIndex - startIndex).Trim().Trim('"');
        }
    }
}