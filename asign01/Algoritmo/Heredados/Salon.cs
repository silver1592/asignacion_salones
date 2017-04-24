using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo01.Heredados
{
    public class Salon : OrigenDatos.Clases.Salon
    {
        #region Constructores
        /// <summary>
        /// Constructor por copia </br>
        /// (Solo usarlo para asignarlo a las variables del algoritmo)
        /// </summary>
        /// <param name="s">Salon a copiar</param>
        public Salon(Salon s):base(s){}

        public Salon(DataRow salon, int hora, DataTable excep=null, DataTable Equipo=null, DataTable AreaEdif=null):base(salon,hora,excep,Equipo){}

        #endregion

        /// <summary>
        /// Checa si hay horario para el grupo que que se le pasa por parametro.
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public bool Disponible_para_grupo(Grupo grupo)
        {
            for(int i=0; i<6;i++)
                if (horario[i] && grupo.dias(hora)[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Puntos que otorga al Grupo
        /// -corregir-
        /// que la salida sea una estructura de multiples puntos
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public float puntosCon(Grupo grupo)
        {
            float p = 0;

            if (gruposAsignados.Grupos_Empalmados(grupo).Count() == 0)
                p += grupo.SalonValido(this);
            else
                p = -1;

            return p;
        }

        /// <summary>
        /// Busca los grupos dentro de una lista que esten asignados en el salon y los almacena 
        /// para poder generar un horario de un salon
        /// </summary>
        /// <param name="hora">Solo se genera el horario por hora</param>
        /// <param name="ciclo">Ciclo escolar a generar el horario</param>
        public void ObtenHorario(List<Grupo> grupos)
        {
            var query = from Grupo g in grupos
                        where g.Salon == Cve_espacio
                        select g;

            foreach (Grupo g in query.ToList<Grupo>())
                gruposAsignados.Add(g);
        }
    }
}
