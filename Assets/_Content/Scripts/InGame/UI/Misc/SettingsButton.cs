using _Content.InGame.Managers;
using Common.UI;

namespace _Content.InGame.UI.Misc
{
	public class SettingsButton: UIViewWrapper
	{
		public void OnSettingsButtonDown()
		{
			GameManager.Instance.ShowSettings();
		}
	}
}