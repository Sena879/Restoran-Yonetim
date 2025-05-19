using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RestoranYonetim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void VerileriYenile()
        {
            string sorgu = "SELECT * FROM Urunler";
            DataTable dt = DatabaseHelper.ExecuteQuery(sorgu);
            dataGridView1.DataSource = dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            VerileriYenile();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow seciliSatir = dataGridView1.Rows[e.RowIndex];

                txtUrunID.Text = seciliSatir.Cells["UrunID"].Value.ToString();
                txtUrunAdı.Text = seciliSatir.Cells["UrunAdi"].Value.ToString();
                txtFiyat.Text = seciliSatir.Cells["Fiyat"].Value.ToString();
                txtStok.Text = seciliSatir.Cells["Stok"].Value.ToString();
                txtResim.Text = seciliSatir.Cells["ResimLink"].Value.ToString();
                pictureBox1.ImageLocation = txtResim.Text;
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrunAdı.Text) ||
                string.IsNullOrWhiteSpace(txtFiyat.Text) ||
                string.IsNullOrWhiteSpace(txtStok.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            string sorgu = "INSERT INTO Urunler (UrunAdi, Fiyat, Stok, ResimLink) VALUES (@UrunAdi, @Fiyat, @Stok, @ResimLink)";

            try
            {
                DatabaseHelper.ExecuteNonQuery(sorgu,
                    new SqlParameter("@UrunAdi", txtUrunAdı.Text),
                    new SqlParameter("@Fiyat", txtFiyat.Text),
                    new SqlParameter("@Stok", txtStok.Text),
                    new SqlParameter("@ResimLink", txtResim.Text));

                MessageBox.Show("Ürün başarıyla eklendi.");
                VerileriYenile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrunAdı.Text) ||
                string.IsNullOrWhiteSpace(txtFiyat.Text) ||
                string.IsNullOrWhiteSpace(txtStok.Text))
            {
                MessageBox.Show("Lütfen silme işleminden önce bir ürünü seçip, ilgili alanları doldurun.");
                return;
            }

            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz veriyi seçin.");
                return;
            }

            DialogResult result = MessageBox.Show("Bu ürünü silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                return;
            }

            string urunID = dataGridView1.CurrentRow.Cells["UrunID"].Value.ToString();
            string sorgu = "DELETE FROM Urunler WHERE UrunID = @UrunID";

            try
            {
                DatabaseHelper.ExecuteNonQuery(sorgu, new SqlParameter("@UrunID", urunID));
                MessageBox.Show("Ürün başarıyla silindi.");
                VerileriYenile();

                txtUrunAdı.Clear();
                txtFiyat.Clear();
                txtStok.Clear();
                txtResim.Clear();
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Lütfen güncellenecek bir veri seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUrunAdı.Text) ||
                string.IsNullOrWhiteSpace(txtFiyat.Text) ||
                string.IsNullOrWhiteSpace(txtStok.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            string seciliUrunID = dataGridView1.CurrentRow.Cells["UrunID"].Value.ToString();
            string sorgu = "UPDATE Urunler SET UrunAdi = @UrunAdi, Fiyat = @Fiyat, Stok = @Stok, ResimLink = @ResimLink WHERE UrunID = @UrunID";

            try
            {
                DatabaseHelper.ExecuteNonQuery(sorgu,
                    new SqlParameter("@UrunAdi", txtUrunAdı.Text),
                    new SqlParameter("@Fiyat", txtFiyat.Text),
                    new SqlParameter("@Stok", txtStok.Text),
                    new SqlParameter("@ResimLink", txtResim.Text),
                    new SqlParameter("@UrunID", seciliUrunID));

                MessageBox.Show("Ürün başarıyla güncellendi.");
                VerileriYenile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void txtFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            urunlerEkrani Urunler = new urunlerEkrani();
            Urunler.Show();
        }

        private void btnSiparsler_Click(object sender, EventArgs e)
        {
            Siparishler siparishler = new Siparishler();
            siparishler.ShowDialog();
        }
    }
}
