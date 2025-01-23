using System;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemDetailsList", menuName = "new ItemDetailsList",order = 0)]
public class ItemDetails : ScriptableObject
{
    public string Name;
    public string GUID;
    public Sprite Icon;
    public bool CanDrop;

    public void GenerateGUID()
    {
        GUID = System.Guid.NewGuid().ToString();
    }

    //public  static void PopulateDatabase()
    //{
    //    m_ItemDatabase.Add("8B0EF21A-F2D9-4E6F-8B79-031CA9E202BA", new ItemDetails()
    //    {
    //        Name = "History of the Syndicate: 1501 to 1825 ",
    //        GUID = "8B0EF21A-F2D9-4E6F-8B79-031CA9E202BA",
    //        Icon = IconSprites.FirstOrDefault(x => x.name.Equals("syndicate")),
    //        CanDrop = false
    //    });

    //    m_ItemDatabase.Add("992D3386-B743-4CD3-9BB7-0234A057C265", new ItemDetails()
    //    {
    //        Name = "Health Potion",
    //        GUID = "992D3386-B743-4CD3-9BB7-0234A057C265",
    //        Icon = IconSprites.FirstOrDefault(x => x.name.Equals("potion")),
    //        CanDrop = true
    //    });

    //    m_ItemDatabase.Add("1B9C6CAA-754E-412D-91BF-37F22C9A0E7B", new ItemDetails()
    //    {
    //        Name = "Bottle of Poison",
    //        GUID = "1B9C6CAA-754E-412D-91BF-37F22C9A0E7B",
    //        Icon = IconSprites.FirstOrDefault(x => x.name.Equals("poison")),
    //        CanDrop = true
    //    });

    //}
}
[CustomEditor(typeof(ItemDetails))]
public class ScriptableObjectWithGUIDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Получаем ссылку на целевой объект
        ItemDetails scriptableObject = (ItemDetails)target;

        // Добавляем кнопку в инспектор
        if (GUILayout.Button("Generate GUID"))
        {
            scriptableObject.GenerateGUID();

            // Обновляем изменения и сохраняем объект
            EditorUtility.SetDirty(scriptableObject);
        }
    }
}
