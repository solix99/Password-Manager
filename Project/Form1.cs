using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace Project
{
    public partial class Form1 : Form
    {

        private string conn;
        private MySqlConnection connect;
        private string connectedID;
        private string latestData;
        private string showData;
        private string originalData;

        public Form1()
        {
            InitializeComponent();
        }

        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter};
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Hide();
            textBox1.Hide();
            label4.Hide();
            label8.Hide();
            textBox6.Hide();
            label6.Hide();
            textBox4.Hide();
            label7.Hide();
            textBox5.Hide();
            button2.Hide();
            button5.Hide();

            label2.BackColor = System.Drawing.Color.Transparent;
            label1.BackColor = System.Drawing.Color.Transparent;
            label4.BackColor = System.Drawing.Color.Transparent;
            label8.BackColor = System.Drawing.Color.Transparent;
            label6.BackColor = System.Drawing.Color.Transparent;
            label7.BackColor = System.Drawing.Color.Transparent;
            label3.BackColor = System.Drawing.Color.Transparent;
            label5.BackColor = System.Drawing.Color.Transparent;

            db_connection();

        }
        private void db_connection()
        {
            try
            {
                conn = "Server=localhost;Port=3307;Database=passwordmanager_db;Uid=admin;Pwd=admin;";
                connect = new MySqlConnection(conn);
                connect.Open();

                Console.WriteLine("Connected to SQL!");

            }
            catch (MySqlException e)
            {
                Console.WriteLine("Connection Failed");
                throw;
            }
        }
        private bool validate_login(string user, string pass)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select * FROM accounts_tb WHERE USERNAME=@user AND PASSWORD=@pass";
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Connection = connect;
            MySqlDataReader login = cmd.ExecuteReader();

            if (login.Read())
            {
                connectedID = login.GetString(0);

                label2.Hide();
                button3.Hide();
                button1.Hide();
                textBox2.Hide();
                textBox3.Hide();
                label3.Hide();
                label5.Hide();


                connect.Close();

                main_app();

                return true;
            }
            else
            {
                DialogResult result = MessageBox.Show("Account Does not Exist!", "Info");
                return false;
            }
        }

        private bool attempt_registration(string user, string pass)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT INTO accounts_tb (USERNAME,PASSWORD) VALUES (@user,@pass);";
            cmd.Parameters.AddWithValue("@user", user);
            cmd.Parameters.AddWithValue("@pass", pass);
            cmd.Connection = connect;
            MySqlDataReader data = cmd.ExecuteReader();

            DialogResult result = MessageBox.Show("Account Created!","Info");

            return true;

        }

        private void main_app()
        {
            db_connection();

            label1.Show();
            textBox1.Show();
            label4.Show();
            label8.Show();
            textBox6.Show();
            label6.Show();
            textBox4.Show();
            label7.Show();
            textBox5.Show();
            button2.Show();
            button5.Show();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Select DATA FROM accounts_tb WHERE ID=@connectedID;";
            cmd.Parameters.AddWithValue("@connectedID", connectedID);
            cmd.Connection = connect;

            MySqlDataReader data = cmd.ExecuteReader();
            data.Read();

            latestData = data.GetString(0);
            originalData = latestData;

            processData();

        }

        private void processData()
        {
            showData = "";

            int length = latestData.Length;
            if(length!=0)
            {
                if (latestData.ElementAt(latestData.Length - 1) != ',')
                {
                    latestData += ',';
                }
            }


            int lastComma = 0;
            int nextComma = 0;
            int dataPoints = 0;
            bool firstWord = true;

            for (int i = 0; i < length; i++)
            {
                if (latestData.ElementAt(i) == ',')
                {
                    dataPoints++;
                    nextComma = i;
                    if (dataPoints == 1)
                    {
                        if(firstWord)
                        {
                            firstWord = false;
                            showData += " [Info]";
                            showData += latestData.Substring(lastComma, nextComma - lastComma);
                        }
                        else
                        {
                            showData += " [Info]";
                            showData += latestData.Substring(lastComma + 1, nextComma - lastComma - 1);
                        }

                    }
                    else if (dataPoints == 2)
                    {
                        showData += " [User]:";
                        showData += latestData.Substring(lastComma + 1, nextComma - lastComma - 1);
                    }
                    else
                    {
                        dataPoints = 0;
                        showData += " [Pass]:";
                        showData += latestData.Substring(lastComma + 1, nextComma - lastComma -1 );
                        showData += System.Environment.NewLine;

                    }

                    lastComma = nextComma;
                }
            }
            Console.WriteLine(showData);
            textBox1.Text = showData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            validate_login(textBox2.Text.ToString(),textBox3.Text.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            attempt_registration(textBox2.Text.ToString(),textBox3.Text.ToString());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox6.Text.Length>1 && textBox4.Text.Length > 1 && textBox5.Text.Length > 1)
            {
                string newAccount = textBox6.Text + "," + textBox4.Text + "," + textBox5.Text + ",";

                latestData += newAccount;
              
                processData();

                db_connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = "UPDATE accounts_tb SET DATA =@data WHERE ID=@accountID;";
                cmd.Parameters.AddWithValue("@data", latestData);
                cmd.Parameters.AddWithValue("@accountID", connectedID);
                cmd.Connection = connect;
                MySqlDataReader data = cmd.ExecuteReader();

                DialogResult result = MessageBox.Show("Account Added!", "Info");
            }
            else
            {
                DialogResult result = MessageBox.Show("Please add valid information!", "Info");
            }

        }

        private void updateAccountData(string data)
        {
            db_connection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "UPDATE accounts_tb SET DATA =@data WHERE ID=@accountID;";
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@accountID", connectedID);
            cmd.Connection = connect;
            cmd.ExecuteReader();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("Enter the [INFO] of the account you want to remove.", "Remove Account");

            int lastDataPos = 0, deleteDataPos = 0, kFound = 0;
            bool dataFound = false;
               
            for(int i=0;i<latestData.Length;i++)
            {
                if (latestData.ElementAt(i) ==',')
                {
                    if (latestData.ElementAt(lastDataPos) == ',')
                    {
                        if (latestData.Substring(lastDataPos + 1, i - lastDataPos - 1) == promptValue)
                        {
                            deleteDataPos = lastDataPos;
                            dataFound = true;
                            break;
                        }
                    }
                    else
                    {
                        if (latestData.Substring(lastDataPos, i - lastDataPos) == promptValue)
                        {
                            deleteDataPos = lastDataPos;
                            dataFound = true;
                            break;
                        }
                    }

                    lastDataPos = i;
                }
            }
            if(dataFound)
            {
                for (int i = deleteDataPos; i < latestData.Length; i++)
                {
                    if (latestData.ElementAt(i) == ',')
                    {
                        kFound++;
                        lastDataPos = i;
                    }
                    if(kFound == 3 && deleteDataPos == 0)
                    {
                        latestData = latestData.Remove(deleteDataPos, lastDataPos - deleteDataPos+1);
                        processData();

                        break;
                    }
                    else if(kFound == 4 && deleteDataPos != 0)
                    {
                        latestData = latestData.Remove(deleteDataPos+1, lastDataPos - deleteDataPos);
                      
                        processData();

                        break;
                    }
                }
                updateAccountData(latestData);

                DialogResult result = MessageBox.Show("Account removed succesfully!", "Info");
            }
            else
            {
                DialogResult result = MessageBox.Show("Account does not exist!", "Info");
            }
        }
    }
}
