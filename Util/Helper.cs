namespace api_ai_rag_intent.Util
{
    internal static class Helper
    {
        public static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process) ?? "";
        }

        public static string GetSqlConnectionString(string name)
        {
            var conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable($"SQLCONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr ?? "";
        }

        public static string GetSqlAzureConnectionString(string name)
        {
            var conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr ?? "";
        }

        public static string GetMySqlConnectionString(string name)
        {
            var conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable($"MYSQLCONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr ?? "";
        }

        public static string GetCustomConnectionString(string name)
        {
            var conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr ?? "";
        }
    }
}