namespace CfbuSystemAnalyze
{
	using System.Collections.Generic;
	using System.Linq;

	internal class Permutation
	{
		private readonly IEnumerable<char> input;

		public Permutation(string p)
		{
			this.input = p.ToCharArray();
		}

		internal List<string> List()
		{
			var output = new List<string>();
			Permutate(string.Empty, this.input.ToArray(), ref output);
			return output;
		}

		private static void Permutate(string currentPrefix, IList<char> stack, ref List<string> output)
		{
			if (!stack.Any())
			{
				output.Add(currentPrefix);
				return;
			}

			for (var i = 0; i < stack.Count(); i++)
			{
				var charValue = stack[i];
				var newPrefix = currentPrefix + charValue;
				var newStack = stack.ToList();
				newStack.RemoveAt(i);
				Permutate(newPrefix, newStack, ref output);
			}
		}
	}
}