public static class TextProcessor
{
	public static string GetAmount(int amount)
	{
		if (amount < 1000)
			return amount.ToString();

		if (amount < 1000000)
		{
			var realAmount = amount / 1000f;
			var format = "N2";
			if (amount >= 100000)
				format = "N0";
			else if (amount >= 10000)
				format = "N1";
			
			/*var kilo = amount / 1000;
			var rest = amount % 1000;
			rest = rest / 10;*/
			
			return $"{realAmount.ToString(format)}K";
		}

		return amount.ToString();
	}
}