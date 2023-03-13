using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;

namespace Client;

public partial class MainWindow : Window
{
    private Socket client;
    private EndPoint remoteEP;
    public MainWindow()
    {
        InitializeComponent();
        client = new Socket(AddressFamily.InterNetwork, 
                            SocketType.Dgram, 
                            ProtocolType.Udp);


        var ip = IPAddress.Parse("127.0.0.1");
        var port = 45678;
        remoteEP = new IPEndPoint(ip, port);
    }

    private async void btnRequest_Click(object sender, RoutedEventArgs e)
    {
        var buffer = new byte[ushort.MaxValue - 29];
        await client.SendToAsync(buffer, SocketFlags.None, remoteEP);
        var list = new List<byte>();
        var len = 0;
        do
        {
            var result = await client.ReceiveFromAsync(buffer, SocketFlags.None, remoteEP);
            len = result.ReceivedBytes;

            list.AddRange(buffer.Take(len));

            try
            {
                var image = LoadImage(list.ToArray());
                if (image != null)
                    Image.Source = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }while (len == buffer.Length);

    }
    private static BitmapImage? LoadImage(byte[] imageData)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = new MemoryStream(imageData);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }

}
