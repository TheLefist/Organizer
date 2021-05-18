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
        public frmButtonAddContact(int userID)
        {
            InitializeComponent();
            foreach (var item in this.Controls)
            {
                if (item is TextBox)
                {
                    ((TextBox)item).Enter += TextBoxClear; 
                }
            }
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

        async private void btnAdd_Click(object sender, EventArgs e)
        {
            
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

                dataBase.OpenConnection();

                if (command.ExecuteNonQuery() == 1)
                {

                    dataBase.CloseConnection();
                    MessageBox.Show("Данные успешно внесены");
                }
                else
                {
                    MessageBox.Show("Произошла ошибка");
                    dataBase.CloseConnection();
                }
            });

        }

        private void TextBoxClear(object sender, EventArgs e)
        {
            ((TextBox)sender).Clear();
        }
    }
}
