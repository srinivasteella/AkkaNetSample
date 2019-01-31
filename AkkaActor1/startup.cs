using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;


namespace AkkaActor1
{
    internal class startup
    {
        public static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        public static IConfigurationRoot configuration = builder.Build();

        internal static void StartApp()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Title = configuration.GetSection("AppName").Value;

            var config = ConfigurationFactory.ParseString(File.ReadAllText("actor1.hocon"));

            var ActorManagerSystem = ActorSystem.Create(Console.Title, config);

            var Actor1Manager = ActorManagerSystem.ActorOf<Actor1Manager>("Actor1Manager");

            Console.Clear();
            Console.WriteLine($"{Console.Title} is STARTED");

            Actor1Manager.Tell("Hello");

            Console.ReadLine();
        }
    }
}
