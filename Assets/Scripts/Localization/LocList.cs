using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NLocalization
{
    public class LocList
    {
        string m_path = "";

        LocTable m_table;
        List<LocLanguage> m_languages = new List<LocLanguage>();

        public void Load(string path = "Localization") //path in /resource/ subdirectory
        {
            m_languages.Clear();
            m_table = null;

            m_path = path;

            var locs = Resources.LoadAll<ScriptableObject>(path);

            foreach (var i in locs)
            {
                LocTable table = i as LocTable;
                if (table != null)
                {
                    if (m_table != null)
                    {
                        Debug.LogError("Multiple table found in resource directory \"" + path + "\" - Loaded \"" + m_table.name + "\" - New \"" + table.name + "\"");
                        continue;
                    }
                    m_table = table;
                    continue;
                }

                LocLanguage lang = i as LocLanguage;
                if (lang != null)
                    m_languages.Add(lang);
            }

            if (m_table == null)
                Initialize(path);
        }

        void Initialize(string path)
        {
#if UNITY_EDITOR
            m_table = CreateAsset<LocTable>(path, "Table");

            //todo populate table and make first language if no lang in the list

            throw new NotImplementedException();
#else
            Debug.LogError("You can't initialize languages outside the editor");
#endif
        }

#if UNITY_EDITOR
        //path starting after Assets/Resources
        //name without extention
        T CreateAsset<T>(string path, string name) where T : ScriptableObject
        {
            string[] sub = path.Split('/');

            int index = 0;

            string partialPath = "Assets";
            string addingPath = "Resources";

            do
            {
                if (!AssetDatabase.IsValidFolder(partialPath + "/" + addingPath))
                    AssetDatabase.CreateFolder(partialPath, addingPath);

                partialPath += "/" + addingPath;
                addingPath = sub[index];

                index++;
            } while (index <= sub.Length);

            T asset = ScriptableObject.CreateInstance(typeof(T)) as T;
            if (asset == null)
            {
                Debug.LogError("Can't create a " + typeof(T).Name);
                return null;
            }

            AssetDatabase.CreateAsset(asset, partialPath + "/" + name + ".asset");
            AssetDatabase.SaveAssets();

            return asset;
        }
#endif

    }
}