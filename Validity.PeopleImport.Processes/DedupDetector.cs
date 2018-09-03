using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validity.PeopleImport.Model;

namespace Validity.PeopleImport.Processes
{
    public class DedupDetector : DedupDetectorBase
    {

        public const double computerSimilarityConfig = 0.7;

        string[] keyCols = new string[] { "first_name", "last_name" };
        private static DataColumn[] GetKeyColumn(DataTable dtTbl, string[] keyCols)
        {
            DataColumn[] keyDataCol = new DataColumn[keyCols.Length];
            for (int i = 0; i < keyCols.Length; i++)
            {
                keyDataCol[i] = dtTbl.Columns[keyCols[i]];
            }
            return keyDataCol;
        }

        private static void RemoveDuplicates(DataTable nonDuplicateTable, DataTable duplicateTable, DataColumn[] keyColumns)
        {
            int rowIndex = 0;
            while (rowIndex < nonDuplicateTable.Rows.Count - 1)
            {
                DataRow[] dups = FindDuplicates(nonDuplicateTable, rowIndex, keyColumns);
                if (dups.Length > 0)
                {
                    foreach (DataRow dup in dups)
                    {
                        duplicateTable.Rows.Add(dup.ItemArray);
                        nonDuplicateTable.Rows.Remove(dup);
                    }
                }
                else
                {
                    rowIndex++;
                }
            }
        }

        private static DataRow[] FindDuplicates(DataTable nonDuplicateTable, int sourceNdx, DataColumn[] keyColumns)
        {
            ArrayList retVal = new ArrayList();

            DataRow sourceRow = nonDuplicateTable.Rows[sourceNdx];
            for (int i = sourceNdx + 1; i < nonDuplicateTable.Rows.Count; i++)
            {
                DataRow targetRow = nonDuplicateTable.Rows[i];
                if (CheckIfRowsDuplicate(sourceRow, targetRow, keyColumns))
                {
                    retVal.Add(targetRow);
                }
            }
            return (DataRow[])retVal.ToArray(typeof(DataRow));
        }

        private static bool CheckIfRowsDuplicate(DataRow sourceRow, DataRow targetRow, DataColumn[] keyColumns)
        {
            bool isDuplicate = true;
            string source, target;
            ComputeSimilarity computeSimilarity = new ComputeSimilarity();
            foreach (DataColumn column in keyColumns)
            {
                source = sourceRow[column].ToString();
                target = targetRow[column].ToString();
                isDuplicate = isDuplicate && sourceRow[column].Equals(targetRow[column]);
                if (!isDuplicate)
                {

                    var similarity = computeSimilarity.CalculateSimilarity(source, target);
                    if (similarity > computerSimilarityConfig)
                        isDuplicate = true;
                    else
                    {
                        break;
                    }
                }

            }
            return isDuplicate;
        }

        public override void FindDuplicateAndNonDuplicate(string csv_file_path)
        {
            DataTable nonDuplicateTable = new DataTable();
            DataTable duplicatesTable = new DataTable();
            try
            {
                using (TextFieldParser reader = new TextFieldParser(csv_file_path))
                {
                    reader.SetDelimiters(new string[] { "," });
                    reader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = reader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        nonDuplicateTable.Columns.Add(datecolumn);

                    }
                    
                    duplicatesTable = nonDuplicateTable.Clone();

                    DataColumn[] dataColumns = GetKeyColumn(nonDuplicateTable, keyCols);
                  
                    while (!reader.EndOfData)
                    {
                        string[] fieldData = reader.ReadFields();

                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }

                        }
                        nonDuplicateTable.Rows.Add(fieldData);
                        RemoveDuplicates(nonDuplicateTable, duplicatesTable, dataColumns);
                    }
                }
                DuplicateJson = DataTableToJSONWithJSONNet(duplicatesTable);
                NonDuplicateJson = DataTableToJSONWithJSONNet(nonDuplicateTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
      
    }
}
