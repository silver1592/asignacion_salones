﻿using System;
using System.Collections.Generic;
using System.IO;
using Excel;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;

namespace OrigenDatos.Clases
{
    /// <summary>
    /// Abre un archivo de excel y lee o escribe en el
    /// </summary>
    public class LibroExcel
    {
        private string archivo; // Nombre del archivo
        private string dir;// Direccion del archivo

        #region Atributos a buscar
        public Dictionary<string, string> dHeaders; // Diccionario con los encabezados de la tabla

        public Dictionary<string,string> dDefault; //Valores por default

        /// <summary>
        /// Inicializa los diccionario
        /// </summary>
        private void SetHeaders()
        {
            dHeaders = new Dictionary<string, string>();
            dDefault = new Dictionary<string, string>();

            dHeaders.Add("cve_mat", "CVE_MAT");//1
            dHeaders.Add("cve_gpo", "CVE_GPO");//2
            dHeaders.Add("cve", "CLAVEMAT");//3
            dHeaders.Add("cverpe", "CVERPE");//4
            dHeaders.Add("nom_prof", "Profesor");//5
            dHeaders.Add("tipo", "Tipo");    //6*
            dHeaders.Add("salon", "Salon");  //7*
            dHeaders.Add("lunes", "lunes_ini");//8
            dHeaders.Add("lunesf", "lunes_fin");//9
            dHeaders.Add("martes", "martes_ini");//10
            dHeaders.Add("martesf", "martes_fin");//11
            dHeaders.Add("miercoles", "miercoles_ini");//12
            dHeaders.Add("miercolesf", "miercoles_fin");//13
            dHeaders.Add("jueves", "jueves_ini");//14
            dHeaders.Add("juevesf", "jueves_fin");//15
            dHeaders.Add("viernes", "viernes_ini");//16
            dHeaders.Add("viernesf", "viernes_fin");//17
            dHeaders.Add("sabado", "sabado_ini");//18
            dHeaders.Add("sabadof", "sabado_fin");//19
            dHeaders.Add("cupo", "Cupo");//20
            dHeaders.Add("inscritos", "INSC");//21
            dHeaders.Add("ciclo", "CICLO");  //22*

            //Valores default
            dDefault.Add("ciclo", "");
            dDefault.Add("tipo", "T");
        }
        #endregion

        #region Constructores e Inicializaciones
        /// <summary>
        /// Almacena la informacion y la guarda y checa si existe el archivo
        /// </summary>
        /// <param name="direccion">Direccion del archivo</param>
        /// <param name="ciclo">Semestre al cual se va a considerar</param>
        /// <param name="tipo">tipo por default para los grupos</param>
        public LibroExcel(string direccion, string ciclo, string tipo="")
        {
            string[] split = direccion.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string archivo = split[split.Length - 1];

            if (!File.Exists(direccion))
            {
                SpreadsheetDocument doc = SpreadsheetDocument.Create(direccion, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);

                //Inicializa el documento para que trabaje como libro
                WorkbookPart workbookpart = doc.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                //Obtiene el manejador de las hojas
                Sheets sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                creaHoja(doc,"Hoja1");

                doc.Close();
            }

            SetHeaders();
            this.archivo = archivo;
            this.dir = direccion;
            dHeaders["cicloDefault"] = ciclo;
            dHeaders["tipoDefault"] = tipo;
        }

