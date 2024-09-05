using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnNT106
{
    public partial class signup : Form
    {
        public signup()
        {
            InitializeComponent();
        }

        private static HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080"),
        };

        public class Email{
            public String recipient { get; set; }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (label1.Visible == true || label2.Visible == true || label3.Visible == true)
                {
                    MessageBox.Show("Nhập lại đúng định dạng");
                }
                else
                {
                    String username = textBox1.Text;
                    String email = textBox2.Text;
                    String password = textBox3.Text;
                    User user = new User();
                    user.username = username;
                    user.email = email;
                    user.password = password;
                    String json = JsonConvert.SerializeObject(user);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/auth/register", content);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var data = await httpResponseMessage.Content.ReadAsStringAsync();
                        if (data.ToString().Contains("fail"))
                        {
                            throw new Exception(data);
                        }
                        MessageBox.Show("Register successfully.Please check your mail to verify your account");
                        Close();
                        Email _email = new Email();
                        _email.recipient = email;
                        String json1 = JsonConvert.SerializeObject(_email);
                        HttpContent content1 = new StringContent(json1, Encoding.UTF8, "application/json");
                        HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsync("/auth/sendVerifyMail", content1);
                    }
                    else
                    {
                        MessageBox.Show("Register fail. Email is invalid or exist!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void signup_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.UseSystemPasswordChar == true)
            {
                textBox3.UseSystemPasswordChar = false;
            }
            else
            {
                textBox3.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox2.Text))
            {
                label1.Text = "Username không được để trống";
                label1.Visible = true;
            }
            else
            {
                label1.Visible = false;
            }
            if (!textBox2.Text.Contains("@gmail.com"))
            {
                label1.Text = "Email phải có dạng @gmail.com";
                label1.Visible = true;
            }
            else
            {
                label1.Visible = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox3.Text))
            {
                label2.Text = "Username không được để trống";
                label2.Visible = true;
            }
            else
            {
                label2.Visible = false;
            }
            if (textBox3.Text.Length < 8)
            {
                label2.Text = "Mật khẩu phải dài ít nhất 8 kí tự";
                label2.Visible = true;
            }
            else 
            { 
                label2.Visible = false; 
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                label3.Text = "Username không được để trống";
                label3.Visible = true;
            }
            else
            {
                label3.Visible = false;
            }
        }
    }
}
