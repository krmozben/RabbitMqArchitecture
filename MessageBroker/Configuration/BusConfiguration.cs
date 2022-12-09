﻿namespace MessageBroker.Configuration;
public class BusConfiguration
{
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VHost { get; set; } = "/";
}
