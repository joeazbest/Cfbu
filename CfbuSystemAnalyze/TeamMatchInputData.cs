namespace CfbuSystemAnalyze
{
	using System;

	internal class TeamMatchInputData
	{
		// stav vstupu je
		// Řídící orgán, Liga, Kód utkání, Kolo, Datum, Čas, Domácí, Vedoucí, Trenér, Hosté, Vedoucí, Trenér, Pořadatel, Hala, Rozhodčí 1, Rozhodčí 2, Delegát, Skóre domácí, Skóre hosté, Statut

		private readonly string leagueCode;
		public int Round { get; private set; }
		public string FirstTeam { get; private set; }
		public string SecondTeam { get; private set; }
		public string Organiser { get; private set; }
		public string SportHall { get; private set; }
		public int FirstTeamGoals { get; private set; }
		public int SecondTeamGoals { get; private set; }
		public string GoalsStatus { get; private set; }

		internal TeamMatchInputData(string inputLine)
		{
			if (string.IsNullOrEmpty(inputLine))
			{
				throw new NotSupportedException("inputLine");
			}

			var splitInputLine = inputLine.Split('\t');
			this.leagueCode = splitInputLine[2];
			this.Round = int.Parse(splitInputLine[3]);

			this.FirstTeam = splitInputLine[6];
			this.SecondTeam = splitInputLine[9];
			this.Organiser = splitInputLine[12];
			this.SportHall = splitInputLine[13];
			this.FirstTeamGoals = int.Parse(splitInputLine[17]);
			this.SecondTeamGoals = int.Parse(splitInputLine[18]);
			this.GoalsStatus = splitInputLine[19];
		}

		public int Basket
		{
			get
			{
				return this.leagueCode[17] - 64;
			}
		}
	}
}