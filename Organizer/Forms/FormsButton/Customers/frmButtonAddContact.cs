﻿using MySql.Data.MySqlClient;
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
    public partial class frmButtonAddContact : Form
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

        int userID;
        static string name = "Имя";
        static string surname = "Фамилия";
        static string patronymic = "Отчество";

        public frmButtonAddContact(int userID)
        {
            InitializeComponent();
            this.userID = userID;
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

            //Заполнены ли стандартные поля
            if ((tbName.Text == name) ||
                (tbSurname.Text == surname) ||
                (tbPatronymic.Text == patronymic))
            {
                MessageBox.Show("Не все поля были заполнены");
                return;
            }


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

            CheakButton(Globals.cheakButtonAddContact);
            await Task.Run(() => {

                DataBase dataBase = new DataBase();

                string request = "INSERT INTO Сustomers (user_id, name, surname, patronymic, date_of_birth, phone) VALUES (@userID, @name, @surname, @patronymic, @date_of_birth, @phone)";
                MySqlCommand command = new MySqlCommand(request, dataBase.GetConnection());
                command.Parameters.Add("@userID", MySqlDbType.Int32).Value = userID;
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
                    Globals.cheakButtonAddContact = 1;
                    BeginInvoke((MethodInvoker)(() => { CheakButton(Globals.cheakButtonAddContact); }));
                    return;
                }

                if (command.ExecuteNonQuery() == 1)
                {

                    dataBase.CloseConnection();
                    MessageBox.Show("Данные успешно внесены. Для отображения обновите вкладку");
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

        private void TextBoxClear(object sender, EventArgs e)
        {
            ((TextBox)sender).Clear();
        }

        private void tbBirthday_Enter(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                tbBirthday.Clear();
                tbBirthday.Select(0, 0);
            }));
                
        }

        private void tbPhone_Enter(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                tbPhone.Clear();
                tbPhone.Select(1, 0);
            }));
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

        private void tbSurname_Enter(object sender, EventArgs e)
        {
            if (tbSurname.Text == surname)
                tbSurname.Clear();

        }

        private void tbSurname_Leave(object sender, EventArgs e)
        {
            if (tbSurname.Text == "")
                tbSurname.Text = surname;
        }

        private void tbPatronymic_Enter(object sender, EventArgs e)
        {
            if (tbPatronymic.Text == patronymic)
                tbPatronymic.Clear();
        }

        private void tbPatronymic_Leave(object sender, EventArgs e)
        {
            if (tbPatronymic.Text == "")
                tbPatronymic.Text = patronymic;
        }
    }
}
