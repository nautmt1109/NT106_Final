
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnNT106
{
    public partial class Lobby : Form
    {
        bool sidebarExpand;
        String ip = "127.0.0.1";
        int port = 8081;
        public static TcpClient tcpClient = new TcpClient();
        public static StreamReader sr;
        public static StreamWriter sw;
        private Boolean enableListenLobby = true;
        private String usernameTran;

        public Lobby(String username)
        {
            InitializeComponent();
            usernameTran = username;
            label2.Text = "Username: " + username;
            //Init a connection to server
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpClient.Connect(iPEndPoint);
                sr = new StreamReader(tcpClient.GetStream());
                sw = new StreamWriter(tcpClient.GetStream());
                sw.AutoFlush = true;
                sendMessage(username + " connected");
                Thread received = new Thread(new ThreadStart(receivedMessage));
                received.IsBackground = true;
                received.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public StreamReader GetStreamReader()
        {
            return sr;
        }

        public StreamWriter GetStreamWriter()
        {
            return sw;
        }

        //Update listroom when notification from server
        private void receivedMessage()
        {
            try
            {
                while (enableListenLobby)
                {
                    String notification = sr.ReadLine();
                    Console.WriteLine(notification);
                    if (notification != null  && (notification.ToLower().Contains("join") || notification.ToLower().Contains("out") || notification.ToLower().Contains("create")))
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(updateRoom));
                        }
                        else
                        {
                            updateRoom();
                        }
                    }
                    if (notification != null && notification.ToLower().Contains("update"))
                    {
                        if (usernameTran.Equals(notification.Substring(0, notification.IndexOf(" "))))
                        {
                            usernameTran = notification.Substring(notification.IndexOf(":") + 1);
                            Console.WriteLine(usernameTran);
                            this.Invoke(new Action(() =>
                            {
                                label2.Text = "Username:" + usernameTran;
                            }));
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Send message to server
        private void sendMessage(String message)
        {
            try
            {
                sw.WriteLine(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //Enter to a exist room
            var joinRoomDto = new
            {
                username = usernameTran,
                roomId = listView1.SelectedItems[0].Text,
            };
            //Fetch api
            String joinRoomDtoJson = JsonConvert.SerializeObject(joinRoomDto);
            HttpContent httpContent = new StringContent(joinRoomDtoJson, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/user/joinRoom", httpContent);
            //Send notification to server
            sendMessage(usernameTran + " join room with id: " + joinRoomDto.roomId);
            //Go to play screen
            this.Hide();
            enableListenLobby = false;
            Play pl = new Play(listView1.SelectedItems[0].Text, usernameTran);
            pl.Hide();
            pl.ShowDialog();
            this.Show();
            updateRoom();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            sidebarTimer.Start();
        }

        private void sidebarTimer_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand)
            {
                sidebar.Width -= 10;
                if (sidebar.Width == sidebar.MinimumSize.Width) 
                {
                    sidebarExpand = false;
                    sidebarTimer.Stop();
                }
            }
            else
            {
                sidebar.Width += 10;
                if (sidebar.Width == sidebar.MaximumSize.Width)
                {
                    sidebarExpand= true;
                    sidebarTimer.Stop();
                }
            }
        }

        private static HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080")
        };

        private void bt_info_Click(object sender, EventArgs e)
        {
            Info frmInfo = new Info(usernameTran, tcpClient, sw);
            frmInfo.Hide();
            frmInfo.ShowDialog();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //Create room with random id
                Random random = new Random();
                CreateRoom newRoom = new CreateRoom();
                newRoom.username = usernameTran;
                newRoom.roomId = random.Next(1000, 9999).ToString();
                newRoom.typeMoney = 100;
                newRoom.numberPeople = 1;
                //Fetch api
                String request = JsonConvert.SerializeObject(newRoom);
                HttpContent httpContent = new StringContent(request, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/user/createRoom", httpContent);
                String result = await httpResponseMessage.Content.ReadAsStringAsync();
                MessageBox.Show(result);
                //Add room to listview
                ListViewItem item = new ListViewItem(newRoom.roomId);
                item.SubItems.Add(newRoom.typeMoney.ToString());
                item.SubItems.Add(newRoom.numberPeople.ToString());
                listView1.Items.Add(item);
                //Send notification to server
                sendMessage(usernameTran + " create new room with id: " + newRoom.roomId);
                //Go to play screen
                enableListenLobby = false;
                this.Hide();
                Play pl = new Play(newRoom.roomId, usernameTran);
                pl.Hide();
                pl.ShowDialog();
                this.Show();
                enableListenLobby = true;
                updateRoom();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void Lobby_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            updateRoom();
        }

        public async void updateRoom()
        {
            try
            {
                var roomResponse = await httpClient.GetAsync("/user/allRoom");
                var allRoom = await roomResponse.Content.ReadAsStringAsync();
                var allRoomJson = JsonConvert.DeserializeObject<List<CreateRoom>>(allRoom);

                if (listView1.InvokeRequired)
                {
                    listView1.Invoke(new Action(() =>
                    {
                        listView1.Items.Clear();
                        foreach (var roomJson in allRoomJson)
                        {
                            var item = new ListViewItem(roomJson.roomId);
                            item.SubItems.Add(roomJson.typeMoney.ToString());
                            item.SubItems.Add(roomJson.numberPeople.ToString());
                            if (roomJson.numberPeople > 0)
                            {
                                listView1.Items.Add(item);
                            }
                        }
                    }));
                }
                else
                {
                    listView1.Items.Clear();
                    foreach (var roomJson in allRoomJson)
                    {
                        var item = new ListViewItem(roomJson.roomId);
                        item.SubItems.Add(roomJson.typeMoney.ToString());
                        item.SubItems.Add(roomJson.numberPeople.ToString());
                        if (roomJson.numberPeople > 0)
                        {
                            listView1.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            enableListenLobby = false;
            sendMessage(usernameTran + " disconnected");
            sw.Close();
            sr.Close();
            if (tcpClient.Connected)
            {
                tcpClient.Close();
            }
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Feedback feedback = new Feedback();
            feedback.Hide();
            feedback.ShowDialog();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Chatbot chatbot = new Chatbot(usernameTran);
            chatbot.Hide();
            chatbot.ShowDialog();
        }
    }
}
