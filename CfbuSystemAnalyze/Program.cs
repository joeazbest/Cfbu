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
			var inputFiles = new List<string>
			{
				"3XE12014",
				"3XS12014",
				"3XZ12014"
			};

			foreach (var fileName in inputFiles)
			{
				var file = new StreamReader(string.Format("..\\..\\CategoryTxtData\\{0}.txt", fileName));

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

					if (!teams.ContainsKey(teamInputData.Organiser))
					{
						teams.Add(teamInputData.Organiser, new Team(teamInputData.Organiser));
					}

					teams[teamInputData.FirstTeam].AddRoundData(teamInputData.Round, teamInputData.Basket, teamInputData.SecondTeam);
					teams[teamInputData.SecondTeam].AddRoundData(teamInputData.Round, teamInputData.Basket, teamInputData.FirstTeam);
					teams[teamInputData.Organiser].AddOrganiser(teamInputData.Round, teamInputData.Basket);
				}

				// zmeny v kosich
				using (var outputBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-Basket.txt", fileName)))
				{
					foreach (var team in teams.Values)
					{
						outputBasketFile.WriteLine("{0}\t{1}", team.Name, string.Join("\t", team.Rounds.Select(t => t.Value.Basket)));
					}
				}

				// zmeny v kosich a poradatelstvi
				using (var outputBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-BasketOrganiser.txt", fileName)))
				{
					foreach (var team in teams.Values)
					{
						outputBasketFile.WriteLine(
							"{0}\t{1}",
							team.Name,
							string.Join("\t", team.BasketOrganiser())
						);
					}
				}

				// krizova tabulka kdo s kym kolikrat hral
				using (var outputCrossPlayFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-CrossPlay.txt", fileName)))
				{
					var allTeams = teams.OrderBy(t => t.Value.BasketSum).Select(t => t.Key).ToArray();
					outputCrossPlayFile.WriteLine("\t{0}", string.Join("\t", allTeams));

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

			}
		}
	}
}