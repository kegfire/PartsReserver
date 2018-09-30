using System.Collections.Generic;

namespace PartsReserver.Models
{
	public class AutoResponse
	{
		public int Results { get; set; }

		public List<string> Rows { get; set; }

		public Meta MetaData { get; set; }
	}

	public class Meta
	{
		public List<FieldsMeta> Fields { get; set; }
	}

	public class FieldsMeta
	{
		public string Name { get; set; }
		public string Header { get; set; }
	}
}