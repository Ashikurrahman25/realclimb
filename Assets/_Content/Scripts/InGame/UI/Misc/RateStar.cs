using UnityEngine;
using UnityEngine.EventSystems;

namespace _Content.InGame.UI.Misc
{
	public class RateStar: MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private int _ratingOnClick = 1;
		[SerializeField] private GameObject _fillImage;
		private RateController _rateController;

		public void Initialize(RateController rateController)
		{
			_rateController = rateController;
			SetFilling(false);
		}

		public void SetFilling(bool fill)
		{
			_fillImage.SetActive(fill);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_rateController?.SetRating(_ratingOnClick);
		}
	}
}