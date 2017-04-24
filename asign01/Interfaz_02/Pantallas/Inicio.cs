using Interfaz_02.Heredados;
using OrigenDatos.Clases;
using System;
using System.Windows.Forms;

namespace Interfaz_02
{
    public partial class Inicio : Form
    {
        private string dir;
        private string archivo;
        private LibroExcel excel;
        private string ciclo;

        public Inicio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Abre ventana de busqueda de archivos
            OpenFileDialog dlgOpen = new OpenFileDialog();

            //Si se acepta
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                dir = dlgOpen.FileName.Replace(dlgOpen.SafeFileName, "");
                archivo = dlgOpen.SafeFileName;
                //guarda dir
                //guarda nombreArchivo

                cargaExcel();
            }
        }

        private void cargaExcel()
        {
            excel = new LibroExcel(dir, archivo, ciclo);

            archivo_textBox.Text = dir + archivo;

            //llena comboBox
            hojas_comboBox.DataSource = excel.getHojas();
        }

        private void hojas_comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            string hojaSeleccionada = hojas_comboBox.SelectedValue.ToString();

            if (hojaSeleccionada!=null && hojaSeleccionada!="")
            {
                excel.setHojaHorarios(hojaSeleccionada);
                datos_dataGridView.DataSource = excel.RawGrupos;
            }
            
        }

        /// <summary>
        /// Asigna las características predefinidas para los DataGrid
        /// (solo lectura)
        /// </summary>
        /// <param name="dlg_DGV">DataGrid a inicializar</param>
        public static void iniciaDataGrid(DataGridView dlg_DGV)
        {
            dlg_DGV.AllowDrop = false;
            dlg_DGV.AllowUserToAddRows = false;
            dlg_DGV.AllowUserToDeleteRows = false;
            dlg_DGV.AllowUserToOrderColumns = true;
            dlg_DGV.AllowUserToResizeRows = false;
            dlg_DGV.ReadOnly = true;
            dlg_DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            iniciaDataGrid(datos_dataGridView);
        }

        private void datos_dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
