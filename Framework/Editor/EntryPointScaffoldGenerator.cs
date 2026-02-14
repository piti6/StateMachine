using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Misokatsu.Framework.Editor
{
    public static class EntryPointScaffoldGenerator
    {
        private const string EntryPointSuffix = "EntryPoint";

        private const string DefaultPresenterTemplatePath =
            "Packages/io.github.piti6.misokatsu.framework/Framework/Editor/Templates/EntryPointPresenter.txt";
        private const string DefaultControllerTemplatePath =
            "Packages/io.github.piti6.misokatsu.framework/Framework/Editor/Templates/EntryPointController.txt";
        private const string DefaultInstallerTemplatePath =
            "Packages/io.github.piti6.misokatsu.framework/Framework/Editor/Templates/EntryPointInstaller.txt";

        [MenuItem("Tools/Misokatsu/Generate EntryPoint Scaffold")]
        public static void Generate()
        {
            Generate(null);
        }

        public static void Generate(IEnumerable<string> entryPointScriptAssetPaths)
        {
            var settings = EntryPointScaffoldSettings.instance;
            var presenterTemplate = ReadTemplate(
                settings.presenterTemplate,
                DefaultPresenterTemplatePath,
                "Presenter");
            var controllerTemplate = ReadTemplate(
                settings.controllerTemplate,
                DefaultControllerTemplatePath,
                "Controller");
            var installerTemplate = ReadTemplate(
                settings.installerTemplate,
                DefaultInstallerTemplatePath,
                "Installer");

            if (presenterTemplate == null || controllerTemplate == null || installerTemplate == null)
            {
                Debug.LogError("EntryPoint scaffold templates are missing. Check Project Settings > Misokatsu.Framework.");
                return;
            }

            var scripts = ResolveScripts(entryPointScriptAssetPaths);

            var created = 0;
            var processed = 0;
            var found = 0;
            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

            foreach (var script in scripts)
            {
                var type = script.GetClass();
                if (type == null) continue;

                if (!Attribute.IsDefined(type, typeof(EntryPointAttribute), false)) continue;
                found++;
                if (!typeof(EntryPointBase).IsAssignableFrom(type))
                {
                    Debug.LogError($"[EntryPoint] must inherit EntryPointBase: {type.FullName}");
                    continue;
                }

                var className = type.Name;
                var featureName = className.EndsWith(EntryPointSuffix, StringComparison.Ordinal)
                    ? className.Substring(0, className.Length - EntryPointSuffix.Length)
                    : className;

                if (string.IsNullOrWhiteSpace(featureName))
                {
                    featureName = className;
                }

                var namespaceName = type.Namespace ?? string.Empty;

                var entryPath = AssetDatabase.GetAssetPath(script);
                if (string.IsNullOrEmpty(entryPath)) continue;

                var entryFullPath = Path.GetFullPath(Path.Combine(projectRoot, entryPath));
                var baseDir = Path.GetDirectoryName(entryFullPath);
                if (string.IsNullOrEmpty(baseDir)) continue;

                created += GenerateGroup(baseDir, featureName, namespaceName, "Presenter",
                    presenterTemplate, $"{featureName}Presenter.cs", settings.presenterAsmdef);
                created += GenerateGroup(baseDir, featureName, namespaceName, "Controller",
                    controllerTemplate, $"{featureName}Controller.cs", settings.controllerAsmdef);
                created += GenerateGroup(baseDir, featureName, namespaceName, "Installer",
                    installerTemplate, $"{featureName}Installer.cs", settings.installerAsmdef);

                processed++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"EntryPoint scaffold: created {created} files. Found {found} entry points. Processed {processed}.");
        }

        private static List<MonoScript> ResolveScripts(IEnumerable<string> entryPointScriptAssetPaths)
        {
            if (entryPointScriptAssetPaths == null)
            {
                return AssetDatabase.FindAssets("t:MonoScript")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                    .Where(x => x != null)
                    .ToList();
            }

            return entryPointScriptAssetPaths
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .Where(x => x != null)
                .ToList();
        }

        private static int GenerateGroup(
            string baseDir,
            string featureName,
            string namespaceName,
            string suffix,
            string template,
            string fileName,
            AssemblyDefinitionAsset asmdef)
        {
            var groupDir = Path.Combine(baseDir, $"{featureName}{suffix}");
            Directory.CreateDirectory(groupDir);

            var asmdefName = GetAsmdefName(asmdef);
            if (!string.IsNullOrEmpty(asmdefName))
            {
                var asmrefPath = Path.Combine(groupDir, $"{featureName}{suffix}.asmref");
                if (!File.Exists(asmrefPath))
                {
                    File.WriteAllText(asmrefPath, $"{{\n  \"reference\": \"{asmdefName}\"\n}}\n");
                }
            }

            var filePath = Path.Combine(groupDir, fileName);
            if (File.Exists(filePath)) return 0;

            var content = ApplyTemplate(template, namespaceName, featureName);
            File.WriteAllText(filePath, content);
            return 1;
        }


        private static string ReadTemplate(TextAsset selectedTemplate, string defaultAssetPath, string label)
        {
            if (selectedTemplate != null)
            {
                return selectedTemplate.text;
            }

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(defaultAssetPath);
            if (textAsset != null)
            {
                return textAsset.text;
            }

            Debug.LogError($"{label} template not found. Default path: {defaultAssetPath}");
            return null;
        }

        private static string GetAsmdefName(AssemblyDefinitionAsset asmdef)
        {
            if (asmdef == null) return null;
            var path = AssetDatabase.GetAssetPath(asmdef);
            if (string.IsNullOrWhiteSpace(path)) return null;

            var json = File.ReadAllText(path);
            var def = JsonUtility.FromJson<AssemblyDefinitionJson>(json);
            if (string.IsNullOrWhiteSpace(def?.name))
            {
                Debug.LogWarning($"Invalid asmdef name at path: {path}");
                return null;
            }

            return def.name;
        }

        private static string ApplyTemplate(string template, string namespaceName, string featureName)
        {
            var content = template
                .Replace("{{Namespace}}", namespaceName ?? string.Empty)
                .Replace("{{FeatureName}}", featureName);

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                return content;
            }

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            if (lines.Count >= 2 && lines[0].StartsWith("namespace ", StringComparison.Ordinal))
            {
                lines.RemoveAt(0);
                if (lines.Count > 0 && lines[0].Trim() == "{")
                {
                    lines.RemoveAt(0);
                }
                if (lines.Count > 0 && lines[^1].Trim() == "}")
                {
                    lines.RemoveAt(lines.Count - 1);
                }
            }

            return string.Join("\n", lines);
        }

        [Serializable]
        private sealed class AssemblyDefinitionJson
        {
            public string name;
        }
    }
}
