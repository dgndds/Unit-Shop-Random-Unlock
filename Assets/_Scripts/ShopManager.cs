using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    //Class represents the each skins properties
    [System.Serializable] class ShopSkin{
        public Sprite skinImage;
        public Transform panel;
        public int skinId;
        public bool unlocked;
    }

    //Event Handler for skin change that other class can subscribe to
    public event EventHandler<onSkinChangeEventArgs> onSkinChange;

    //Pass panel transform as event argument when triggered
    public class onSkinChangeEventArgs : EventArgs
    {
        public Transform chosenPanel;
        public Sprite chosenSkin;
    }

    // List of all skins
    [SerializeField] List<ShopSkin> ShopItems;

    //List of locked skins
    [SerializeField] List<ShopSkin> lockedShopItems;

    //Array of UI  panels
   // [SerializeField] Transform[] ShopItemsPanel;

    //Number of rolls before opening the random skin
    [SerializeField] int randomRollCount;

    //Skin user chose to equip
    private Transform selectedItem;

    // Green color (Chosen/random)
    Color greenColor = new Color(41f / 255f, 135f / 255f, 45f / 255f);

    // Grey color (Locked)
    Color greyColor = new Color(128f / 255f, 128f / 255f, 128f / 255f);

    //Selected button/panel id
    private int selectedId = 0;

    //Unlock button
    [SerializeField] Button unlockBtn;

    // Start is called before the first frame update
    void Start()
    {
        // Give each panel and button under them a unique id
        for (int i = 0; i < ShopItems.Count; i++)
        {
            int currenIndex = i;
            ShopItems[i].skinId = i;
            ShopItems[i].panel.GetComponent<Button>().onClick.AddListener(() => onSkinSelect(currenIndex));
        }

        //ShopItems[selectedId].panel.GetComponent<Image>().color = greenColor;

        // Update list of locked skins by checking all skins
        InitShop();

        //Buttons are interactable if they are unlocked
        UpdateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        ShopItems[selectedId].panel.GetComponent<Image>().color = greenColor;
    }

    public void Unlock() {
            StartCoroutine(ChoseRandomUnlocked());
    }

    
    private IEnumerator ChoseRandomUnlocked() {
        unlockBtn.interactable = false;
        int randomIndex = -1;

        for (int i = 0; i < randomRollCount; i++){

            //Random Index
            randomIndex = UnityEngine.Random.Range(0, lockedShopItems.Count);

            Debug.Log("unlocking " + randomIndex.ToString() + " ...");

            //Update random index color to green
            lockedShopItems[randomIndex].panel.GetComponent<Image>().color = greenColor;

            //Wait for 0.5 seconds before reroll
            yield return new WaitForSeconds(.5f);

            //Change color back to grey
            lockedShopItems[randomIndex].panel.GetComponent<Image>().color = greyColor;
        }

        //Record last random index 
        int unlockedIndex = randomIndex;

        // Higlight it with green
        //lockedShopItems[unlockedIndex].panel.GetComponent<Image>().color = greenColor;
        ShopItems[selectedId].panel.GetComponent<Image>().color = greyColor;
        selectedId = lockedShopItems[unlockedIndex].skinId;

        onSkinSelect(selectedId);

        // Change image to unlocked skin
        lockedShopItems[unlockedIndex].panel.GetChild(1).GetComponent<Image>().sprite = lockedShopItems[unlockedIndex].skinImage;

        //Unlock the skin
        lockedShopItems[unlockedIndex].unlocked = true;

        //Remove unlocked item from locked list
        lockedShopItems.RemoveAt(unlockedIndex);


        //ShopItems[unlockedIndex].unlocked = true;

        //UpdateLocked();

        //Update unlocked buttons according change of shopskins
        UpdateButtons();

        //onSkinChange?.Invoke(this, new onSkinChangeEventArgs { chosenPanel = ShopItems[selectedId].panel, chosenSkin = ShopItems[selectedId].skinImage });

        unlockBtn.interactable = true;
    }

    private void InitShop(){
        for (int i = 0; i < ShopItems.Count; i++)
        {
            if (!ShopItems[i].unlocked)
                lockedShopItems.Add(ShopItems[i]);
            else
                ShopItems[i].panel.GetChild(1).GetComponent<Image>().sprite = ShopItems[i].skinImage;
            
                
        }
    }

    private void UpdateButtons(){
        for (int i = 0; i < ShopItems.Count; i++)
        {
            ShopItems[i].panel.GetComponent<Button>().interactable = ShopItems[i].unlocked;
        }
    }

    private void onSkinSelect(int index)
    {
        Debug.Log("Chosen skin index:" + index);
        ShopItems[selectedId].panel.GetComponent<Image>().color = greyColor;
        selectedId = index;

        onSkinChange?.Invoke(this, new  onSkinChangeEventArgs { chosenPanel = ShopItems[selectedId].panel, chosenSkin = ShopItems[selectedId].skinImage });
    }
}
