using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using System;
using UnityEngine;

[DefaultExecutionOrder(-100)] // run _before_ Mirror’s own scripts
public class MyNetworkManager : NetworkManager
{
    public KcpTransport kcp;
    public SimpleWebTransport web;
    public bool AlwaysConnectToWebServer;

    public override void Awake()
    {
        // 1) assign transport _before_ any StartClient/StartServer
#if UNITY_SERVER
            transport      = web;
            networkAddress = "games.guk.ca";
            base.Awake();
            StartServer();
#elif UNITY_EDITOR
        if (AlwaysConnectToWebServer)
        {
            transport = web;
            networkAddress = "games.guk.ca";
            base.Awake();
            StartClient();
        }
        else
        {
            transport = kcp;
            networkAddress = "localhost";
            base.Awake();
            
        }



#elif UNITY_WEBGL
       transport = web;
       networkAddress = "games.guk.ca";
       web.clientWebsocketSettings.ClientPortOption = WebsocketPortOption.SpecifyPort;
       web.clientWebsocketSettings.CustomClientPort = 443;
       base.Awake();
       StartClient();

#else
            base.Awake();
#endif
    }

    public override void Start()
    {
        base.Start();
    }
}
