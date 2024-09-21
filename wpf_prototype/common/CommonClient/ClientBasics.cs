using Makaretu.Dns;
using ServerInfo;
using System.Diagnostics;
using System.Net;

namespace CommonClient;

public static class ClientBasics
{
    private static readonly object TraceLock = new();
    private static IPAddress _serverIp = IPAddress.None;

    public static async Task ConnectToClient()
    {

    }

    private static async Task FindClient()
    {
        using var serviceDiscovery = new ServiceDiscovery();
        serviceDiscovery.ServiceInstanceDiscovered += ServiceDiscoveryOnServiceInstanceDiscovered;

        // Todo find out how to check if _serverIp is set and then unsubscribe event
        // TODO I think there's a better way to send a specific query and do it async so can cancel after so long
        // TODO There are exceptions being thrown, find out why
        serviceDiscovery.QueryAllServices();
    }

    private static void ServiceDiscoveryOnServiceInstanceDiscovered(object? sender, ServiceInstanceDiscoveryEventArgs e)
    {
        lock (TraceLock)
        {
            Trace.WriteLine($"Discovered endpoint: {e.ServiceInstanceName} - {e.RemoteEndPoint.Address}:{e.RemoteEndPoint.Port}");

            if (!e.ServiceInstanceName.Equals(ServerConstants.Name)) return;

            _serverIp = e.RemoteEndPoint.Address;
        }
    }


    public static async Task Test()
    {
        await FindClient();
    }
}
