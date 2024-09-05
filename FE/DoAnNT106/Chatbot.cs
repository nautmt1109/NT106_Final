using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnNT106
{
    public partial class Chatbot : Form
    {
        private String username;
        public Chatbot(String username)
        {
            InitializeComponent();
            this.username = username;
            richTextBox1.ReadOnly = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                String prompt = textBox1.Text.ToString();
                richTextBox1.AppendText(username + ":" +prompt + "\n");
                textBox1.Text = "";
                String uri = "http://localhost:8080/user/gemini";
                var requestBody = new
                {
                    text = prompt
                };
                var requestBodyJson = JsonConvert.SerializeObject(requestBody);
                HttpContent httpContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, httpContent);
                if (httpResponseMessage.IsSuccessStatusCode == true)
                {
                    String response = await httpResponseMessage.Content.ReadAsStringAsync();
                    richTextBox1.AppendText(response + "\n");
                    button1.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
