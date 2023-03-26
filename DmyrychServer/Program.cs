using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


public class Database
{
    private MySqlConnection connection;

    public Database(string connectionString)
    {
        connection = new MySqlConnection(connectionString + ";database=ServerDb");
    }

    public void Open()
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public void Close()
    {
        if (connection.State != ConnectionState.Closed)
        {
            connection.Close();
        }
    }

    public DataTable GetUsers()
    {
        Open();
        DataTable dataTable = new DataTable();
        using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM users", connection))
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(dataTable);
            }
        }
        Close();
        return dataTable;
    }
}
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

            Task.Run(async () =>
            {
                while (true)
                {
                    Socket clientSocket = null;
                    try
                    {
                        //Waiting for connections
                        clientSocket = await serverSocket.AcceptAsync();
                        Console.WriteLine("Client connected: {0}", clientSocket.RemoteEndPoint);

                        //Read client data
                        while (true)
                        {
                            int bytesRead = clientSocket.Receive(buffer);
                            if (bytesRead == 0)
                            {
                                Console.WriteLine("Client disconnected: {0}", clientSocket.RemoteEndPoint);
                                clientSocket.Shutdown(SocketShutdown.Both);
                                clientSocket.Close();
                                break;
                            }

                            string message = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                            Console.WriteLine("Received message from {0}: {1}", clientSocket.RemoteEndPoint, message);
                        }
                    }
                    catch (SocketException e)
                    {
                        //Console.WriteLine("Socket exception: {0}", e);
                        if (clientSocket != null)
                        {
                            Console.WriteLine("Client disconnected: {0}", clientSocket.RemoteEndPoint);
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: {0}", e);
                    }
                }
            });
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e.ToString());
        }
    }

    static void Main(string[] args)
    {
        Server server = new Server();
        server.Start(29875);
        Console.ReadLine();
    }
}

