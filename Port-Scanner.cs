using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

class PortScanner
{
    static string GetCountryInfo(string lazIpAddress)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"http://ip-api.com/json/{lazIpAddress}";
                string response = client.GetStringAsync(apiUrl).Result;
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                if (data.status == "success")
                {
                    return $"Country: {data.country}, Region: {data.regionName}, City: {data.city}";
                }
                else
                {
                    return "Failed to get VPS information.";
                }
            }
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    static async Task SendToWebhook(string lazWebhookUrl, string messageContent)
    {
        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent($"{{\"content\":\"{messageContent}\"}}");
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/json");
            await client.PostAsync(lazWebhookUrl, content);
        }
    }

    static string GetServiceName(int lazPort)
    {
        try
        {
            var ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnections = ipGlobalProperties.GetActiveTcpConnections();

            var connection = tcpConnections.FirstOrDefault(conn => conn.LocalEndPoint.Port == lazPort);

            if (connection != null)
            {
                return $"Port {lazPort} ({connection.LocalEndPoint.Port})";
            }
            else
            {
                return $"Port {lazPort} (Unknown)";
            }
        }
        catch (Exception)
        {
            return $"Port {lazPort} (Unknown)";
        }
    }

    static async Task ScanAllPorts(string lazIpAddress, string lazWebhookUrl, string outputFilePath)
    {
        string countryInfo = GetCountryInfo(lazIpAddress);

        using (StreamWriter outputFile = new StreamWriter(outputFilePath))
        {
            for (int port = 1; port <= 65535; port++)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.ReceiveTimeout = 1000;
                    try
                    {
                        tcpClient.Connect(lazIpAddress, port);
                        string serviceName = GetServiceName(port);
                        string link = $"http://{lazIpAddress}:{port}";

                        string messageContent = $"{countryInfo}\n\nPort: {port} is open. Service: {serviceName}, Link: {link}";
                        await SendToWebhook(lazWebhookUrl, messageContent);

                        // Save information to the text file
                        outputFile.WriteLine(messageContent);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Port {port} is not open.");
                    }
                }
            }
        }
    }

    static async Task Main()
    {
        Console.WriteLine(@"
        01010110 01010000 01010011  01010000 01101111 01110010 01110100  01010011 01100011 01100001 01101110 01101110 01100101 01110010 
        ██╗   ██╗██████╗ ███████╗    ██████╗  ██████╗ ██████╗ ████████╗    ███████╗ ██████╗ █████╗ ███╗   ██╗███╗   ██╗███████╗██████╗ 
        ██║   ██║██╔══██╗██╔════╝    ██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝    ██╔════╝██╔════╝██╔══██╗████╗  ██║████╗  ██║██╔════╝██╔══██╗
        ██║   ██║██████╔╝███████╗    ██████╔╝██║   ██║██████╔╝   ██║       ███████╗██║     ███████║██╔██╗ ██║██╔██╗ ██║█████╗  ██████╔╝
        ╚██╗ ██╔╝██╔═══╝ ╚════██║    ██╔═══╝ ██║   ██║██╔══██╗   ██║       ╚════██║██║     ██╔══██║██║╚██╗██║██║╚██╗██║██╔══╝  ██╔══██╗
         ╚████╔╝ ██║     ███████║    ██║     ╚██████╔╝██║  ██║   ██║       ███████║╚██████╗██║  ██║██║ ╚████║██║ ╚████║███████╗██║  ██║
          ╚═══╝  ╚═╝     ╚══════╝    ╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝       ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝
          Nice tool for scan vps port by @lfillaz github                                                                                                                             
        ");

        Console.Write("Please enter the VPS Host (example: 103.101.203.43): ");
        string lazIpAddress = Console.ReadLine();
        Console.Write("Please enter your Discord Webhook URL: ");
        string lazWebhookUrl = Console.ReadLine();
        Console.Write("Please enter the output file path (example: output.txt): ");
        string outputFilePath = Console.ReadLine();

        await ScanAllPorts(lazIpAddress, lazWebhookUrl, outputFilePath);
    }
}
