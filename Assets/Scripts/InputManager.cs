using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

public class InputManager : View, IInputManager
{
    /// <summary>
    /// Card Manager reference
    /// </summary>
    /// <returns>Card Manager service</returns>
    [Inject]
    public ICardManager CardManager { get; set; }

    [Inject]
    public IEventManager EventManager { get; set; }

    public float accelerometerUpdateInterval = 1.0f / 60.0f;

    [Tooltip("The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).")]
    public float lowPassKernelWidthInSeconds = 1.0f;

    /// <summary>
    /// This next parameter is initialized to 2.0 per Apple's recommendation, or at least according to Brady! ;) 
    /// </summary>
    [Tooltip("The threshold before a shake is detected")]
    public float shakeDetectionThreshold = 2.0f;

    [Tooltip("The percent amount that the image needs to be dragged before a swipe action occurs.")]
    [Range(0.1f, 0.9f)]
    public float dragForActionThreshold = 0.4f;

    /// <summary>
    /// The Drawn Card UI game object reference
    /// </summary>
    public GameObject DrawnCard;

    /// <summary>
    /// The card deck image
    /// </summary>
    public GameObject DeckBack;

    /// <summary>
    /// Detail panel reference
    /// </summary>
    public GameObject DetailPanel;

    /// <summary>
    /// Detail panel name text panel reference 
    /// </summary>
    public Text NameText;

    /// <summary>
    /// Detail panel description text panel reference 
    /// </summary>
    public Text DescriptionText;

    public GameObject DebugPanel;

    private RawImage deckBackImage;

    private RawImage drawnCardImage;

    private Vector3 drawnCardPos;

    /// <summary>
    /// The low pass value
    /// </summary>
    private Vector3 lowPassValue = Vector3.zero;

    private Vector3 dragPos = Vector3.zero;

    private bool dragging = false;

    private Text debugText;

    private Animator deckAnimator;

    private Animator drawnAnimator;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        drawnCardImage = DrawnCard.GetComponent<RawImage>();
        drawnCardPos = new Vector3(DrawnCard.transform.position.x, DrawnCard.transform.position.y, DrawnCard.transform.position.z);
        drawnCardImage.enabled = false;

        deckBackImage = DeckBack.GetComponent<RawImage>();
        deckBackImage.texture = CardManager.CardBack;
        DeckBack.SetActive(true);

        deckAnimator = DeckBack.GetComponent<Animator>();
        drawnAnimator = DrawnCard.GetComponent<Animator>();

