using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;

namespace Organizer
{
    public partial class frmMain : Form
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool PostMessage(IntPtr hWnd, uint Msg, uint WParam, uint LParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool ReleaseCapture();

        const uint WM_SYSCOMMAND = 0x0112;
        const uint DOMOVE = 0xF012;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
        (int nLeftRect,int nTopRect, int nRightRect,int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        void Move(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, WM_SYSCOMMAND, DOMOVE, 0);
        }

        public frmMain(string login)
        {
            DataBase dataBase = new DataBase();  

            MySqlCommand command = new MySqlCommand("SELECT name FROM Users WHERE login = @uL", dataBase.GetConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;

            dataBase.OpenConnection();
            string userName = command.ExecuteScalar().ToString();
            dataBase.CloseConnection();

            InitializeComponent();
            lbUser.Text = $"{userName}";
            lbHello.Text = $"Добрый день, {userName}";
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnHome.Top;
            lbltitle.Text = btnHome.Text;
            tabControl1.SelectedIndex = 0;

            using (DataBase dataBase = new DataBase())

            {

            }


                DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM Users", dataBase.GetConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            dataGridView1.DataSource = table;
            dataBase.CloseConnection();


        }

        private void btnTask_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnTask.Top;
            lbltitle.Text = btnTask.Text;
            tabControl1.SelectedIndex = 1;

        }

        private void btnCalls_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnCalls.Top;
            lbltitle.Text = btnCalls.Text;
            tabControl1.SelectedIndex = 2;
        }

        private void btnMeeting_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnMeeting.Top;
            lbltitle.Text = btnMeeting.Text;
            tabControl1.SelectedIndex = 3;
        }
        private void btnBirthday_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnBirthday.Top;
            lbltitle.Text = btnBirthday.Text;
            tabControl1.SelectedIndex = 4;
        }
        private void btnContacts_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnContacts.Top;
            lbltitle.Text = btnContacts.Text;
            tabControl1.SelectedIndex = 5;

        }
        private void btnsettings_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnsettings.Top;
            lbltitle.Text = btnsettings.Text;
            tabControl1.SelectedIndex = 6;
        }

        private void btnHome_Leave(object sender, EventArgs e)
        {
            btnHome.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnTask_Leave(object sender, EventArgs e)
        {
            btnTask.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnCalls_Leave(object sender, EventArgs e)
        {
            btnCalls.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnMeeting_Leave(object sender, EventArgs e)
        {
            btnMeeting.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnBirthday_Leave(object sender, EventArgs e)
        {
            btnBirthday.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnContacts_Leave(object sender, EventArgs e)
        {
            btnContacts.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnsettings_Leave(object sender, EventArgs e)
        {
            btnsettings.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
