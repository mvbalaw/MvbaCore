using System;

namespace MvbaCore
{
	public class PropertyMappingInfo
	{
		public Type DestinationPropertyType { get; set; }
		public Func<object, object> GetValueFromSource { get; set; }
		public string Name { get; set; }
		public Action<object, object> SetValueToDestination { get; set; }
		public Type SourcePropertyType { get; set; }
	}
}