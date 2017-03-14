using UnityEngine;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

public class CardManager : View, ICardManager
{

    public CardInfo[] cards;

    public bool runThroughDeck;

    private CardInfo[] currentCards;

    private List<CardInfo> deckInPlay = new List<CardInfo>();

    private System.Random randomizer = new System.Random();

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
        currentCards = cards;
        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCards != cards)
        {
            currentCards = cards;
            ShuffleDeck();
        }
    }

    public CardInfo SelectCard()
    {
        CardInfo info = null;

        if(runThroughDeck)
        {
            info = RetrieveFromDeck();
        }
        else if (cards.Length >= 1)
        {
            info = cards[Random.Range(0, cards.Length)];
        }

        return info;
    }

    public CardInfo SelectCard(string previousName)
    {
        CardInfo info = null;
        if(runThroughDeck)
        {
            info = RetrieveFromDeck();
        }
        else
        {
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
        }

        return info;
    }

    public void ShuffleDeck()
    {
        deckInPlay.Clear();
        foreach(CardInfo card in currentCards)
        {
            deckInPlay.Add(card);
        }
    }

    private CardInfo RetrieveFromDeck()
    {
        CardInfo info = null;
        if(deckInPlay.Count == 0)
        {
            ShuffleDeck();
        } 
        if(deckInPlay.Count == 1)
        {
            info = deckInPlay[0];
            deckInPlay.RemoveAt(0);
        } 
        else
        {
            int index = randomizer.Next(deckInPlay.Count);
            info = deckInPlay[index];
            deckInPlay.RemoveAt(index);
        }
        return info;
    }
}
