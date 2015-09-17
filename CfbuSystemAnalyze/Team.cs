namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
	using System.Linq;

	internal class Team
	{
		public string Name { get; private set; }
		public Dictionary<int, Round> Rounds { get; private set; }		// int poradi kol
		public Dictionary<int, int> Organiser { get; private set; }		// kolo, ktery kos
		public Dictionary<int, List<Match>> Matches { get; private set; }	// kolo, seznam zapasu	// TODO

		internal Team(string name)
		{
			this.Name = name;
			this.Rounds = new Dictionary<int, Round>();
			this.Organiser = new Dictionary<int, int>();
		}

		internal void AddRoundData(
			int round,
			int basket,
			string rivalName
		)
		{
			if (!this.Rounds.ContainsKey(round))
			{
				this.Rounds.Add(round, new Round(basket));
			}

			if (this.Name != rivalName)
			{
				this.Rounds[round].AddRival(rivalName);
			}
		}

		internal void AddOrganiser(
			int round,
			int basket
		)
		{
			if (!this.Organiser.ContainsKey(round))
			{
				this.Organiser.Add(round, basket);
			}
		}

		internal Dictionary<string, int> GetRivals()
		{
			var output = new Dictionary<string, int>();

			foreach (var round in Rounds)
			{
				foreach (var rival in round.Value.Rivals)
				{
					if (!output.ContainsKey(rival))
					{
						output.Add(rival, 0);
					}

					output[rival]++;
				}
			}
			return output;
		}

		internal decimal BasketSum
		{
			get
			{
				return this.Rounds.Sum(t => t.Value.Basket);
			}
		}

		internal IEnumerable<string> BasketOrganiser()
		{
			var output = new List<string>();

			foreach (var round in this.Rounds)
			{
				if (this.Organiser.ContainsKey(round.Key))
				{
					output.Add(string.Format("{0}({1})", round.Value.Basket, this.Organiser[round.Key]));
				}
				else
				{
					output.Add(round.Value.Basket.ToString());
				}
			}

			return output;
		}
	}
}