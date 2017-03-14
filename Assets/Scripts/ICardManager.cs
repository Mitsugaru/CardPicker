using UnityEngine;
public interface ICardManager {

	CardInfo SelectCard();

	CardInfo SelectCard(string previousName);

	void ShuffleDeck();

	Texture2D CardBack { get; }

	AudioClip PlayAudio { get; }

	AudioClip BurnAudio { get; }
	
	AudioClip StoreAudio { get; }
}
