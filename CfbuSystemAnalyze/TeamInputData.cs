namespace CfbuSystemAnalyze
{
	using System;
	using System.Globalization;

	internal class TeamInputData
	{
		private readonly string leagueCode;
		public int Round { get; private set; }
		private DateTime date;
		public string FirstTeam { get; private set; }
		public string SecondTeam { get; private set; }
		public string Organiser { get; private set; }

		internal TeamInputData(string inputLine)
		{
			if (string.IsNullOrEmpty(inputLine))
			{
				throw new NotSupportedException("inputLine");
			}

			var splitInputLine = inputLine.Split('\t');
			this.leagueCode = splitInputLine[0];
			this.Round = int.Parse(splitInputLine[1]);

			this.date = DateTime.Parse(splitInputLine[2]) + TimeSpan.Parse(splitInputLine[3]);

			this.FirstTeam = splitInputLine[4];
			this.SecondTeam = splitInputLine[5];
			this.Organiser = splitInputLine[6];
		}

		public string Basket
		{
			get
			{
				return this.leagueCode[17].ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}