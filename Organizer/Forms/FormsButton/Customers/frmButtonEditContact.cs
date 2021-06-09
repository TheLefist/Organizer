using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Organizer
{
    public partial class frmButtonEditContact : Form
    {
        #region Import
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool PostMessage(IntPtr hWnd, uint Msg, uint WParam, uint LParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool ReleaseCapture();

        const uint WM_SYSCOMMAND = 0x0112;
        const uint DOMOVE = 0xF012;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
        (int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        #endregion 

        int customersID;
        static string name;
        static string surname;
        static string patronymic;
        static string data;
        static string phone;

        public frmButtonEditContact(int customersID)
        {
            InitializeComponent();
            this.customersID = customersID;

            DataBase dataBase = new DataBase();
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM Сustomers WHERE id_customers = @customersID", dataBase.GetConnection()))
            {
                command.Parameters.Add("@customersID", MySqlDbType.Int32).Value = customersID;
                dataBase.OpenConnection();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    name = reader[2].ToString();
                    tbName.Text = name;
                    surname = reader[3].ToString();
                    tbSurname.Text = surname;
                    patronymic = reader[4].ToString();
                    tbPatronymic.Text = patronymic;
                    data = reader[5].ToString();
                    data = data.Substring(0, 10);
                    tbBirthday.Text = data;
                    phone = reader[6].ToString();
                    tbPhone.Text = phone;
                }
                reader.Close();
                dataBase.CloseConnection();
            }

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, WM_SYSCOMMAND, DOMOVE, 0);
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void CheakButton(int cheakButton)
        {
            if (cheakButton == 1)
            {
                this.tbName.Enabled =       true;
                this.tbSurname.Enabled =    true;
                this.tbPatronymic.Enabled = true;
                this.tbBirthday.Enabled =   true;
                this.tbPhone.Enabled =      true;
                this.btnAdd.Enabled =       true;
                this.btnCancel.Enabled =    true;
                this.btnAdd.Text="Добавление..."; 
                Globals.cheakButtonAddContact= 0;

            }
            if (cheakButton == 0)
            {
                this.tbName.Enabled =       false;
                this.tbSurname.Enabled =    false;
                this.tbPatronymic.Enabled = false;
                this.tbBirthday.Enabled =   false;
                this.tbPhone.Enabled =      false;
                this.btnAdd.Enabled =       false;
                this.btnCancel.Enabled =    false;
                this.btnAdd.Text =     "Добавить";
            }
        }

        async private void btnAdd_Click(object sender, EventArgs e)
        {

            DateTime dt;
            if (!DateTime.TryParse(tbBirthday.Text, out dt))
            {
                MessageBox.Show("Неверно указана дата рождения");
                return;
            }

            if (tbPhone.Text.Length < 16)
            {
                MessageBox.Show("Введен некорректный номер телефона");
                return;
            }

            await Task.Run(() => {

                DataBase dataBase = new DataBase();

                string request = "UPDATE Сustomers SET name = @name, surname = @surname, patronymic = @patronymic, date_of_birth = @date_of_birth, phone = @phone WHERE Сustomers.id_customers = @customersID;";
                MySqlCommand command = new MySqlCommand(request, dataBase.GetConnection());
                command.Parameters.Add("@customersID", MySqlDbType.Int32).Value = customersID;
                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = tbName.Text;
                command.Parameters.Add("@surname", MySqlDbType.VarChar).Value = tbSurname.Text;
                command.Parameters.Add("@patronymic", MySqlDbType.VarChar).Value = tbPatronymic.Text;
                command.Parameters.Add("@date_of_birth", MySqlDbType.VarChar).Value = tbBirthday.Text;
                command.Parameters.Add("@phone", MySqlDbType.VarChar).Value = tbPhone.Text;

                try
                {
                    dataBase.OpenConnection();
                }
                catch
                {
                    MessageBox.Show("Ошибка установки соединения с базой данных");
                    return;
                }

                if (command.ExecuteNonQuery() == 1)
                {
                    dataBase.CloseConnection();
                    MessageBox.Show("Данные успешно изменены. Для отображения обновите вкладку");
                    BeginInvoke((MethodInvoker)(() => {
                        this.Close();
                    }));
                }
                else
                {
                    MessageBox.Show("Произошла ошибка");
                    dataBase.CloseConnection();
                    Globals.cheakButtonAddContact = 1;
                }
            });

        }
    }
}
