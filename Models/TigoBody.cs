using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testApi.Models
{
    public class TigoBody
    {        
        public String ID_AGENTE { get; set; } 
        public String ID_TRANSACCION { get; set; }
        public String TELEFONO_CLIENTE { get; set; }
        public String FECHA_TRANSACCION { get; set; }
        public String HORA_TRANSACCION { get; set; }
        public String ESTATUS_LLAMADA { get; set; }
        public String TIPO_PRODUCTO { get; set; }
        public String TIPO_TRANSACCION_1 { get; set; }
        public String TIPO_TRANSACCION_2 { get; set; }
        public String TIPO_TRANSACCION_3 { get; set; }
        public String TIEMPO_ESPERA { get; set; }
        public String TIEMPO_ATENCION { get; set; }
        public String FLAG_B2B { get; set; }
        public String VDN { get; set; }
    }
}