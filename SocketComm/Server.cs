using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SocketComm
{
    public class Server
    {
        StreamSocket ServerSocket;
        HostName localHostName;
        StreamSocketListener listener;

        string hostname = "192.168.0.2";
        string port = "15000";

        public Server()
        {

        }

        ~Server()
        {
            ServerSocket.Dispose();
        }

        public async Task InitServer()
        {
            localHostName = new HostName(hostname);
            listener = new StreamSocketListener();
            listener.ConnectionReceived += (ss, ee) =>
            {
                ServerSocket = ee.Socket;
            };


            await listener.BindEndpointAsync(localHostName, port);
        }

        public async Task SendMessage()
        {
            string str = "hoge";
            using (var writer = new DataWriter(ServerSocket.OutputStream))
            {
                writer.WriteUInt32(writer.MeasureString(str));
                writer.WriteString(str);
                await writer.StoreAsync();
            }
        }

        public async Task ReceiveMessage()
        {
            string str = string.Empty;

            using (var reader = new DataReader(ServerSocket.InputStream))
            {
                uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                uint size = reader.ReadUInt32();
                uint sizeFieldCount2 = await reader.LoadAsync(size);
                str = reader.ReadString(sizeFieldCount2);
            }
        }
    }
}
