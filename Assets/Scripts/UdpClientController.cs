using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class UdpClientController
{
    private UdpClient client;
    private IPEndPoint source;
    public string data;

    public UdpClientController(int port)
    {
        client = new UdpClient(port);
        source = new IPEndPoint(IPAddress.Any, port);
        data = "0,0,0,0,0,0";
        client.BeginReceive(Receiver, null);
    }

    // Check if client is connected
    public bool IsConnected()
    {
        return client.Client.Connected;
    }

    private void Receiver(IAsyncResult result)
    {
        // Get and parse the message received from the source
        byte[] messageBytes = client.EndReceive(result, ref source);
        data = System.Text.Encoding.UTF8.GetString(messageBytes);
        
        // Continue listening
        client.BeginReceive(Receiver, null);
    }

    private void SendMessageToClient(string message)
    {
        // Convert and send the response data to source
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        client.Send(messageBytes, messageBytes.Length, source);
    }

    void Close()
    {
        client.Close();
    }
}
