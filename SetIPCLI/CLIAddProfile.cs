﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetIPLib;
using System.Net;

namespace SetIPCLI {

    /// <summary>
    /// Expected CLI syntax:
    ///   -a "Profile name" dhcp
    ///   -add "Other name" static 192.168.1.1 255.255.255.0
    /// </summary>
    class CLIAddProfile : ICLICommand {
        private enum ExpectedParameter {
            Name,
            Source,
            IP,
            Sub,
            Gateway,
            DNS,
            None
        }

        /// <summary>
        /// Arguments that will be parsed to generate a new profile.
        /// </summary>
        public ArgumentGroup Arguments { get; }

        public CLIAddProfile(ArgumentGroup args) {
            Arguments = args;
        }


        public void Execute(ref IProfileStore store) {
            var currentProfiles = store?.Retrieve().ToList();
            ExpectedParameter nextParm = ExpectedParameter.Name;

            string name = string.Empty;
            bool UseDHCP = true;
            IPAddress ip = IPAddress.Any;
            IPAddress sub = IPAddress.Any;
            IPAddress gateway = IPAddress.None;
            List<IPAddress> DNSServers = new List<IPAddress>();
            foreach (var parm in Arguments.Arguments) {
                switch (nextParm) {
                    case ExpectedParameter.Name:
                        name = parm;
                        nextParm = ExpectedParameter.Source;
                        break;
                    case ExpectedParameter.Source:
                        if (parm.ToUpper() == "DHCP") {
                            UseDHCP = true;
                            nextParm = ExpectedParameter.None;
                        }
                        else {
                            UseDHCP = false;
                            ip = IPAddress.Parse(parm);
                            nextParm = ExpectedParameter.Sub;
                        }
                        break;
                    case ExpectedParameter.IP:
                        ip = IPAddress.Parse(parm);
                        nextParm = ExpectedParameter.Sub;
                        break;
                    case ExpectedParameter.Sub:
                        sub = IPAddress.Parse(parm);
                        nextParm = ExpectedParameter.Gateway;
                        break;
                    case ExpectedParameter.Gateway:
                        gateway = IPAddress.Parse(parm);
                        nextParm = ExpectedParameter.DNS;
                        break;
                    case ExpectedParameter.DNS:
                        DNSServers.Add(IPAddress.Parse(parm));
                        nextParm = ExpectedParameter.DNS;
                        break;
                    case ExpectedParameter.None:
                        break;
                }
                if (nextParm == ExpectedParameter.None) {
                    break;
                }
            }
            if (UseDHCP) {
                currentProfiles?.Add(new Profile(name));
            }
            else {
                if (gateway == IPAddress.None) {
                    currentProfiles?.Add(new Profile(name, ip, sub));
                }
                else {
                    if (DNSServers.Count > 0) {
                        currentProfiles?.Add(new Profile(name, ip, sub, gateway, DNSServers));
                    }
                    else {
                        currentProfiles?.Add(new Profile(name, ip, sub, gateway));
                    }
                }
            }

            store?.Store(currentProfiles);
        }

        public string Help() {
            return "Usage: setipcli -a \"Profile Name\" ip-address subnet-mask [default-gateway] [[dns-1] [dns-2]...]\n" +
                   "  - All IP addresses are decimal-dot notation (111.111.111.111)\n" +
                   "  - Only IPv4 addresses are supported\n" +
                   "  - Items listed in brackets [] are optional\n" +
                   "  - multiple DNS servers can be specified";
        }
    }
}
