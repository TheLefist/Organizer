using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Organizer
{
    public partial class frmLogin : Form
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool PostMessage(IntPtr hWnd, uint Msg, uint WParam, uint LParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool ReleaseCapture();

        const uint WM_SYSCOMMAND = 0x0112;
        const uint DOMOVE = 0xF012;

        static string user = "UserName";
        static string pass = "PassWord";
        public frmLogin()
        {
            InitializeComponent();
            tbUser.Text = user;
            tbPass.Text = pass;
        }
        
        
        


        private void btnHome_Click(object sender, EventArgs e)
        {
            string loginUser = tbUser.Text;
            string passUser = tbPass.Text;

            DataBase dataBase = new DataBase();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `Users` WHERE `login` = @uL AND `pass` = @uP", dataBase.GetConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = passUser;

            dataBase.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                dataBase.CloseConnection();
                this.Hide();
                var frmMain = new frmMain(loginUser);
                frmMain.Show();
            }
            else
            {
                dataBase.CloseConnection();
                MessageBox.Show("Неверно указан логин или пароль");
                user = Convert.ToString(tbUser);
                pass = Convert.ToString(tbPass);
            }
                

        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, WM_SYSCOMMAND, DOMOVE, 0);
        }


        private void tbUser_Enter(object sender, EventArgs e)
        {
            if (tbUser.Text == user)
                tbUser.Clear();
        }

        private void tbPass_Enter(object sender, EventArgs e)
        {
            if (tbPass.Text == pass)
                tbPass.Clear();
        }

        private void tbUser_Leave(object sender, EventArgs e)
        {
            if (tbUser.Text == "")
                tbUser.Text = "UserName";
          
        }

        private void tbPass_Leave(object sender, EventArgs e)
        {
            if (tbPass.Text == "")
                tbPass.Text = "PassWord";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var frmRegistration = new frmRegistration();
            frmRegistration.Show();
        }

        private void cbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPass.Checked == true)
                tbPass.PasswordChar = '\0';
            else
                tbPass.PasswordChar = '*';
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
