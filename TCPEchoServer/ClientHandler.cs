using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Text.Json;
using System.IO;
using TCPSPServerJson;

namespace TCPSPServerJson
{
    public class ClientHandler
    {
        public static void HandleClient(TcpClient client)
        {
            Console.WriteLine(client.Client.RemoteEndPoint);
            NetworkStream ns = client.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns) { AutoFlush = true };

            bool IsRunning = true;

            while (IsRunning)
            {
                string message = reader.ReadLine();
                if (message == null)
                {
                    // Client has closed the connection
                    break;
                }

                Answers answerObj;
                answerObj = JsonSerializer.Deserialize<Answers>(message);
                Console.WriteLine("Client sent: " + message);


                if (answerObj.Method == "Random")
                {
                    Random random = new Random();
                    answerObj.Result = random.Next(answerObj.Num1, answerObj.Num2 + 1);
                }
                else if (answerObj.Method == "Add")
                {
                    answerObj.Result = answerObj.Num1 + answerObj.Num2;
                }
                else if (answerObj.Method == "Subtract")
                {
                    answerObj.Result = answerObj.Num1 - answerObj.Num2;
                }
                else
                {
                    SendErrorMessage(writer, "Unsuported method");
                    break;
                }

                // Create a response object
                Answers response = new Answers
                {
                    Method = answerObj.Method,
                    Num1 = answerObj.Num1,
                    Num2 = answerObj.Num2,
                    Result = answerObj.Result
                };
                string jsonResponse = JsonSerializer.Serialize(response);
                writer.WriteLine(jsonResponse);
            }
            client.Close();
        }
        private static void SendErrorMessage(StreamWriter writer, string errorMessage)
        {
            var errorResponse = new
            {
                Error = errorMessage
            };
            string jsonErrorResponse = JsonSerializer.Serialize(errorResponse);
            writer.WriteLine(jsonErrorResponse);
        }







        //Answers answerObj;
        //        try
        //        {
        //            answerObj = JsonSerializer.Deserialize<Answers>(message);
        //            if (answerObj == null)
        //            {
        //                throw new JsonException("Deserialized object is null");
        //            }
        //        }
        //        catch (JsonException)
        //        {
        //            // Handle deserialization failure
        //            SendErrorMessage(writer, "Invalid JSON format");
        //            continue;
        //        }
    }
}
