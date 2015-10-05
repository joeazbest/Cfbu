namespace CfbuSystemAnalyze
{
	using System;

	internal class RoundBasket : IEquatable<RoundBasket>
	{
		public int Round { get; private set; }
		public int Basket { get; private set; }

		public RoundBasket(
			int round,
			int basket
			)
		{
			this.Round = round;
			this.Basket = basket;
		}

		public bool Equals(RoundBasket other)
		{
			return this.Basket == other.Basket && this.Round == other.Round;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			var roundBasketObj = obj as RoundBasket;
			if (roundBasketObj == null)
				return false;

			return this.Equals(roundBasketObj);
		}

		public override int GetHashCode()
		{
			return this.Round.GetHashCode() ^ this.Basket.GetHashCode();
		}
	}
}