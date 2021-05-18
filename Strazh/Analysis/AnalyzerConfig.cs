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

        public enum Tiers : int
        {
            All = 0,
            Project = 1,
            Code = 2
        }

        public CredentialsConfig Credentials { get; }
        public Tiers Tier { get; }
        public string Solution { get; }
        public string[] Projects { get; }
        public bool IsDelete { get; }

        public bool IsSolutionBased => !string.IsNullOrEmpty(Solution);

        public bool IsValid => (!string.IsNullOrEmpty(Solution) && Projects.Length == 0)
            || (string.IsNullOrEmpty(Solution) && Projects.Length > 0);

        public AnalyzerConfig(string credentials, string tier, string delete, string solution, string[] projects)
        {
            solution = solution == "none" ? "" : solution;
            Credentials = new CredentialsConfig(credentials);
            Tier = MapTier(tier);
            IsDelete = delete != "false";
            Solution = solution;
            Projects = projects ?? new string[] { };
        }

        private Tiers MapTier(string mode)
            => (mode ?? "").ToLowerInvariant() switch
                {
                    "project" => Tiers.Project,
                    "code" => Tiers.Code,
                    _ => Tiers.All,
                };
    }
}