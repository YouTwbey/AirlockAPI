using AirlockAPI.Data;
using AirlockAPI.Main;
using MelonLoader;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: MelonInfo(typeof(Loader), Info.Name, Info.Version, Info.Developer, Info.DownloadLink)]
[assembly: MelonGame("Schell Games", null)]
[assembly: MelonColor(Info.ColorA, Info.ColorR, Info.ColorG, Info.ColorB)]

[assembly: AssemblyTitle("AirlockAPI")]
[assembly: AssemblyDescription("Modding API for Among Us 3D/VR")]
[assembly: AssemblyProduct("AirlockAPI")]
[assembly: AssemblyCopyright("Copyright ©  2025")]
[assembly: ComVisible(false)]
[assembly: Guid("5f9af3a0-9611-47fa-95e6-c2d57052dedc")]
[assembly: AssemblyVersion(Info.Version + ".0")]
[assembly: AssemblyFileVersion(Info.Version + ".0")]
