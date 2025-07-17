public static class ValueParser 
{
	private static string[] names = { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az"};
    
	public static string Parse(double digit)
	{
		if(digit <= 0) return "0";
		if (digit < 1000) return digit.ToString("#");

		int n = 0;
		while (n + 1 < names.Length && digit >= 1000)
		{
			digit /= 1000;
			n++;
		}
		return digit.ToString("#.##") + names[n];
	}

	public static string UnParse(string val)
	{
		char suffix=val[val.Length - 1];
		float multiplier = 0;
		switch(suffix)
		{
		case 'K':
			multiplier = 1000;
			break;
		case 'M':
			multiplier = 1000000;
			break;
		case 'B':
			multiplier = 1000000000;
			break;
		case 'T':
			multiplier = 1000000000000;
			break;
		}
		if (multiplier != 0)
		{
			val = val.Remove(val.Length - 1);
			float res= float.Parse(val) * multiplier;
			return res.ToString();
		}
		else
		{
			return float.Parse(val).ToString();
		}
	}

	public static float CalculatePercent(float value,float percentValue) =>(value*percentValue)/ 100;
}