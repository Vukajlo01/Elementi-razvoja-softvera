﻿using Common_Class_Library.Implementations;
using DumpingBuffer_Component.Implementations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.Json;
using System.Threading;

namespace Dumping_Buffer_Component
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        static void Main(string[] args)
        {
            // .Net Remoting
            DumpingBuffer server = new DumpingBuffer();
            TcpChannel channel = new TcpChannel(8085);
            ChannelServices.RegisterChannel(channel, false);
            string uri = "DumpingBuffer";
            RemotingServices.Marshal(server, uri, server.GetType());

            Console.WriteLine("DUMPING BUFFER KOMPONENTA ZAPOCINJE SA RADOM");
            Console.WriteLine("Interaktivni rezim rada nije podrzan!");

            // na zatvaranje aplikacije cuvanje podataka
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((sender, e) => ConsoleExit(sender, e, server));

            // load data if exist
            LoadData(server);

            while (true)
            {
                Thread.Sleep(1000); //svake 1 sekunde prikazi poruku za kraj
                Console.WriteLine("\n[APP] Pritisnite 'q' za izlazak");
                string str = Console.ReadLine();

                if (str.Equals("q"))
                {
                    SaveData(server);
                    return;
                }
            }
        }

        private static void ConsoleExit(object sender, EventArgs e, DumpingBuffer server)
        {
            // save data on close
            SaveData(server);
        }

        // kada se zatvori dumping buffer preostali ne upisani podaci u bazi se cuvaju u fajl
        public static void LoadData(DumpingBuffer server)
        {
            server.LoadDataToQueue();
        }

        public static void SaveData(DumpingBuffer server)
        {
            server.SaveDataToJson();
        }
    }
}
