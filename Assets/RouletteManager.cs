using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // LoadSceneを使用するために必要

public class RouletteManager : MonoBehaviour
{
    // ここから

    [HideInInspector]
    [SerializeField]
    Transform tf;

    // ルーレット本体部分のRectTransform
    [HideInInspector]
    [SerializeField]
    RectTransform elementsRt;

    [Header("白い円形の画像を紐付け")]
    [SerializeField]
    Sprite circleSprite;

    [Header("ルーレットの針の画像を紐付け")]
    [SerializeField]
    Sprite clapperSprite;

    [Header("ルーレットの要素数")]
    [SerializeField]
    [Range(2, 100)]
    int elementCount = 6;

    [Header("ルーレットの要素の色を指定 (要素数と配列数を同じにしてください)")]
    [SerializeField]
    Color[] elementColors = {
        new Color32(255, 0, 0, 255),
        new Color32(255, 128, 0, 255),
        new Color32(255, 255, 0, 255),
        new Color32(128, 255, 0, 255),
        new Color32(0, 255, 128, 255),
        new Color32(0, 255, 255, 255),
    };

    //[Header("ルーレットの画像サイズ")]
    //[SerializeField]
    //[Range(100, 1024)]
    //float rouletteSize = 512;

    //[Header("ルーレットの針の画像サイズ")]
    //[SerializeField]
    //[Range(10, 128)]
    //float clapperSize = 64;

    [HideInInspector]
    [SerializeField]
    // パイの大きさ
    float elementRotationToAdd;

    [HideInInspector]
    [SerializeField]
    float elementFillAmount;

    //コルーチン管理用。
    Coroutine spin;

    [Header("結果のインデックス (elementCountが6の場合、0～5)")]
    public int resultElementIndex;

    [Header("(回転終了時の)慣性の長さ (秒)")]
    [SerializeField]
    [Range(0.1f, 10.0f)]
    float inertiaDuration = 0.5f;

    float inertiaElapsedTime;


    //要素に入れる文字関係。

    //フォントを指定しない場合はデフォルトの物が使われる。
    [Header("文字のフォント")]
    [SerializeField]
    TMP_FontAsset font;

    [Header("要素毎に入れる文字のサイズ")]
    [SerializeField]
    [Range(16, 128)]
    int fontSize = 64;

    [Header("文字の色")]
    [SerializeField]
    Color tmpColor = new Color32(255, 255, 255, 255);

    // メニュー画面から、ルーレット要素に表示させる配列を表示させる
    [Header("各要素に表示するテキスト (要素数と配列数を同じにしてください)")]
    [SerializeField]
    string[] tmpTexts = new string[] {
        "通行人A",
        "通行人B",
        "通行人C",
        "通行人D",
        "通行人E",
        "通行人F",
    };

    //ルーレット生成時に一時的に使用。
    GameObject elementGo;
    RectTransform elementRt;
    Image elementImg;

    GameObject elementTmpGo;
    RectTransform elementTmpRt;
    TextMeshProUGUI elementTmp;



    void Awake()
    {
        if (tf == null)
        {
            tf = transform;
        }

        if (tf.childCount == 0)
        {
            CreateRoulette();
        }
    }


    //ルーレットの回転をスタートしたい時にコレを呼ぶ。
    public void StartSpin()
    {
        //一応、多重実行を防止。
        if (spin != null)
        {
            StopCoroutine(spin);
        }

        spin = StartCoroutine(Spin());
    }


