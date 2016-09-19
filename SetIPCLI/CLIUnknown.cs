﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetIPLib;

namespace SetIPCLI {

    /// <summary>
    /// A default, non-null CLI command class.  Should be created when a command line argument is unknonw.
    /// </summary>
    class CLIUnknown : ICLICommand {
        public ArgumentGroup Arguments { get; }
        public CLIUnknown(ArgumentGroup args) {
            Arguments = args;
        }


        /// <summary>
        /// Always throws an UnknownCommandException
        /// </summary>
        /// <param name="store"></param>
        public void Execute(ref IProfileStore store) {
            throw new UnknownCommandException(Arguments.Command);
        }
    }


}
