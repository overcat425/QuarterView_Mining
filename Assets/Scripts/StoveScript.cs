using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class StoveScript : MonoBehaviour
{
    UpgradeScript upgradeScript;
    public enum StoveType { Donut, Cake }
    public StoveType stoveType;
    PlayerHand playerHand;

    public int stoveDesserts;
    [SerializeField] Transform[] dessertsPlace = new Transform[4];      // 디저트 놓는곳
    public Stack<Transform> dessertsStack = new Stack<Transform>(); //디저트 스택(정보저장)
    [SerializeField] GameObject[] dessertPrefab;        // 디저트 프리팹
    [SerializeField] GameObject dessertBasket;          // 디저트 하이어라키
    public Transform[] playerBaskets;                  // 플레이어 하이어라키
    [SerializeField] GameObject placeTrans;

    [SerializeField] GameObject stoveStack;
    [SerializeField] GameObject[] stoveImg;
    int maxStove = 4;
    int speed;
    float timer;
    void Start()
    {
        playerHand = GameManager.instance.playerHand;
        //StartCoroutine("MakeBurger");
        for(int i = 0; i < dessertsPlace.Length; i++)
        {
            dessertsPlace[i] = placeTrans.transform.GetChild(i);
        }
        StartCoroutine(MakeDesserts(stoveType));
    }
    private void LateUpdate()
    {
        stoveStack.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 3f, 0));
        BurgerUi();
    }
    IEnumerator MakeDesserts( StoveType type)
    {
        GameObject prefab = type == StoveType.Donut ? dessertPrefab[0] : dessertPrefab[1];
        while (true)
        {
            speed = GameManager.instance.upgradeScript.bakeSpeed;
            yield return new WaitForSecondsRealtime(speed);
            if(dessertBasket.transform.childCount < maxStove)
            {
                GameObject dessert = Instantiate(prefab, new Vector3(transform.position.x, -3f, transform.position.z), Quaternion.identity, dessertBasket.transform); ;
                dessert.transform.DOJump(new Vector3(dessertsPlace[dessertBasket.transform.childCount-1].position.x, dessertsPlace[dessertBasket.transform.childCount-1].position.y, dessertsPlace[dessertBasket.transform.childCount-1].position.z), 1f, 1, 0.2f).SetEase(Ease.OutQuad);
                ItemData dessertItem = dessert.GetComponent<ItemData>(); ;
                dessertsStack.Push(dessertItem.transform);
            }
        }
    }
    //IEnumerator MakeBurger()
    //{
    //    while (true)
    //    {
    //        speed = GameManager.instance.upgradeScript.bakeSpeed;
    //        yield return new WaitForSecondsRealtime(speed);
    //        if (stoveDesserts < maxStove)
    //        {
    //            stoveDesserts++;
    //            stoveImg[stoveDesserts - 1].SetActive(true);
    //        }
    //    }
    //}
    void BurgerUi()
    {
        if (dessertsStack.Count > 0)
        {
            stoveStack.SetActive(true);
        }
        else if (dessertsStack.Count <= 0) stoveStack.SetActive(false);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        ////for(int  i = 0; i< maxStove; i++)
    //        ////{
    //        ////    if (i < stoveDesserts)
    //        ////    {
    //        ////        stoveImg[i].SetActive(true);
    //        ////    }else stoveImg[i].SetActive(false);
    //        ////}
    //        int maxDessert = 4;
    //        int remainEmpty = Mathf.Min(playerHand.maxPlayerDesserts - playerBasket.childCount, this.dessertsStack.Count);
    //        float above = stoveType == StoveType.Donut ? 0.08f : 0.12f;
    //        for (int i = 0; i < remainEmpty; i++)
    //        {
    //            Transform dessert = dessertsStack.Pop();
    //            dessert.transform.SetParent(playerBasket);
    //            Vector3 pos = Vector3.zero + Vector3.up * playerHand.playerHand.Count * above;
    //            dessert.transform.DOLocalJump(pos, 1.0f, 0, 0.5f);

    //            dessert.transform.localRotation = Quaternion.identity;
    //            playerHand.playerHand.Push(dessert.transform);

    //            maxDessert--;
    //            playerHand.isDessertHand = true;
    //        }
    //    }
    //}
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (dessertsStack.Count <= 0) return;   // 오븐에 아무것도 없으면 작동X
            int i = stoveType == StoveType.Donut ? 0 : 1;
            float above = stoveType == StoveType.Donut ? 0.08f : 0.12f;
            timer += Time.deltaTime;
            if (timer > 0.08f && playerBaskets[i].childCount < playerHand.maxPlayerDesserts)
            {
                Transform dessert = dessertsStack.Pop();
                dessert.SetParent(playerBaskets[i]);
                Vector3 pos = Vector3.zero + Vector3.up * playerHand.playerHands[i].Count * above;
                dessert.DOLocalJump(pos, 1.0f, 0, 0.3f);

                dessert.localRotation = Quaternion.identity;
                playerHand.playerHands[i].Push(dessert);

                playerHand.isDessertHand = true;
                timer = 0f;
            }
        }
    }
}
