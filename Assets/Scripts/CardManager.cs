using UnityEngine;
using strange.extensions.mediation.impl;

public class CardManager : View, ICardManager
{

    public CardInfo[] cards;

    public Texture2D cardBack;
    public Texture2D CardBack
    {
        get
        {
            return cardBack;
        }
    }

    public AudioClip playAudio;
	public AudioClip PlayAudio
	{
		get{
			return playAudio;
		}
	}

    public AudioClip burnAudio;
	public AudioClip BurnAudio
	{
		get{
			return burnAudio;
		}
	}

    public AudioClip storeAudio;
	public AudioClip StoreAudio
	{
		get{
			return storeAudio;
		}
	}

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public CardInfo SelectCard()
    {
        CardInfo info = null;

        if (cards.Length >= 1)
        {
            info = cards[Random.Range(0, cards.Length)];
        }

        return info;
    }

    public CardInfo SelectCard(string previousName)
    {
        CardInfo info = null;
		int limit = 20;
		int count = 0;

        if (cards.Length > 1)
        {
			info = cards[Random.Range(0, cards.Length)];
            while (info.Name.Equals(previousName))
            {
                info = cards[Random.Range(0, cards.Length)];
				if(count++ > limit)
				{
					break;
				}
            }
        }
        else
        {
            info = SelectCard();
        }

        return info;
    }
}
