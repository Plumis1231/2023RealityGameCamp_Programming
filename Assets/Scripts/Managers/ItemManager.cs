using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    [SerializeField] ItemRegistry[] registries;  //�ⲿ�༭��Ʒ��ע����Ϣ
    public static ItemManager Instance;
    Dictionary<string, ItemRegistry> itemRegistries = new Dictionary<string, ItemRegistry>();  //ע���

    [Serializable]
    public class ItemRegistry
    {
        public string id;
        public string name;
        public string description;
        public int maxStorage = 8; //һ��������ܷż���

        public GameObject Prefab;  //��Ʒʵ����Ԥ����
        public Sprite ImgOnGUI;    //��Ʒ����Ʒ���������ʾͼƬ
    }


    private void Awake()
    {
        Instance = this;
        RegisterAllItems();  //ע����Ʒ
    }

    void RegisterAllItems()
    {
        for (int i = 0; i < registries.Length; i++)
        {
            ItemRegistry item = registries[i];
            if (!itemRegistries.ContainsKey(item.id))
                itemRegistries.Add(item.id, item);
            else
                throw new Exception(string.Format("Id {0} is registied", item.id));
        }
    }

    public GameObject GetItemPrefab(string id)
    {
        if (itemRegistries.TryGetValue(id, out ItemRegistry reg))
        {
            return reg.Prefab;
        }

        return null;
    }

    public ItemRegistry GetItemRegistry(InventoryData.ItemStack stack)  //��ȡ��Ʒע����Ϣ
    {
        return GetItemRegistry(stack.id);
    }

    public ItemRegistry GetItemRegistry(string id)
    {
        if (itemRegistries.TryGetValue(id, out ItemRegistry reg))
        {
            return reg;
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
