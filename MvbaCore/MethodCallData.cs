using System.Collections.Generic;

namespace MvbaCore
{
	public class MethodCallData
	{
		public string ClassName { get; set; }
		public string MethodName { get; set; }
		public Dictionary<string, string> ParameterValues { get; set; }
	}
}