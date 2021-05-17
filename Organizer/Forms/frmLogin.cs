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

        public void CheakButton(int cheakButton)
        {
            if (cheakButton == 1)
            {
                this.btnLogin.Enabled = true;
                this.btnLogin.Text = "Войти";
                this.button1.Enabled = true;
                this.tbUser.Enabled = true;
                this.tbPass.Enabled = true;
                this.cbShowPass.Enabled = true;
                this.cbRememberMe.Enabled = true;
                this.pbLoading.Visible = false;
                Globals.cheakButton = 0;

            }
            if (cheakButton == 0)
            {
                this.btnLogin.Enabled = false;
                this.btnLogin.Text = "Соединение...";
                this.button1.Enabled = false;
                this.tbUser.Enabled = false;
                this.tbPass.Enabled = false;
                this.cbShowPass.Enabled = false;
                this.cbRememberMe.Enabled = false;
                this.pbLoading.Visible = true;
            }
        }

        async private void btnHome_Click(object sender, EventArgs e)
        {

            string loginUser = tbUser.Text;
            string passUser = tbPass.Text;
            CheakButton(Globals.cheakButton);

            await Task.Run(() =>
            {
                DataBase dataBase = new DataBase();
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();

                MySqlCommand command = new MySqlCommand("SELECT * FROM `Users` WHERE `login` = @uL AND `pass` = @uP", dataBase.GetConnection());
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;
                command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = passUser;

                try
                {
                    dataBase.OpenConnection();
                }
                catch
                {
                    MessageBox.Show("Ошибка установки соединения с базой данных");
                    Globals.cheakButton = 1;
                    BeginInvoke((MethodInvoker)(() => { CheakButton(Globals.cheakButton); }));
                    return;
                }


                adapter.SelectCommand = command;
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    dataBase.CloseConnection();

                    BeginInvoke((MethodInvoker)(() => {
                        this.Close();
                        new Thread(OpenFormMain).Start();
                    }));

                }
                else
                {
                    dataBase.CloseConnection();

                    MessageBox.Show("Неверно указан логин или пароль");
                    user = tbUser.Text;
                    pass = tbPass.Text;
                    Globals.cheakButton = 1;

                    BeginInvoke((MethodInvoker)(() => { CheakButton(Globals.cheakButton); }));
                }
            });
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
                tbUser.Text = user;

        }

        private void tbPass_Leave(object sender, EventArgs e)
        {
            if (tbPass.Text == "")
                tbPass.Text = pass;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            new Thread(OpenFormRegistration).Start();
        }
        private void OpenFormRegistration()
        {
            Application.Run(new frmRegistration());
        }
        private void OpenFormMain()
        {
            Application.Run(new frmMain());
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
