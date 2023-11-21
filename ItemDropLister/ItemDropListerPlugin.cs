using BepInEx;
using Jotunn;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;

namespace ItemDropLister
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Main.ModGuid)]
    internal class ItemDropListerPlugin : BaseUnityPlugin
    {
        private const string PluginAuthor = "FixItFelix";
        private const string PluginName = "ItemDropLister";
        internal const string PluginVersion = "1.0.8";
        internal const string PluginGuid = PluginAuthor + "." + PluginName;

        private void Awake()
        {
            ModQuery.Enable();
            PrefabManager.OnPrefabsRegistered += ItemDropRegistry.Initialize;
            CommandManager.Instance.AddConsoleCommand(new TranslationsPrinterController());
        }

        private class TranslationsPrinterController : ConsoleCommand
        {
            public override void Run(string[] args)
            {
                ItemDropPrinter.WriteData(args.Length > 0 ? args[0] : "");
            }

            public override string Name => "print_items_to_file";

            public override string Help =>
                "Write all ItemDrop prefabs loaded in-game into a YAML file inside the " +
                "BepInEx root folder 'BepInEx/Debug'";
        }
    }
}