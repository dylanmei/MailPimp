using System;
using System.Linq;
using Spark.Parser;

namespace MailPimp.ViewEngine
{
	class MasterGrammar : CharGrammar
	{
		public MasterGrammar(string prefix)
		{
			var whiteSpace0 = Rep(Ch(Char.IsWhiteSpace));
			var whiteSpace1 = Rep1(Ch(Char.IsWhiteSpace));
			var startOfElement = !String.IsNullOrEmpty(prefix) ? Ch("<" + prefix + ":use") : Ch("<use");
			var startOfAttribute = Ch("master").And(whiteSpace0).And(Ch('=')).And(whiteSpace0);
			var attrValue = Ch('\'').And(Rep(ChNot('\''))).And(Ch('\''))
				.Or(Ch('\"').And(Rep(ChNot('\"'))).And(Ch('\"')));

			var endOfElement = Ch("/>");

			var useMaster = startOfElement
				.And(whiteSpace1)
				.And(startOfAttribute)
				.And(attrValue)
				.And(whiteSpace0)
				.And(endOfElement)
				.Build(hit => new string(hit.Left.Left.Down.Left.Down.ToArray()));

			ParseUseMaster =
				pos =>
				{
					for (Position scan = pos; scan.PotentialLength() != 0; scan = scan.Advance(1))
					{
						ParseResult<string> result = useMaster(scan);
						if (result != null)
							return result;
					}
					return null;
				};
		}

		public ParseAction<string> ParseUseMaster { get; private set; }
	}
}