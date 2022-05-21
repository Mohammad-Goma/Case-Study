using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseAccess
{
    public class SQLServer
    {
        SqlConnection connection;

        public SQLServer(String serverName, String dataBase, String userName, String password, Boolean IntegratedSecurity)
        {
            String connectionString = "";

            if (userName == "" || password == "")
            { connectionString = "Data Source=" + serverName + ";Initial Catalog=" + dataBase + ";Integrated Security=" + IntegratedSecurity; }
            else
            {
                connectionString = "Data Source=" + serverName + ";Initial Catalog=" + dataBase +
                                   ";User Id=" + userName + ";Password=" + password + ";"; // +"Connect Timeout=300";
            }
            connection = new SqlConnection(connectionString);

            try
            { connection.Open(); }
            catch (SqlException e)
            {
                if (e.Number == 2) // حالة لايتوفر اتصال بالخدمة
                { MessageBox.Show("مزود خدمة قاعدة البيانات لا يعمل يجب الاتصال بمدير النظام", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }
                else if (e.Number == 4060) // حالة توفر اتصال ولكن القاعدة غير متوفرة
                {
                    // الطريقة الأولى لم تنجح
                    // بناء الربط بقاعدة البيانات
                    //String mypath = Application.StartupPath.ToString();
                    //SqlConnection attachCon = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=master;Integrated Security=True");
                    //SqlCommand cmd = new SqlCommand();
                    //cm.CommandText = "sp_attach_db @dbname = N'RealEstate', " +
                    //" @filename1 = N'" + mypath + "\\DataBase\\RealEstate" + "'," +
                    //"@filename2 = N'" + mypath + "\\DataBase\\RealEstate_logF" + "'";
                    //attachCon.Open();
                    //cm.Connection = attachCon;
                    //cm.ExecuteNonQuery();

                    // الطريقة الثانية نجحت
                    // استرداد قاعدة البيانات من نسخة احتياطية
                    int StatRB = 0;
                    //SqlConnection RestoreDB = new SqlConnection("Data Source=localhost\\sqlexpress;Initial Catalog=master;User Id=sa; Password=1782003; Integrated Security=False");
                    SqlConnection RestoreDB = new SqlConnection("Data Source=localhost\\SQLUIT;Initial Catalog=master; Integrated Security=True");

                    string query = string.Format(
                        @"restore database {1} from disk='{2}{0}'",
                        "RealEstate",//m_backUpFilePath,
                        "RealEstate",//m_dbName,
                        "E:\CaseStudy\DB\\");//m_datFilePath);
                    SqlCommand cmd;
                    cmd = new SqlCommand(query, RestoreDB);

                    RestoreDB.Open();
                    try
                    {
                        StatRB = cmd.ExecuteNonQuery();
                        int milliseconds = 3000;
                        System.Threading.Thread.Sleep(milliseconds);
                        if (StatRB == -1)
                        { connection.Open(); }
                        else { MessageBox.Show("لم يتمكن البرنامج من استعادة النسخة الأصلية لقاعدة البيانات يرجى الاتصال بمدير النظام", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }
                    }
                    catch
                    { MessageBox.Show("لم يتمكن البرنامج من استعادة النسخة الأصلية لقاعدة البيانات يرجى الاتصال بمدير النظام", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }
                }
                else // حالة اخرى 
                { MessageBox.Show("يوجد خلل بالاتصال بقاعدة البيانات يرجى الاتصال بمدير النظام", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }
            }
        }

        public Boolean OpenConnection()
        {
            return true;
        }

        public Boolean CloseConection()
        {
            return false;
        }

        public Int32 runDDLSQL(String sqlString)
        {
            connection.Close();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = sqlString;
            connection.Open();
            try
            { return command.ExecuteNonQuery(); }
            catch (InvalidCastException e)
            {
                throw (e);
                //connection.Close();
            }
            finally 
            { connection.Close();}
        }

        public Int32 runDDLSQL(String sqlString, SqlParameter par)
        {
            connection.Close();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Parameters.Add(par);
            command.CommandText = sqlString;
            connection.Open();
            try
            { return command.ExecuteNonQuery(); }
            catch (InvalidCastException e)
            {
                throw (e);
                //connection.Close();
            }
            finally
            { connection.Close(); }
        }

        public Int32 runDDLSQL(String sqlString, SqlParameter par1, SqlParameter par2)
        {
            connection.Close();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Parameters.Add(par1);
            command.Parameters.Add(par2);           
            command.CommandText = sqlString;
            connection.Open();
            try
            { return command.ExecuteNonQuery(); }
            catch (InvalidCastException e)
            {
                throw (e);
                //connection.Close();
            }
            finally
            { connection.Close(); }
        }

        public Int32 runDDLSQL(String sqlString, SqlParameter par1, SqlParameter par2, SqlParameter par3)
        {
            connection.Close();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Parameters.Add(par1);
            command.Parameters.Add(par2);
            command.Parameters.Add(par3);
            command.CommandText = sqlString;
            connection.Open();
            try
            { return command.ExecuteNonQuery(); }
            catch (InvalidCastException e)
            {
                throw (e);
                //connection.Close();
            }
            finally
            { connection.Close(); }
        }

        public DataSet runSQLDataSet(String sqlString)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = sqlString;
                DataSet Return_DataSet = new DataSet();
                SqlDataAdapter DataAdapter = new SqlDataAdapter(command);
                DataAdapter.Fill(Return_DataSet);
                return Return_DataSet;
            }
            catch (InvalidCastException e)
            {
                throw (e);
                //connection.Close();
            }
        }

        public DataTable ExecuteQuery(String sqlString)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = sqlString;
            IDataReader dataReader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            dataReader.Close();
            return dataTable;
        }

        public IDataReader ExecuteQueryDataReader(String sqlString)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = sqlString;
            IDataReader dataReader = command.ExecuteReader();
            return dataReader;
        }

    }
}
