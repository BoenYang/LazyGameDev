using Excel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace ExcelSerializer {
    public struct FieldData {
        public string type;
        public string name;
    }

    public class ExcelSerializer {
        [MenuItem("Tools/Excel/Generate Code")]
        private static void GenerateCode() {
            var path = string.Empty;
            if(CheckExcelPath(out path) == false)
                return;
            string[] files = Directory.GetFiles(path);
            foreach(var file in files) {
                if(!file.EndsWith(".xlsx"))
                    continue;
                var stream = File.Open(file, FileMode.Open, FileAccess.Read);
                var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var dt = reader.AsDataSet();

                for(int i = 0; i < dt.Tables.Count; i++) {
                    var table = dt.Tables[i];
                    var fields = new List<FieldData>();

                    for(int j = 0; j < table.Columns.Count; j++) {
                        if(table.Rows[0][j].ToString().Trim().StartsWith("#"))
                            continue;
                        var field = new FieldData {
                            name = table.Rows[0][j].ToString().Trim(),
                            type = table.Rows[1][j].ToString().Trim()
                        };
                        fields.Add(field);
                    }
                    GenerateCS(table.TableName, fields);
                }
                reader.Close();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Excel/Generate Bytes")]
        private static void GenerateBytes() {
            var path = string.Empty;
            if(CheckExcelPath(out path) == false)
                return;

            var ns = EditorPrefs.GetString(ExcelPrefWindow.NamespaceKey);
            var bytesFolder = EditorPrefs.GetString(ExcelPrefWindow.BytesFolderKey);
            var bathPath = Application.dataPath + "/" + bytesFolder;
            if(!Directory.Exists(bathPath))
                Directory.CreateDirectory(bathPath);
            string[] files = Directory.GetFiles(path);
            var assembly = Assembly.Load(new AssemblyName("Assembly-CSharp"));

            foreach(var file in files) {
                if(!file.EndsWith(".xlsx"))
                    continue;
                var stream = File.Open(file, FileMode.Open, FileAccess.Read);
                var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var dt = reader.AsDataSet();

                for(int i = 0; i < dt.Tables.Count; i++) {
                    var table = dt.Tables[i];
                    var clz = assembly.GetType(ns + "." + table.TableName);
                    var listClzName = ns + "." + table.TableName + "List";
                    var listClz = assembly.GetType(listClzName);
                    var listObj = System.Activator.CreateInstance(listClz);
                    var listField = listClz.GetField("data");
                    var configs = listField.GetValue(listObj) as IList;

                    for(int r = 3; r < table.Rows.Count; r++) {
                        var obj = System.Activator.CreateInstance(clz);
                        for(int c = 0; c < table.Columns.Count; c++) {
                            if(table.Rows[0][c].ToString().Trim().StartsWith("#"))
                                continue;

                            var fieldName = table.Rows[0][c].ToString().Trim();
                            var fieldType = table.Rows[1][c].ToString().Trim();
                            var fieldVal = table.Rows[r][c].ToString().Trim();

                            //Debug.Log("fieldName: " + fieldName + ", fiedlType: " + fieldType + ", fieldVal: " + fieldVal);

                            var field = clz.GetField(fieldName);
                            field.SetValue(obj,
                                System.Convert.ChangeType(fieldVal,
                                field.FieldType));
                        }
                        configs.Add(obj);
                    }

                    listField.SetValue(listObj, configs);
                    using(FileStream fstream = File.Create(bathPath + "/" + table.TableName + ".bytes")) {
                        Serialize(fstream, listObj);
                    }
                }

                reader.Close();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Excel/Preferences")]
        public static void Preferences() {
            var wind = EditorWindow.GetWindow<ExcelPrefWindow>("ExcelSerializer");
            wind.Show();
        }

        public static void Serialize(Stream stream, object obj) {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        private static void GenerateCS(string clsName, List<FieldData> fields) {
            Debug.Log("Generating " + clsName + "...");
            var dir = Application.dataPath + "/" + EditorPrefs.GetString(ExcelPrefWindow.ScriptsFolderKey);
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var fileName = string.Format("{0}/{1}.cs", dir
                , clsName);
            var stream = File.Open(fileName, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("// =====================================");
            writer.WriteLine("// Auto generated by ExcelSerializer");
            writer.WriteLine("// Tools created by atom");
            writer.WriteLine("// =====================================");
            writer.WriteLine("using System.Collections.Generic;\n");
            writer.WriteLine(string.Format("namespace {0}", EditorPrefs.GetString(ExcelPrefWindow.NamespaceKey)) + " {");
            writer.WriteLine("    [System.Serializable]");
            writer.WriteLine("    public class " + clsName + " {");

            foreach(var field in fields) {
                writer.WriteLine("        public " + field.type + " " + field.name + ";");
            }
            writer.WriteLine("    }");

            // generate config list
            writer.WriteLine("    [System.Serializable]");
            writer.WriteLine("    public class " + clsName + "List {");
            writer.WriteLine("        public List<" + clsName + ">" + " data = new List<" + clsName + ">();");
            writer.WriteLine("    }");

            writer.WriteLine("}");

            writer.Close();
        }

        private static bool CheckExcelPath(out string path) {
            path = EditorPrefs.GetString(ExcelPrefWindow.ExcelsFolderKey);
            if(path == null || path == string.Empty) {
                Debug.LogError("Excel Path can't be empty!");
                return false;
            }
            if(!Directory.Exists(path)) {
                Debug.LogError("Excel Path: (" + path + ") can't be found");
                return false;
            }
            return true;
        }
    }

    public class ExcelPrefWindow : EditorWindow {
        public const string InitializedKey = "ExcelPref_Initialized";
        public const string BytesFolderKey = "ExcelPref_DefaultBytesFolder";
        public const string ExcelsFolderKey = "ExcelPref_ExcelsFolder";
        public const string NamespaceKey = "ExcelPref_Namespace";
        public const string ScriptsFolderKey = "ExcelPref_Scripts";
        private string m_DefaultBytesFolder;
        private string m_ExcelsFolder;
        private string m_Namespace;
        private string m_ScriptsFolder;

        private void OnGUI() {
            var rect = new Rect(10, 10, 200, 16f);
            var titleStyle = new GUIStyle() { fontSize = 20, fontStyle = FontStyle.Bold };
            titleStyle.normal.textColor = Color.gray;
            GUI.Box(rect, "Preferences", titleStyle);

            // namespace
            rect.y = rect.yMax + 20f;
            GUI.Label(rect, "Class Namespace:");
            rect.x = 148f;
            m_Namespace = EditorGUI.TextField(rect, m_Namespace);

            // scripts folder
            rect.x = 10f;
            rect.y = rect.yMax + 10f;
            EditorGUI.LabelField(rect, "Scripts Folder:");
            rect.x = 148f;
            m_ScriptsFolder = EditorGUI.TextField(rect, m_ScriptsFolder);

            // bytes folder 
            rect.x = 10f;
            rect.y = rect.yMax + 10f;
            EditorGUI.LabelField(rect, "Default Bytes Folder:");
            rect.x = 148f;
            m_DefaultBytesFolder = EditorGUI.TextField(rect, m_DefaultBytesFolder);

            // excels folder
            rect.x = 10f;
            rect.y = rect.yMax + 10f;
            EditorGUI.LabelField(rect, "Excels Folder: ");
            rect.x = 148f;
            m_ExcelsFolder = EditorGUI.TextField(rect, m_ExcelsFolder);

            rect.y = rect.yMax + 10f;
            rect.height = 20f;
            GUI.backgroundColor = Color.green;
            if(GUI.Button(rect, "Save")) {
                EditorPrefs.SetString(BytesFolderKey, m_DefaultBytesFolder);
                EditorPrefs.SetString(ExcelsFolderKey, m_ExcelsFolder);
                EditorPrefs.SetString(ScriptsFolderKey, m_ScriptsFolder);
                EditorPrefs.SetString(NamespaceKey, m_Namespace);
                Debug.Log("Excel Serializer Preferences Saved");
            }
        }

        private void OnEnable() {
            m_DefaultBytesFolder = EditorPrefs.GetString(BytesFolderKey
                , "Resources/Bytes");
            m_ExcelsFolder = EditorPrefs.GetString(ExcelsFolderKey);
            m_Namespace = EditorPrefs.GetString(NamespaceKey, "GameConfigs");
            m_ScriptsFolder = EditorPrefs.GetString(ScriptsFolderKey, "Scripts/GaemConfigs");
        }
    }
}