namespace CfbuSystemAnalyze
{
	using System;

	internal class Organization
	{
		public string CurrentOrganiserName { get; private set; }
		public int DrawBasketOrganisation { get; private set; }
		public int CurrentBasketPlay { get; private set; }
		public int? PreviousBasketPlay { get; private set; }

		public Organization(
			string currentOrganiserName,
			int drawBasketOrganisation,
			int currentBasketPlay,
			int? previousBasketPlay
		)
		{
			this.CurrentBasketPlay = currentBasketPlay;
			this.DrawBasketOrganisation = drawBasketOrganisation;
			this.PreviousBasketPlay = previousBasketPlay;
			this.CurrentOrganiserName = currentOrganiserName;
		}

		public int Entropy
		{
			get
			{
				var diff = Math.Abs(this.DrawBasketOrganisation - this.CurrentBasketPlay);

				return (int)Math.Pow(10, diff);
			}
		}
	}
}