using System;
using DG.Tweening;
using UnityEngine;

namespace _Content.InGame.UI.Misc
{
	public class HasSkinAnimation: MonoBehaviour
	{
		private Sequence _seq;

		private void OnEnable()
		{
			_seq = DOTween.Sequence();
			_seq.Append(transform.DOLocalRotate(new Vector3(0f, 0f, -15f), 0.2f));
			_seq.Append(transform.DOLocalRotate(new Vector3(0f, 0f, 15f), 0.4f));
			_seq.Append(transform.DOLocalRotate(new Vector3(0f, 0f, 0), 0.2f));
			_seq.AppendInterval(2f);
			_seq.SetAutoKill(false);
			_seq.SetLoops(-1, LoopType.Restart);
		}

		private void OnDisable()
		{
			_seq.Kill();
		}
	}
}