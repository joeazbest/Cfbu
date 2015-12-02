namespace CfbuSystemAnalyze
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class EntropyMinimalize
	{
		private readonly int count;
		private readonly List<int> posibility;

		internal EntropyMinimalize(int basketCount)	// kde hraju, co poradam
		{
			this.count = basketCount;
			this.posibility = new List<int>();
			for (var i = 1; i <= basketCount; i++)
			{
				this.posibility.Add(i);
			}
		}

		internal int MinEntropy(IList<int> currentPlay)
		{
			var currentMin = ComputeEntropy(this.posibility, currentPlay);
			var posibilityx = this.posibility.ToList();
			bool isChange;
			do
			{
				isChange = false;
				for (var i = 1; i < this.count; i++)
				{
					for (var j = 0; j < i; j++)
					{
						var newPosility = Svamp(posibilityx, i, j);
						if (currentMin > ComputeEntropy(newPosility, currentPlay))
						{
							posibilityx = newPosility.ToList();
							currentMin = ComputeEntropy(newPosility, currentPlay);
							isChange = true;
						}
					}
				}

			} while (isChange);

			return currentMin;

		}

		private IList<int> Svamp(List<int> list, int i, int j)
		{
			var output = list.ToArray();
			var helper = output[i];
			output[i] = output[j];
			output[j] = helper;
			return output;
		}

		private int ComputeEntropy(IList<int> list, IList<int> currentPlay)
		{
			var output = 0;
			for (var i = 0; i < this.count; i++)
			{
				output += (int)Math.Pow(10, Math.Abs(list[i] - currentPlay[i]));
			}
			return output;
		}
	}
}