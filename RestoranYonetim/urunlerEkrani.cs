using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace RestoranYonetim
{
    public partial class urunlerEkrani : Form
    {
        private readonly string connectionString = @"Server=MONSTER\SQLEXPRESS;Database=Restoran;Trusted_Connection=True;";

        public urunlerEkrani()
        {
            InitializeComponent();
        }

        private void urunlerEkrani_Load(object sender, EventArgs e)
        {
            UrunleriYukle();
        }

        private void UrunleriYukle()
        {
            flowLayoutPanel1.Controls.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sorgu = "SELECT UrunID, UrunAdi, Fiyat, Stok, ResimLink FROM Urunler";

                    using (SqlCommand command = new SqlCommand(sorgu, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int urunId = reader.GetInt32(0);
                            string urunAdi = reader.GetString(1);
                            decimal fiyat = reader.GetDecimal(2);
                            int stok = reader.GetInt32(3);
                            string resimLink = reader.IsDBNull(4) ? null : reader.GetString(4);

                            flowLayoutPanel1.Controls.Add(UrunPaneliOlustur(urunAdi, fiyat, stok, resimLink));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürünler yüklenirken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Panel UrunPaneliOlustur(string urunAdi, decimal fiyat, int stok, string resimLink)
        {
            // Ana Panel
            Panel panel = new Panel
            {
                Width = 250,
                Height = 150,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            // Resim
            PictureBox pictureBox = new PictureBox
            {
                Width = 100,
                Height = 100,
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Resmi yükle
            if (!string.IsNullOrWhiteSpace(resimLink))
            {
                try
                {
                    WebRequest request = WebRequest.Create(resimLink);
                    using (WebResponse response = request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }
                }
                catch
                {
                    pictureBox.Image = SystemIcons.Warning.ToBitmap();
                }
            }

            // Etiketler
            Label lblAdi = new Label { Text = $"Ad: {urunAdi}", Location = new Point(120, 10), AutoSize = true };
            Label lblFiyat = new Label { Text = $"Fiyat: {fiyat:C}", Location = new Point(120, 35), AutoSize = true };
            Label lblStok = new Label { Text = $"Stok: {stok}", Location = new Point(120, 60), AutoSize = true };

            // Buton
            Button btnSiparis = new Button
            {
                Text = "Sipariş Ver",
                Location = new Point(120, 90),
                Size = new Size(100, 30)
            };
            btnSiparis.Click += (s, e) =>
            {
                Image kopyaResim = pictureBox.Image != null ? (Image)pictureBox.Image.Clone() : null;
                MusteriBilgileri musteriForm = new MusteriBilgileri(urunAdi, fiyat, stok, kopyaResim);
                musteriForm.ShowDialog();
            };

            // Panel'e ekle
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(lblAdi);
            panel.Controls.Add(lblFiyat);
            panel.Controls.Add(lblStok);
            panel.Controls.Add(btnSiparis);

            return panel;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Gerekirse özel çizimler burada yapılabilir.
        }
    }
}
