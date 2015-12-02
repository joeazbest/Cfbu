namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Linq.Expressions;

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
						teamMatchInputData.GoalsStatus,
						teamMatchInputData.Organiser
					);

					var roundBasket = new RoundBasket(teamMatchInputData.Round, teamMatchInputData.Basket, teamMatchInputData.SportHall);
					if (!tournaments.ContainsKey(roundBasket))
					{
						tournaments.Add(roundBasket, new Tournament());
					}
					tournaments[roundBasket].AddMatch(match);

					SetTeamsValues(teams, teamMatchInputData);
				}

				// zmeny v kosich
				WriteBaskedChange(fileName, teams);

				// zmeny v kosich a poradatelstvi
				WriteBasketOrganiser(fileName, teams);

				// krizova tabulka kdo s kym kolikrat hral
				WrieCrossTable(fileName, teams);

				// vystup po kolech v prumerny rozdil - dva vystupy, jednou pro excel a podruhy pro LaTeX
				WriteAvarageDiff(fileName, tournaments);

				// vystup po kolech v maximalni rozdil rozdil
				WrieMaxDiff(fileName, tournaments);

				// vystup kolik kterych uktkani skoncilo jakym vysledkem
				WriteSumMatchDiff(fileName, tournaments);

				// poradatelstvi a jeho zmeny
				WriteOrganization(fileName, tournaments, teams);

				// haly
				using (
					var sportHallFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-SportHall.txt", fileName))
				)
				{
					var sportHalls = new HashSet<string>();
					foreach (var tournament in tournaments)
					{
						sportHalls.Add(tournament.Key.SportHall);
					}

					var teamSportHalls = new Dictionary<string, Dictionary<string, int>>();	//string - naze tymu, string - hala, int - kolikrat
					foreach (var team in teams)
					{
						teamSportHalls.Add(team.Key, new Dictionary<string, int>());
						foreach (var sportHall in sportHalls)
						{
							teamSportHalls[team.Key].Add(sportHall, 0);
						}
					}

					foreach (var tournament in tournaments)
					{
						foreach (var team in tournament.Value.GetTournametTeams())
						{
							teamSportHalls[team][tournament.Key.SportHall]++;
						}
					}

					foreach (var hall in teamSportHalls.First().Value)
					{
						sportHallFile.Write("\t{0}", hall.Key);
					}
					sportHallFile.WriteLine();

					foreach (var teamSportHall in teamSportHalls)
					{
						sportHallFile.Write(teamSportHall.Key);

						foreach (var hall in teamSportHall.Value)
						{
							sportHallFile.Write("\t{0}", hall.Value);
						}
						sportHallFile.WriteLine();
					}

				}
			}
		}

		private static void WriteOrganization(string fileName, Dictionary<RoundBasket, Tournament> tournaments, Dictionary<string, Team> teams)
		{
			using (
				StreamWriter organizationFile =
					new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-Organiser.txt", fileName)),
					organizationEntropy = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-OrganiserEntropy.txt", fileName))
				)
			{
				var organisation = new Dictionary<RoundBasket, Organization>();

				foreach (var tournament in tournaments)
				{
					var tournametOrganiser = tournament.Value.GetTournamentOrganiser();

					organisation.Add(
						tournament.Key,
						new Organization(
							tournametOrganiser,
							teams[tournametOrganiser].Rounds[1].Basket,
							teams[tournametOrganiser].Rounds[tournament.Key.Round].Basket,
							tournament.Key.Round == 1 ? (int?) null : teams[tournametOrganiser].Rounds[tournament.Key.Round - 1].Basket
							)
						);
				}

				var maxBasket = tournaments.Max(t => t.Key.Basket);
				var maxRound = tournaments.Max(t => t.Key.Round);

				for (var basket = 1; basket <= maxBasket; basket++)
				{
					var basketLocal = basket;
					var allRoundOrganization = organisation.Where(t => t.Key.Basket == basketLocal);
					foreach (var tournament in allRoundOrganization.OrderBy(t => t.Key.Round))
					{
						organizationFile.Write(
							"{0} -  Hraje {2} mel poradat {4}\t",
							tournament.Value.CurrentOrganiserName,
							tournament.Key.Basket,
							tournament.Value.CurrentBasketPlay,
							tournament.Value.PreviousBasketPlay,
							tournament.Value.DrawBasketOrganisation
							);
					}
					organizationFile.WriteLine();
				}

				var entropyMinimalize = new EntropyMinimalize(maxBasket);

				for (var round = 1; round <= maxRound; round++)
				{
					organizationEntropy.WriteLine(
						"{0}\t{1}\t{2}",
						round,
						organisation.Where(t => t.Key.Round == round).Sum(t => t.Value.Entropy),
						entropyMinimalize.MinEntropy(
							organisation.Where(t => t.Key.Round == round).Select(t => t.Value.CurrentBasketPlay).ToList())
						);
				}
			}
		}


		private static void WriteSumMatchDiff(string fileName, Dictionary<RoundBasket, Tournament> tournaments)
		{
			using (
				var totalDiffScoreMatches =
					new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-totalDiffScoreMatches.txt", fileName)))
			{
				var output = new Dictionary<int, int>(); // prvni cislo je hodnota rozdilu a druhe kolikrat nastala
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

		private static void WrieMaxDiff(string fileName, Dictionary<RoundBasket, Tournament> tournaments)
		{
			using (
				StreamWriter roundBasketFileExcel =
					new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketMaxDiff.txt", fileName)),
					roundBasketFileLaTeX =
						new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketMaxDiffLaTeX.txt", fileName))
				)
			{
				var maxRount = tournaments.Max(t => t.Key.Round);
				var maxBasket = tournaments.Max(t => t.Key.Basket);

				roundBasketFileLaTeX.Write("Kolo");
				for (var round = 1; round <= maxRount; round++)
				{
					roundBasketFileExcel.Write("\t{0}", round);
					roundBasketFileLaTeX.Write(" & {0}", round);
				}
				roundBasketFileExcel.WriteLine();
				roundBasketFileLaTeX.Write("\\\\");
				roundBasketFileLaTeX.WriteLine();

				for (var basket = 1; basket <= maxBasket; basket++)
				{
					roundBasketFileExcel.Write(basket);
					roundBasketFileLaTeX.Write("Koš {0}", basket);

					var basketLocal = basket;
					var allRoundForOneBasket = tournaments.Where(t => t.Key.Basket == basketLocal);

					foreach (var tournament in allRoundForOneBasket.OrderBy(t=> t.Key.Round))
					{
						var maxDiff = tournament.Value.Matches.Max(t => t.DiffScore);

						roundBasketFileExcel.Write("\t{0}", maxDiff);
						if (maxDiff > 9)
						{
							roundBasketFileLaTeX.Write(" & \\alert{{{0} }}", maxDiff);
						}
						else
						{
							roundBasketFileLaTeX.Write(" & {0}", maxDiff);
						}
					}
					roundBasketFileExcel.WriteLine();
					roundBasketFileLaTeX.Write("\\\\");
					roundBasketFileLaTeX.WriteLine();
				}
			}
		}

		private static void WriteAvarageDiff(string fileName, Dictionary<RoundBasket, Tournament> tournaments)
		{
			using (
				StreamWriter roundBasketFileExcel =
					new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketAvarageDiff.txt", fileName)),
					roundBasketFileLaTeX = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-RoundBasketAvarageDiffLaTeX.txt", fileName))
				)
			{
				var maxRount = tournaments.Max(t => t.Key.Round);
				var maxBasket = tournaments.Max(t => t.Key.Basket);

				roundBasketFileLaTeX.Write("Kolo");
				for (var round = 1; round <= maxRount; round++)
				{
					roundBasketFileExcel.Write("\t{0}", round);
					roundBasketFileLaTeX.Write(" & {0}", round);
				}
				roundBasketFileExcel.WriteLine();
				roundBasketFileLaTeX.Write("\\\\");
				roundBasketFileLaTeX.WriteLine();

				for (var basket = 1; basket <= maxBasket; basket++)
				{
					roundBasketFileExcel.Write(basket);
					roundBasketFileLaTeX.Write("Koš {0}", basket);

					var basketLocal = basket;
					var allRoundForOneBasket = tournaments.Where(t => t.Key.Basket == basketLocal);

					foreach (var tournament in allRoundForOneBasket.OrderBy(t => t.Key.Round))
					{
						var avarage = tournament.Value.Matches.Average(t => t.DiffScore);
						roundBasketFileExcel.Write("\t{0}", avarage);
						if (avarage > 5)
						{
							roundBasketFileLaTeX.Write(" & \\alert{{{0:F2} }}", avarage);
						}
						else
						{
							roundBasketFileLaTeX.Write(" & {0:F2}", avarage);
						}
					}
					roundBasketFileExcel.WriteLine();
					roundBasketFileLaTeX.Write("\\\\");
					roundBasketFileLaTeX.WriteLine();
				}
			}
		}

		private static void WrieCrossTable(string fileName, Dictionary<string, Team> teams)
		{
			using (
				var outputCrossPlayFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-CrossPlay.txt", fileName)))
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

		private static void WriteBasketOrganiser(string fileName, Dictionary<string, Team> teams)
		{
			using (
				var outputBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-BasketOrganiser.txt", fileName)))
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
		}

		private static void WriteBaskedChange(string fileName, Dictionary<string, Team> teams)
		{
			using (var outputBasketFile = new StreamWriter(string.Format("..\\..\\CategoryTxtOuput\\{0}-Basket.txt", fileName)))
			{
				foreach (var team in teams.Values)
				{
					outputBasketFile.WriteLine("{0}\t{1}", team.Name, string.Join("\t", team.Rounds.Select(t => t.Value.Basket)));
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