using UnityEngine;
public interface ICardManager {

	CardInfo SelectCard();

	Texture2D CardBack { get; }
}
