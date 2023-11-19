using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemManager.ItemRegistry;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public static ItemRegister registerEvent = (i) => { };

    Dictionary<string, ItemRegistry> itemRegistries = new Dictionary<string, ItemRegistry>();  //ע���
    public delegate void ItemRegister(ItemManager itemRegistry);


    [Serializable]
    public class ItemRegistry
    {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public int maxStorage { get; private set; } = 8;//һ��������ܷż���
        public delegate DroppedItem DropItemConstructor();

        public DropItemConstructor constructor { get; private set; }  //��Ʒʵ����Ԥ����
        public Sprite ImgOnGUI { get; private set; }    //��Ʒ����Ʒ���������ʾͼƬ

        public ItemRegistry(string id, string name, string description, int maxStorage, DropItemConstructor prefab, Sprite imgOnGUI)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.maxStorage = maxStorage;
            constructor = prefab;
            ImgOnGUI = imgOnGUI;
        }
    }


    private void Awake()
    {
        Instance = this;
        RegisterAllItem();
    }

    public ItemRegistry Register(string id, string name, string description, int maxStorage, DropItemConstructor prefab, Sprite imgOnGUI)
    {
        ItemRegistry reg = new ItemRegistry(id, name, description, maxStorage, prefab, imgOnGUI);
        if (!itemRegistries.ContainsKey(id)) itemRegistries.Add(id, reg);
        else throw new Exception(string.Format("Id: {0} has been used.", id));
        return reg;
    }



    public ItemBase CreateItem(string id)
    {
        if (itemRegistries.TryGetValue(id, out ItemRegistry reg))
        {
            return reg.constructor();
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

    public ItemBase CreateDropItem(ItemRegistry item)
    {
        return item.constructor();
    }

    public DroppedItem CreateDropItem(InventoryData.ItemStack item)
    {
        return GetItemRegistry(item).constructor();
    }


    // Start is called before the first frame update
    void Start()
    {
        registerEvent(this);
    }

    void RegisterAllItem()
    {
        Register("apple", "ƻ��", "�óԵ�", 4, 
            () => DroppedItem.Create("Items/Apple/Apple"),
            Resources.Load<Sprite>("Items/Apple/gui")
            );

        Register("banana", "�㽶", "�óԵ�", 8, 
            () => DroppedItem.Create("Items/Banana/Banana"),
            Resources.Load<Sprite>("Items/Banana/gui")
            );
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
