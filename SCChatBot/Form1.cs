﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WebSocketSharp;

namespace SCChatBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WebSocket ws = new WebSocket("wss://connect-bot.classic.blizzard.com/v1/rpc/chat", "json");

        public int lastRequestId = 0;

        //| Area  |  Code  |  Reason
        //| 8     |   1    | Not connected to chat
        //| 8     |   2    | Bad request
        //| 6     |   5    | Request timed out
        //| 6     |   8    | Hit rate limit


        // websocket-sharp: https://github.com/sta/websocket-sharp/blob/master/README.md
        // JSON.NET documentation: http://www.newtonsoft.com/json/help/html/Introduction.htm

        // When connection is established, the client will need to send an authentication request with the API key:
        public void Authentication()
        {
            var apiKeyFile = @"C:\Users\mdinger\Desktop\ChatBotAPIKey.txt";
            var apiKey = File.ReadAllText(apiKeyFile);
            var messageSpacer = @"--------------------------------------------------------------";
            var greenDot = @"C:\Users\mdinger\documents\visual studio 2017\Projects\SCChatBot\SCChatBot\Resources\GreenDot.png";
            var redDot = @"C:\Users\mdinger\documents\visual studio 2017\Projects\SCChatBot\SCChatBot\Resources\RedDot.png";

            var auth = new MessageClasses.Auth
            {
                command = "Botapiauth.AuthenticateRequest",
                request_id = ++lastRequestId,
                payload = new Dictionary<string, string>
                {
                    { "api_key", apiKey }
                }
            };
            var authJson = JsonConvert.SerializeObject(auth, Formatting.Indented);

            ws.OnOpen += (sender, e) =>
            {
                ws.Send(authJson);
                pictureBox1.Update();
                pictureBox1.Load(greenDot);
            };

            ws.OnError += (sender, e) =>
            {
                Console.WriteLine(@"OnError - Server Response:");
                Console.WriteLine(e.Message);
                Console.WriteLine(messageSpacer);
            };

            ws.OnMessage += (sender, e) =>
            {

                Console.WriteLine(@"OnMessage - Server Response:");
                Console.WriteLine(e.Data);
                Console.WriteLine(messageSpacer);

                var _items = new List<string> {"One", "Two"};
                listBoxUsersInChannel.DataSource = _items;
            };

            ws.OnClose += (sender, e) =>
            {
                Console.WriteLine(@"OnClose - WebSocket connection was closed:");
                Console.WriteLine(e.Reason);
                Console.WriteLine(messageSpacer);
                pictureBox1.Load(redDot);
            };

            ws.Connect();
        }

        // Connect the bot to the gateway and chat channel
        public void ChatConnect()
        {

            var chatConnect = new MessageClasses.ChatConnection
            {
                command = "Botapichat.ConnectRequest",
                request_id = ++lastRequestId,
                payload = new Dictionary<string, string>
                {
                    
                }
            };
            var chatConnectionJson = JsonConvert.SerializeObject(chatConnect, Formatting.Indented);

            ws.Send(chatConnectionJson);
        }

        // Disconnects the bot from the gateway and chat channel
        public void ChatDisconnect()
        {
            var disc = new MessageClasses.ChatDisconnection
            {
                command = "Botapichat.DisconnectRequest",
                request_id = ++lastRequestId,
                payload = new Dictionary<string, string>
                {

                }
            };
            var discJson = JsonConvert.SerializeObject(disc, Formatting.Indented);

            ws.Send(discJson);
           
           
        }

        // Sends a chat message to the channel
        public void ChatSendMessage()
        {
            var msg = txtBoxChatMessage.Text;

            var sendChat = new MessageClasses.SendChatMessage
            {
                command = "Botapichat.SendMessageRequest",
                request_id = ++lastRequestId,
                payload = new Dictionary<string, string>
                {
                    {"message", msg }
                }

            };
            var chatMessage = JsonConvert.SerializeObject(sendChat, Formatting.Indented);

            ws.Send(chatMessage);
            txtBoxChatMessage.Text = string.Empty;
        }

        // Sends a chat message to one user in the channel
        public void ChatSendWhisper()
        {

        }

        // A message was posted to the channel
        // Type is one of: Whisper, Channel, ServerInfo, ServerError, Emote
        public void OnMessageEvent()
        {

        }

        // A user has joined the current channel or got an update
        // Flags are: Admin, Moderator, Speaker, MuteGlobal, MuteWhisper 
        // Attributes are: ProgramId, Rate, Rank, Wins
        public void OnUserUpdateEvent()
        {

        }

        // A user in the current channel has left
        public void OnUserLeaveEvent()
        {

        }

        private void btnAuthConnect_Click(object sender, EventArgs e)
        {
            Authentication();
            
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            ChatSendMessage();
        }

        private void txtBoxChatMessage_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                if (txtBoxChatMessage.Text != String.Empty)
                {
                    ChatSendMessage();
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
           ChatDisconnect();
        }

        private void btnChatConnect_Click(object sender, EventArgs e)
        {
           ChatConnect();
        }
    }
}
