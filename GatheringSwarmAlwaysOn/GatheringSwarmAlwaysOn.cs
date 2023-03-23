using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace GatheringSwarmAlwaysOn
{
    public class GatheringSwarmAlwaysOn : Mod, ITogglableMod
    {
        internal static GatheringSwarmAlwaysOn instance;
        public const string EnabledBool = $"{nameof(GatheringSwarmAlwaysOn)}.Enabled";

        public GatheringSwarmAlwaysOn() : base(null)
        {
            instance = this;
        }
        
        public override string GetVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        
        public override void Initialize()
        {
            Log("Initializing Mod...");

            IL.GeoControl.OnEnable += ModifySwarmBool;
            ModHooks.GetPlayerBoolHook += InterpretSwarmBool;
        }

        public void Unload()
        {
            IL.GeoControl.OnEnable -= ModifySwarmBool;
            ModHooks.GetPlayerBoolHook -= InterpretSwarmBool;
        }

        private bool InterpretSwarmBool(string name, bool orig)
        {
            return orig || (name == EnabledBool);
        }

        private void ModifySwarmBool(ILContext il)
        {
            ILCursor cursor = new(il);

            while (cursor.TryGotoNext
            (
                i => i.MatchLdstr(nameof(PlayerData.equippedCharm_1)),
                i => (
                    i.MatchCallOrCallvirt<PlayerData>(nameof(PlayerData.GetBool))
                    || i.MatchCallOrCallvirt<GameManager>(nameof(GameManager.GetPlayerDataBool))
                    )
            ))
            {
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, EnabledBool);
            }
        }

        public override int LoadPriority()
        {
            return 1023;
        }
    }
}