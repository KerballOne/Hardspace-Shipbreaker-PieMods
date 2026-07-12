# Stripped reference assemblies

These are the Hardspace: Shipbreaker `Shipbreaker_Data/Managed` DLLs referenced by
the mods, with **all method bodies stripped** (`throw null;`) and accessibility left
untouched. They contain only public type/method signatures and constants — no game
logic — and exist so the solution compiles locally and in CI without a game install.

All projects reference them through `$(ManagedDir)` (defined in the repo-root
`Directory.Build.props`), which defaults to this folder.

## Regenerating after a game update

Uses [BepInEx.AssemblyPublicizer.Cli](https://github.com/BepInEx/BepInEx.AssemblyPublicizer):

```bash
dotnet tool install -g BepInEx.AssemblyPublicizer.Cli   # once

MANAGED="/path/to/Hardspace Shipbreaker/Shipbreaker_Data/Managed"
OUT="lib/ManagedStripped"

DLLS="AntiCheatToolKit Assembly-CSharp BBI.Unity.Game Carbon.Audio Carbon.Core \
Carbon.Core.Unity Carbon.Localization.Core InControl Newtonsoft.Json \
Unity.Addressables Unity.Collections UnityEngine.CoreModule UnityEngine \
UnityEngine.UI Unity.Entities Unity.Mathematics Unity.RenderPipelines.Core.Runtime \
Unity.RenderPipelines.HighDefinition.Runtime Unity.ResourceManager Unity.TextMeshPro"

inputs=(); for d in $DLLS; do inputs+=("$MANAGED/$d.dll"); done
assembly-publicizer "${inputs[@]}" --strip-only -f -o "$OUT"
```

`--strip-only` strips method bodies **without** publicizing, matching how the mods
compile against the raw game assemblies. If a mod later needs access to private
members, switch that to `--strip` (publicize + strip) instead.

> The CLI targets .NET 6; on a machine with only a newer runtime, prefix the command
> with `DOTNET_ROLL_FORWARD=Major`.
