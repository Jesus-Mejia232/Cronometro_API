namespace Cronometro.API_.Models
{
    public class ProyectoTiempoParcialDto
    {
        public int RegistroID { get; set; }

        public string ProyectoCode { get; set; }

        public string Descripcion { get; set; }

        public TimeSpan? HoraInicio { get; set; }

        public DateTime? FechaWork { get; set; }

        public DateTime? FechaSystema { get; set; }

        public string Nombreusuario { get; set; }

        public bool? EstadoTrabajo { get; set; }

        public string Referencia { get; set; }
    }
}
