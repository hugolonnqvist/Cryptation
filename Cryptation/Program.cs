using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Cryptation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            bool run = true;
            List<Message> messages = new List<Message>();
            List<Message> savedMessages = new List<Message>();

            Console.WriteLine("Write your username: ");
            Console.Write("Enter username: ");
            string userName = Console.ReadLine();

            while (run)
            {
                try
                {

                    Console.WriteLine("What do you want to do?");
                    Console.WriteLine("1. Change username");
                    Console.WriteLine("2. Write a new message and send to server");
                    Console.WriteLine("3. Get messages from server");
                    Console.WriteLine("4. Show messages");
                    Console.WriteLine("5. Quit program");

                    int choice = int.Parse(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:
                            Console.Write("Username: ");
                            userName = Console.ReadLine();
                            Console.Clear();
                            break;
                        case 2:
                            SendMessage(messages, userName);
                            Console.Clear();
                            break;
                        case 3:
                            RecieveMessagesFromServer(savedMessages);
                            break;
                        case 4:
                            if (savedMessages.Count > 0)
                            {
                                foreach (Message m in savedMessages)
                                {
                                    Console.WriteLine($"User: {m.Username}");
                                    Console.WriteLine($"Message Id: {m.MessageId}");
                                    Console.WriteLine($"Message: {DecryptMessage(m.Text)}");
                                    Console.WriteLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("You have not saved any messages");
                            }
                            break;
                        case 5:
                            string address = "127.0.0.1";
                            int port = 8001;
                            
                            TcpClient tcpClient = new TcpClient();
                            tcpClient.Connect(address, port); 
                            
                            Byte[] bMessage = System.Text.Encoding.ASCII.GetBytes("1");
                            NetworkStream tcpStream = tcpClient.GetStream();
                            tcpStream.Write(bMessage, 0, bMessage.Length);
                            
                            tcpClient.Close();
                            run = false;
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        static string EncryptMessage(string message)
        {
            char[] charArr = message.ToCharArray(0, message.Length);

            for (int i = 0; i < charArr.Length; i++)
            {
                charArr[i]++;
            }

            string encryptedMessage = new string(charArr);
            return encryptedMessage;
        }
        
         static string DecryptMessage(string message)
        {
            char[] charArr = message.ToCharArray(0, message.Length);

            for (int i = 0; i < charArr.Length; i++)
            {
                charArr[i]--;
            }

            string decryptedMessage = new string(charArr);
            return decryptedMessage;
        }

         static void SendMessage(List<Message> messages, string userName)
         {
             try
             {
                 Console.Write("Write a message: ");
                 string message = Console.ReadLine();
                 message = EncryptMessage(message);
                 messages.Add(new Message(userName, messages.Count + 1, message));

                 string address = "127.0.0.1";
                 int port = 8001;

                 // Anslut till servern:
                 Console.WriteLine("Ansluter...");
                 TcpClient tcpClient = new TcpClient();
                 tcpClient.Connect(address, port);
                 Console.WriteLine("Ansluten!");

                 Byte[] bMessage = System.Text.Encoding.ASCII.GetBytes(messages[messages.Count - 1].ToString());

                 Console.WriteLine("Skickar...");
                 NetworkStream tcpStream = tcpClient.GetStream();
                 tcpStream.Write(bMessage, 0, bMessage.Length);
                 Console.WriteLine("Skickat medelande till servern");

                 // Stäng anslutningen:
                 Console.ReadLine();
                 tcpClient.Close();

             }
             catch (Exception e)
             {
                 Console.WriteLine("Error: " + e.Message);

             }
         }

         static void RecieveMessagesFromServer(List<Message> savedMessages)
         {
             try
             {
                 string address = "127.0.0.1";
                 int port = 8001;

                 TcpClient tcpClient = new TcpClient();
                 tcpClient.Connect(address, port);

                 Byte[] bMessage = System.Text.Encoding.ASCII.GetBytes("0");

                 NetworkStream tcpStream = tcpClient.GetStream();
                 tcpStream.Write(bMessage, 0, bMessage.Length);
 
                 // Tag emot meddelande från servern:
                 byte[] bRecievedMessage = new byte[256];
                 int bReadSize = tcpStream.Read(bRecievedMessage, 0, bRecievedMessage.Length);

                 // Konvertera meddelandet till ett string-objekt och skriv ut:
                 string recievedMessage = "";
                 for (int i = 0; i < bReadSize; i++)
                 {
                     recievedMessage += Convert.ToChar(bRecievedMessage[i]);
                 }

                 //Delete old elements in list
                 savedMessages.Clear();

                 //Key for separating different messages
                 string[] splitKey1 = {"/??/"};
                 string[] splitedString1 = recievedMessage.Split(splitKey1, StringSplitOptions.RemoveEmptyEntries);

                 //Key for separating user, messageId and text information
                 string[] splitKey2 = {"#//#"};

                 //Loops and creates message elements from the information and adds it to list
                 foreach (string split in splitedString1)
                 {
                     string[] splitedString2 = split.Split(splitKey2, StringSplitOptions.RemoveEmptyEntries);
                     string username = splitedString2[0];
                     int messageId = int.Parse(splitedString2[1]);
                     string text = splitedString2[2];
                     savedMessages.Add(new Message(username, messageId, text));
                 }

                 Console.Clear();
                 if (savedMessages.Count > 0)
                 {
                     Console.WriteLine("Messages recieved and saved");
                 }
                 else if (savedMessages.Count == 0)
                 {
                     throw new NoMessagesException(0);
                 }
             }
             catch (NoMessagesException noMessage)
             {
                 Console.WriteLine($"Error: You have {noMessage.MessagesAmount} message");
             }
             catch (Exception e)
             {
                 Console.WriteLine(e.Message);
             }
         }
    }
}