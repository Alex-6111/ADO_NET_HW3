using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace ADO_NET_HW3
{
    public partial class Form1 : Form
    {
        string connStr;
        DataTable dt;
        List<StationeryType> types;
        public Form1()
        {
            InitializeComponent();
            connStr = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Stationaries;Integrated Security=True";
            types = new List<StationeryType>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("The connection was successful!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    conn?.Close();
                }
                finally
                {
                    conn?.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT * FROM Stationeries");
        }


        private void button4_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT Salesmans.Name, Salesmans.Surname, Sales.DateOfSale FROM Salesmans JOIN Sales ON Sales.SalesmanId = Salesmans.Id");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT * FROM StationeryTypes");
        }


        private void button5_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT Stationeries.Name, Stationeries.Amount FROM Stationeries WHERE Stationeries.Amount = (SELECT MAX(Amount) FROM Stationeries)");
        }


        private void button7_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT Stationeries.Name, Stationeries.Amount FROM Stationeries WHERE Stationeries.Amount = (SELECT MIN(Amount) FROM Stationeries)");
        }


        private void button6_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT Stationeries.Name, Stationeries.Price FROM Stationeries WHERE Stationeries.Price = (SELECT MAX(Price) FROM Stationeries)");
        }


        private void button8_Click(object sender, EventArgs e)
        {
            ShowFromDb("SELECT Stationeries.Name, Stationeries.Price FROM Stationeries WHERE Stationeries.Price = (SELECT MIN(Price) FROM Stationeries)");
        }

        private void ShowFromDb(string query)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    dt = new DataTable();
                    int line = 0;
                    while (reader.Read())
                    {
                        if (line == 0)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dt.Columns.Add(reader.GetName(i));
                            }
                        }
                        DataRow row = dt.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        dt.Rows.Add(row);
                        line++;
                    }
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    reader?.Close();
                    connection?.Close();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string queryStr = "SELECT * FROM StationeryTypes";
                SqlCommand command = new SqlCommand(queryStr, connection);
                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    types.Clear();
                    while (reader.Read())
                    {
                        StationeryType type = new StationeryType();
                        type.Id = reader.GetInt32(0);
                        type.Name = reader.GetString(1);
                        types.Add(type);
                    }
                    comboBox1.DataSource = null;
                    comboBox1.DisplayMember = "Name";
                    comboBox1.ValueMember = "Id";
                    comboBox1.DataSource = types;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    reader?.Close();
                    connection?.Close();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ShowFromDb($"SELECT * FROM Stationeries WHERE Stationeries.TypeId = {comboBox1.SelectedValue}");
        }


        private void button10_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string title = textBox1.Text;
                string queryStr = $"SELECT Stationeries.Name, Sales.DateOfSale, FirmBuyers.Name AS Firm FROM Stationeries " +
                    $"JOIN Sales ON Sales.StationeryId = Stationeries.Id " +
                    $"JOIN FirmBuyers ON Sales.FirmBuyerId = FirmBuyers.Id " +
                    $"WHERE FirmBuyers.Name = @name";
                SqlCommand command = new SqlCommand(queryStr, connection);
                SqlParameter titleParam = command.Parameters.Add("@name", SqlDbType.NVarChar, 1000);
                titleParam.Value = textBox1.Text;
                SqlDataReader reader = null;
                try
                {
                    connection.Open();
                    reader = command.ExecuteReader();
                    dt = new DataTable();
                    int line = 0;
                    while (reader.Read())
                    {
                        if (line == 0)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                dt.Columns.Add(reader.GetName(i));
                            }
                        }
                        DataRow row = dt.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        dt.Rows.Add(row);
                        line++;
                    }
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    reader?.Close();
                    connection?.Close();
                }
            }
        }


        private void button11_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("ShowAVGSellingsOnYear", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@year", SqlDbType.Int).Value = Convert.ToInt32(textBox2.Text);
                    SqlParameter outputParam = new SqlParameter("@avgPrice", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(cmd.Parameters["@avgPrice"].Value.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection?.Close();
                }
            }
        }


    }
}
