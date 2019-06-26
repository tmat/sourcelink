// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Text;

// Implementation based on https://www.freebsd.org/cgi/man.cgi?ssh_config(5)

namespace Microsoft.Build.Tasks.Git
{
    internal sealed class SshConfig
    {
        public static readonly SshConfig Empty = new SshConfig(ImmutableDictionary<string, string>.Empty);

        /// <summary>
        /// Maps host alias to host name (domain name or IP address).
        /// </summary>
        public ImmutableDictionary<string, string> HostNames { get; }

        private SshConfig(ImmutableDictionary<string, string> hostNames)
        {
            Debug.Assert(hostNames != null);
            HostNames = hostNames;
        }

        internal sealed class Reader
        {
            // reused for parsing names
            private readonly StringBuilder _reusableBuffer = new StringBuilder();

            private readonly GitEnvironment _environment;

            public Reader(GitEnvironment environment)
            {
                Debug.Assert(environment != null);
                _environment = environment;
            }

            private IEnumerable<string> EnumerateConfigurationFiles()
            {
                if (_environment.SystemDirectory != null)
                {
                    yield return Path.Combine(_environment.SystemDirectory, "ssh", "ssh_config");
                }

                yield return Path.Combine(_environment.HomeDirectory, ".ssh", "config");
            }

            public SshConfig Load()
            {
                var hostNames = new Dictionary<string, string>();

                foreach (var path in EnumerateConfigurationFiles())
                {
                    LoadHostNamesFromFile(path, hostNames);
                }

                return new SshConfig(hostNames.ToImmutableDictionary());
            }

            private void LoadHostNamesFromFile(string path, Dictionary<string, string> hostNames)
            {
                TextReader reader;

                if (!File.Exists(path))
                {
                    return;
                }

                try
                {
                    reader = File.OpenText(path);
                }
                catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
                {
                    return;
                }
                catch (Exception e) when (!(e is IOException))
                {
                    throw new IOException(e.Message, e);
                }

                using (reader)
                {
                    LoadHostNamesFromFile(reader, hostNames);
                }
            }

            // internal for testing
            internal void LoadHostNamesFromFile(TextReader reader, Dictionary<string, string> hostNames)
            {
                while (true)
                {
                    

                }
            }

        }
    }
}
