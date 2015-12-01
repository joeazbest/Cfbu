namespace CfbuSystemAnalyze
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	internal class Tournament
	{
		public List<Match> Matches { get; private set; }

		public Tournament()
		{
			this.Matches = new List<Match>();
		}

		public void AddMatch(Match match)
		{
			this.Matches.Add(match);
		}

		public IEnumerable<string> GetTournametTeams()
		{
			var homeTeams = this.Matches.Select(t => t.HomeTeam);
			var foreignTeams = this.Matches.Select(t => t.ForeignTeam);
			return homeTeams.Union(foreignTeams).Distinct().OrderBy(t => t);
		}

		public string GetTournamentOrganiser()
		{
			var organiser = this.Matches.Select(t => t.Organiser).Distinct().ToArray();

			if (organiser.Count() != 1)
			{
				throw new NotSupportedException(organiser.Count().ToString(CultureInfo.InvariantCulture));
			}

			return organiser.First();
		}

		//public Dictionary<string, int> GetTeamTournamentOrder()
		//{
		//	// pro ruzny pocet lidi v teamu ruzny vypocet poradi
		//	var currentTournamentTeam = this.GetTournametTeams();

		//	// todo nevim jak to dobre udelat jinak nez pres switch

		//	if (currentTournamentTeam.Count() == 6)
		//	{
		//	}
		//	return null;
		//}
	}
}