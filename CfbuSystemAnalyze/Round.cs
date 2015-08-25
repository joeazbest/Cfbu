namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;

	internal class Round
	{
		public string Basket { get; private set; }
		public List<string> Rivals { get; private set; }

		public Round(
			string basket
		)
		{
			this.Basket = basket;
			this.Rivals = new List<string>();
		}

		public void AddRival(string rivalName)
		{
			this.Rivals.Add(rivalName);
		}
	}
}