public string GetLocalIPv4()
{
return Dns.GetHostEntry(Dns.GetHostName())
.AddressList.First(
f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
.ToString();
}
