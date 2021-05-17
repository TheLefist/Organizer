using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Organizer
{
    public partial class frmRegistration : Form
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool PostMessage(IntPtr hWnd, uint Msg, uint WParam, uint LParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool ReleaseCapture();

        const uint WM_SYSCOMMAND = 0x0112;
        const uint DOMOVE = 0xF012;

        public frmRegistration()
        {
            InitializeComponent();
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, WM_SYSCOMMAND, DOMOVE, 0);
        }

        private void tbUser_Enter(object sender, EventArgs e)
        {
            if (tbUser.Text == "UserName")
                tbUser.Clear();
        }

        private void tbPass_Enter(object sender, EventArgs e)
        {
            if (tbPass.Text == "PassWord")
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

        private void btnRegistration_Click(object sender, EventArgs e)
        {
            if (tbUser.Text == "UserName")
            {
                MessageBox.Show("Введи имя дэбил");
                return;
            }

            if(tbPass.Text == "PassWord")
            {
                MessageBox.Show("Введи пароль дэбил");
                return;
            }

            if (IsUserExists())
                return;


            string loginUser = tbUser.Text;
            string passUser = tbPass.Text;

            DataBase dataBase = new DataBase();
            MySqlCommand command = new MySqlCommand("INSERT INTO `Users` (`login`, `pass`) VALUES (@login, @pass);", dataBase.GetConnection());

            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = passUser;

            dataBase.OpenConnection();

            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Зареган");
                dataBase.CloseConnection();
            }
            else
            {
                MessageBox.Show("Уже есть");
                dataBase.CloseConnection();
            }            
        }

        public bool IsUserExists()
        {
            string loginUser = tbUser.Text;

            DataBase dataBase = new DataBase();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM `Users` WHERE `login` = @uL", dataBase.GetConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;

            dataBase.OpenConnection();

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Указанная учетная запись уже существует!");
                dataBase.CloseConnection();
                return true;
            }
            else
            {
                dataBase.CloseConnection();
                return false;
            }        
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            new Thread(OpenForm).Start();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void cbShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShowPass.Checked == true)
                tbPass.PasswordChar = '\0';
            else
                tbPass.PasswordChar = '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Close();
            new Thread(OpenForm).Start();
        }
        private void OpenForm()
        {
            Application.Run(new frmLogin());
        }

    }
}
