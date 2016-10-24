using UnityEngine;
using strange.extensions.mediation.impl;

public class CardManager : View, ICardManager {

	public CardInfo[] cards;

	public Texture2D cardBack;
	public Texture2D CardBack
	{
		get
		{
			return cardBack;
		}
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public CardInfo SelectCard()
	{
		CardInfo info = null;

		if(cards.Length >= 1)
		{
			info = cards[Random.Range(0, cards.Length)];
		}

		return info;
	}
}
