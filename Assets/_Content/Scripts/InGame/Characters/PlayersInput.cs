using _Content.InGame.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Content.InGame.Characters
{
	public class PlayersInput : MonoBehaviour
	{
		[SerializeField] private Transform _parentTransform;
		[SerializeField] private Slider _slider;
		[SerializeField] private RectTransform _circle;
		[SerializeField] private Vector2 _scaleByRatio = new Vector2(0.1f, 1f);
		private bool _shown;

		private void Awake()
		{
			_parentTransform.localScale = Vector3.zero;
		}

		public void Update()
		{
			if (!_shown)
				return;

			var angle = UIManager.Instance.PlayersInputUi.LastAngle;
			var ratio = UIManager.Instance.PlayersInputUi.LastRatio;
			_slider.value = ratio;
			_parentTransform.rotation = Quaternion.Euler(0f, 0f, angle);
			_circle.localScale = Vector3.one * Mathf.Lerp(_scaleByRatio.x, _scaleByRatio.y, ratio) ;
		}

		public void Show()
		{
			_parentTransform.DOKill();
			_parentTransform.DOScale(Vector3.one, 0.15f)
				.SetEase(Ease.OutBack);
			_shown = true;
		}

		public void Hide()
		{
			_parentTransform.DOKill();
			_parentTransform.DOScale(Vector3.zero, 0.15f);
			_shown = false;
		}
	}
}