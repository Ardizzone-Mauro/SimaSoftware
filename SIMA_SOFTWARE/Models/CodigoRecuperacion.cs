namespace SIMA_SOFTWARE.Models
{
    public class CodigoRecuperacion
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Codigo { get; set; }
        public DateTime Expiracion { get; set; }
        public bool Usado { get; set; }

    }
}
