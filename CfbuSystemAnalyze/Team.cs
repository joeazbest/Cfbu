namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
	using System.Globalization;

	internal class Team
	{
		public string Name { get; private set; }
		public Dictionary<int, Round> Rounds { get; private set; }

		internal Team(string name)
		{
			this.Name = name;
			this.Rounds = new Dictionary<int, Round>();
		}

		internal void AddRoundData(
			int round,
			string basketName,
			string rivalName
		)
		{
			if (!this.Rounds.ContainsKey(round))
			{
				this.Rounds.Add(round, new Round(basketName));
			}

			if(this.Name != rivalName)
			{
				this.Rounds[round].AddRival(rivalName);
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
	}
}