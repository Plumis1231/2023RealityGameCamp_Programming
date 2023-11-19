using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static UnityEditor.Progress;
using Newtonsoft.Json.Linq;

public class InventoryData : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2Int shape;  //��Ʒ����������
    public int maxStorage { get => shape.x * shape.y; }  //��Ʒ����С
    
    public ItemStack[] data { get; private set; } //��¼��Ʒ������
    public bool[] dirty { get; private set; }    //��¼���������Ƿ����
   
    private void Awake()
    {
        InitSlots(); //��ʼ��
    }

    void InitSlots()
    {
        data = new ItemStack[maxStorage];
        for (int i = 0; i < data.Length; i++) data[i] = ItemStack.Empty;
        
        dirty = new bool[maxStorage];
        for (int i = 0; i < dirty.Length; i++) dirty[i] = false;

    }


    [Serializable]
    public class ItemStack
    {
        public string id = "";
        public int count = 0;
        public int maxSize = 0;
        public bool isFull { get => count >= maxSize; }
        public bool isEmpty { get => id == ""; }

        public readonly static ItemStack Empty = new ItemStack(); // ����Ʒ�ѣ���null��

        public ItemStack(ItemBase item)
        {
            id = item.Id;
            maxSize = item.MaxStorage;
        }

        public ItemStack(ItemManager.ItemRegistry item)
        {
            id = item.id;
            maxSize = item.maxStorage;
        }

        public ItemStack() { }

        public ItemStack(JObject json)
        {
            id = (string)json.GetValue("id");
            count = (int)json.GetValue("count");
            maxSize = (int)json.GetValue("size");
        }
        public ItemStack Clone()
        {
            ItemStack i = new ItemStack();
            i.id = id;
            i.count = count;
            i.maxSize = maxSize;
            return i;
        }

        public void Clear()
        {
            id = "";
            count = 0;
            maxSize = 0;
        }

        public void SetItem(ItemBase item)
        {
            id = item.Id;
            maxSize = item.MaxStorage;
            count = 0;
        }

        public bool IsItem(ItemBase item)
        {
            return item.Id == id;
        }

        public bool IsItem(ItemManager.ItemRegistry item)
        {
            return item.id == id;
        }

        public void CopyFrom(ItemStack stack)
        {
            id = stack.id;
            count = stack.count;
            maxSize = stack.maxSize;
        }

        public ItemStack Merge(ItemStack stack) //�ϲ�����ͬ����Ʒ��
        {
            if (stack.id != id) //��ͬ�࣬����
            {
                ItemStack i = Clone();
                this.CopyFrom(stack);
                stack.CopyFrom(i);
                return stack;
            }
            count = Mathf.Clamp(count + stack.count, 0, maxSize);
            
            if (count + stack.count > maxSize) //����������һ���ʣ�ģ��ͷ���һ��ʣ��Ķ�
            {
                ItemStack s = Clone();
                s.count = count + stack.count - maxSize;
                return s;
            }

            return Empty;
        }

        public JObject Serialize()
        {
            JObject jo = new JObject
            {
                { "id", id },
                { "count", count },
                { "size", maxSize }
            };

            return jo;
        }




    }
    
    //����ұ�������������
    public bool PushItem(ItemBase item)
    {
        return PushItem(item.registry);
    }

    public bool PushItem(ItemManager.ItemRegistry item)
    {
        int index = FindItem(item);    //�����ڲ�����������Ƿ��и���Ʒ�����ڸ���δ��
        
        if (index >= 0)
        {
            data[index].count++;
            MarkDirty(index);
            return true;
        }
        else  //��������㣬��������ϵĿո�������ȥ
        {
            index = FindEmptySlot();
            if (index >= 0)
            {
                data[index] = new ItemStack(item);
                data[index].count++;
                MarkDirty(index);
                return true;
            }
        }
        // ��������������ȥ�ˣ�ʧ��
        return false;
    }

    public int RemoveItem(int slotIndex, int count)  //����ұ����е�ĳһ�������Ƴ�һ����������Ʒ�����سɹ��Ƴ�������
    {
        if (slotIndex >= data.Length) return 0;

        MarkDirty(slotIndex);
        if (data[slotIndex].count >= count)
        {
            data[slotIndex].count -= count; 
            return count;
        }
        else                                          //����˾ͰѸ����ÿ�
        {
            int c = data[slotIndex].count;
            data[slotIndex] = ItemStack.Empty;
            return c;
        }
    }

    public int RemoveItem(ItemManager.ItemRegistry item, int count)  //����ұ������Ƴ�һ���������ض���Ʒ�����سɹ��Ƴ�����
    {
        int idx = FindItem(item);
        int rm_count = 0, rm_current;
        while (idx >= 0 && count > 0)
        {
            rm_current = RemoveItem(idx, count);
            rm_count += rm_current;
            count -= rm_current;
            idx = FindItem(item);
        }
        return rm_count;
    }

    public int CountOfItem(ItemManager.ItemRegistry item)  //ͳ����ұ����е���Ʒ����
    {
        int count = 0;
        for (int i = 0; i < data.Length; ++i)
            if (data[i].IsItem(item)) count += data[i].count;

        return count;
    }

    public int CountOfItem(ItemBase item)
    {
        return CountOfItem(item.registry);
    }

    public int FindItem(ItemManager.ItemRegistry item)  //�������Ҹ���Ʒδ���ĸ���
    {
        for (int i = 0; i < data.Length; ++i)
            if (data[i].IsItem(item) && !data[i].isFull) return i;

        return -1;
    }


    public int FindItem(ItemBase item)  //�������Ҹ���Ʒδ���ĸ���
    {
        for (int i = 0; i<data.Length; ++i)
            if (data[i].IsItem(item) && !data[i].isFull) return i;
        
        return -1;
    }

    public int FindEmptySlot()  //��һ���ո���
    {
        for (int i = 0; i < data.Length; ++i)
            if (data[i].isEmpty) return i;
        
        return -1;
    }

    public void MarkDirty(int slot)  //��Ǹ������ݸ���
    {
        dirty[slot] = true;
    }

    public void MardUIUpdated(int slot)  //���UI�Ѹ���
    {
        dirty[slot] = false;
    }

    public void UnserializeFromJson(string jstring)
    {

        JArray ja = (JArray)JsonConvert.DeserializeObject(jstring);

        for (int i = 0; i < data.Length; i++)
        {
            data[i].Clear();
            dirty[i] = true;
        }


        for (int i = 0; i < ja.Count; i++)
        {
            JObject slot = (JObject)ja[i];
            data[(int)slot.GetValue("index")] = new ItemStack((JObject)slot.GetValue("stack"));
        }

    }

    public string SerializeToJson()
    {
        JArray ja = new JArray();
        for (int i = 0; i < data.Length; i++)
        {
            if (!data[i].isEmpty) {
                JObject jo = new JObject
                {
                    { "index", i},
                    { "stack",  data[i].Serialize() }
                };
                ja.Add(jo);
            }
        }
        return ja.ToString();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            FileUtils.WriteString(Application.dataPath + "\\inventory.json", SerializeToJson());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            UnserializeFromJson(FileUtils.ReadString(Application.dataPath + "\\inventory.json"));
            
        }
    }
}
