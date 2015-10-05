namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
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

		public IEnumerable<Team> GetTournametTeams()
		{
			var teams = 
		} 

	}
}