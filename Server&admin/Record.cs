using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp6
{
    public partial class Record : Form
    {

        private string connectionString = "Server=10.10.21.103;Database=fdsolution;Uid=tangtang;Pwd=00000000;";

        public Record()
        {
            InitializeComponent();
        }

        private void Record_Load(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                string query = "SELECT * FROM doze_record"; // replace yourTableName with the actual name of your table
                MySqlCommand command = new MySqlCommand(query, connection);

                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(table);

                dataGridView1.DataSource = table;

                dataGridView1.Columns[0].HeaderText = "번호";

                dataGridView1.Columns[1].HeaderText = "구역";

                dataGridView1.Columns[2].HeaderText = "졸음 시작";

                dataGridView1.Columns[3].HeaderText = "졸음 종료";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btn_graph_Click(object sender, EventArgs e)
        {
            {
                try
                {
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    string query = "SELECT id_dl, COUNT(*) as count FROM doze_record GROUP BY id_dl";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    DataTable table = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);

                    // update the data source of the chart control
                    chart1.Series.Clear();


                    chart1.DataSource = table;
                    chart1.Series.Add("Data");
                    chart1.Series["Data"].XValueMember = "id_dl";

                    chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 10, FontStyle.Bold);

                    // Define the custom labels for the X-axis
                    string[] customLabels = new string[] { "1", "2", "3", "4", "(구역)" };

                    // Set the custom labels for the X-axis
                    chart1.ChartAreas[0].AxisX.CustomLabels.Clear();
                    for (int i = 0; i < customLabels.Length; i++)
                    {
                        chart1.ChartAreas[0].AxisX.CustomLabels.Add(i + 0.5, i + 1.5, customLabels[i]);
                    }

                    chart1.Series["Data"].YValueMembers = "count";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    string query = "SELECT DATE_FORMAT(doze_start, '%a') AS day_of_week, COUNT(*) AS count FROM doze_record GROUP BY day_of_week";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    DataTable table = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);

                    // update the data source of the chart control
                    chart2.Series.Clear();
                    chart2.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 10, FontStyle.Bold);

                    chart2.DataSource = table;
                    chart2.Series.Add("Data");
                    chart2.Series["Data"].XValueMember = "day_of_week";
                    chart2.Series["Data"].YValueMembers = "count";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            try
            {
                string year = comboBox3.SelectedItem.ToString();
                int month = comboBox1.SelectedIndex + 1;
                int day = comboBox2.SelectedIndex + 1;

                // Construct the date string in the format "yyyy-mm-dd"
                string dateStr = $"{year}-{month}-{day}";
                // Retrieve the data from the database using the constructed date string
                MySqlConnection connection = new MySqlConnection(connectionString);
                string query = "SELECT * FROM doze_record WHERE DATE(doze_start) = @dateStr";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@dateStr", dateStr);

                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(table);

                // Update the data source of the DataGridView control
                dataGridView1.DataSource = table;
                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            }
            catch (Exception ex)
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                string query = "SELECT * FROM doze_record"; // replace yourTableName with the actual name of your table
                MySqlCommand command = new MySqlCommand(query, connection);

                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(table);

                dataGridView1.DataSource = table;

                dataGridView1.Columns[0].HeaderText = "번호";

                dataGridView1.Columns[1].HeaderText = "구역";

                dataGridView1.Columns[2].HeaderText = "졸음 시작";

                dataGridView1.Columns[3].HeaderText = "졸음 종료";
            }

        }
    }

}

