using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Media;
using System.Threading;


namespace WindowsFormsApp6
{
    public partial class Main : Form
    {
        private TcpListener _listener;
        private string _connectionString = "Server=10.10.21.103;Database=fdsolution;Uid=tangtang;Pwd=00000000;";
        public fd_solution _fd_solution;
        public Record record;
        public List<PictureBox> pictureBoxList;

        public Main()
        {
            InitializeComponent();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            // carImg를 admin의 자식폼으로 선언
            _fd_solution = new fd_solution();
            _fd_solution.MdiParent = this;
            // 이벤트 핸들러 등록
            /*            _fd_solution.EnterandExit_Click += allow_EnterandExit;*/
            _fd_solution.Show();

/*            record = new Record();
            record.MdiParent = this;
            record.StartPosition = FormStartPosition.Manual;
            record.Location = new Point(960, 0);
            // 이벤트 핸들러 등록
            record.Show();*/

            IPAddress ipAddress = IPAddress.Parse("10.10.21.103");
            _listener = new TcpListener(ipAddress, 9000);
            _listener.Start();
            pictureBoxList = new List<PictureBox>();
            pictureBoxList.Add(_fd_solution.pictureBox11);
            pictureBoxList.Add(_fd_solution.pictureBox12);
            pictureBoxList.Add(_fd_solution.pictureBox13);
            pictureBoxList.Add(_fd_solution.pictureBox14);

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Task.Factory.StartNew(AsyncTcpProcess, client);
            }
        }
        private async void AsyncTcpProcess(object o)
        {
            TcpClient client = (TcpClient)o;
            string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            Console.WriteLine(clientIp);
            NetworkStream stream = client.GetStream();
            string area = CheckIPArea(clientIp);
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        // 클라이언트 연결이 종료되었습니다.
                        break;
                    }
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    dynamic message = JsonConvert.DeserializeObject(json);
                    string signal = message.signal;
                    Console.WriteLine(signal);
                    if (signal == "졸음시작")
                    {
                        await InsertSleepStart(area);
                    }
                    else if (signal == "졸음종료")
                    {
                        await UpdateSleepEnd(area);
                    }
                    else if (signal == "구역알림")
                    {
                        //.. 알림음 및 구역 경보 깜빡임
                        foreach (PictureBox pb in pictureBoxList)
                        {
                            if (pb.Name == $"pictureBox1{area}")
                            {
                                Thread soundThread = new Thread(PlaySound);
                                soundThread.Start();
                                for (int i = 0; i < 5; i++)
                                {
                                    pb.Image = Properties.Resources.redalarm;
                                    await Task.Delay(500);
                                    pb.Image = Properties.Resources.bluealarm;
                                    await Task.Delay(500);
                                }
                                break;
                            }
                        }
                    }
                    else if (signal == "서버연결")
                    {
                        foreach (PictureBox pb in pictureBoxList)
                        {
                            if (pb.Name == $"pictureBox1{area}")
                            {
                                pb.Image = Properties.Resources.bluealarm;
                                break;
                            }
                        }
                    }
                    else if (signal == "연결종료")
                    {
                        foreach (PictureBox pb in pictureBoxList)
                        {
                            if (pb.Name == $"pictureBox1{area}")
                            {
                                pb.Image = Properties.Resources.off_alarm;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리 (예: 연결이 비정상적으로 종료되었을 때)
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }
        private async Task InsertSleepStart(string area)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand("INSERT INTO doze_record (id_dl,doze_start) VALUES (@area,now())", connection))
                {
                    command.Parameters.AddWithValue("@area", area);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UpdateSleepEnd(string area)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (MySqlCommand updateCommand = new MySqlCommand("UPDATE doze_record SET doze_end = now() WHERE id_dl = @id order by id_dr desc limit 1", connection))
                {
                    updateCommand.Parameters.AddWithValue("@id", area);
                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
        }
        private string CheckIPArea(string ip)
        {
            string area = "";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand("Select id_dl from location where ip = @ip ", connection))
                {
                    command.Parameters.AddWithValue("@ip", ip);
                    MySqlDataReader rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        area = rdr.GetString(0);
                    }
                    rdr.Close();

                }
                connection.Close();
            }
            return area;
        }
        private void PlaySound()
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @"D:\test6.wav";
            player.PlaySync();
        }

        private void 구역알림ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isFormOpen = false;
            foreach (Form form in this.MdiChildren)
            {
                if (form is fd_solution)
                {
                    form.Activate();
                    isFormOpen = true;
                }
            }
            if (!isFormOpen)
            {
                fd_solution fdSolution = new fd_solution();
                fdSolution.MdiParent = this;
                fdSolution.Dock = DockStyle.None;
                fdSolution.Show();
            }
        }

            private void 기록조회ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isFormOpen = false;
            foreach (Form form in this.MdiChildren)
            {
                if (form is Record)
                {
                    form.Activate();
                    isFormOpen = true;
                }
            }
            if (!isFormOpen)
            {
                Record record = new Record();
                record.MdiParent = this;
                record.Dock = DockStyle.None;
                record.Show();
            }
        }
    }
}
