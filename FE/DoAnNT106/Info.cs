using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnNT106
{
    public partial class Info : Form
    {
        private String username;
        private TcpClient tcpClient;
        private StreamWriter sw;
        public Info(String username, TcpClient client, StreamWriter writer)
        {
            InitializeComponent();
            this.username = username;
            tcpClient = client;
            this.sw = writer;
            sw.AutoFlush = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var userEditInfo = new
            {
                username = textBox1.Text,
                email = textBox2.Text,
                money = Double.Parse(textBox3.Text),
                matchWin = int.Parse(textBox4.Text),
                matchLose = int.Parse(textBox5.Text),
                password = textBox6.Text,
            };
            String queryParam = "http://localhost:8080/user/edit?username=" + username;
            HttpClient client = new HttpClient();
            var userEditJson = JsonConvert.SerializeObject(userEditInfo);
            var requestBody = new StringContent(userEditJson, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PutAsync(queryParam, requestBody);
            MessageBox.Show("Success");
            sw.WriteLine(username + " update:"+textBox1.Text);
            Thread.Sleep(300);
            Close();
        }

        private async void Info_Load(object sender, EventArgs e)
        {
            try
            {
                String queryParam = "http://localhost:8080/user/info?username=" + username;
                HttpClient client = new HttpClient();
                HttpResponseMessage httpResponseMessage = await client.GetAsync(queryParam);
                Thread.Sleep(200);
                var data = await httpResponseMessage.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(data);
                textBox1.Text = userInfo.username;
                textBox2.Text = userInfo.email;
                textBox3.Text = userInfo.money.ToString();
                textBox4.Text = userInfo.matchWin.ToString();
                textBox5.Text = userInfo.matchLose.ToString();
                textBox6.Text = userInfo.password.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
