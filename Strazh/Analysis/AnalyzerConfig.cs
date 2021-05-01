namespace Strazh.Analysis
{
    public class AnalyzerConfig
    {
        public class CredentialsConfig
        {
            public string Database { get; }
            public string User { get; }
            public string Password { get; }

            public CredentialsConfig(string credentials)
            {
                if (!string.IsNullOrEmpty(credentials))
                {
                    var args = credentials.Split(":");
                    if (args.Length == 3)
                    {
                        Database = args[0];
                        User = args[1];
                        Password = args[2];
                    }
                }
            }
        }

        public enum AnalyzeMode : int
        {
            All = 0,
            Structure = 1,
            Code = 2
        }

        public CredentialsConfig Credentials { get; }
        public AnalyzeMode Mode { get; }
        public string Solution { get; }
        public string[] Projects { get; }
        public bool IsDelete { get; }

        public bool IsSolutionBased => !string.IsNullOrEmpty(Solution);

        public bool IsValid => (!string.IsNullOrEmpty(Solution) && Projects.Length == 0)
            || (string.IsNullOrEmpty(Solution) && Projects.Length > 0);

        public AnalyzerConfig(string credentials, string mode, string delete, string solution, string[] projects)
        {
            solution = solution == "none" ? "" : solution;
            Credentials = new CredentialsConfig(credentials);
            Mode = MapMode(mode);
            IsDelete = delete != "false";
            Solution = solution;
            Projects = projects ?? new string[] { };
        }

        private AnalyzeMode MapMode(string mode)
            => (mode ?? "").ToLowerInvariant() switch
                {
                    "structure" => AnalyzeMode.Structure,
                    "code" => AnalyzeMode.Code,
                    _ => AnalyzeMode.All,
                };
    }
}