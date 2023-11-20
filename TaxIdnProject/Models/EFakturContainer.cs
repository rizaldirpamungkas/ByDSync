using System.ComponentModel.DataAnnotations.Schema;

namespace TaxIdnProject.Models
{
    public class EFakturContainer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string invNum { get; set; }
        public string? refInvNum { get; set; }
        public bool isMasukan { get; set; }
        public bool isKreditable { get; set; }
        public string? KD_JENIS_TRANSAKSI { get; set; }
        public string? FG_PENGGANTI { get; set; }
        public string? NOMOR_FAKTUR { get; set; }
        public string? MASA_PAJAK { get; set; }
        public string? TAHUN_PAJAK { get; set; }
        public DateTime? TANGGAL_FAKTUR { get; set; }
        public string? NPWP { get; set; }
        public string? NAMA { get; set; }
        public string? ALAMAT_LENGKAP { get; set; }
        public double? JUMLAH_DPP { get; set; }
        public double? JUMLAH_PPN { get; set; }
        public double? JUMLAH_PPNBM { get; set; }
        public string? ID_KETERANGAN_TAMBAHAN { get; set; }
        public double? FG_UANG_MUKA { get; set; }
        public double? UANG_MUKA_DPP { get; set; }
        public double? UANG_MUKA_PPN { get; set; }
        public double? UANG_MUKA_PPNBM { get; set; }
        public string? REFERENSI { get; set; }
        public string? KODE_DOKUMEN_PENDUKUNG { get; set; }
        public string? OF { get; set; }
        public string? FK { get; set; }
        public string? FM { get; set; }
        public string? KODE_OBJEK { get; set; }
        public string? NAMA_OBJEK { get; set; }
        public double? HARGA_SATUAN { get; set; }
        public double? JUMLAH_BARANG { get; set; }
        public double? HARGA_TOTAL { get; set; }
        public double? DISKON { get; set; }
        public double? DPP { get; set; }
        public double? PPN { get; set; }
        public double? TARIF_PPNBM { get; set; }
        public double? PPNBM { get; set; }
    }
}
