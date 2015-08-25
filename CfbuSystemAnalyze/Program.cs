namespace CfbuSystemAnalyze
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	internal class Program
	{
		private static void Main(string[] args)
		{
			var file = new StreamReader("..\\..\\CategoryTxtData\\3XE12014.txt");

			string line;

			var teams = new Dictionary<string, Team>();

			while ((line = file.ReadLine()) != null)
			{
				var teamInputData = new TeamInputData(line);

				if (!teams.ContainsKey(teamInputData.FirstTeam))
				{
					teams.Add(teamInputData.FirstTeam, new Team(teamInputData.FirstTeam));
				}

				if (!teams.ContainsKey(teamInputData.SecondTeam))
				{
					teams.Add(teamInputData.SecondTeam, new Team(teamInputData.SecondTeam));
				}

				teams[teamInputData.FirstTeam].AddRoundData(teamInputData.Round, teamInputData.Basket, teamInputData.SecondTeam);
				teams[teamInputData.SecondTeam].AddRoundData(teamInputData.Round, teamInputData.Basket, teamInputData.FirstTeam);
			}

			using (var outputBasketFile = new StreamWriter("..\\..\\CategoryTxtOuput\\3XE12014-Basket.txt"))
			{
				foreach (var team in teams.Values)
				{
					// Console.WriteLine("{0}\t{1}", team.Name, string.Join("\t", team.Rounds.Select(t => t.Value.Basket.First() - 64)));
					outputBasketFile.WriteLine("{0}\t{1}", team.Name, string.Join("\t", team.Rounds.Select(t => t.Value.Basket.First() - 64)));
				}
			}

			using (var outputCrossPlayFile = new StreamWriter("..\\..\\CategoryTxtOuput\\3XE12014-CrossPlay.txt"))
			{
				var allTeams = teams.Select(t => t.Key).ToArray();
				outputCrossPlayFile.WriteLine(string.Join("\t", allTeams));

				foreach (var t1 in allTeams)
				{
					var rivals = teams[t1].GetRivals();
					outputCrossPlayFile.Write(t1);

					foreach (var t2 in allTeams)
					{
						var value = 0;
						if (rivals.ContainsKey(t2))
						{
							value = rivals[t2];
						}
						outputCrossPlayFile.Write("\t{0}", value);
					}
					outputCrossPlayFile.WriteLine();
				}
			}

			foreach (var team in teams.Values)
			{
				Console.WriteLine("{0} {1}", team.Name, string.Join(";", team.GetRivals().Select(t => string.Format("{0} - {1}", t.Key, t.Value))));
			}
		}
	}
}