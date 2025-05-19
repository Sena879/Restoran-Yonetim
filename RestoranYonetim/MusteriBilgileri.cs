using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace RestoranYonetim
{
    public partial class MusteriBilgileri : Form
    {
        public MusteriBilgileri(string urunAdi, decimal fiyat, int stok, Image resim)
        {
            InitializeComponent();
            lblUrunAdi.Text = urunAdi;
            lblFiyat.Text = fiyat.ToString("C", new CultureInfo("tr-TR"));
            lblStok.Text = $"Stok: {stok}";

            if (resim != null)
                pictureBox1.Image = resim;
        }

        private void MusteriBilgileri_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string adSoyad = txtAdSoyad.Text.Trim();
            string telefon = txtTelefon.Text.Trim();
            string adres = txtAdres.Text.Trim();
            string urun = lblUrunAdi.Text.Trim();
            int fiyat;

            try
            {
                fiyat = (int)decimal.Parse(lblFiyat.Text, NumberStyles.Currency, new CultureInfo("tr-TR"));
            }
            catch
            {
                MessageBox.Show("Fiyat bilgisinde hata var.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(adSoyad) || string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(adres))
            {
                MessageBox.Show("Lütfen müşteri bilgilerini eksiksiz doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mesaj = $"Ad Soyad: {adSoyad}\nTelefon: {telefon}\nAdres: {adres}\nÜrün: {urun}\nFiyat: {fiyat} ₺\n\nBilgiler doğru mu?";

            DialogResult sonuc = MessageBox.Show(mesaj, "Siparişi Onayla", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (sonuc == DialogResult.Yes)
            {
                try
                {
                    string connectionString = @"Server=MONSTER\SQLEXPRESS;Database=Restoran;Trusted_Connection=True;";
                    using (SqlConnection baglanti = new SqlConnection(connectionString))
                    {
                        baglanti.Open();

                        string sql = @"INSERT INTO Siparisler (AdSoyad, Telefon, Adres, Urun, Fiyat) 
                                       VALUES (@AdSoyad, @Telefon, @Adres, @Urun, @Fiyat)";

                        using (SqlCommand komut = new SqlCommand(sql, baglanti))
                        {
                            komut.Parameters.AddWithValue("@AdSoyad", adSoyad);
                            komut.Parameters.AddWithValue("@Telefon", telefon);
                            komut.Parameters.AddWithValue("@Adres", adres);
                            komut.Parameters.AddWithValue("@Urun", urun);
                            komut.Parameters.AddWithValue("@Fiyat", fiyat);

                            komut.ExecuteNonQuery();
                        }

                        MessageBox.Show("Sipariş başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Sipariş iptal edildi.", "İptal", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
