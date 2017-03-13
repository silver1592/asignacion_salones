using Algoritmo01.Clases;
using OrigenDatos.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// Notas:
///     Agregar un modulo para que pueda leer la informacion de un excel y 
///     asignarle la informacion de las necesidades de las materias a la 
///     base de datos por medio de el
/// </summary>

namespace Interfaz_02.Pantallas
{
    public partial class Requerimientos_Materia : Form
    {
        //Bandera que indica si el programa esta inicializando los controles
        private bool EnCarga;

        public Requerimientos_Materia()
        {
            InitializeComponent();
        }

        #region Load
        private void Requerimientos_Materia_Load(object sender, EventArgs e)
        {
            EnCarga = true;

            //Carga los combo box con la informacion correspondiente
            cargaMateria();
            cargaProfesor();
            cargaEquipo();

            actualizaGrid();

            EnCarga = false;
        }

        private void cargaMateria()
        {
            //consulta en la base de datos y obten la informacion sobre las materias.
            Conexion cn = new Conexion(Conexion.datosConexionPrueba);

            DataTable dt = cn.Materias();
            cve_comboBox.DataSource = dt;
            materia_comboBox.DataSource = dt;

            cve_comboBox.ValueMember = "Materia";
            cve_comboBox.DisplayMember = "Materia";

            materia_comboBox.DisplayMember = "Nombre";
            materia_comboBox.ValueMember = "Materia";

            cve_comboBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            materia_comboBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();

            foreach(DataRow r in dt.Rows)
            {
                cve_comboBox.AutoCompleteCustomSource.Add(r["Materia"].ToString());
                materia_comboBox.AutoCompleteCustomSource.Add(r["Nombre"].ToString());
            }
        }

        private void cargaProfesor()
        {
            //consulta en la base de datos y obten la informacion sobre los profesores
            Conexion cn = new Conexion(Conexion.datosConexionPrueba);

            DataTable dt = cn.Profesores();
            rpe_comboBox.DataSource = dt;
            nombre_Profesor_comboBox.DataSource = dt;

            rpe_comboBox.ValueMember = "rpe";
            rpe_comboBox.DisplayMember = "rpe";

            nombre_Profesor_comboBox.ValueMember = "rpe";
            nombre_Profesor_comboBox.DisplayMember = "Nombre";

            foreach (DataRow r in dt.Rows)
            {
                nombre_Profesor_comboBox.AutoCompleteCustomSource.Add(r["Nombre"].ToString());
                rpe_comboBox.AutoCompleteCustomSource.Add(r["rpe"].ToString());
            }
        }

        private void cargaEquipo()
        {
            //Consulta la base de datos y obtiene la informacion de los 
            //equipos que pueden estar instalados en los salones
            Conexion cn = new Conexion(Conexion.datosConexionPrueba);

            DataTable dt = cn.Equipo();
            equipo_comboBox.DataSource = dt;

            equipo_comboBox.ValueMember = "cve_equipo";
            equipo_comboBox.DisplayMember = "equipo";

            foreach (DataRow r in dt.Rows)
                equipo_comboBox.AutoCompleteCustomSource.Add(r["equipo"].ToString());
        }

        private void actualizaGrid()
        {
            Conexion cn = new Conexion(Conexion.datosConexionPrueba);
            string query = "select equipo, peso from asignacion.ae_necesidades_curso inner join asignacion.ae_cat_equipo on idEquipo = cve_equipo where cve_materia = '"+ cve_comboBox.SelectedValue.ToString()+"'";

            if (rpe_comboBox.SelectedValue.ToString() != "0" && rpe_comboBox.SelectedValue.ToString()!="")
                query += " rpe = " + rpe_comboBox.SelectedValue.ToString();

            DataTable dt = cn.Querry(query);

            detalles_equipo_dataGrid.DataSource = dt;
        }
        #endregion

        #region Eventos Materia
        private void materia_comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //Debe checar si cve_comboBox no tiene el mismo valor, y si no entonces lo cambia a ese valor
            if (!EnCarga)
            {
                if (cve_comboBox.SelectedValue != materia_comboBox.SelectedValue)
                    cve_comboBox.SelectedValue = materia_comboBox.SelectedValue;

                actualizaGrid();
            }
        }

