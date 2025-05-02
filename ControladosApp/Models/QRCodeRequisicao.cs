using SQLite;

namespace ControladosApp.Models
{
    public class QRCodeRequisicao
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int NumeroRequisicao { get; set; }
        public string Barcode { get; set; }
        public string InseridoPor { get; set; } = "api"!;
        public DateTime InseridoEm { get; set; } = DateTime.Now;
        public bool Sincronizado { get; set; } = false;
    }
}
