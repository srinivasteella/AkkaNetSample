using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AkkaActor
{
    internal class startup
    {
        public static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        public static IConfigurationRoot configuration = builder.Build();

        internal static void StartApp()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = configuration.GetSection("AppTitle").Value;

            var config = ConfigurationFactory.ParseString(File.ReadAllText("Actor.hocon"));

            var ActorManagerSystem = ActorSystem.Create(configuration.GetSection("AppName").Value, config);
            ActorManagerSystem.ActorOf<ActorManager>("ActorManager");
            Console.Clear();

            Console.WriteLine($"{Console.Title} is STARTED");
            Console.ReadLine();
        }
    }
}
