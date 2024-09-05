using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnNT106
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private static HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080"),
        };

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                String email = textBox1.Text;
                String password = textBox2.Text;
                User user = new User();
                user.email = email;
                user.password = password;
                String json = JsonConvert.SerializeObject(user);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/auth/login", content);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    String message = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (!message.Contains("fail"))
                    {
                        MessageBox.Show("Success");
                        this.Hide();
                        Lobby lobby = new Lobby(message);
                        lobby.ShowDialog();
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }
                }
                else
                {
                    MessageBox.Show("login fail");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread thread = new Thread(new ThreadStart(signup));
            thread.Start();
        }

        private void signup()
        {
            Application.Run(new signup());
        }

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    throw new Exception("Enter your email to recover your password");
                }
                String email = textBox1.Text;
                if (String.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Please enter your email");
                }
                User user = new User();
                user.recipient = email;
                String body = JsonConvert.SerializeObject(user);
                HttpContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await httpClient.PostAsync("/auth/sendForgetMail", httpContent);
                var data = await httpResponse.Content.ReadAsStringAsync();
                if (data.ToString().Contains("fail"))
                {
                    throw new Exception(data.ToString());
                }
                else
                {
                    MessageBox.Show("Please check your mail to restore the password");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }
    }
}
