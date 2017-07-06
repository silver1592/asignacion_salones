using System;
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
        /// <summary>
        /// Nombre del archivo
        /// </summary>
        private string archivo;
        /// <summary>
        /// Direccion del archivo
        /// </summary>
        private string dir;

        #region Atributos a buscar
        /// <summary>
        /// Diccionario con los encabezados de la tabla
        /// </summary>
        public Dictionary<string, string> dHeaders;

        public Dictionary<string,string> dDefault;

        /// <summary>
        /// Inicializa el diccionario
        /// </summary>
        /// <remarks>
        /// Solo se llama cuando se crea el objeto
        /// </remarks>
        private void SetHeaders()
        {
            dHeaders = new Dictionary<string, string>();
            dDefault = new Dictionary<string, string>();

            dHeaders.Add("cve_mat", "CVE_MAT");
            dHeaders.Add("cve_gpo", "CVE_GPO");
            dHeaders.Add("cve", "CLAVEMAT");
            dHeaders.Add("cverpe", "CVERPE");
            dHeaders.Add("tipo", "TIPO");    //*
            dHeaders.Add("salon", "SALON");  //*
            dHeaders.Add("lunes", "LUNES");
            dHeaders.Add("lunesf", "LUNESF");
            dHeaders.Add("martes", "MARTES");
            dHeaders.Add("martesf", "MARTESF");
            dHeaders.Add("miercoles", "MIERCOLES");
            dHeaders.Add("miercolesf", "MIERCOLESF");
            dHeaders.Add("jueves", "JUEVES");
            dHeaders.Add("juevesf", "JUEVESF");
            dHeaders.Add("viernes", "VIERNES");
            dHeaders.Add("viernesf", "VIERNESF");
            dHeaders.Add("sabado", "SABADO");
            dHeaders.Add("sabadof", "SABADOF");
            dHeaders.Add("cupo", "CUPO");
            dHeaders.Add("ciclo", "CICLO");  //*

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
        /// <returns>Hojas del excel</returns>
        private DataTableCollection GetSheets()
        {
            //Es necesaria para que se inicialize ContCoumn
            FileStream stream = File.Open(dir, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            if (dir.Contains(".xlsx"))
                // De lo contrario descomentaremos esta (2007 format; *.xlsx)
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
        /// <returns></returns>
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
        /// Lee el excel e inicializa los grupos con la ainfromacion
        /// </summary>
        /// <param name="hoja">Hoja de excel</param>
        public List<Grupo> GetGrupos(string hoja)
        {
            DataTable dt = GetSheets()[hoja];

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
                    worksheet = (doc.GetPartById(aux.Id.Value) as WorksheetPart).Worksheet;
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
                refCell = CreaCelda(materia[g.Cve_materia], r, 3);
            else
                refCell = CreaCelda(Convert.ToInt32(g.Cve_materia) * 100 + g.num_Grupo, r, 3);
            if(profesor!=null)
                refCell = CreaCelda(profesor[g.RPE], r, 4);
            else
                refCell = CreaCelda(g.RPE, r, 4);

            refCell = CreaCelda(g.Tipo, r, 5);
            refCell = CreaCelda(g.Salon, r, 6);
            refCell = CreaCelda(g.horario_ini[0], r, 7);
            refCell = CreaCelda(g.horario_fin[0], r, 8);
            refCell = CreaCelda(g.horario_ini[1], r, 9);
            refCell = CreaCelda(g.horario_fin[1], r, 10);
            refCell = CreaCelda(g.horario_ini[2], r, 11);
            refCell = CreaCelda(g.horario_fin[2], r, 12);
            refCell = CreaCelda(g.horario_ini[3], r, 13);
            refCell = CreaCelda(g.horario_fin[3], r, 14);
            refCell = CreaCelda(g.horario_ini[4], r, 15);
            refCell = CreaCelda(g.horario_fin[4], r, 16);
            refCell = CreaCelda(g.horario_ini[5], r, 17);
            refCell = CreaCelda(g.horario_fin[5], r, 18);
            refCell = CreaCelda(g.Cupo, r, 19);
            refCell = CreaCelda(g.Ciclo, r, 20);
            refCell = CreaCelda("-"+g.observaciones, r, 21);
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