        private void cve_comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //Debe checar si materia_comboBox no tiene el mismo valor, y si no entonces lo cambia por el suyo
            if (!EnCarga)
            {
                if (cve_comboBox.SelectedValue != materia_comboBox.SelectedValue)
                    materia_comboBox.SelectedValue = cve_comboBox.SelectedValue;

                actualizaGrid();
            }
        }

        #endregion

        #region Eventos Profesor
        private void rpe_comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!EnCarga)
            {
                if (rpe_comboBox.SelectedValue != nombre_Profesor_comboBox.SelectedValue)
                    nombre_Profesor_comboBox.SelectedValue = rpe_comboBox.SelectedValue;

                actualizaGrid();
            }
        }

        private void nombre_Profesor_comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!EnCarga)
            {
                if (rpe_comboBox.SelectedValue != nombre_Profesor_comboBox.SelectedValue)
                    rpe_comboBox.SelectedValue = nombre_Profesor_comboBox.SelectedValue;

                actualizaGrid();
            }
        }
        #endregion

        #region Eventos Detalles de Materia
        private void puntos_trackBar_Scroll(object sender, EventArgs e)
        {
            puntos_textBox.Text = puntos_trackBar.Value.ToString();
        }

        private void aniadir_button_Click(object sender, EventArgs e)
        {
            //Obtiene informacion de los textbox
            string comando;

            string claveMat = cve_comboBox.SelectedValue.ToString() == materia_comboBox.SelectedValue.ToString() ? cve_comboBox.SelectedValue.ToString() : "";

            string rpe = rpe_comboBox.SelectedValue.ToString();

            string equipo = equipo_comboBox.SelectedValue.ToString();
            string puntos = puntos_textBox.Text!="" ? puntos_textBox.Text : "0";

            string tipo = tipo_Materia_comboBox.Text != "" ? tipo_Materia_comboBox.Text : "T";

            //checa si existe el registro
            Conexion cn = new Conexion(Conexion.datosConexionPrueba);
            DataTable dt = cn.Necesidades_Grupo(claveMat, "T", rpe);

            var query = from DataRow r in dt.Rows
                        where r["idEquipo"].ToString() == equipo
                        select r;

            if (query.ToList().Count != 0)
            {
                //modifica
                comando = "UPDATE[asignacion].[ae_necesidades_curso] "
                          + "SET[peso] = " + puntos
                          + " WHERE [cve_materia] = '"+claveMat+"' and [rpe] = "+rpe+" and [idEquipo] = "+equipo;
            }
            else
            {
                //crear
                comando = "INSERT INTO[asignacion].[ae_necesidades_curso] ([cve_materia],[rpe],[tipo],[idEquipo],[peso]) "
                          + "VALUES('"+claveMat
                          + "'," + rpe
                          + ",'"+ tipo
                          + "'," + equipo
                          + ","+puntos+")";
            }

            cn.Comando(comando);

            actualizaGrid();
        }

        private void eliminar_button_Click(object sender, EventArgs e)
        {
            string comando;

            string claveMat = cve_comboBox.SelectedValue.ToString() == materia_comboBox.SelectedValue.ToString() ? cve_comboBox.SelectedValue.ToString() : "";

            string rpe = rpe_comboBox.SelectedValue.ToString();

            string equipo = equipo_comboBox.SelectedValue.ToString();
            string puntos = puntos_textBox.Text != "" ? puntos_textBox.Text : "0";

            //checa si existe el registro
            Conexion cn = new Conexion(Conexion.datosConexionPrueba);
            DataTable dt = cn.Necesidades_Grupo(claveMat, "T", rpe);

            var query = from DataRow r in dt.Rows
                        where r["idEquipo"].ToString() == equipo
                        select r;

            if (query.ToList().Count != 0)
            {
                //elimina
                comando = "DELETE FROM[asignacion].[ae_necesidades_curso]"
                          + " WHERE [cve_materia] = '" + claveMat + "'and [rpe] = " + rpe + " and [idEquipo] = " + equipo;

                cn.Comando(comando);

                actualizaGrid();
            }
        }
        #endregion
    }
}
