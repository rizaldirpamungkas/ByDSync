using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace TaxIdnProject.Data.EFakturStructure
{
    public class HeaderFakturOut
    {
        public string FK { get; set; }
        public string KD_JENIS_TRANSAKSI { get; set; }
        public string FG_PENGGANTI { get; set; }
        public string NOMOR_FAKTUR { get; set; }
        public int MASA_PAJAK { get; set; }
        public int TAHUN_PAJAK { get; set; }
        public DateOnly TANGGAL_FAKTUR { get; set; }
        public string NPWP { get; set; }
        public string NAMA { get; set; }
        public string ALAMAT_LENGKAP { get; set; }
        public  double JUMLAH_DPP { get; set; }
        public double JUMLAH_PPN { get; set; }
        public double JUMLAH_PPNBM { get; set; }
        public string? ID_KETERANGAN_TAMBAHAN { get; set; }
        public double FG_UANG_MUKA { get; set; }
        public double UANG_MUKA_DPP { get; set; }
        public double UANG_MUKA_PPN { get; set; }
        public double UANG_MUKA_PPNBM { get; set; }
        public string? REFERENSI { get; set; }
    }

    public class LtFakturOut
    {
        public string LT { get; set; }
        public string NPWP { get; set; }
        public string NAMA { get; set; }
        public string JALAN { get; set; }
        public string BLOK { get; set; }
        public string NOMOR { get; set; }
        public string RT { get; set; }
        public string RW { get; set; }
        public string KECAMATAN { get; set; }
        public string KELURAHAN { get; set; }
        public string KABUPATEN { get; set; }
        public string PROPINSI { get; set; }
        public string KODE_POS { get; set; }
        public string NOMOR_TELEPON { get; set; }
    }

    public class ObjekFakturOut
    {
        public string OF { get; set; }
        public string? KODE_OBJEK { get; set; }
        public string NAMA { get; set; }
        public double HARGA_SATUAN { get; set; }
        public double JUMLAH_BARANG { get; set; }
        public double HARGA_TOTAL { get; set; }
        public double DISKON { get; set; }
        public double DPP { get; set; }
        public double PPN { get; set; }
        public double TARIF_PPNBM { get; set; }
        public double PPNBM { get; set; }
    }

    public class BodyFakturIn
    {
        public string FM { get; set; }
        public string KD_JENIS_TRANSAKSI { get; set; }
        public string FG_PENGGANTI { get; set; }
        public string NOMOR_FAKTUR { get; set; }
        public int MASA_PAJAK { get; set; }
        public int TAHUN_PAJAK { get; set; }
        public DateOnly TANGGAL_FAKTUR { get; set; }
        public string NPWP { get; set; }
        public string NAMA { get; set; }
        public string ALAMAT_LENGKAP { get; set; }
        public double JUMLAH_DPP { get; set; }
        public double JUMLAH_PPN { get; set; }
        public double JUMLAH_PPNBM { get; set; }
        public int IS_CREDITABLE { get; set; }

        public string REFERENSI { get; set; }
    }
}
