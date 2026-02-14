using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Misokatsu.Framework.Editor
{
    [FilePath("ProjectSettings/MisokatsuFrameworkScaffoldSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class EntryPointScaffoldSettings : ScriptableSingleton<EntryPointScaffoldSettings>
    {
        private const string DefaultPresenterTemplatePath =
            "Packages/io.github.piti6.misokatsu.framework/Framework/Editor/Templates/EntryPointPresenter.txt";
        private const string DefaultControllerTemplatePath =
            "Packages/io.github.piti6.misokatsu.framework/Framework/Editor/Templates/EntryPointController.txt";
        private const string DefaultInstallerTemplatePath =
            "Packages/io.github.piti6.misokatsu.framework/Framework/Editor/Templates/EntryPointInstaller.txt";

        public AssemblyDefinitionAsset presenterAsmdef;
        public AssemblyDefinitionAsset controllerAsmdef;
        public AssemblyDefinitionAsset installerAsmdef;
        public TextAsset presenterTemplate;
        public TextAsset controllerTemplate;
        public TextAsset installerTemplate;

        public void SaveSettings()
        {
            Save(true);
        }

        [SettingsProvider]
        private static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider("Project/Misokatsu.Framework", SettingsScope.Project)
            {
                label = "Misokatsu.Framework",
                guiHandler = _ =>
                {
                    var settings = instance;
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.LabelField("Assembly Definitions", EditorStyles.boldLabel);
                    settings.presenterAsmdef = (AssemblyDefinitionAsset)EditorGUILayout.ObjectField(
                        "Presenter asmdef", settings.presenterAsmdef, typeof(AssemblyDefinitionAsset), false);
                    settings.controllerAsmdef = (AssemblyDefinitionAsset)EditorGUILayout.ObjectField(
                        "Controller asmdef", settings.controllerAsmdef, typeof(AssemblyDefinitionAsset), false);
                    settings.installerAsmdef = (AssemblyDefinitionAsset)EditorGUILayout.ObjectField(
                        "Installer asmdef", settings.installerAsmdef, typeof(AssemblyDefinitionAsset), false);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Templates (Optional)", EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox("If template is empty, the built-in package template is used.", MessageType.Info);
                    settings.presenterTemplate = (TextAsset)EditorGUILayout.ObjectField(
                        "Presenter template", settings.presenterTemplate, typeof(TextAsset), false);
                    EditorGUILayout.SelectableLabel(
                        $"Default: {DefaultPresenterTemplatePath}",
                        EditorStyles.textField,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    settings.controllerTemplate = (TextAsset)EditorGUILayout.ObjectField(
                        "Controller template", settings.controllerTemplate, typeof(TextAsset), false);
                    EditorGUILayout.SelectableLabel(
                        $"Default: {DefaultControllerTemplatePath}",
                        EditorStyles.textField,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    settings.installerTemplate = (TextAsset)EditorGUILayout.ObjectField(
                        "Installer template", settings.installerTemplate, typeof(TextAsset), false);
                    EditorGUILayout.SelectableLabel(
                        $"Default: {DefaultInstallerTemplatePath}",
                        EditorStyles.textField,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    if (EditorGUI.EndChangeCheck())
                    {
                        settings.SaveSettings();
                    }
                }
            };

            return provider;
        }
    }
}
