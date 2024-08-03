using _Content.Data;
using _Content.InGame.Managers;
using Common.UI;
using TMPro;
using UnityEngine;

namespace _Content.InGame.UI.Main
{
    public class ChestUi : UIViewWrapper
    {
        public GameObject notEligible;
        public GameObject win;
        public GameObject success;
        public GameObject box1Open;
        public GameObject box2Open;
        public TMP_Text rewardText;
        public TMP_InputField plName;
        public TMP_InputField plEmail;
        public TMP_InputField plPhone;
        public TMP_InputField plCity;
        public TMP_InputField plPostal;
        public TMP_InputField plAddress;
        bool isBox1;

        public override void ShowView(bool force = false)
        {
            base.ShowView(force);
        }

        public void onBox1()
        {
            isBox1 = true;
            var lvl = PlayerData.Instance.LevelNumberThroughLoops;
            if (lvl >= 300 * PlayerData.Instance.LevelClaim+1)
            {
                rewardText.text = "You just won the following package-\r\n•T-shirt,\r\n•cap, \r\n•school bag,\r\n•power bank, \r\n•mouse, \r\n•wallet, \r\n•headphones and a \r\n•smart watch";

                box1Open.SetActive(true);
                box2Open.SetActive(false);
                win.SetActive(true);
                PlayerData.Instance.AddLvlClaim(1);
            }
            else
            {
                notEligible.SetActive(true);
            }
        }

        public void onBox2()
        {
            var coins = PlayerData.Instance.CoinsCount;
            isBox1 = false;
            if (coins >= 300000)
            {
                rewardText.text = "You just won the following package-\r\n•Sunglasses, \r\n•Water bottle, \r\n•Wireless mouse,\r\n•Wireless charger, \r\n•keyboard, \r\n•wallet, \r\n•VR headset, \r\n•Game controller, \r\n•Bluetooth speaker";

                box1Open.SetActive(false);
                box2Open.SetActive(true);
                win.SetActive(true);
                PlayerData.Instance.AddCoins(-300000);
            }
            else
            {
                notEligible.SetActive(true);
            }
        }

        public void Submit()
        {
            if(isValid(plName) && isValid(plEmail) && isValid(plPhone) && isValid(plCity) && isValid(plPostal) && isValid(plAddress))
            {
                FormData form = new FormData();
                form.plName = plName.text;
                form.plEmail = plEmail.text;
                form.plPhone = plPhone.text;
                form.plCity = plCity.text;
                form.plPostal = plPostal.text;
                form.plAddress = plAddress.text;
                form.box = isBox1 ? "Box 1" : "Box 2";
                form.status = "Pending";
                UIManager.Instance.LoadingUi.ShowView();
                BackendController.Instance.SubmitRequest(form, () =>
                {
                    UIManager.Instance.LoadingUi.HideView();
                    success.SetActive(true);
                    Debug.Log("Success");
                    isBox1 = false;
                });
            }
            else
            {
                UIManager.Instance.LoadingUi.HideView();
                Debug.Log("Please fill all fields");
            }
        }

        bool isValid(TMP_InputField inpf)
        {
            if (string.IsNullOrEmpty(inpf.text) || string.IsNullOrWhiteSpace(inpf.text))
                return false;
            else
                return true;
        }

        public void CloseButton()
        {
            GameManager.Instance.HideChestUI();
        }


    }
}

public class FormData
{
    public string plName;
    public string plEmail;
    public string plPhone;
    public string plCity;
    public string plPostal;
    public string plAddress;
    public string box;
    public string status;

}

