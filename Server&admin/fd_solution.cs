using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp6
{
    public partial class fd_solution : Form
    {
        public fd_solution()
        {
            InitializeComponent();
        }

        private void fd_solution_Load(object sender, EventArgs e)
        {
            timer1.Interval = 100; //타이머 간격 100m
            timer1.Start(); //타이머 시작        
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time_lb.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        }//실시간으로 나타나는 시간 라벨
        private void fd_solution_FormClosing(object sender, FormClosingEventArgs e)
        {
            // "X" 버튼을 클릭해도 폼이 닫히지 않도록 합니다.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }
    }
}
