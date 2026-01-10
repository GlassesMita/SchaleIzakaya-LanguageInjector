using System;

namespace SchaleIzakaya.LanguageInjector.Models
{
    public class ModInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public uint VersionCode { get; set; }
        public string Author { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ModLink { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        
        public bool IsDisabled => FileName.EndsWith(".disabled");
    }
}
