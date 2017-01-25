﻿using System;
using System.Net;
using System.Net.Sockets;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ForkedExecution.Logging;

namespace Jobbr.Server.ForkedExecution
{
    public class ConfigurationValidator: IConfigurationValidator
    {
        private static readonly ILog Logger = LogProvider.For<ConfigurationValidator>();

        public Type ConfigurationType { get; set; } = typeof(ForkedExecutionConfiguration);

        private static int NextFreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public bool Validate(object configuration)
        {
            var config = (ForkedExecutionConfiguration)configuration;

            if (config == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(config.BackendAddress))
            {
                // Fallback to automatic endpoint port
                Logger.Warn("There was no BackendAdress specified. Falling back to random port, which is not guaranteed to work in production scenarios");
                var port = NextFreeTcpPort();

                config.BackendAddress = $"http://localhost:{port}";
            }

            // TODO: Move this check to forked executor
            //if (this.configuration.JobRunnerExeResolver == null)
            //{
            //    throw new ArgumentException("You should set a runner-Executable which runs your jobs later!");
            //}

            // TODO: Discuss why this check was commented out.
            // TODO: Move this check to forked executor
            //var executableFullPath = Path.GetFullPath(this.configuration.JobRunnerExeResolver());

            //if (!File.Exists(executableFullPath))
            //{
            //    throw new ArgumentException(string.Format("The RunnerExecutable '{0}' cannot be found!", executableFullPath));
            //}

            return true;
        }
    }
}