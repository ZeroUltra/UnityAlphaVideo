using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 对话框管理
/// </summary>
public class DialogMgr : Singleton<DialogMgr>
{
    private GameObject btnOnePrefab, btnTwoPrefab;
    private Transform mainCanvas;
    private bool init = false;
    private void Init()
    {
        if (init) return;
        init = true;
        mainCanvas = FindObjectOfType<Canvas>().transform;
        btnOnePrefab = Resources.Load<GameObject>("Go/Dialog/BtnOne");
        btnTwoPrefab = Resources.Load<GameObject>("Go/Dialog/BtnTwo");
    }
    public void ShowDialogTypeBtnOne(string msg, string title = null, string btnTip = "OK", UnityAction btnClickAct = null)
    {
        Init();
        RectTransform rectTransform = GameObject.Instantiate(btnOnePrefab).transform as RectTransform;
        rectTransform.SetParent(mainCanvas, false);
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        rectTransform.SetAsLastSibling();

        rectTransform.Find("txtInfo").GetComponent<Text>().text = msg;
        rectTransform.Find("txtTitle").GetComponent<Text>().text = title;

        Button btn = rectTransform.Find("Btn").GetComponent<Button>();
        btn.GetComponentInChildren<Text>().text = btnTip;
        btn.onClick.AddListener(() =>
        {
            btnClickAct?.Invoke();
            rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => Destroy(rectTransform.gameObject));
        });
    }
    public void ShowDialogTypeBtnTwo(string msg, string title=null, string btnOneTip = "OK", string btnTwoTip = "Cancel",UnityAction btnOneClickAct = null, UnityAction btnTwoClickAct = null)
    {
        Init();
        RectTransform rectTransform = GameObject.Instantiate(btnTwoPrefab).transform as RectTransform;
        rectTransform.SetParent(mainCanvas, false);
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        rectTransform.SetAsLastSibling();
        rectTransform.Find("txtInfo").GetComponent<Text>().text = msg;
        rectTransform.Find("txtTitle").GetComponent<Text>().text = title;

        Button btnleft= rectTransform.Find("btnleft").GetComponent<Button>();
        btnleft.GetComponentInChildren<Text>().text = btnOneTip;
        btnleft.onClick.AddListener(() =>
        {
            btnOneClickAct?.Invoke();
            rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => Destroy(rectTransform.gameObject));
        });

        Button btnright = rectTransform.Find("btnright").GetComponent<Button>();
        btnright.GetComponentInChildren<Text>().text = btnTwoTip;
        btnright.onClick.AddListener(() =>
        {
            btnTwoClickAct?.Invoke();
            rectTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => Destroy(rectTransform.gameObject));
        });
    }
}
