using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RestoranYonetim
{
    public partial class Siparishler : Form
    {
        public Siparishler()
        {
            InitializeComponent();
        }

        private void Siparishler_Load(object sender, EventArgs e)
        {
            SiparisleriYukle();
        }

        private void SiparisleriYukle()
        {
            string connectionString = @"Server=MONSTER\SQLEXPRESS;Database=Restoran;Trusted_Connection=True;";
            using (SqlConnection baglanti = new SqlConnection(connectionString))
            {
                try
                {
                    baglanti.Open();
                    string sql = "SELECT * FROM Siparisler";
                    SqlDataAdapter da = new SqlDataAdapter(sql, baglanti);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veriler yüklenirken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz siparişi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow seciliSatir = dataGridView1.SelectedRows[0];

            if (seciliSatir.Cells["Id"].Value != null && int.TryParse(seciliSatir.Cells["Id"].Value.ToString(), out int id))
            {
                DialogResult onay = MessageBox.Show(
                    $"ID {id} olan siparişi silmek istiyor musunuz?",
                    "Sipariş Sil",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (onay == DialogResult.Yes)
                {
                    string connectionString = @"Server=MONSTER\SQLEXPRESS;Database=Restoran;Trusted_Connection=True;";
                    using (SqlConnection baglanti = new SqlConnection(connectionString))
                    {
                        try
                        {
                            baglanti.Open();
                            string sql = "DELETE FROM Siparisler WHERE Id = @Id";

                            using (SqlCommand komut = new SqlCommand(sql, baglanti))
                            {
                                komut.Parameters.AddWithValue("@Id", id);
                                komut.ExecuteNonQuery();
                            }

                            MessageBox.Show("Sipariş başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            SiparisleriYukle(); // Listeyi yenile
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Sipariş silinirken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Geçerli bir sipariş ID’si bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