    IEnumerator Spin()
    {
        while (true)
        {

            elementsRt.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);


            //画面をタップ(クリック)で、徐々に停止。
            if (Input.GetMouseButtonDown(0))
            {
                inertiaElapsedTime = 0;

                while (true)
                {
                    inertiaElapsedTime += Time.deltaTime;

                    elementsRt.Rotate(Vector3.forward * Mathf.Lerp(spinSpeed, 0, inertiaElapsedTime / inertiaDuration) * Time.deltaTime);


                    if (inertiaDuration <= inertiaElapsedTime)
                    {

                        resultElementIndex = Mathf.FloorToInt(elementsRt.eulerAngles.z / elementRotationToAdd % elementCount);

                        //ここでresultElementIndexに応じた処理を入れる。
                        //止まった領域の文字はtmpTexts[resultElementIndex]で取得可能。

                        spin = null;
                        yield break;
                    }

                    yield return null;
                }
            }

            yield return null;
        }
    }


    //Createボタンを押すと、各種設定に応じてルーレットと針を自動生成。
    public void CreateRoulette()
    {
        if (circleSprite == null || clapperSprite == null)
        {
            print("インスペクターから各Spriteを紐付けしてください！");
            return;
        }

        if (tf == null)
        {
            tf = transform;
        }

        // 対象人数に応じ、パイの角度を決定
        elementRotationToAdd = 360.0f / elementCount;
        // これ何？
        elementFillAmount = 1.0f / elementCount;


        elementGo = new GameObject("RouletteElements");
        elementsRt = elementGo.AddComponent<RectTransform>();
        elementsRt.SetParent(tf);
        elementsRt.localScale = Vector3.one;
        elementsRt.anchoredPosition = Vector2.zero;
        elementsRt.sizeDelta = new Vector2(rouletteSize, rouletteSize);


        // パイを生成→ルーレット完成
        for (int i = 0; i < elementCount; i++)
        {
            elementGo = new GameObject(System.String.Format("RouletteElement{0}", i + 1));
            elementRt = elementGo.AddComponent<RectTransform>();
            elementRt.SetParent(elementsRt);
            elementRt.localScale = Vector3.one;
            elementRt.anchoredPosition = Vector2.zero;
            elementRt.sizeDelta = new Vector2(rouletteSize, rouletteSize);


            elementTmpGo = new GameObject(System.String.Format("Tmp{0}", i + 1));
            elementTmpRt = elementTmpGo.AddComponent<RectTransform>();
            elementTmpRt.SetParent(elementRt);
            elementTmpRt.localScale = Vector3.one;
            elementTmpRt.anchoredPosition = new Vector2(0, rouletteSize / 4 + fontSize / 2);
            elementTmpRt.RotateAround(elementsRt.position, Vector3.back, elementRotationToAdd / 2);
            elementTmp = elementTmpGo.AddComponent<TextMeshProUGUI>();

            if (font != null)
            {
                elementTmp.font = font;
            }

            elementTmp.fontSize = fontSize;
            elementTmp.enableWordWrapping = false;
            elementTmp.alignment = TextAlignmentOptions.Center;
            //一応、RaycastTargetと、リッチテキストを無効化している(使う場合は戻してください)。
            elementTmp.raycastTarget = false;
            elementTmp.richText = false;


            if (i < tmpTexts.Length)
            {
                elementTmp.text = tmpTexts[i];
            }
            elementTmp.color = tmpColor;

            elementRt.Rotate(Vector3.back * (elementRotationToAdd * i));


            elementImg = elementGo.AddComponent<Image>();
            elementImg.sprite = circleSprite;
            elementImg.type = Image.Type.Filled;
            elementImg.fillMethod = Image.FillMethod.Radial360;
            elementImg.fillOrigin = (int)Image.Origin360.Top;
            elementImg.fillAmount = elementFillAmount;
            //一応、RaycastTargetを無効化している(使う場合は戻してください)。
            elementImg.raycastTarget = false;

            if (i < elementColors.Length)
            {
                elementImg.color = elementColors[i];
            }
        }


        elementGo = new GameObject("Clapper");
        elementRt = elementGo.AddComponent<RectTransform>();
        elementRt.SetParent(tf);
        elementRt.localScale = Vector3.one;
        elementRt.anchoredPosition = new Vector2(0, rouletteSize / 2);
        elementRt.sizeDelta = new Vector2(clapperSize, clapperSize);

        elementImg = elementGo.AddComponent<Image>();
        elementImg.sprite = clapperSprite;
    }


    // ここまで


    float rotSpeed = 0; // 回転速度
    Boolean isRightClicked = false; //右クリックチェック
    Boolean isStopped = false; //停止チェック


    // Start is called before the first frame update
    // Startメソッド：スクリプト起動直後に一度だけ実行
    void Start()
    {
        // フレームレートを60に固定(デバイスによる性能差をなくす)
        Application.targetFrameRate = 60;


    }

    // Update is called once per frame
    // Updateメソッド：フレームごとに実行
    void Update()
    {
        // マウスで左クリックされたらルーレットを回す
        if (Input.GetMouseButtonDown(0) && this.rotSpeed == 0)
        {
            // 回転速度をセット
            setRotSpeed();
        }

        // 回転速度ぶん、ルーレットを回転させる
        transform.Rotate(0, 0, this.rotSpeed);


        // マウスで右クリックされたらルーレットを減速→停止
        if (Input.GetMouseButtonDown(1))
        {
            this.isRightClicked = true;
        }
        if (isRightClicked == true)
        {
            SlowDown();
        }

        // ルーレット再開
        if (isStopped == true && Input.GetMouseButtonDown(0))
        {
            this.isRightClicked = false;
            setRotSpeed();
        }

    }


    void setRotSpeed()
    {
        this.rotSpeed = 30;
    }

    void SlowDown()
    {
        if (this.rotSpeed >= 0.1)
        {
            // ルーレットを減速させていく
            this.rotSpeed *= 0.975f;

        }
        else
        {
            isStopped = true;
            this.rotSpeed = 0;
        }
    }


    //[SerializeField] private Text hyouji;//Text型の変数を宣言

    ////void Start()
    ////{
    ////}

    ////ボタンを押したら実行する関数　実行するためにはボタンへ関数登録が必要
    ////int型の引数numberを宣言
    //public void Push_Button(int number)
    //{
    //    hyouji.text = "" + number;//suuji_textに引数の数値を代入
    //}


    // 以下未実装
    //// 「もう一度」を押下したらシーンをリロード
    //public void RetryButtonDown()
    //{
    //    GameObject director = GameObject.Find("GameDirector");
    //    director.GetComponent<GameDirector>().RetryButtonDown();
    //}
}
