using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Misokatsu.Framework.Editor
{
    public sealed class EntryPointScaffoldAutoGenerator : AssetPostprocessor
    {
        private static bool pendingGenerate;
        private static readonly HashSet<string> pendingEntryPointPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            CollectEntryPointPaths(importedAssets);
            CollectEntryPointPaths(movedAssets);

            if (pendingEntryPointPaths.Count == 0)
            {
                return;
            }

            if (pendingGenerate)
            {
                return;
            }

            pendingGenerate = true;
            EditorApplication.delayCall += RunGenerate;
        }

        private static void CollectEntryPointPaths(string[] paths)
        {
            if (paths == null || paths.Length == 0) return;

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;
                if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;

                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script == null) continue;

                var type = script.GetClass();
                if (type == null) continue;

                if (!Attribute.IsDefined(type, typeof(EntryPointAttribute), false)) continue;

                pendingEntryPointPaths.Add(path);
            }
        }

        private static void RunGenerate()
        {
            pendingGenerate = false;

            try
            {
                var paths = pendingEntryPointPaths.ToArray();
                pendingEntryPointPaths.Clear();
                EntryPointScaffoldGenerator.Generate(paths);
            }
            catch (Exception ex)
            {
                pendingEntryPointPaths.Clear();
                Debug.LogError($"EntryPoint scaffold auto-generate failed: {ex}");
            }
        }
    }
}
