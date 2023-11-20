using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.ServiceModel;
using System.Text;
using TaxIdnProject.Miscs;
using TaxIdnProject.Models;
using CsvHelper;
using System.Globalization;
using TaxIdnProject.Data.EFakturStructure;
using System.Reflection.Metadata;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

namespace TaxIdnProject.Data
{
    public class TextCollection
    {
        public string ParentObjectID { get; set; }
        public string TypeCode { get; set; }
        public string Text { get; set; }
    }

    public class ByDSync
    {
        TaxIdnContext context;

        public ByDSync(TaxIdnContext _context) 
        {
            context = _context;
        }

        public string? getSupplierNPWP(HttpClient client, Parameters param, Utils utils, string sellerPartyNum)
        {
            string npwp = "";
            
            string endpointAddress = param.uriAPI["SupplierMaster"] + String.Format("&$filter=InternalID eq '{0}'", sellerPartyNum);

            var responseSupplier = client.GetAsync(endpointAddress);
            responseSupplier.Wait();

            var resultSupplier = responseSupplier.Result;
            string? jsonRawSupplier;
            JArray? jsonSupplierEnumerable = null;
            
            if (resultSupplier.IsSuccessStatusCode)
            {
                var messageResponseSupplier = resultSupplier.Content.ReadAsStringAsync();
                messageResponseSupplier.Wait();

                jsonRawSupplier = messageResponseSupplier.Result;
                jsonSupplierEnumerable = utils.parseJson(jsonRawSupplier);
            }

            if (jsonSupplierEnumerable?.Count > 0)
            {
                JArray? supplierTaxNumber = jsonSupplierEnumerable.First().Value<JArray>("TaxNumber");

                JToken? npwpToken =  supplierTaxNumber?.FirstOrDefault(x => (string)x["TaxIdentificationNumberTypeCode"] == "1");

                if (npwpToken != null)
                {
                    npwp = npwpToken.Value<JToken>("PartyTaxID").ToString();
                }
                else
                {
                    if (supplierTaxNumber.Count > 0)
                    {
                        npwpToken = supplierTaxNumber?.First();

                        if (npwpToken != null)
                        {
                            npwp = npwpToken.Value<JToken>("PartyTaxID").ToString();
                        }
                    }
                }

            }

            return npwp;
        }

        public string? getCustomerNPWP(HttpClient client, Parameters param, Utils utils, string sellerPartyNum)
        {
            string npwp = "";

            string endpointAddress = param.uriAPI["CustomerMaster"] + String.Format("&$filter=InternalID eq '{0}'", sellerPartyNum);

            var responseSupplier = client.GetAsync(endpointAddress);
            responseSupplier.Wait();

            var resultSupplier = responseSupplier.Result;
            string? jsonRawSupplier;
            JArray? jsonSupplierEnumerable = null;

            if (resultSupplier.IsSuccessStatusCode)
            {
                var messageResponseSupplier = resultSupplier.Content.ReadAsStringAsync();
                messageResponseSupplier.Wait();

                jsonRawSupplier = messageResponseSupplier.Result;
                jsonSupplierEnumerable = utils.parseJson(jsonRawSupplier);
            }

            if (jsonSupplierEnumerable?.Count > 0)
            {
                JArray? supplierTaxNumber = jsonSupplierEnumerable.First().Value<JArray>("TaxNumber");

                JToken? npwpToken = supplierTaxNumber?.FirstOrDefault(x => (string)x["TaxIdentificationNumberTypeCode"] == "1");

                if (npwpToken != null)
                {
                    npwp = npwpToken.Value<JToken>("TaxNumberID").ToString();
                }
                else
                {
                    if (supplierTaxNumber.Count > 0)
                    {
                        npwpToken = supplierTaxNumber?.First();

                        if (npwpToken != null)
                        {
                            npwp = npwpToken.Value<JToken>("TaxNumberID").ToString();
                        }
                    }
                }

            }

            return npwp;
        }

        public void populateEfakturIn(DateOnly dariTanggal, DateOnly sampaiTanggal, string kodeSupplier)
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            string tanggal;
            DateTime date = DateTime.MinValue;