        /// <summary>
        /// Lee el excel y regresa las hojas en una coleccion de tablas
        /// </summary>
        /// <returns>Coleccion de Hojas del excel</returns>
        private DataTableCollection GetSheets()
        {
            //Es necesaria para que se inicialize ContCoumn
            FileStream stream = File.Open(dir, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            if (dir.Contains(".xlsx"))
                // De lo contrario descomentaremos esta (2007 format; *.xlsx)
                //TODO:Usar OpenXML sin usar otras librerias (hay problemas al abrir los archivos creados en el servidor)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            else if (dir.Contains(".xls"))
                // Si es un archivo de excel anterior a 2009, debemos usar esta línea
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                throw new Exception("No es un archivo valido");

            // Cada grupo de elementos en el DataSet tomará el nombre de la primera celda en cada columna
            excelReader.IsFirstRowAsColumnNames = true;

            DataSet result = excelReader.AsDataSet();
            excelReader.Close();

            return result.Tables;
        }

        /// <summary>
        /// Convierte un DataTable a List de tipo grupo
        /// </summary>
        /// <param name="dtGrupos">Tabla de grupos</param>
        /// <returns>Lista de Grupos</returns>
        private List<Grupo> AsList(DataTable dtGrupos)
        {
            List<Grupo> res = new List<Grupo>();

            foreach (DataRow r in dtGrupos.Rows)
                res.Add(new Grupo(r, dHeaders,dDefault));

            return res;
        }
        #endregion

        #region Comandos
        /// <summary>
        /// Escribe la informacion de los grupos
        /// </summary>
        /// <param name="grupos">Nombre de la hoja que se quiere poner </param>
        /// <param name="hoja">Hoja en la que se va a escribir</param>
        public void EscribeGrupos(IList<Grupo> grupos, string hoja, IDictionary<string, string> materia = null, IDictionary<int, string> profesor = null)
        {
            SpreadsheetDocument doc = SpreadsheetDocument.Open(dir,true);
            Worksheet worksheet = GetSheet(hoja,doc);
            SheetData sheet = worksheet.GetFirstChild<SheetData>();

            foreach (Grupo item in grupos)
                EscribeGrupo(item,sheet,materia,profesor);

            worksheet.Save();
            doc.Close();
            doc.Dispose();
        }

        /// <summary>
        /// Obtiene el nombre de las hojas del excel e inicializa la variable Sheets
        /// </summary>
        public string[] GetStringSheets()
        {
            DataTableCollection tables = GetSheets();

            string[] sheets = new string[tables.Count];
            for (int i = 0; i < tables.Count; i++)
                sheets[i] = tables[i].ToString();

            return sheets;
        }

        /// <summary>
        /// Lee el excel e inicializa los grupos con la infromacion
        /// </summary>
        /// <param name="hoja">Hoja de excel</param>
        public List<Grupo> GetGrupos(string hoja,string ciclo = "2016-2017/II", string tipo = "T")
        {
            DataTable dt = GetSheets()[hoja];
            dDefault["ciclo"]=ciclo;
            dDefault["tipo"]= tipo;

            return dt != null ? AsList(dt) : new List<Grupo>();
        }
        #endregion

        #region OpenXML
        public Worksheet GetSheet(string hoja, SpreadsheetDocument doc)
        {
            int count = doc.WorkbookPart.Workbook.Sheets.ChildElements.Count;
            Worksheet worksheet = null;
            SheetData sheet = null;
            Sheet aux;

            for (int i = 0; i < count; i++)
            {
                aux = (Sheet)doc.WorkbookPart.Workbook.Sheets.ChildElements.GetItem(i);
                if (aux.Name == hoja)
                {
                    worksheet = (doc.WorkbookPart.GetPartById(aux.Id) as WorksheetPart).Worksheet;
                    sheet = worksheet.GetFirstChild<SheetData>();
                }
            }

            if (sheet == null)
            {
                worksheet = creaHoja(doc, hoja);
                sheet = worksheet.GetFirstChild<SheetData>();
                EscribeEncabezados(sheet);
            }

            return worksheet;
        }

        private void EscribeEncabezados(SheetData sheet, uint rowIndex = 0)
        {
            List<string> keyList = new List<string>(dHeaders.Keys);
            Row r = GetRow(sheet);

            //Escribe los encabezados de las hojas
            for (int i = 1; i <= dHeaders.Count + 1; i++)
                if (i < dHeaders.Count)
                    CreaCelda(dHeaders[keyList[i - 1]], r, i);
                else if (i < dHeaders.Count + 1)
                    CreaCelda("observaciones", r, i);
        }

        /// <summary>
        /// Escribe un grupo en la hoja dada
        /// </summary>
        /// <param name="g">Grupo a escribir</param>
        /// <param name="sheet">Hoja a modificar</param>
        public void EscribeGrupo(Grupo g, SheetData sheet, IDictionary<string, string> materia, IDictionary<int, string> profesor)
        {
            Cell refCell = null;
            Row r = GetRow(sheet);

            refCell = CreaCelda(Convert.ToInt32(g.Cve_materia), r, 1);
            refCell = CreaCelda(g.num_Grupo, r, 2);

            if(materia!=null)
                if(materia.ContainsKey(g.Cve_materia))
                    refCell = CreaCelda(materia[g.Cve_materia], r, 3);
                else
                    refCell = CreaCelda("-----------", r, 3);
            else
                refCell = CreaCelda(Convert.ToInt32(g.Cve_materia) * 100 + g.num_Grupo, r, 3);

                refCell = CreaCelda(g.RPE, r, 4);
            if (profesor != null)
                if (profesor.ContainsKey(g.RPE))
                    refCell = CreaCelda(profesor[g.RPE], r, 5);
                else
                    refCell = CreaCelda("------------", r, 5);
            else
                refCell = CreaCelda(g.RPE, r, 5);

            refCell = CreaCelda(g.Tipo, r, 6);
            refCell = CreaCelda(g.Cve_espacio, r, 7);
            refCell = CreaCelda(g.horario_ini[0], r, 8);
            refCell = CreaCelda(g.horario_fin[0], r, 9);
            refCell = CreaCelda(g.horario_ini[1], r, 10);
            refCell = CreaCelda(g.horario_fin[1], r, 11);
            refCell = CreaCelda(g.horario_ini[2], r, 12);
            refCell = CreaCelda(g.horario_fin[2], r, 13);
            refCell = CreaCelda(g.horario_ini[3], r, 14);
            refCell = CreaCelda(g.horario_fin[3], r, 15);
            refCell = CreaCelda(g.horario_ini[4], r, 16);
            refCell = CreaCelda(g.horario_fin[4], r, 17);
            refCell = CreaCelda(g.horario_ini[5], r, 18);
            refCell = CreaCelda(g.horario_fin[5], r, 19);
            refCell = CreaCelda(g.cupo, r, 20);
            refCell = CreaCelda(g.inscritos, r, 21);
            refCell = CreaCelda(g.Ciclo, r, 22);
            refCell = CreaCelda("-"+g.observaciones, r, 23);
        }

        private Worksheet creaHoja(SpreadsheetDocument doc, string nombre)
        {
            //Crea el archivo de la hoja
            WorksheetPart worksheetPart = doc.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            //Crea el apuntador al archivo
            Sheets sheets = doc.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;

            Sheet sheet = new Sheet() { Id = doc.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = nombre };
            sheets.Append(sheet);

            // The SheetData object will contain all the data.
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            return worksheetPart.Worksheet;
        }

        public Row GetRow(SheetData sheetData)
        {
            Row lastRow = sheetData.Elements<Row>().LastOrDefault();

            if (lastRow != null)
            {
                Row row = new Row() { RowIndex = (lastRow.RowIndex + 1) };
                sheetData.InsertAfter(row, lastRow);
            }
            else
            {
                sheetData.Append(new Row() { RowIndex = 1 });
            }

            return sheetData.Elements<Row>().LastOrDefault();
        }

        private Cell CreaCelda(string valor, Row row, int col)
        {
            Cell refCell = row.LastChild as Cell;
            Cell newCell = new Cell() { CellReference = GetExcelColumnName(col) + row.RowIndex };
            row.InsertAfter(newCell, refCell);

            newCell.DataType = CellValues.String;

            newCell.CellValue = new CellValue(valor);

            return newCell;
        }

        private Cell CreaCelda(int valor, Row row, int col)
        {
            Cell refCell = row.LastChild as Cell;
            Cell newCell = new Cell() { CellReference = GetExcelColumnName(col) + row.RowIndex };
            row.InsertAfter(newCell, refCell);

            newCell.DataType = CellValues.Number;

            newCell.CellValue = new CellValue(valor.ToString());

            return newCell;
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
        #endregion
    }
}