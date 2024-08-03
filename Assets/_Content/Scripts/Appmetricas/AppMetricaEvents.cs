using System.Collections.Generic;
using _Content.Data;

namespace _Content.Appmetricas
{
	public class AppMetricaEvents
	{
		public static void SendPurchaseEvent(string inappId, string currency, string price /*, InAppCategory category*/)
		{
			//var stringCategory = GetInappCategoryString(category);

			var parameters = new Dictionary<string, object>();
			parameters.Add("inapp_id", inappId);
			parameters.Add("currency", currency);
			parameters.Add("price", price);
			//parameters.Add("inapp_type", stringCategory);
			//AppMetrica.Instance.ReportEvent("payment_succeed", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}

		public static void SendRateUs(int rateResult)
		{
			var parameters = new Dictionary<string, object>();
			var reason = PlayerData.Instance.RateUsAttempts <= 1 ? "new_player" : "retry";
			parameters.Add("show_reason", reason);
			parameters.Add("rate_result", rateResult);
			//AppMetrica.Instance.ReportEvent("rate_us", parameters);
		}

		public static void SendLevelStartEvent(int levelNumber, int levelCount, string levelName, int levelLoop)
		{
			var parameters = new Dictionary<string, object>();
			parameters.Add("level_number", levelNumber);
			parameters.Add("level_name", ProcessString(levelName));
			parameters.Add("level_count", levelCount);
			parameters.Add("level_type", "normal");
			parameters.Add("level_diff", "medium");
			parameters.Add("level_loop", 1);
			parameters.Add("level_random", levelLoop > 1);
			//AppMetrica.Instance.ReportEvent("level_start", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}
		
		public static void SkinUnlock(string skinName)
		{
			var parameters = new Dictionary<string, object>();
			parameters.Add("skin_type", "hero");
			parameters.Add("skin_name", ProcessString(skinName));
			parameters.Add("skin_rarity", "common");
			parameters.Add("unlock_type", ProcessString("specific_currency"));
			//AppMetrica.Instance.ReportEvent("skin_unlock", parameters);
		}

		public static void SendLevelEndEvent(int levelNumber, int levelCount, string levelName, int levelLoop,
			LevelResult levelResult,
			int spentSeconds, int progress, bool keyObtained)
		{
			var parameters = new Dictionary<string, object>();
			parameters.Add("level_number", levelNumber);
			parameters.Add("level_name", ProcessString(levelName));
			parameters.Add("level_count", levelCount);
			parameters.Add("level_type", "normal");
			parameters.Add("level_diff", "medium");
			parameters.Add("level_loop", 1);
			parameters.Add("level_random", levelLoop > 1);

			parameters.Add("result", GetLevelResult(levelResult));
			parameters.Add("time", spentSeconds);
			parameters.Add("progress", progress);
			parameters.Add("key_obtained", keyObtained);
			//AppMetrica.Instance.ReportEvent("level_finish", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}

		public static void SendTutorialStage(int stage)
		{
			var parameters = new Dictionary<string, object>();
			parameters.Add("stepNumber", stage);
			parameters.Add("step", $"stage_{stage}");

			//AppMetrica.Instance.ReportEvent("tutorial", parameters);
		}
		
		public static void SendUnlockLevel(int levelId, string levelName, int levelCount)
		{
			var parameters = new Dictionary<string, object>();
			parameters.Add("level_id", levelId);
			parameters.Add("level_name", ProcessString(levelName));
			parameters.Add("level_count", levelCount);

			//AppMetrica.Instance.ReportEvent("unlock_level", parameters);
		}

		private static string ProcessString(string levelName)
		{
			levelName = levelName.Replace(' ', '_');
			return levelName.ToLowerInvariant();
		}

		public static void SendTutorialEvent(string name)
		{
			var parameters = new Dictionary<string, object>();
			parameters.Add("step_name", name);
			//AppMetrica.Instance.ReportEvent("tutorial", parameters);
			//AppMetrica.Instance.SendEventsBuffer();
		}

		private static string GetLevelResult(LevelResult levelResult)
		{
			switch (levelResult)
			{
				case LevelResult.Win: return "win";
				case LevelResult.Lose: return "lose";
				case LevelResult.Restart: return "restart";
				case LevelResult.ReturnToMenu: return "return_to_menu";
				case LevelResult.SkipLevel: return "skip_level";
				default: return "win";
			}
		}
	}
}