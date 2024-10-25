using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS06118 // Type or member is obsolete

namespace SylmarDev.RoR2BossRush
{
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)]

    public class Main : BaseUnityPlugin
    {
        public const string PluginAuthor = "SylmarDev";
        public const string PluginName = "FalseSonPortal";
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginVersion = "1.0.0";

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);

            Log.LogInfo($"{PluginGUID} // ver {PluginVersion}");

            Log.LogInfo("Assigning hooks. . .");

            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += SpawnOutroPortal_Hook;

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        private void SpawnOutroPortal_Hook(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, VoidRaidGauntletController self)
        {
            orig(self);

            // spawn Prime Meridian portal
            if (NetworkServer.active && self.currentDonut != null && self.currentDonut.returnPoint)
            {
                Xoroshiro128Plus rng = new Xoroshiro128Plus(self.rngSeed + 1UL);
                DirectorPlacementRule placementRule = new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    minDistance = self.minOutroPortalDistance,
                    maxDistance = self.maxOutroPortalDistance,
                    spawnOnTarget = self.currentDonut.returnPoint
                };

                SpawnCard primeMeridianPortalCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC2/iscColossusPortal.asset").WaitForCompletion();

                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(primeMeridianPortalCard, placementRule, rng);
                DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

                // force portal to take you to Prime Meridian so you don't have to seek the whole storm
                Stage.instance.sceneDef.stageOrder = 3;
            }
        }

        private void Update()
        {

        }
    }
}