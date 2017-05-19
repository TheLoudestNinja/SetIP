﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SetIPLib {


    /// <summary>
    /// Allows the storage and retrieval of Profiles to a file as XML.
    /// </summary>
    /// 
    public class ProfileFileStore : IProfileStore {

        public string FilePath { get; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SetIP\\profiles.xml");

        public IProfileEncoder Encoder { get; } = new XMLProfileEncoder();

        public IEnumerable<Profile> Retrieve() {
            byte[] contents;
            ValidateFile();
            using (System.IO.FileStream fs = new System.IO.FileStream(FilePath, System.IO.FileMode.OpenOrCreate)) {
                contents = new byte[fs.Length];
                if (fs.Length < int.MaxValue) {
                    fs.Read(contents, 0, (int)fs.Length);
                }
            }
            return Encoder.Decode(contents);
        }

        protected void ValidateFile() {
            if (!System.IO.Directory.Exists(FilePath)) {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath));
            }
        }

        private void CheckDirectory() {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        }

        public void Store(IEnumerable<Profile> profiles) {
            using (System.IO.FileStream fs = new System.IO.FileStream(FilePath, System.IO.FileMode.Create)) {
                fs.Write(Encoder.Header, 0, Encoder.Header.Length);

                foreach (var p in profiles) {
                    byte[] pBytes = Encoder.Encode(p);
                    fs.Write(pBytes, 0, pBytes.Length);
                }
                fs.Write(Encoder.Footer, 0, Encoder.Footer.Length);
            }
        }

        /// <summary>
        /// Uses the default appdata path to store profiles with 
        /// the default XML Profile encoder.
        /// </summary>
        public ProfileFileStore() {
        }

        /// <summary>
        /// Uses the default XML profile encoder to store profiles
        /// at the specified path
        /// </summary>
        /// <param name="filePath">Full path to file, including file name.</param>
        public ProfileFileStore(string filePath) {
            FilePath = filePath;
        }

        /// <summary>
        /// Stores profiles at the specified file path using the
        /// provided encoder.
        /// </summary>
        /// <param name="filePath">Full path to file, including file name.</param>
        /// <param name="encoder">Encoder used to write profile information to the specified file.</param>
        public ProfileFileStore(string filePath, IProfileEncoder encoder) {
            FilePath = filePath;
            Encoder = encoder;
        }

    }
}
