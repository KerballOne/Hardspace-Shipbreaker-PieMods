using HarmonyLib;
using System;
using System.Diagnostics;
using System.Linq;

namespace ModdedShipLoader
{
    // The game's jointing step creates 24 workers, and each one reserves two lists sized
    // largestMeshVertexCount * numParts (JointHelper.JointingWorkerData constructor). On a large
    // ship that is tens of GB of reserved memory (out of memory on 16 GB machines), and past
    // 2^27 elements the size overflows and the game crashes on any machine.
    // Those lists only ever hold the joint polygons that were found and they grow on demand, so
    // a small starting size is enough. Only numParts is clamped: the buffers sized by
    // largestVertexCount alone silently drop points when full and must keep their size.
    internal static class JointMemoryFix
    {
        const int MaxInitialParts = 16;

        // Workers created and not yet disposed. Jointing of one ship runs from the first
        // constructor to the last Dispose.
        static int liveWorkers;
        static readonly Stopwatch jointingTimer = new Stopwatch();

        // Patched by hand, not through PatchAll: a failure here must not stop the other patches.
        public static void Apply(Harmony harmony)
        {
            Type workerType = null;
            try
            {
                workerType = AccessTools.Inner(AccessTools.TypeByName("BBI.Unity.Game.JointHelper"), "JointingWorkerData");
                var constructor = workerType == null ? null : AccessTools.GetDeclaredConstructors(workerType)
                    .FirstOrDefault(c => !c.IsStatic
                        && c.GetParameters().Any(p => p.Name == "numParts" && p.ParameterType == typeof(int))
                        && c.GetParameters().Any(p => p.Name == "largestVertexCount" && p.ParameterType == typeof(int)));

                if (constructor == null)
                {
                    ModdedShipLoader.LoggerInstance.LogWarning("JointMemoryFix: JointingWorkerData constructor not found, jointing memory is not reduced");
                    return;
                }

                harmony.Patch(constructor, prefix: new HarmonyMethod(typeof(JointMemoryFix), nameof(ConstructorPrefix)));
                ModdedShipLoader.LoggerInstance.LogInfo("JointMemoryFix: patched");
            }
            catch (Exception ex)
            {
                ModdedShipLoader.LoggerInstance.LogWarning($"JointMemoryFix: not applied, jointing memory is not reduced ({ex.Message})");
                return;
            }

            // Reporting only: the end of jointing.
            try
            {
                var dispose = AccessTools.DeclaredMethod(workerType, "Dispose", Type.EmptyTypes);
                if (dispose != null)
                    harmony.Patch(dispose, postfix: new HarmonyMethod(typeof(JointMemoryFix), nameof(DisposePostfix)));
            }
            catch (Exception ex)
            {
                ModdedShipLoader.LoggerInstance.LogWarning($"JointMemoryFix: end-of-jointing report not available ({ex.Message})");
            }
        }

        static void ConstructorPrefix(int largestVertexCount, ref int numParts)
        {
            if (liveWorkers++ == 0)
            {
                try
                {
                    jointingTimer.Restart();

                    // What the unpatched game would reserve: 24 workers x (8 + 4 bytes) x the element
                    // count rounded up to a power of two.
                    long elements = (long)largestVertexCount * numParts;
                    long capacity = 1;
                    while (capacity < elements) capacity <<= 1;
                    double unpatchedGb = 24.0 * 12.0 * capacity / (1024.0 * 1024.0 * 1024.0);
                    string overflow = capacity * 8 > int.MaxValue ? " (over the game's size limit: crash on any machine)" : "";

                    SystemDiagnostics.Log($"Jointing start: {numParts} parts, largest mesh {largestVertexCount} vertices, unpatched reservation about {unpatchedGb.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} GB{overflow}, initial list size limited to {Math.Min(numParts, MaxInitialParts)} parts");
                    SystemDiagnostics.LogMemory("Before jointing");
                }
                catch (Exception ex)
                {
                    ModdedShipLoader.LoggerInstance.LogWarning($"JointMemoryFix: start report failed ({ex.Message})");
                }
            }

            if (numParts > MaxInitialParts) numParts = MaxInitialParts;
        }

        static void DisposePostfix()
        {
            if (liveWorkers <= 0 || --liveWorkers > 0) return;

            try
            {
                jointingTimer.Stop();
                SystemDiagnostics.LogMemory($"Jointing done in {jointingTimer.Elapsed.TotalSeconds.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} s");
            }
            catch (Exception ex)
            {
                ModdedShipLoader.LoggerInstance.LogWarning($"JointMemoryFix: end report failed ({ex.Message})");
            }
        }
    }
}
