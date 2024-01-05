using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


/// <summary>
/// Represents a UDP client controller that can send and receive messages over UDP.
/// </summary>
public class UdpClientController
{
    private UdpClient client;
    private IPEndPoint source;
    public string data;

    /// <summary>
    /// Initializes a new instance of the <see cref="UdpClientController"/> class with the specified port number.
    /// </summary>
    /// <param name="port">The port number to listen on.</param>
    public UdpClientController(int port)
    {
        client = new UdpClient(port);
        source = new IPEndPoint(IPAddress.Any, port);
        data = "0,0,0,0,0,0";
        client.BeginReceive(Receiver, null);
    }

    /// <summary>
    /// Checks if the client is connected.
    /// </summary>
    /// <returns><c>true</c> if the client is connected; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Sends the specified message to the client.
    /// </summary>
    /// <param name="message">The message to send.</param>
    private void SendMessageToClient(string message)
    {
        // Convert and send the response data to source
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        client.Send(messageBytes, messageBytes.Length, source);
    }

    /// <summary>
    /// Closes the UDP client.
    /// </summary>
    public void Close()
    {
        client.Close();
    }
}
