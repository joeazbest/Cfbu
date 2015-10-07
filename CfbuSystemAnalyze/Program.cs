namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	internal class Program
	{
		private static void Main()
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
				var tournaments = new Dictionary<RoundBasket, Tournament>();

				while ((line = file.ReadLine()) != null)
				{
					var teamMatchInputData = new TeamMatchInputData(line);

					var match = new Match(
						teamMatchInputData.FirstTeam,
						teamMatchInputData.SecondTeam,
						teamMatchInputData.FirstTeamGoals,
						teamMatchInputData.SecondTeamGoals,
						teamMatchInputData.GoalsStatus
					);

					var roundBasket = new RoundBasket(teamMatchInputData.Round, teamMatchInputData.Basket);
					if (!tournaments.ContainsKey(roundBasket))
					{
						tournaments.Add(roundBasket, new Tournament());
					}
					tournaments[roundBasket].AddMatch(match);

					SetTeamsValues(teams, teamMatchInputData);
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
					var maxRount = tournaments.Max(t => t.Key.Round);
					var maxBasket = tournaments.Max(t => t.Key.Basket);

					for (var round = 1; round <= maxRount; round++)
					{
						roundBasketFile.Write("\t{0}", round);
					}
					roundBasketFile.WriteLine();

					for (var basket = 1; basket <= maxBasket; basket++)
					{
						roundBasketFile.Write(basket);
						var allRoundForOneBasket = tournaments.Where(t => t.Key.Basket == basket);

						foreach (var tournament in allRoundForOneBasket)
						{
							roundBasketFile.Write("\t{0}", tournament.Value.Matches.Average(t => t.DiffScore));
						}
						roundBasketFile.WriteLine();
					}
				}

				// vystup po kolech v maximalni rozdil rozdil
				using (var roundBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketMaxDiff.txt", fileName)))
				{
					var maxRount = tournaments.Max(t => t.Key.Round);
					var maxBasket = tournaments.Max(t => t.Key.Basket);

					for (var round = 1; round <= maxRount; round++)
					{
						roundBasketFile.Write("\t{0}", round);
					}
					roundBasketFile.WriteLine();

					for (var basket = 1; basket <= maxBasket; basket++)
					{
						roundBasketFile.Write(basket);
						var allRoundForOneBasket = tournaments.Where(t => t.Key.Basket == basket);

						foreach (var tournament in allRoundForOneBasket)
						{
							roundBasketFile.Write("\t{0}", tournament.Value.Matches.Max(t => t.DiffScore));
						}
						roundBasketFile.WriteLine();
					}
				}

				using (var totalDiffScoreMatches = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-totalDiffScoreMatches.txt", fileName)))
				{
					var output = new Dictionary<int, int>();	// prvni cislo je hodnota rozdilu a druhe kolikrat nastala
					foreach (var tournament in tournaments)
					{
						foreach (var match in tournament.Value.Matches)
						{
							if (!output.ContainsKey(match.DiffScore))
							{
								output.Add(match.DiffScore, 0);
							}
							output[match.DiffScore]++;
						}
					}

					for (var diff = 0; diff <= output.Keys.Max(); diff++)
					{
						totalDiffScoreMatches.WriteLine("{0}\t{1}", diff, output.ContainsKey(diff) ? output[diff] : 0);
					}
				}
			}
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