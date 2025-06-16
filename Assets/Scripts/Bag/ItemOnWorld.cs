using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    public Item item;
    public Inventory inventory;
    public int index;

    public void UpdateIndex(int idx)
    {
        index = idx;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AddItemtoInventory();
            //��GameSaveAndLoad�и�����Ʒ����ֵ�б�
            GameSaveAndLoad.instance.UpdateitemBoolsList(index);
            Destroy(gameObject);
        }
    }

    private void AddItemtoInventory()
    {
        if (!inventory.itemList.Contains(item))
        {
            inventory.itemList.Add(item);
        }
        else
        {
            item.itemHeld += 1;
        }

        Main.RefreshItem();
    }
}
