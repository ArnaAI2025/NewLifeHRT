namespace NewLifeHRT.Infrastructure.Settings

{
    public class SecuritySettings
    {
        public bool UseEncryption { get; set; } = false;
        public string[] ExcludedPaths { get; set; } = new[] { "/swagger" };
        public string Key { get; set; } = string.Empty; // 32-byte key
        public string IV { get; set; } = string.Empty;  // 16-byte IV
    }


}