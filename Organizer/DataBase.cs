using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Organizer
{
    class DataBase
    {

        static string keyConnection = "server=remotemysql.com;port=3306;username=siHHuaKv3R;password=OGpWnQ1Ofy;database=siHHuaKv3R";

        MySqlConnection connection = new MySqlConnection(keyConnection); // Конектимся к бд


        public void OpenConnection() // Открываемся соединение
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public void CloseConnection() //Закрываем соединение
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка закрытия соединения с базой данных");
            } 
        }
        public MySqlConnection GetConnection()
        {
            return connection;
        }
            
       
    }
}
