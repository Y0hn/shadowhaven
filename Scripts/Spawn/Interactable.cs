using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;
    public Rarity maxRarity = Rarity.Rare;

    private void Start()
    {
        // References
        gameObject.layer = LayerMask.NameToLayer("ItemDrop");
        SpriteRenderer sRend = GetComponent<SpriteRenderer>();
        ItemsList itList = ItemsList.instance;

        maxRarity += GameManager.instance.level;

        if (item == null)
        {
            switch (transform.name)
            {
                case "weapon":
                    item = itList.GetWeaponOfRarityAndBelow(maxRarity);
                    break;
                case "armor":
                    item = itList.GetArmorOfRarityAndBelow(maxRarity);
                    break;
                case "weapon/armor":
                    int r = Random.Range(0, 2);
                    if (r == 0)
                        item = itList.GetWeaponOfRarityAndBelow(maxRarity);
                    else
                        item = itList.GetArmorOfRarityAndBelow(maxRarity);
                    break;
                default:
                    item = itList.GetItemOfRarityAndBelow(maxRarity);
                    break;
            }
            if (item == null)
            {
                Debug.Log($"Interactable was unable to spawn at Max Rarity {maxRarity}");
                Destroy(gameObject);
            }
            else
            {
                sRend.sprite = item.icon;
                sRend.color = item.color;
            }
        }
        else
        {
            sRend.sprite = item.icon;
            sRend.color = item.color;
        }
    }
    public void AddToInventory()
    {
        GameManager.audio.Play("item-pick-up-1");
        if (GameManager.inventory.Add(item))
            Destroy(gameObject);
    }
}