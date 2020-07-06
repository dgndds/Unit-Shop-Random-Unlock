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

        // Init list of locked skins by checking all skins
        InitShop();

        //Buttons are interactable if they are unlocked
        UpdateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        // Set color of selected panel to green
        ShopItems[selectedId].panel.GetComponent<Image>().color = greenColor;
    }

    public void Unlock() {
        StartCoroutine(ChoseRandomUnlocked());
    }

    
    private IEnumerator ChoseRandomUnlocked() {

        // Disable unlock button while random selection
        unlockBtn.interactable = false;

        //Random index where green canvas will land on each roll
        int randomIndex = -1;

        for (int i = 0; i < randomRollCount; i++){

            //Random Index chosen among locked skins
            randomIndex = UnityEngine.Random.Range(0, lockedShopItems.Count);

            //Update random index color to green
            lockedShopItems[randomIndex].panel.GetComponent<Image>().color = greenColor;

            //Wait for 0.5 seconds before reroll
            yield return new WaitForSeconds(.5f);

            //Change color back to grey
            lockedShopItems[randomIndex].panel.GetComponent<Image>().color = greyColor;
        }

        //Record last random index 
        int unlockedIndex = randomIndex;

        // Higlight old selected panel back to grey
        ShopItems[selectedId].panel.GetComponent<Image>().color = greyColor;

        //Pass the id of the selected panel
        selectedId = lockedShopItems[unlockedIndex].skinId;

        onSkinSelect(selectedId);

        // Change image to unlocked skin
        lockedShopItems[unlockedIndex].panel.GetChild(1).GetComponent<Image>().sprite = lockedShopItems[unlockedIndex].skinImage;

        //Unlock the skin
        lockedShopItems[unlockedIndex].unlocked = true;

        //Remove unlocked item from locked list
        lockedShopItems.RemoveAt(unlockedIndex);

        //Update unlocked buttons according change of shopskins
        UpdateButtons();

        //Enable the unlock button since done unlocking skin
        unlockBtn.interactable = true;
    }

    /* 
     * This method adds locked skins to locked list while shows unlocked skins
     * Can be used to load data (like in case of save manager)
     */
    private void InitShop(){
        for (int i = 0; i < ShopItems.Count; i++)
        {
            if (!ShopItems[i].unlocked)
                lockedShopItems.Add(ShopItems[i]);
            else
                ShopItems[i].panel.GetChild(1).GetComponent<Image>().sprite = ShopItems[i].skinImage;
            
                
        }
    }

    /*
     * This method enable/disables panel buttons according to unlock status of all shop items
     * By this user can only select unlocked skins
     */
    private void UpdateButtons(){
        for (int i = 0; i < ShopItems.Count; i++)
        {
            ShopItems[i].panel.GetComponent<Button>().interactable = ShopItems[i].unlocked;
        }
    }

     /*
     * This method is called when user selects a unlocked skin
     * index parameter is the index of the selected button (0-8)
     * Also invokes a skin change event which other scripts can subscribe to
     * Passes selected image and panel as event arguments
     * Subscribed scripts can use event args to change ingame skin
     * Event args can be expanded as need arises
     */
    private void onSkinSelect(int index)
    {
        ShopItems[selectedId].panel.GetComponent<Image>().color = greyColor;
        selectedId = index;

        onSkinChange?.Invoke(this, new  onSkinChangeEventArgs { chosenPanel = ShopItems[selectedId].panel, chosenSkin = ShopItems[selectedId].skinImage });
    }
}
