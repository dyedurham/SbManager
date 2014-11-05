using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace SbManager
{
    public interface IConfig
    {
        string BusConnectionString { get; }
        string WebAppUrl { get; }
    }
    public class Config : IConfig
    {
        private static string _connectionString = null;
        public string BusConnectionString
        {
            get
            {
                if (_connectionString != null) return _connectionString;

                var connectionString = ConfigurationManager.ConnectionStrings["busConnectionString"].ConnectionString;

                if (new Regex("^[a-z]:|^//", RegexOptions.IgnoreCase).IsMatch(connectionString))
                    connectionString = File.ReadAllText(connectionString);

                _connectionString = connectionString;
                return _connectionString;
            }
        }
        public string WebAppUrl
        {
            get { return ConfigurationManager.AppSettings["WebAppUrl"]; }
        }
    }
}
