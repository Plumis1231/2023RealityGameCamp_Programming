using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //                                    �������Ƴ��¼��Ľӿ�


    [SerializeField] Image sprite;
    [SerializeField] Button button;
    [SerializeField] Text countNumber;

    public int index;
    public InventoryData.ItemStack itemStack {        //�ֶ�ӳ��
        get { return GetItemStack(); }
        set { SetItemStack(value); }
    }

    protected virtual InventoryData.ItemStack GetItemStack()
    {
        return InventoryUI.Instance.inventoryData.data[index];
    }

    protected virtual void SetItemStack(InventoryData.ItemStack itemStack)
    {
        InventoryUI.Instance.inventoryData.data[index] = itemStack;
    }

    public void Init(int idx)
    {
        index = idx;
        Flush();
    }

    public void Flush()  //ˢ�¸�����ʾ
    {
        ItemManager.ItemRegistry reg = ItemManager.Instance.GetItemRegistry(itemStack);
        if(reg != null)
        {
            sprite.sprite = reg.ImgOnGUI;
            sprite.color = Color.white;
            countNumber.text = string.Format("{0}", itemStack.count);
            countNumber.enabled = true;
        }
        else
        {
            sprite.sprite = null;
            sprite.color = new Color(0,0,0,0);
            countNumber.enabled = false;
        }
        
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if(InventoryUI.Instance.current.isEmpty)   //��ǰ������û�������Ѹ����ﶫ���ó���
        {
            InventoryUI.Instance.current = itemStack;
            itemStack = InventoryData.ItemStack.Empty;
        }
        else
        {
            if (itemStack.isEmpty)                //��ǰ�������ж���������������û�����������ϵķŽ�ȥ
            {
                itemStack = InventoryUI.Instance.current;
                InventoryUI.Instance.current = InventoryData.ItemStack.Empty;
            }
            else                                   //��ǰ�������ж��������������ж����������Ѷ���
            {
                InventoryUI.Instance.current = itemStack.Merge(InventoryUI.Instance.current);
            }
        }
        Flush();
        
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)     //�������������
    {
        InventoryUI.Instance.descriptionStack = itemStack;
    }

    public void OnPointerExit(PointerEventData eventData)       //�������ÿ�����
    {
        InventoryUI.Instance.descriptionStack = InventoryData.ItemStack.Empty;
    }
}
