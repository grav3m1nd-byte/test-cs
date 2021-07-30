using System.Data.SqlClient;

namespace WebApplication {
    public class Database {

        public static string IPAddress { get; } = @"WEB\WEBDB";
        public static string UserID { get; } = "webappusr";
        public static bool Encrypt { get; } = true;
        public static bool TrustServerCertificate { get; } = true;

        private static string Password { get; } = <OBFUSCATED>;

        public static string ConnectionString {
            get {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {
                    DataSource = IPAddress,
                    UserID = UserID,
                    Password = Password,
                    Encrypt = true,
                    TrustServerCertificate = true,
                    InitialCatalog = "webapp"
                };

                return builder.ConnectionString;
            }
        }
    }
}
