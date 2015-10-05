namespace CfbuSystemAnalyze
{
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
				var roundBasketStatistics = new Dictionary<int, Dictionary<int, List<Match>>>();
				var basketRoundStatistics = new Dictionary<int, Dictionary<int, List<Match>>>();

				var tournaments = new Dictionary<RoundBasket, Tournament>();

				while ((line = file.ReadLine()) != null)
				{
					var teamMatchInputData = new TeamMatchInputData(line);

					SetTeamsValues(teams, teamMatchInputData);

					var match = new Match(
						teamMatchInputData.FirstTeam,
						teamMatchInputData.SecondTeam,
						teamMatchInputData.FirstTeamGoals,
						teamMatchInputData.SecondTeamGoals,
						teamMatchInputData.GoalsStatus
					);

					SetRounBasketStatiscsValue(roundBasketStatistics, teamMatchInputData, match);
					SetBasketRoundStatisticsValue(basketRoundStatistics, teamMatchInputData, match);
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

				// vystup po kolech v prumerny rozdil
				using (var roundBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketAvarageDiff.txt", fileName)))
				{
					// kola
					foreach (var round in roundBasketStatistics)
					{
						roundBasketFile.Write("\t{0}", round.Key);
					}
					roundBasketFile.WriteLine();

					// radek je kos a pak prumery
					foreach (var basket in basketRoundStatistics)
					{
						roundBasketFile.Write(basket.Key);

						foreach (var round in basket.Value)
						{
							roundBasketFile.Write("\t{0}", round.Value.Average(t => t.DiffScore));
						}
						roundBasketFile.WriteLine();
					}
				}

				// vystup po kolech v maximalni rozdil rozdil
				using (var roundBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketMaxDiff.txt", fileName)))
				{
					// kola
					foreach (var round in roundBasketStatistics)
					{
						roundBasketFile.Write("\t{0}", round.Key);
					}
					roundBasketFile.WriteLine();

					// radek je kos a pak prumery
					foreach (var basket in basketRoundStatistics)
					{
						roundBasketFile.Write(basket.Key);

						foreach (var round in basket.Value)
						{
							roundBasketFile.Write("\t{0}", round.Value.Max(t => t.DiffScore));
						}
						roundBasketFile.WriteLine();
					}
				}
			}
		}

		private static void SetBasketRoundStatisticsValue(Dictionary<int, Dictionary<int, List<Match>>> basketRoundStatistics,
			TeamMatchInputData teamMatchInputData, Match match)
		{
			{
				if (!basketRoundStatistics.ContainsKey(teamMatchInputData.Basket))
				{
					basketRoundStatistics.Add(teamMatchInputData.Basket, new Dictionary<int, List<Match>>());
				}

				if (!basketRoundStatistics[teamMatchInputData.Basket].ContainsKey(teamMatchInputData.Round))
				{
					basketRoundStatistics[teamMatchInputData.Basket].Add(teamMatchInputData.Round, new List<Match>());
				}

				basketRoundStatistics[teamMatchInputData.Basket][teamMatchInputData.Round].Add(match);
			}
		}

		private static void SetRounBasketStatiscsValue(Dictionary<int, Dictionary<int, List<Match>>> roundBasketStatistics, TeamMatchInputData teamMatchInputData,
			Match match)
		{
			if (!roundBasketStatistics.ContainsKey(teamMatchInputData.Round))
			{
				roundBasketStatistics.Add(teamMatchInputData.Round, new Dictionary<int, List<Match>>());
			}

			if (!roundBasketStatistics[teamMatchInputData.Round].ContainsKey(teamMatchInputData.Basket))
			{
				roundBasketStatistics[teamMatchInputData.Round].Add(teamMatchInputData.Basket, new List<Match>());
			}

			roundBasketStatistics[teamMatchInputData.Round][teamMatchInputData.Basket].Add(match);
		}

		private static void SetTeamsValues(
			IDictionary<string, Team> teams,
			TeamMatchInputData teamMatchInputData
		)
		{
			if (!teams.ContainsKey(teamMatchInputData.FirstTeam))
			{
				teams.Add(teamMatchInputData.FirstTeam, new Team(teamMatchInputData.FirstTeam));
			}

			if (!teams.ContainsKey(teamMatchInputData.SecondTeam))
			{
				teams.Add(teamMatchInputData.SecondTeam, new Team(teamMatchInputData.SecondTeam));
			}

			if (!teams.ContainsKey(teamMatchInputData.Organiser))
			{
				teams.Add(teamMatchInputData.Organiser, new Team(teamMatchInputData.Organiser));
			}

			teams[teamMatchInputData.FirstTeam].AddRoundData(teamMatchInputData.Round, teamMatchInputData.Basket,
				teamMatchInputData.SecondTeam);
			teams[teamMatchInputData.SecondTeam].AddRoundData(teamMatchInputData.Round, teamMatchInputData.Basket,
				teamMatchInputData.FirstTeam);
			teams[teamMatchInputData.Organiser].AddOrganiser(teamMatchInputData.Round, teamMatchInputData.Basket);
		}
	}
}