        debugText = DebugPanel.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchSupported)
        {
            HandleTouch();
        }
        else
        {
            HandleMouseAndKeyboard();
        }

        HandleDeviceShake();
    }

    private void HandleMouseAndKeyboard()
    {
        // Mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (!drawnCardImage.enabled)
            {
                DrawCard();
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            ClearCard();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!drawnCardImage.enabled)
            {
                DrawCard();
            }
            else
            {
                RedrawCard();
            }
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            if (drawnCardImage.enabled)
            {
                //Toggle detail panel
                DetailPanel.SetActive(!DetailPanel.activeInHierarchy);
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            ClearCard();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ClearCard();
            CardManager.ShuffleDeck();
        }
    }

    private void HandleTouch()
    {
        // Look for all fingers
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // -- Tap: quick touch & release
            // ------------------------------------------------
            if (touch.phase == TouchPhase.Ended && touch.tapCount == 1 && !dragging)
            {
                if (drawnCardImage.enabled)
                {
                    //Toggle detail panel
                    DetailPanel.SetActive(!DetailPanel.activeInHierarchy);
                }
                else
                {
                    DrawCard();
                }
            }
            else
            {
                // -- Drag
                // ------------------------------------------------
                if (touch.phase == TouchPhase.Began)
                {
                    dragPos = touch.position;
                    break;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    dragging = true;
                    DrawnCard.transform.position += -(dragPos - (Vector3)touch.position);
                    dragPos = touch.position;
                    debugText.text = Screen.width + ":" + Screen.height + " relative to " + (drawnCardPos - DrawnCard.transform.position).ToString();
                    break;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    // figure out if there was a swipe action, the direction of this swipe, and what to do
                    Vector3 dragDiff = drawnCardPos - DrawnCard.transform.position;
                    float xDiff = Mathf.Abs(dragDiff.x) / Screen.width;
                    float yDiff = Mathf.Abs(dragDiff.y) / Screen.height;
                    //First we figure out, if a diagonal swipe, which one direction precidence by percentage
                    if (xDiff > yDiff)
                    {
                        //We swiped  horizontally
                        if (xDiff > dragForActionThreshold)
                        {
                            if (dragDiff.x > 0)
                            {
                                debugText.text = "Spread";
                                ClearCard();
                                if (CardManager.StoreAudio != null)
                                {
                                    EventManager.Raise(new PlayAudioEvent(CardManager.StoreAudio));
                                }
                            }
                            else
                            {
                                debugText.text = "Redraw";
                                RedrawCard();
                            }
                        }
                    }
                    else if (yDiff > dragForActionThreshold)
                    {
                        //We swiped vertically
                        if (drawnCardImage.enabled)
                        {
                            if (dragDiff.y > 0)
                            {
                                //South
                                //TODO burn + cooldown
                                debugText.text = "Burn";
                                ClearCard();
                                if (CardManager.BurnAudio != null)
                                {
                                    EventManager.Raise(new PlayAudioEvent(CardManager.BurnAudio));
                                }
                            }
                            else
                            {
                                //North
                                //TODO cooldown
                                debugText.text = "Play";
                                ClearCard();
                                if (CardManager.PlayAudio != null)
                                {
                                    EventManager.Raise(new PlayAudioEvent(CardManager.PlayAudio));
                                }
                            }
                        }
                        else
                        {
                            DrawCard();
                        }
                    }
                    //Reset drag stats
                    DrawnCard.transform.position = drawnCardPos;
                    dragPos = Vector3.zero;
                    dragging = false;
                    break;
                }
            }
        }
    }

    private void HandleDeviceShake()
    {
        float lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;
        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold && !drawnCardImage.enabled)
        {
            // Perform your "shaking actions" here, with suitable guards in the if check above, if necessary to not, to not fire again if they're already being performed.
            DrawCard();
        }
    }

    public void DrawCard()
    {
        CardInfo card = CardManager.SelectCard();
        if (card != null)
        {
            drawnCardImage.texture = card.Front;
            NameText.text = card.Name;
            DescriptionText.text = card.Description;
            drawnCardImage.enabled = true;
            deckAnimator.ResetTrigger("Reset");
            drawnAnimator.ResetTrigger("Reset");
            deckAnimator.SetTrigger("Draw");
            drawnAnimator.SetTrigger("Draw");
            if (card.drawAudio != null)
            {
                EventManager.Raise(new PlayAudioEvent(card.drawAudio));
            }
        }
    }

    public void RedrawCard()
    {
        if (drawnCardImage.enabled)
        {
            drawnCardImage.enabled = false;
            DetailPanel.SetActive(false);
            deckAnimator.SetTrigger("Reset");
            drawnAnimator.SetTrigger("Reset");
            deckAnimator.ResetTrigger("Draw");
            drawnAnimator.ResetTrigger("Draw");
            CardInfo card = CardManager.SelectCard(NameText.text);
            if (card != null)
            {
                deckAnimator.SetTrigger("Draw");
                drawnAnimator.SetTrigger("Draw");
                drawnCardImage.texture = card.Front;
                NameText.text = card.Name;
                DescriptionText.text = card.Description;
                AudioClip audio = null;
                if (card.redrawAudio != null)
                {
                    audio = card.redrawAudio;
                }
                else if (card.drawAudio != null)
                {
                    audio = card.drawAudio;
                }
                if (audio != null)
                {
                    EventManager.Raise(new PlayAudioEvent(audio));
                }
            }
        }
        else
        {
            DrawCard();
        }
    }

    public void ClearCard()
    {
        drawnCardImage.enabled = false;
        DetailPanel.SetActive(false);
        deckAnimator.ResetTrigger("Draw");
        drawnAnimator.ResetTrigger("Draw");
        deckAnimator.SetTrigger("Reset");
        drawnAnimator.SetTrigger("Reset");
    }

}
