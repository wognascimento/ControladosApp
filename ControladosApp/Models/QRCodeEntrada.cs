using SQLite;

namespace ControladosApp.Models
{
    public class QRCodeEntrada
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string InseridoPor { get; set; } = "api"!;
        public DateTime InseridoEm { get; set; } = DateTime.Now;
        public bool Sincronizado { get; set; } = false;
    }
}
