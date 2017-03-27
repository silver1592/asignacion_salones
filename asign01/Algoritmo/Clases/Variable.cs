using Algoritmo01.Heredados;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo01.Clases
{
    public class Variable
    {
        private Salon salon;
        private Grupo grupo;
        private int Hora;

        public int horario { get { return Hora; } }

        public Salon Salon
        {
            get { return salon; }
            set
            {
                if (salon != null)
                {
                    salon.remueveGrupo(grupo);
                    grupo.Salon = "";
                }

                salon = value;

                if (salon != null)
                {
                    salon.agregaGrupo(grupo);
                    grupo.Salon = salon.Cve_espacio;
                }
            }
        }
        public Grupo Grupo { get { return grupo; } }

        /// <summary>
        /// Constructor de una variable (cromosoma) del individuo
        /// --No es necesario asignarle un salon
        /// </summary>
        /// <param name="salon">Salon que va a representar</param>
        public Variable(Grupo grupo, int hora)
        {
            this.grupo = grupo;
            Hora = hora;
            salon = null;
        }

        /// <summary>
        /// Retorna cuantos son los puntos que tiene esta asignacion para el algoritmo
        /// </summary>
        public float puntos
        {
            get
            {
                if (salon != null)
                {
                    return calcula_PuntosSalon(salon);
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Retorna los puntos que se tiene con el equipo instalado en el salon y los que necesita el grupo
        /// </summary>
        public float puntosEquipo
        {
            get
            {
                float pT = grupo.valorTotalEquipo;
                float pE = grupo.valorEquipo(salon);

                if (pT == 0)
                    return 1;

                return pE / pT;
            }
        }

        /// <summary>
        /// checa si un grupo es compatible con el salon
        /// </summary>
        /// <param name="salon">salon a checar compatibilidad</param>
        /// <returns>Si el grupo es valido para asignarse en este salon</returns>
        public bool valido(Salon salon)
        {
            bool res = false;

            if (grupo.SalonValido(salon) > 0)
                res = true;

            return res;
        }

        /// <summary>
        /// Calcula los puntos que tiene el grupo con el salon solicitado
        /// </summary>
        /// <param name="s">Salon con que se quiere checar</param>
        /// <returns>Valor que se obtubo de los puntos. \nSi es -1 quiere decir que no es un salon valido</returns>
        public float calcula_PuntosSalon(Salon s)
        {
            //CHECA SI EL PROFESOR TIENE UN SALON FIJO Y CHECA SI ES EL SALON AL QUE SE LE ASIGNO, Y DE SER ASI REGRESA EL VALOR INMEDIATAMENTE, SI NO 
            if (grupo.Salon_fijo != null && grupo.Salon_fijo != "")
            if (grupo.Salon_fijo == s.Cve_espacio)
                return 10;
            else
                return -1;

            //INICIALIZA EL VALOR DEL GRUPO AL VALOR QUE LE DA EL AREA.
            float fRes = grupo.SalonValido(s);

            //CHECA SI EL PROFESOR REQUERE ESTAR EN LA PLANTA BAJA Y SI EL SALON TAMBIEN ESTA EN LA PLANTA BAJA
            //La validacion comentada es para cuando se haya instalado un elevador en el edificio T
            if (grupo.PlantaBaja && (!s.plantaBaja /*|| salon.Edificio == "T"*/))
                fRes = -1;

            if (fRes > -1)
                fRes *= puntosEquipo;

            return fRes;
        }

        public bool salonHoraAnterior(ListaSalones s)
        {
            return ((ListaGrupos)grupo.GruposAnteriores).EnSalones(s,grupo);
        }
    }
}
