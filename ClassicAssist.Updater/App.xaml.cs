﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using ClassicAssist.Shared;
using ClassicAssist.Updater.Properties;
using CommandLine;
using Exceptionless;
using IOPath = System.IO.Path;

namespace ClassicAssist.Updater
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static CommandLineOptions CurrentOptions { get; set; }

        public static UpdaterSettings UpdaterSettings { get; set; }

        private void Application_Startup( object sender, StartupEventArgs e )
        {
            ExceptionlessClient.Default.Configuration.DefaultData.Add( "Locale",
                Thread.CurrentThread.CurrentUICulture.Name );
            ExceptionlessClient.Default.Startup( Settings.Default.ExceptionlessKey );

            Parser.Default.ParseArguments<CommandLineOptions>( e.Args ).WithParsed( o => CurrentOptions = o );

            if ( string.IsNullOrEmpty( CurrentOptions.Path ) )
            {
                CurrentOptions.Path = Environment.CurrentDirectory;
            }

            UpdaterSettings = UpdaterSettings.Load( CurrentOptions.Path ?? Environment.CurrentDirectory );
            Exit += ( s, ea ) =>
                UpdaterSettings.Save( UpdaterSettings, CurrentOptions.Path ?? Environment.CurrentDirectory );

            if ( CurrentOptions.CurrentVersion != null )
            {
                return;
            }

            if ( File.Exists( IOPath.Combine( CurrentOptions.Path, "ClassicAssist.dll" ) ) )
            {
                if ( Version.TryParse(
                    FileVersionInfo.GetVersionInfo( IOPath.Combine( CurrentOptions.Path, "ClassicAssist.dll" ) )
                        .ProductVersion, out Version version ) )
                {
                    CurrentOptions.CurrentVersion = version;
                }
            }
            else
            {
                CurrentOptions.CurrentVersion = new Version( 0, 0, 0, 0 );
            }
        }
    }
}