            context.Database.ExecuteSqlRaw("DELETE FROM " + param.tableName["SupplierInvoice"]+ " WHERE isMasukan = 1");

            using (var client = new HttpClient())
            {                
                var byteArray = Encoding.ASCII.GetBytes(param.securityWorkcenter["Username"] + ":" + param.securityWorkcenter["Password"]); 
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                string filter = "";

                if (kodeSupplier == "")
                {
                    filter = String.Format("&$filter=TransactionDate ge datetime'{0}' and " +
                    "TransactionDate le datetime'{1}' and PostingStatusCode eq '3'"
                    , dariTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), sampaiTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
                }
                else
                {
                    filter = String.Format("&$filter=TransactionDate ge datetime'{0}' and " +
                    "TransactionDate le datetime'{1}' and PostingStatusCode eq '3' and " +
                    "SellerParty/PartyID eq '{2}'", dariTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), 
                    sampaiTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), kodeSupplier);
                }

                string uriTarget = param.uriAPI["SupplierInvoiceItem"] + filter;

                var responseTask = client.GetAsync(uriTarget);
                responseTask.Wait();

                var result = responseTask.Result;

                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception();
                }

                var message = result.Content.ReadAsStringAsync();
                message.Wait();

                string jsonRaw = message.Result;
                JArray jsonEnumerableData = utils.parseJson(jsonRaw);

                List<EFakturContainer> eFaktur = new List<EFakturContainer>();

                foreach (JObject data in jsonEnumerableData)
                {
                    EFakturContainer eFakturContainer = new EFakturContainer();
                    
                    JToken? sellerParty = data.Value<JToken>("SellerParty");
                    JArray? sellerPartyName = sellerParty?.Value<JArray>("SellerPartyName");
                    string? sellerName = sellerPartyName?.First()?.Value<JToken>("FormattedName")?.ToString();
                    JArray? sellerPartyAddress = sellerParty?.Value<JArray>("SellerPartyAddress");
                    string? sellerAddress = sellerPartyAddress?.First()?.Value<JToken>("FormattedPostalAddressDescription")?.ToString();
                    string? sellerPartyNum = sellerParty?.Value<JToken>("PartyID")?.ToString();
                    JArray? invoiceItem = data.Value<JArray?>("Item");

                    string? npwp = getSupplierNPWP(client, param, utils, sellerPartyNum);

                    tanggal = data.Value<JToken>("InvoiceDate").ToString();

                    if(tanggal != "")
                        date = DateTime.Parse(tanggal);

                    eFakturContainer.isMasukan = true;
                    eFakturContainer.FM = "FM";
                    eFakturContainer.KD_JENIS_TRANSAKSI = "01";
                    eFakturContainer.FG_PENGGANTI = "0";

                    eFakturContainer.NOMOR_FAKTUR = "";
                    eFakturContainer.MASA_PAJAK = date.Year.ToString();
                    eFakturContainer.NPWP = npwp;
                    eFakturContainer.NAMA = sellerName;
                    eFakturContainer.ALAMAT_LENGKAP = sellerAddress;

                    string jumlahDppText = data.Value<JToken>("TotalNetAmount").ToString();
                    string jumlahPpnText = data.Value<JToken>("TotalTaxAmount").ToString();

                    string[] dppText, ppnText;

                    dppText = jumlahDppText.Split('.', StringSplitOptions.TrimEntries);
                    ppnText = jumlahPpnText.Split('.', StringSplitOptions.TrimEntries);

                    eFakturContainer.JUMLAH_DPP = double.Parse(dppText[0]);
                    eFakturContainer.JUMLAH_PPN = double.Parse(ppnText[0]);
                    eFakturContainer.JUMLAH_PPNBM = 0;

                    eFakturContainer.isKreditable = false;
                    eFakturContainer.TANGGAL_FAKTUR = date;

                    eFakturContainer.invNum = data.Value<JToken>("ObjectID").ToString();

                    if (eFakturContainer.invNum == null)
                        eFakturContainer.invNum = "-";

                    eFaktur.Add(eFakturContainer);

                    /*foreach (JObject invcItem in invoiceItem)
                    {
                        string hargaNetText = invcItem.Value<JToken>("NetAmount")?.ToString();
                        hargaNetText = hargaNetText.Replace('.', ',');
                        //string[] hargaNetSplit = hargaNetText.Split('.', StringSplitOptions.TrimEntries);

                        string qtyText = invcItem.Value<JToken>("Quantity")?.ToString();
                        qtyText = qtyText.Replace(".", ",");
                        //string[] qtySplit = qtyText.Split('.', StringSplitOptions.TrimEntries);

                        string hargaTotalText = invcItem.Value<JToken>("GrossAmount")?.ToString();
                        hargaTotalText = hargaTotalText.Replace('.', ',');
                        //string[] hargaTotalSplit = hargaTotalText.Split('.', StringSplitOptions.TrimEntries);

                        string nilaiPpnText = invcItem.Value<JToken>("TaxAmount")?.ToString();
                        nilaiPpnText = nilaiPpnText.Replace('.', ',');
                        //string[] nilaiPpnSplit = nilaiPpnText.Split('.', StringSplitOptions.TrimEntries);

                        double hargaNetAmount = double.Parse(hargaNetText);
                        double qtyAmount = double.Parse(qtyText);
                        double hargaTotalAmount = double.Parse(hargaTotalText);
                        double nilaiPpnAmount = double.Parse(nilaiPpnText);

                        eFakturContainer = new EFakturContainer();

                        eFakturContainer.isMasukan = true;
                        eFakturContainer.OF = "OF";
                        eFakturContainer.refInvNum = data?.Value<JToken>("ID")?.ToString();
                        eFakturContainer.KODE_OBJEK = invcItem.Value<JToken>("ProductID")?.ToString();
                        eFakturContainer.NAMA_OBJEK = invcItem.Value<JToken>("Description")?.ToString();

                        if (hargaNetAmount != 0 && qtyAmount != 0)
                        {
                            eFakturContainer.HARGA_SATUAN = Math.Round(hargaNetAmount / qtyAmount, 2);
                        }

                        eFakturContainer.HARGA_TOTAL = hargaTotalAmount;
                        eFakturContainer.DPP = hargaNetAmount;
                        eFakturContainer.PPN = nilaiPpnAmount;

                        eFakturContainer.invNum = data.Value<JToken>("ID").ToString();

                        if (eFakturContainer.invNum == null)
                            eFakturContainer.invNum = "-";

                        eFaktur.Add(eFakturContainer);
                    }*/
                }

                context.EFakturContainer.AddRange(eFaktur);
                context.SaveChanges();
            }
        }

        public static string? OnGetSrfToken(CookieContainer cookieContainer)
        {
            Parameters param = new Parameters();
            HttpHeaders header;

            string token;

            using (var handler = new HttpClientHandler())
            {
                handler.CookieContainer = cookieContainer;
                handler.UseCookies = true;

                using (var client = new HttpClient(handler))
                {
                    var byteArray = Encoding.ASCII.GetBytes(param.securityWorkcenter["Username"] + ":" + param.securityWorkcenter["Password"]);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string uriTarget = param.uriAPI["CustomerInvoice"];

                    client.DefaultRequestHeaders.Add("x-csrf-token", "fetch");

                    var responseTask = client.GetAsync(uriTarget);
                    responseTask.Wait();

                    header = responseTask.Result.Headers;

                    IEnumerable<Cookie> responseCookies = cookieContainer.GetCookies(new Uri(uriTarget));
                }
            }

            token = header.GetValues("x-csrf-token").First();

            return token;
        }

        public void populateEfakturOut(DateOnly dariTanggal, DateOnly sampaiTanggal, string kodeCustomer)
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            string tanggal;
            DateTime date = DateTime.MinValue;

            context.Database.ExecuteSqlRaw("DELETE FROM " + param.tableName["CustomerInvoice"] + " WHERE isMasukan = 0");

            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(param.securityWorkcenter["Username"] + ":" + param.securityWorkcenter["Password"]);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                //HTTP GET
                string filter = "";

                if (kodeCustomer == "")
                {
                    filter = String.Format("&$filter=Date ge datetime'{0}' and " +
                    "Date le datetime'{1}' and ReleaseStatusCode eq '3'"
                    , dariTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), sampaiTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
                }
                else
                {
                    filter = String.Format("&$filter=Date ge datetime'{0}' and " +
                    "Date le datetime'{1}' and ReleaseStatusCode eq '3' and " +
                    "BuyerParty/PartyID eq '{2}'", dariTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"),
                    sampaiTanggal.ToDateTime(TimeOnly.MinValue).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), kodeCustomer);
                }

                string uriTarget = param.uriAPI["CustomerInvoice"] + filter;

                var responseTask = client.GetAsync(uriTarget);
                responseTask.Wait();

                var result = responseTask.Result;

                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception();
                }

                var message = result.Content.ReadAsStringAsync();
                message.Wait();

                string jsonRaw = message.Result;
                JArray jsonEnumerableData = utils.parseJson(jsonRaw);

                List<EFakturContainer> eFaktur = new List<EFakturContainer>();

                /*MaintainCustInvoiceRequest.MaintainBundleRequest bundleRequest = new MaintainCustInvoiceRequest.MaintainBundleRequest();
                MaintainCustInvoiceRequest.MaintainBundleResponse bundleResponse = new MaintainCustInvoiceRequest.MaintainBundleResponse();

                BasicHttpsBinding httpsBinding = new BasicHttpsBinding();
                EndpointAddress endpointAddress = new EndpointAddress("");

                MaintainCustInvoiceRequest.ManageCustomerInvoiceRequestInClient requestInClient = new MaintainCustInvoiceRequest.ManageCustomerInvoiceRequestInClient(httpsBinding, endpointAddress);

                requestInClient.MaintainBundle(bundleRequest.CustomerInvoiceRequestBundleMaintainRequest_sync);*/

                foreach (JObject data in jsonEnumerableData)
                {
                    EFakturContainer eFakturContainer = new EFakturContainer();

                    JToken? BuyerParty = data.Value<JToken>("BuyerParty");
                    JArray? BuyerPartyName = BuyerParty?.Value<JArray>("BuyerPartyName");
                    string? buyerName = BuyerPartyName?.First()?.Value<JToken>("FormattedName")?.ToString();
                    JArray? BuyerPartyAddress = BuyerParty?.Value<JArray>("BuyerPartyFormattedAddress");
                    string? buyerAddress = BuyerPartyAddress?.First()?.Value<JToken>("FormattedPostalAddressDescription")?.ToString();
                    string? buyerPartyNum = BuyerParty?.Value<JToken>("PartyID")?.ToString();
                    JArray? invoiceItem = data.Value<JArray?>("Item");

                    string? npwp = this.getCustomerNPWP(client, param, utils, buyerPartyNum);

                    tanggal = data.Value<JToken>("Date").ToString();

                    if (tanggal != "")
                        date = DateTime.Parse(tanggal);

                    eFakturContainer.isMasukan = false;
                    eFakturContainer.FK = "FK";
                    eFakturContainer.KD_JENIS_TRANSAKSI = "01";
                    eFakturContainer.FG_PENGGANTI = "0";

                    eFakturContainer.NOMOR_FAKTUR = "";
                    eFakturContainer.MASA_PAJAK = date.Year.ToString();
                    eFakturContainer.NPWP = npwp;
                    eFakturContainer.NAMA = buyerName;
                    eFakturContainer.ALAMAT_LENGKAP = buyerAddress;

                    string jumlahDppText = data.Value<JToken>("TotalNetAmount").ToString();
                    string jumlahPpnText = data.Value<JToken>("TotalTaxAmount").ToString();

                    string[] dppText, ppnText;

                    //dppText = jumlahDppText.Split('.', StringSplitOptions.TrimEntries);
                    //ppnText = jumlahPpnText.Split('.', StringSplitOptions.TrimEntries);

                    jumlahPpnText = jumlahPpnText.Replace('.', ',');
                    jumlahDppText = jumlahDppText.Replace('.', ',');

                    eFakturContainer.JUMLAH_DPP = double.Parse(jumlahDppText);
                    eFakturContainer.JUMLAH_PPN = double.Parse(jumlahPpnText);
                    eFakturContainer.JUMLAH_PPNBM = 0;

                    eFakturContainer.isKreditable = false;
                    eFakturContainer.TANGGAL_FAKTUR = date;

                    eFakturContainer.invNum = data.Value<JToken>("ObjectID").ToString();

                    if (eFakturContainer.invNum == null)
                        eFakturContainer.invNum = "-";

                    if (buyerPartyNum == null)
                        continue;

                    if (date >= dariTanggal.ToDateTime(TimeOnly.MinValue) &&
                        date <= sampaiTanggal.ToDateTime(TimeOnly.MinValue))
                    {
                        eFaktur.Add(eFakturContainer);
                    }
                    else
                    {
                        continue;
                    }

                    foreach (JObject invcItem in invoiceItem)
                    {
                        string hargaNetText = invcItem.Value<JToken>("NetAmount")?.ToString();
                        hargaNetText = hargaNetText.Replace('.', ',');
                        //string[] hargaNetSplit = hargaNetText.Split('.', StringSplitOptions.TrimEntries);

                        string qtyText = invcItem.Value<JToken>("Quantity")?.ToString();
                        qtyText = qtyText.Replace(".", ",");
                        //string[] qtySplit = qtyText.Split('.', StringSplitOptions.TrimEntries);

                        string hargaTotalText = invcItem.Value<JToken>("GrossAmount")?.ToString();
                        hargaTotalText = hargaTotalText.Replace('.', ',');
                        //string[] hargaTotalSplit = hargaTotalText.Split('.', StringSplitOptions.TrimEntries);

                        string nilaiPpnText = invcItem.Value<JToken>("TaxAmount")?.ToString();
                        nilaiPpnText = nilaiPpnText.Replace('.', ',');
                        //string[] nilaiPpnSplit = nilaiPpnText.Split('.', StringSplitOptions.TrimEntries);

                        double hargaNetAmount = double.Parse(hargaNetText);
                        double qtyAmount = double.Parse(qtyText);
                        double hargaTotalAmount = double.Parse(hargaTotalText);
                        double nilaiPpnAmount = double.Parse(nilaiPpnText);

                        eFakturContainer = new EFakturContainer();

                        eFakturContainer.isMasukan = false;
                        eFakturContainer.OF = "OF";
                        eFakturContainer.refInvNum = data?.Value<JToken>("ObjectID")?.ToString();
                        eFakturContainer.KODE_OBJEK = invcItem.Value<JToken>("ProductID")?.ToString();
                        eFakturContainer.NAMA_OBJEK = invcItem.Value<JToken>("Description")?.ToString();

                        if (hargaNetAmount != 0 && qtyAmount != 0)
                        {
                            eFakturContainer.HARGA_SATUAN = Math.Round(hargaNetAmount / qtyAmount, 2);
                        }

                        eFakturContainer.HARGA_TOTAL = hargaTotalAmount;
                        eFakturContainer.DPP = hargaNetAmount;
                        eFakturContainer.PPN = nilaiPpnAmount;

                        eFakturContainer.invNum = data.Value<JToken>("ObjectID").ToString();

                        if (eFakturContainer.invNum == null)
                            eFakturContainer.invNum = "-";

                        eFaktur.Add(eFakturContainer);
                    }
                }

                context.EFakturContainer.AddRange(eFaktur);
                context.SaveChanges();
            }
        }

        public LtFakturOut standarLTRow()
        {
            LtFakturOut ltFakturOut = new LtFakturOut();

            ltFakturOut.LT = "FAPR";
            ltFakturOut.BLOK = "Plaza Bank Index lt. 7";
            ltFakturOut.KELURAHAN = "Gondangdia";
            ltFakturOut.NPWP = "32054485650220002";
            ltFakturOut.JALAN = "Jl. M.H. Thamrin No.57";
            ltFakturOut.KABUPATEN = "Jakarta Pusat";
            ltFakturOut.KECAMATAN = "Menteng";
            ltFakturOut.KODE_POS = "10350";
            ltFakturOut.NAMA = "PT. Standard Toyo Polymer";
            ltFakturOut.NOMOR = "57";
            ltFakturOut.NOMOR_TELEPON = "021-2235568";
            ltFakturOut.PROPINSI = "DKI Jakarta";
            ltFakturOut.RT = "09";
            ltFakturOut.RW = "05";

            return ltFakturOut;
        }
    
        public void generateCSV(MemoryStream stream, bool isOut = true)
        {
            var fakturContainers = context.EFakturContainer;

            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, leaveOpen: true))
                {
                    if (isOut)
                    {
                        csv.WriteHeader<HeaderFakturOut>();
                        csv.NextRecord();
                        csv.WriteHeader<LtFakturOut>();
                        csv.NextRecord();
                        csv.WriteHeader<ObjekFakturOut>();
                        csv.NextRecord();

                        LtFakturOut     ltFaktur = standarLTRow();
                        HeaderFakturOut headerFaktur;
                        ObjekFakturOut  objekFaktur;

                        foreach (EFakturContainer container in fakturContainers.Where(x => x.isMasukan == false
                                                                                        && x.OF == null))
                        {
                            headerFaktur = new HeaderFakturOut();

                            headerFaktur.FG_PENGGANTI = "0";
                            headerFaktur.FG_UANG_MUKA = 0;
                            headerFaktur.FK = "FK";
                            headerFaktur.ID_KETERANGAN_TAMBAHAN = "";
                            headerFaktur.JUMLAH_DPP = container.JUMLAH_DPP != null ? (double) container.JUMLAH_DPP : 0;
                            headerFaktur.JUMLAH_DPP = Math.Floor(headerFaktur.JUMLAH_DPP);
                            headerFaktur.JUMLAH_PPN = container.JUMLAH_PPN != null ? (double) container.JUMLAH_PPN : 0;
                            headerFaktur.JUMLAH_PPN = Math.Floor(headerFaktur.JUMLAH_PPN);

                            headerFaktur.JUMLAH_PPNBM = 0;
                            headerFaktur.KD_JENIS_TRANSAKSI = "01";
                            headerFaktur.MASA_PAJAK = container.TANGGAL_FAKTUR != null ? container.TANGGAL_FAKTUR.Value.Month : 0;
                            headerFaktur.NAMA = container.NAMA;
                            headerFaktur.NPWP = container.NPWP;
                            headerFaktur.TAHUN_PAJAK = container.TANGGAL_FAKTUR != null ? container.TANGGAL_FAKTUR.Value.Year : 0;
                            headerFaktur.TANGGAL_FAKTUR = container.TANGGAL_FAKTUR != null ? DateOnly.FromDateTime(container.TANGGAL_FAKTUR.Value) : DateOnly.MinValue;
                            headerFaktur.REFERENSI = container.invNum;

                            csv.WriteRecord<HeaderFakturOut>(headerFaktur);
                            csv.NextRecord();
                            csv.WriteRecord<LtFakturOut>(ltFaktur);
                            csv.NextRecord();

                            foreach(EFakturContainer ofContainer in fakturContainers.Where(x => x.isMasukan == false
                                                                                             && x.OF == "OF"
                                                                                             && x.refInvNum == container.invNum))
                            {
                                objekFaktur = new ObjekFakturOut();

                                objekFaktur.DPP = ofContainer.DPP != null ? (double) ofContainer.DPP : 0;
                                objekFaktur.PPN = ofContainer.PPN != null ? (double)ofContainer.PPN : 0;
                                objekFaktur.DISKON = 0;
                                objekFaktur.HARGA_SATUAN = ofContainer.HARGA_SATUAN != null ? (double)ofContainer.HARGA_SATUAN : 0;
                                objekFaktur.HARGA_TOTAL = ofContainer.HARGA_TOTAL != null ? (double) ofContainer.HARGA_TOTAL : 0;
                                objekFaktur.JUMLAH_BARANG = ofContainer.JUMLAH_BARANG != null ? (double)ofContainer.JUMLAH_BARANG : 0;
                                objekFaktur.KODE_OBJEK = ofContainer.KODE_OBJEK;
                                objekFaktur.NAMA = ofContainer.NAMA_OBJEK;
                                objekFaktur.OF = "OF";
                                objekFaktur.PPNBM = 0;
                                objekFaktur.TARIF_PPNBM = 0;

                                csv.WriteRecord<ObjekFakturOut>(objekFaktur);
                                csv.NextRecord();
                            }
                        }
                    }
                    else
                    {
                        csv.WriteHeader<BodyFakturIn>();
                        csv.NextRecord();

                        BodyFakturIn fakturIn;

                        foreach (EFakturContainer container in fakturContainers.Where(x => x.isMasukan == true))
                        {
                            fakturIn = new BodyFakturIn();

                            fakturIn.ALAMAT_LENGKAP = container.ALAMAT_LENGKAP;
                            fakturIn.FG_PENGGANTI = "0";
                            fakturIn.FM = "FM";
                            fakturIn.IS_CREDITABLE = 0;
                            fakturIn.JUMLAH_DPP = container.JUMLAH_DPP != null ? (double) container.JUMLAH_DPP : 0;
                            fakturIn.JUMLAH_DPP = Math.Floor(fakturIn.JUMLAH_DPP);
                            fakturIn.JUMLAH_PPN = container.JUMLAH_PPN != null ? (double) container.JUMLAH_PPN : 0;
                            fakturIn.JUMLAH_PPN = Math.Floor(fakturIn.JUMLAH_PPN);
                            fakturIn.JUMLAH_PPNBM = 0;
                            fakturIn.KD_JENIS_TRANSAKSI = "01";
                            fakturIn.MASA_PAJAK = container.TANGGAL_FAKTUR != null ? container.TANGGAL_FAKTUR.Value.Month : 0;
                            fakturIn.NAMA = container.NAMA;
                            fakturIn.NPWP = container.NPWP;
                            fakturIn.TAHUN_PAJAK = container.TANGGAL_FAKTUR != null ? container.TANGGAL_FAKTUR.Value.Year : 0;
                            fakturIn.TANGGAL_FAKTUR = container.TANGGAL_FAKTUR != null ? DateOnly.FromDateTime(container.TANGGAL_FAKTUR.Value) : DateOnly.MinValue;
                            fakturIn.REFERENSI = container.invNum;

                            csv.WriteRecord<BodyFakturIn>(fakturIn);
                            csv.NextRecord();
                        }
                    }
                }
            }
        }
    
        public  static async Task insertFakturNumber(string ParentObjectId, string NoFakturPajak, string token, CookieContainer cookieContainer, string TypeCode = "10024")
        {
            Parameters param = new Parameters();
            Utils utils = new Utils();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.CookieContainer = cookieContainer;

                IEnumerable<Cookie> responseCookies = cookieContainer.GetCookies(new Uri(param.uriAPI["CustomerInvoice"]));

                using (var client = new HttpClient(handler))
                {
                    var byteArray = Encoding.ASCII.GetBytes(param.securityWorkcenter["Username"] + ":" + param.securityWorkcenter["Password"]);
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Add("x-csrf-token", token);
                    client.DefaultRequestHeaders.Add("ContentType", "application/json");

                    TextCollection textCollection = new TextCollection();

                    textCollection.Text = NoFakturPajak;
                    textCollection.ParentObjectID = ParentObjectId;
                    textCollection.TypeCode = TypeCode;

                    string json = JsonConvert.SerializeObject(textCollection);
                    StringContent stringContent = new StringContent(json, null, "application/json");

                    var response = await client.PostAsync(param.uriAPI["PostNoFakturOut"], stringContent);

                        //PostAsJsonAsync(param.uriAPI["PostNoFakturOut"], textCollection);
                }
            }
        }

    }
}
