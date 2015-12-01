namespace CfbuSystemAnalyze
{
	using System;

	internal class Match
	{
		internal string HomeTeam { get; private set; }
		internal string ForeignTeam { get; private set; }
		internal int HomeScore { get; private set; }
		internal int ForeignScore { get; private set; }
		internal string StatusScore { get; private set; }
		internal string Organiser { get; private set; }

		internal Match(
			string homeTeam,
			string foreignTeam,
			int homeScore,
			int foreignScore,
			string statusScore,
			string organiser
		)
		{
			this.HomeTeam = homeTeam;
			this.ForeignTeam = foreignTeam;
			this.HomeScore = homeScore;
			this.ForeignScore = foreignScore;
			this.StatusScore = statusScore;
			this.Organiser = organiser;
		}

		internal int DiffScore
		{
			get
			{
				return Math.Abs(this.HomeScore - this.ForeignScore);
			}
		}
	}
}