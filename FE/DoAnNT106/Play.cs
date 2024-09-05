using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DoAnNT106
{
    public partial class Play : Form
    {
        private TcpClient tcpClient = Lobby.tcpClient;
        private StreamWriter sw;
        private StreamReader sr;
        private String roomID;
        private String userName;
        private Boolean enableListenPlay;
        private System.Timers.Timer timer;
        public Play(String roomId, String username)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //Set username and roomId
            enableListenPlay = true;
            label1.Text = "Phòng " + roomId;
            roomID = roomId;
            userName = username;
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerEvent;
            sr = Lobby.sr;
            sw = Lobby.sw;
            //Thread receive message from server
            try
            {
                Thread thread = new Thread(new ThreadStart(receiveMessage));
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Boolean handleTimeEvent = true;

        //Count down time for start
        private void TimerEvent(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("count down fun is running");
            Invoke(new Action(() =>
            {
                if (handleTimeCountDown == 5)
                {
                    if (timeCountDown >= 0)
                    {
                        button7.Visible = true;
                        label8.Text = timeCountDown--.ToString();
                    }
                    if (timeCountDown == -1)
                    {
                        label8.Text = "Bắt đầu";
                        sendMessage(label1.Text + ": go");
                    }
                    if (timeCountDown < -1)
                    {
                        label8.Text = "";
                        label8.Visible = false;
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = true;
                        button2.Enabled = true;
                        button3.Enabled = true;
                        button4.Enabled = true;
                        timer.Stop();
                    }
                }
                else
                {
                    if (timeCountDown > 0)
                    {
                        label8.Text = timeCountDown--.ToString();
                    }
                    if (timeCountDown == 0)
                    {
                        label8.Text = "Hết giờ";
                        timeCountDown--;
                    }
                    if (timeCountDown < 0)
                    {
                        label8.Text = "";
                        label8.Visible = false;
                        label3.Visible = true;
                        if ((button1.Enabled == true) && (button2.Enabled == true) && (button1.Enabled == true))
                        {
                            handleNotChoose();
                        }
                        timer.Stop();
                    }
                }
            }));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            sendMessage(roomID + ":cancle");
        }

        //Handle if you don't choose any option
        private void handleNotChoose()
        {
            chooseOption = 0;
            sendMessage(label1.Text + "(play):" + userName + " choose not any option");
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }

        //Update UI to start a new game
        private void updateWhenReplay()
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible=false;
            label3.Visible=false;
            button5.Visible = true;
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

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            String message = richTextBox2.Text;
            //Send message
            sendMessage(label1.Text + "(chat):" + userName + ":" + message);
            richTextBox2.Text = "";
        }

        private String username2;

        //Receive message from server
        private void receiveMessage()
        {
            while (enableListenPlay)
            {
                String message = sr.ReadLine();
                //Ensure message will come to right room
                if (message != null && message.Contains(roomID))
                {
                    //Receive message from chat
                    if (message != null && message.Contains("chat"))
                    {
                        String finalMessage = message.Substring(message.IndexOf(":") + 1);
                        richTextBox1.AppendText(finalMessage + "\r\n");
                    }
                    //Receive result and display the result
                    if (message != null && message.ToLower().Contains("result"))
                    {
                        label8.Text = "";
                        Console.WriteLine("display result");
                        label8.Visible = false;
                        timer.Stop();
                        label3.Visible = true;
                        String finalResult = message.Substring(message.IndexOf(":") + 1);
                        Console.WriteLine(finalResult);
                        String[] finalResults = finalResult.Split('.');
                        String finallyResult = finalResults[0].Substring(0, finalResults[0].Length-1);
                        Console.WriteLine(finallyResult);
                        String usernameChoose = finalResults[1];
                        String[] handleUserChoose = usernameChoose.Split(',');
                        if (handleUserChoose[0].Contains(userName))
                        {
                            if (handleUserChoose[3].ToLower().Equals("kéo"))
                            {
                                pictureBox2.BackgroundImage = Properties.Resources.cardkeo;
                                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            }
                            else if (handleUserChoose[3].ToLower().Equals("búa"))
                            {
                                pictureBox2.BackgroundImage = Properties.Resources.cardbua;
                                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            }
                            else if (handleUserChoose[3].ToLower().Equals("bao"))
                            {
                                pictureBox2.BackgroundImage = Properties.Resources.cardbao;
                                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            }
                        }
                        else
                        {
                            if (handleUserChoose[1].ToLower().Equals("kéo"))
                            {
                                pictureBox2.BackgroundImage = Properties.Resources.cardkeo;
                                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            }
                            else if (handleUserChoose[1].ToLower().Equals("búa"))
                            {
                                pictureBox2.BackgroundImage = Properties.Resources.cardbua;
                                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            }
                            else if (handleUserChoose[1].ToLower().Equals("bao"))
                            {
                                pictureBox2.BackgroundImage = Properties.Resources.cardbao;
                                pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            }
                        }

                        label3.Text = finallyResult;
                        //If result is draw display result
                        if (finalResult != null && finalResult.ToLower().Contains("draw"))
                        {
                            ListViewItem viewItem = new ListViewItem("draw");
                            listView1.Items.Add(viewItem);
                        }
                        else
                        {
                            String[] history = finalResult.Split(' ');
                            Console.WriteLine("username1:" + userName);
                            Console.WriteLine("username2:" + username2);
                            //Display result to history
                            if (history[1].ToLower().Contains("win"))
                            {
                                //If username is equal player win save it, else save lose
                                if (userName.Equals(history[0]))
                                {
                                    ListViewItem viewItem = new ListViewItem("Win");
                                    listView1.Items.Add(viewItem);
                                    addMoney();
                                    callApiAddMoney(userName);
                                }
                                else
                                {
                                    ListViewItem viewItem = new ListViewItem("Lose");
                                    listView1.Items.Add(viewItem);
                                    subMoney();
                                    callApiSubMoney(userName);
                                }
                            }
                            if (history[1].ToLower().Equals("lose"))
                            {
                                //If username is equal player win save it, else save lose
                                if (userName.Equals(history[0]))
                                {
                                    ListViewItem viewItem = new ListViewItem("Lose");
                                    listView1.Items.Add(viewItem);
                                    subMoney();
                                    callApiSubMoney(userName);
                                }
                                else
                                {
                                    ListViewItem viewItem = new ListViewItem("Win");
                                    listView1.Items.Add(viewItem);
                                    addMoney();
                                    callApiAddMoney(userName);
                                }
                            }
                        }
                        rollbackStartUI();
                    }
                    //Notification when new user join room
                    if (message != null && message.ToLower().Contains("join"))
                    {
                        Console.WriteLine(message + "rec in playroom");
                        label5.Visible = true;
                        String noti = message.Substring(0, message.IndexOf("with") - 1);
                        String usernamePlayer2 = message.Substring(0, message.IndexOf(" "));
                        username2 = usernamePlayer2;
                        getMoney(username2);
                        waitTime(0.2);
                        Console.WriteLine(competitorMoney);
                        label5.Text = noti;
                        label6.Visible = true;
                        label7.Visible = true;
                        label6.Text = "Username: " + usernamePlayer2;
                        label7.Text = "Money: " + competitorMoney.ToString();
                        button5.Enabled = true;
                        waitTime(3);
                        label5.Visible = false;
                    }
                    //Update UI when start game
                    if (message != null && message.ToLower().Contains("start"))
                    {
                        setTimer(5);
                    }
                    //Update UI when player out room
                    if (message != null && message.ToLower().Contains("out"))
                    {
                        updateUIOutRoom();
                    }
                    //Set count down play time 15s
                    if (message != null && message.ToLower().Contains("go"))
                    {
                        button7.Visible = false;
                        setTimer(15);
                    }
                    //If button cancle is clicked cancle the play
                    if (message != null && message.ToLower().Contains("cancle"))
                    {
                        label8.Text = "";
                        label8.Visible = false;
                        button5.Visible = true;
                        button7.Visible = false;
                        timer.Stop();
                    }
                }
            }
        }

        private async void callApiAddMoney(String username)
        {
            String param = "/user/addMoney?username=" + username;
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(param);
        }

        private async void callApiSubMoney(String username)
        {
            String param = "/user/subMoney?username=" + userName;
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(param);
        }

        //Add money if you win and sub money your competitor
        private void addMoney()
        {
            Invoke(new Action(() =>
            {
                Double money1 = Double.Parse(label4.Text.Substring(label4.Text.IndexOf(" "))) + 500;
                label4.Text = "Money: " + money1.ToString();
                Double money2 = Double.Parse(label7.Text.Substring(label7.Text.IndexOf(" "))) - 500;
                label7.Text = "Money: " + money2.ToString();
            }));
        }

        private void subMoney()
        {
            Invoke(new Action(() =>
            {
                Double money1 = Double.Parse(label4.Text.Substring(label4.Text.IndexOf(" "))) - 500;
                label4.Text = "Money: " + money1.ToString();
                Double money2 = Double.Parse(label7.Text.Substring(label7.Text.IndexOf(" "))) + 500;
                label7.Text = "Money: " + money2.ToString();
            }));
        }

        private int chooseOption;

        //Display result into 3s and replay start room
        private void rollbackStartUI()
        {
            Thread.Sleep(3000);
            updateWhenReplay();
        }

        //Update UI when people out room
        private void updateUIOutRoom()
        {
            label3.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            button5.Visible = true;
            button5.Enabled = false;
            button2.Enabled=false;
            button3.Enabled=false;
            button4.Enabled=false;
        }

        /*private void updateStartRoom()
        {
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            button5.Visible = false;
        }*/

        private double competitorMoney = 10000;

        //Get money
        private async void getMoney(string username)
        {
            Console.WriteLine(username);
            String requestParam = "/user/money?username=" + username;
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestParam);
            String money = await httpResponseMessage.Content.ReadAsStringAsync();
            competitorMoney = double.Parse(money);
        }


        //Set time waiting
        private void waitTime(double second)
        {
            Thread.Sleep((int)(second * 1000));
        }

        private static HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080")
        };

        private async void handleOutRoom(String username, String roomId)
        {
            try
            {
                var outRoomDto = new
                {
                    username = username,
                    roomId = roomID
                };
                String outRoomDtoJson = JsonConvert.SerializeObject(outRoomDto);
                HttpContent httpContent = new StringContent(outRoomDtoJson, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/user/outRoom", httpContent);
            }
            catch (Exception e) 
            { 
                MessageBox.Show(e.Message);
            }
        }

        //Exit room
        private void button1_Click(object sender, EventArgs e)
        {
            enableListenPlay = false;
            handleOutRoom(userName, label1.Text);
            sendMessage(label1.Text + ":" + userName + " out room");
            Close();
        }

        //Event for choose kéo
        private void button2_Click(object sender, EventArgs e)
        {
            chooseOption = 1;
            sendMessage(label1.Text + "(play):" + userName + " choose kéo");
            button3.Enabled = false;
            button4.Enabled = false;
            pictureBox1.BackgroundImage = Properties.Resources.cardkeo;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
        }
        //Event for choose búa
        private void button3_Click(object sender, EventArgs e)
        {
            chooseOption = 2;
            sendMessage(label1.Text + "(play):" + userName + " choose búa");
            button4.Enabled = false;
            button2.Enabled = false;
            pictureBox1.BackgroundImage = Properties.Resources.cardbua;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
        }
        //Event for choose bao
        private void button4_Click(object sender, EventArgs e)
        {
            chooseOption = 3;
            sendMessage(label1.Text + "(play):" + userName + " choose bao");
            button2.Enabled = false;
            button3.Enabled=false;
            pictureBox1.BackgroundImage = Properties.Resources.cardbao;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
		
        private void Play_Load(object sender, EventArgs e)
        {
            button7.Visible = false;
            label8.Visible = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            updatePlayRoom();
        }

        //Update UI when join or out room
        private async void updatePlayRoom()
        {
            //Get info user in room
            String requestParam = "/user/room/info?roomId=" + roomID;
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestParam);
            String data = await httpResponseMessage.Content.ReadAsStringAsync();
            InfoUserRoom userInfo = JsonConvert.DeserializeObject<InfoUserRoom>(data);
            if (userInfo != null)
            {
                //if you are host (room has 1 people)
                if (String.IsNullOrEmpty(userInfo.username2))
                {
                    label2.Text = "Username: " + userInfo.username1;
                    label4.Text = "Money: " + userInfo.money1.ToString();
                    label3.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;
                    label7.Visible = false;
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                    button5.Enabled = false;
                }
                //room has 2 people
                else
                {
                    Console.WriteLine("update room");
                    if (userName.Equals(userInfo.username1))
                    {
                        label2.Text = "Username: " + userInfo.username1;
                        label4.Text = "Money: " + userInfo.money1.ToString();
                        label6.Text = "Username: " + userInfo.username2;
                        label7.Text = "Money: " + userInfo.money2.ToString();
                    }
                    else
                    {
                        label2.Text = "Username: " + userInfo.username2;
                        label4.Text = "Money: " + userInfo.money2.ToString();
                        label6.Text = "Username: " + userInfo.username1;
                        label7.Text = "Money: " + userInfo.money1.ToString();
                    }
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                    label3.Visible = false;
                    label5.Visible = false;
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        //Event for button start game
        private void button5_Click(object sender, EventArgs e)
        {
            sendMessage(label1.Text + ":start new game");
        }

        private int timeCountDown;
        private int handleTimeCountDown;

        //Set timer count down
        private void setTimer(int time)
        {
            Console.WriteLine("set time fun is running");
            timeCountDown = time;
            handleTimeCountDown = time;
            pictureBox1.BackgroundImage = Properties.Resources.card;
            pictureBox2.BackgroundImage = Properties.Resources.card;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            if (handleTimeCountDown == 15)
            {
                pictureBox1.Visible = true;
                pictureBox2.Visible = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }
            label8.Visible = true;
            button5.Visible = false;
            timer.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Rule frmrule = new Rule();
            frmrule.ShowDialog();
        }
    }
}
