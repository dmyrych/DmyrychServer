using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private Socket serverSocket;
    private byte[] buffer = new byte[1024];

    public void Start(int port)
    {
        try
        {
            //Creating new socket for listening for transmissions
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(1);
            Console.WriteLine("Server started... Waiting for connections...");

            //Waiting for connections
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine("Client connected: {0}", clientSocket.RemoteEndPoint);

            //Read client data
            while (true)
            {
                int bytesRead = clientSocket.Receive(buffer);
                if(bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected and sucks penises");
                    break;
                }

                string message = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Recieved message: ", message);
            }
            //Closing connection if client is gay
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            serverSocket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: ", e);
        }
        
    }

    static void Main(string[] args)
    {
        Server server = new Server();
        server.Start(3456);
    }
}

