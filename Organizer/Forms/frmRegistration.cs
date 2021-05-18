using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
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

        static string name = "Введите ваше имя";
        static string mail = "Введите ваш e-mail";
        static string user = "Введите имя аккаунта";
        static string pass = "Введите пароль";
        static string pass2 ="Введите пароль ещё раз";
        public frmRegistration()
        {
            InitializeComponent();
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, WM_SYSCOMMAND, DOMOVE, 0);
        }

        public void CheakButton(int cheakButton)
        {
            if (cheakButton == 1)
            {
                this.pbLoading.Visible = false;
                this.tbName.Enabled = true;
                this.tbMail.Enabled = true;
                this.tbUser.Enabled = true;
                this.tbPass.Enabled = true;
                this.tbPass2.Enabled = true;
                this.cbShowPass.Enabled = true;
                this.btnRegistration.Enabled = true;
                this.btnRegistration.Text = "Зарегистрироваться";
                this.btnLogin.Enabled = true;
                Globals.cheakButtonRegistration = 0;

            }
            if (cheakButton == 0)
            {
                this.pbLoading.Visible = true;
                this.tbName.Enabled = false;
                this.tbMail.Enabled = false;
                this.tbUser.Enabled = false;
                this.tbPass.Enabled = false;
                this.tbPass2.Enabled = false;
                this.cbShowPass.Enabled = false;
                this.btnRegistration.Enabled = false;
                this.btnRegistration.Text = "Соединение...";
                this.btnLogin.Enabled = false;   
            }
        }
        async private void btnRegistration_Click(object sender, EventArgs e)
        {
            //Заполнены ли стандартные поля
            if ((tbName.Text == "Введите ваше имя")         ||
                (tbMail.Text == "Введите ваш e-mail")       ||
                (tbUser.Text == "Введите имя аккаунта")     ||
                (tbPass.Text == "Введите пароль")           ||
                (tbPass2.Text == "Введите пароль ещё раз"))
            {
                MessageBox.Show("Не все поля были заполнены");
                return;
            }

            if (tbPass.Text != tbPass2.Text) //Совпадают ли пароли
            {
                MessageBox.Show("Введенные пароли не совпадают");
                return;
            }

            if (!IsValidMail(tbMail.Text))
            {
                MessageBox.Show("Некорректный адрес электронной почты");
                return;
            }


            string nameUser =   tbName.Text;
            string mailUser =   tbMail.Text;
            string loginUser =  tbUser.Text;
            string passUser =   tbPass.Text;

            CheakButton(Globals.cheakButtonLogin);

            await Task.Run(() => {

                if (IsUserExists()) //Существует ли пользователь
                    return;

                DataBase dataBase = new DataBase();
                MySqlCommand command = new MySqlCommand("INSERT INTO Users (login, pass, mail, name) VALUES (@login, @pass, @mail, @name);", dataBase.GetConnection());

                command.Parameters.Add("@login", MySqlDbType.VarChar).Value = loginUser;
                command.Parameters.Add("@pass", MySqlDbType.VarChar).Value = passUser;
                command.Parameters.Add("@mail", MySqlDbType.VarChar).Value = mailUser;
                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = nameUser;

                dataBase.OpenConnection();

                if (command.ExecuteNonQuery() == 1)
                {

                    dataBase.CloseConnection();
                    MessageBox.Show("Аккаунт успешно зарегистрирован");
                    Globals.cheakButtonRegistration = 1; 
                    BeginInvoke((MethodInvoker)(() => {
                        this.Close();
                        new Thread(OpenForm).Start();
                    }));
                }
                else
                {
                    MessageBox.Show("Произошла ошибка");
                    dataBase.CloseConnection();
                }
            });
        }


        public bool IsValidMail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
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
            {
                tbPass.PasswordChar = '\0';
                tbPass2.PasswordChar = '\0';
            }
            else
            {
                tbPass.PasswordChar = '*';
                tbPass2.PasswordChar = '*';
            }
                
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

        private void tbName_Enter(object sender, EventArgs e)
        {
            if (tbName.Text == name)
                tbName.Clear();
        }

        private void tbName_Leave(object sender, EventArgs e)
        {
            if (tbName.Text == "")
                tbName.Text = name;
        }

        private void tbMail_Leave(object sender, EventArgs e)
        {
            if (tbMail.Text == "")
                tbMail.Text = mail;
        }

        private void tbMail_Enter(object sender, EventArgs e)
        {
            if (tbMail.Text == mail)
                tbMail.Clear();
        }

        private void tbUser_Enter(object sender, EventArgs e)
        {
            if (tbUser.Text == user)
                tbUser.Clear();
        }

        private void tbUser_Leave(object sender, EventArgs e)
        {
            if (tbUser.Text == "")
                tbUser.Text = user;
        }

        private void tbPass_Enter(object sender, EventArgs e)
        {
            if (tbPass.Text == pass)
            {
                tbPass.Clear();
                tbPass.PasswordChar = '*';
            }
                
        }

        private void tbPass_Leave(object sender, EventArgs e)
        {
            if (tbPass.Text == "")
                tbPass.Text = pass;
        }

        private void tbPass2_Enter(object sender, EventArgs e)
        {
            if (tbPass2.Text == pass2)
            {
                tbPass2.Clear();
                tbPass2.PasswordChar = '*';
            }
                

        }

        private void tbPass2_Leave(object sender, EventArgs e)
        {
            if (tbPass2.Text == "")
                tbPass2.Text = pass2;
        }

        private void ProcessingLenght_Tick(object sender, EventArgs e)
        {
            lbNameLenght.Text = (tbName.MaxLength - tbName.Text.Length).ToString();
            lbMailLenght.Text = (tbMail.MaxLength - tbMail.Text.Length).ToString();
            lbUserLenght.Text = (tbUser.MaxLength - tbUser.Text.Length).ToString();
            lbPassLenght.Text = (tbPass.MaxLength - tbPass.Text.Length).ToString();
            lbPass2Lenght.Text = (tbPass2.MaxLength - tbPass2.Text.Length).ToString();
        }
    }
}
