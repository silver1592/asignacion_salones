using System.Data;

namespace Interfaz_02.Heredados
{
    public class Salon : Algoritmo01.Heredados.Salon
    {
        #region Constructores
        /// <summary>
        /// Constructor por copia </br>
        /// (Solo usarlo para asignarlo a las variables del algoritmo)
        /// </summary>
        /// <param name="s">Salon a copiar</param>
        public Salon(Salon s) : base(s){}

        public Salon(DataRow salon, int hora, DataTable excep=null, DataTable Equipo=null, DataTable AreaEdif=null):base(salon, hora, excep, Equipo, AreaEdif) {}

        #endregion
    }
}
