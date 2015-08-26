namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
	using System.Linq;

	internal class Round
	{
		public int Basket { get; private set; }
		public List<string> Rivals { get; private set; }

		public Round(
			int basket
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