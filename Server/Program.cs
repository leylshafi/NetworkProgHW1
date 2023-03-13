using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

var listener = new Socket(AddressFamily.InterNetwork,
    SocketType.Dgram,
    ProtocolType.Udp);

var ip = IPAddress.Parse("127.0.0.1");
var port = 45678;

var buffer = new byte[ushort.MaxValue - 29];
var ep = new IPEndPoint(ip, port);
listener.Bind(ep);
EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);


while (true)
{
    var result = await listener.ReceiveFromAsync(buffer, SocketFlags.None, remoteEp);
    var img = TakeScreenShot();
    var imgBuffer = ImageToByte(img);
    Console.WriteLine(imgBuffer.Length);
    var chunk = imgBuffer.Chunk(ushort.MaxValue - 29);
    var newBuffer = chunk.ToArray();

    for (int i = 0; i < newBuffer.Length; i++)
    {
        await Task.Delay(50);
        await listener.SendToAsync(newBuffer[i], SocketFlags.None, result.RemoteEndPoint);
    }
}


byte[] ImageToByte(Image img)
{
    using (MemoryStream ms = new MemoryStream())
    {
        img.Save(ms, ImageFormat.Png);
        byte[] imageBytes = ms.ToArray();
        return imageBytes;
    }
}

Image TakeScreenShot()
{
    Rectangle bounds = Screen.PrimaryScreen.Bounds;

    Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

    using (Graphics graphics = Graphics.FromImage(bitmap))
    {
        graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
    }

    return bitmap;
}
