﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
/// <summary>
/// Server is here
/// </summary>
namespace frmServer
{
    public partial class Seerver : Form
    {
        public Seerver()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        void btn_Send_Click(object sender, EventArgs e)
        {
        }

        IPEndPoint IP;
        Socket server;
        List<Socket> listClient = new List<Socket>();

        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Any, 2000);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(IP);
            server.Listen(100);
            Thread listen = new Thread(
            () =>
            {
            RESET:
                try
                {
                    while (true)
                    {
                        
                        Console.WriteLine("");
                        Socket client = server.Accept(); //chap nhan ket noi tu client
                        listClient.Add(client); //them client vao list

                        //tao mot luong rieng de lang nghe tu client nay
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);

                        if (listClient.Count == 1)
                        {
                            throw new Exception();
                        }
                    }
                }
                catch
                {
                    goto RESET;                   
                }
            });
            listen.IsBackground = true;
            listen.Start();
        }

        void Closee()
        {
        }

        void Receive(object client)
        {
            Socket cli = client as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[500 * 1024];
                    if(data!= null)
                    {
                        cli.Receive(data);
                        string test = (string)Deserialize(data);
                        addMessage(test);
                    }                   
                }
            }
            catch
            {
                listClient.Remove(cli);
                cli.Close();
            }
        }

        void Send()
        {
        }

        byte[] Serialize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        object Deserialize(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(ms);
        }

        private void addMessage(string str)
        {
            rtb_history.Text += str + "\n";
        }
    }
}