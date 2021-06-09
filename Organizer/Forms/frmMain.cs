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
using System.Threading;

namespace Organizer
{
    public partial class frmMain : Form
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
        (int nLeftRect,int nTopRect, int nRightRect,int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        #endregion 

        void Move(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, WM_SYSCOMMAND, DOMOVE, 0);
        }

        private string userName;
        private int iD;
        private int tabIndex;
        private int customersID;
        public frmMain(string login)
        {
            DataBase dataBase = new DataBase();

            

            using (MySqlCommand command = new MySqlCommand("SELECT name FROM Users WHERE login = @uL", dataBase.GetConnection()))
            {
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;
                dataBase.OpenConnection();
                userName = command.ExecuteScalar().ToString();
                dataBase.CloseConnection();
            }

            using (MySqlCommand command = new MySqlCommand("SELECT Id FROM Users WHERE login = @uL", dataBase.GetConnection()))
            {
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;
                dataBase.OpenConnection();
                iD = Convert.ToInt32(command.ExecuteScalar());
                dataBase.CloseConnection();
            }

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
            tabControl1.SelectedIndex = 0;

        }

        private void btnTask_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnTask.Top;
            tabControl1.SelectedIndex = 1;

        }

        private void btnCalls_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnCalls.Top;
            tabControl1.SelectedIndex = 2;
        }

        private void btnMeeting_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnMeeting.Top;
            tabControl1.SelectedIndex = 3;
        }
        private void btnBirthday_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnBirthday.Top;
            tabControl1.SelectedIndex = 4;
        }
        private void btnContacts_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnContacts.Top;
            tabControl1.SelectedIndex = 5;
            tabIndex = tabControl1.SelectedIndex;

            DataBase dataBase = new DataBase();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM Сustomers WHERE user_id = {iD}", dataBase.GetConnection());
            adapter.Fill(table);

            dgvContacts.DataSource = table;

            dgvContacts.Columns[0].Visible = false;
            dgvContacts.Columns[1].Visible = false;

            dgvContacts.Columns[2].HeaderText = "Имя";
            dgvContacts.Columns[3].HeaderText = "Фамилия";
            dgvContacts.Columns[4].HeaderText = "Отчество";
            dgvContacts.Columns[5].HeaderText = "Дата рождения";
            dgvContacts.Columns[6].HeaderText = "Телефон";

            dgvContacts.ClearSelection();
            customersID = 0;

            dataBase.CloseConnection();
        }
        private void btnsettings_Click(object sender, EventArgs e)
        {
            pnlNav.Top = btnsettings.Top;
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

        private void tbSearch_Enter(object sender, EventArgs e)
        {
            if (tbSearch.Text == "Строка поиска...")
                tbSearch.Clear();
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            if (tabIndex == 5)
                new Thread(OpenFormButtonAddContact).Start();
        }
        private void OpenFormButtonAddContact()
        {
            Application.Run(new frmButtonAddContact(iD));
        }
        private void OpenFormButtonEditContact()
        {
            Application.Run(new frmButtonEditContact(customersID));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (tabIndex == 5)
                Delete(dgvContacts);
                
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (tabIndex == 5)
                Search(dgvContacts);

        }

        private void Search(DataGridView DGV)
        {
            for (int i = 0; i < DGV.RowCount; i++)
            {
                DGV.Rows[i].Selected = false;
                for (int j = 0; j < DGV.ColumnCount; j++)
                    if (DGV.Rows[i].Cells[j].Value != null)
                        if (DGV.Rows[i].Cells[j].Value.ToString().Contains(tbSearch.Text))
                        {
                            DGV.Rows[i].Selected = true;
                            break;
                        }
            }
        }
        private void Delete(DataGridView DGV)
        {
            int rowsCouint = DGV.Rows.Count;
            if (rowsCouint > 0)
            {
                int i = DGV.SelectedCells[0].RowIndex;
                DGV.Rows.RemoveAt(i);
                if (i > 0)
                    DGV.Rows[i - 1].Selected = true;
            }
            else
                MessageBox.Show("Нет записей на удаление");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            try
            {
                customersID = Convert.ToInt32(dgvContacts.CurrentRow.Cells[0].Value);
            }
            catch
            {
                MessageBox.Show("Строка не была выбрана");
                return;
            }
            
            if (tabIndex == 5)
                new Thread(OpenFormButtonEditContact).Start();

        }
    }
}
