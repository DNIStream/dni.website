using System;
using System.IO;
using System.Linq;

namespace DNI.Testing {
    /// <summary>
    ///     Contains static helper methods for testing
    /// </summary>
    public static class TestHelpers {
        /// <summary>
        ///     Used in tests to secure API keys. Retrieves the string value of the specified key in a key / value pair file named
        ///     'keys.cfg'. keys.cfg should not be checked in to the repository. File should contain a simple KVP list in the
        ///     format KEY=VALUE i.e. YOUTUBE=MYREALAPIKEY
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyValue(string key) {
            var contents = File.ReadAllText("keys.cfg");
            var lines = contents.Split(new[] {"/r/n"}, StringSplitOptions.RemoveEmptyEntries);
            var line = lines.FirstOrDefault(x => x.StartsWith(key));
            if(line == null) {
                throw new ArgumentOutOfRangeException(nameof(key), $"'{key}' does not exist in 'keys.cfg'");
            }

            var value = line.Substring(line.IndexOf('=') + 1);
            return value;
        }
    }
}