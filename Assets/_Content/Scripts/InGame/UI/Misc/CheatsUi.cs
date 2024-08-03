using _Content.Data;
using _Content.InGame.Managers;
using Common.UI;
using UnityEngine;

namespace _Content.InGame.UI.Misc
{
	public class CheatsUi : UIViewWrapper
	{
		[SerializeField] private CanvasGroup _mainCanvas;
		private int _counter;
		private bool _canShowView;

		protected override void OnAwake()
		{
			base.OnAwake();
			_canShowView = gameObject.activeSelf;
		}

		public override void ShowView(bool force = false)
		{
			if (_canShowView)
				base.ShowView(force);
		}

		public void OnTap()
		{
			_counter++;
			if (_counter >= 3)
			{
				_counter = 0;
				if (_mainCanvas.alpha > 0.99f)
					_mainCanvas.alpha = 0f;
				else
				{
					_mainCanvas.alpha = 1f;
				}
			}
		}

		public void ClearData()
		{
			GameManager.Instance.ClearData();
		}

		public void SkipLvl()
		{
			GameManager.Instance.SkipLevel();
		}

		public void AddCoins()
		{
			PlayerData.Instance.AddCoins(100);
		}
	}
}