using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject warriorUpgradeOneObject, warriorUpgradeTwoObject, lancerUpgradeOneObject, lancerUpgradeTwoObject,
        ghostUpgradeOneObject, ghostUpgradeTwoObject;
    [SerializeField]private TextMeshProUGUI upgradeCoinObject;

    public void Awake()
    {
        if(ProgressManager.Instance.warriorUpdgrade1) 
        {
            warriorUpgradeOneObject.SetActive(false);
            ProgressManager.Instance.upgradeCoin--;
        }

        if (ProgressManager.Instance.warriorUpdgrade2)
        {
            warriorUpgradeTwoObject.SetActive(false);
            ProgressManager.Instance.upgradeCoin--;
        }

        if (ProgressManager.Instance.lancerUpdgrade1)
        {
            lancerUpgradeOneObject.SetActive(false);
            ProgressManager.Instance.upgradeCoin--;
        }

        if (ProgressManager.Instance.lancerUpdgrade2)
        {
            lancerUpgradeTwoObject.SetActive(false);
            ProgressManager.Instance.upgradeCoin--;
        }

        if (ProgressManager.Instance.ghostUpdgrade1)
        {
            ghostUpgradeOneObject.SetActive(false);
            ProgressManager.Instance.upgradeCoin--;
        }

        if (ProgressManager.Instance.ghostUpdgrade2)
        {
            ghostUpgradeTwoObject.SetActive(false);
            ProgressManager.Instance.upgradeCoin--;
        }

        upgradeCoinObject.text = ProgressManager.Instance.upgradeCoin.ToString();
    }

    public void warriorUpgradeOne()
    {
        upgradeButtonGeneric(ref ProgressManager.Instance.warriorUpdgrade1, warriorUpgradeOneObject);
    }

    public void warriorUpgradeTwo()
    {
        upgradeButtonGeneric(ref ProgressManager.Instance.warriorUpdgrade2, warriorUpgradeTwoObject);
    }

    public void lancerUpgradeOne()
    {
        upgradeButtonGeneric(ref ProgressManager.Instance.lancerUpdgrade1, lancerUpgradeOneObject);
    }

    public void lancerUpgradeTwo()
    {
        upgradeButtonGeneric(ref ProgressManager.Instance.lancerUpdgrade2, lancerUpgradeTwoObject);
    }

    public void ghostUpgradeOne()
    {
        upgradeButtonGeneric(ref ProgressManager.Instance.ghostUpdgrade1, ghostUpgradeOneObject);
    }

    public void ghostUpgradeTwo()
    {
        upgradeButtonGeneric(ref ProgressManager.Instance.ghostUpdgrade2, ghostUpgradeTwoObject);
    }

    public void upgradeButtonGeneric(ref bool upgrade, GameObject upgradeObject)
    {
        if (ProgressManager.Instance.upgradeCoin == 0)
        {
            return;
        }

        upgrade = true;
        upgradeObject.SetActive(false);
        ProgressManager.Instance.upgradeCoin--;
        upgradeCoinObject.text = ProgressManager.Instance.upgradeCoin.ToString();
        DataManager.Instance.createProgressSave();
    }

    public void returnButton()
    {
        SceneManager.UnloadSceneAsync("Shop Menu");
    }
}
