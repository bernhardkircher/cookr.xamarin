using System;
using SQLite;
using System.Collections.Generic;

namespace CookR
{
	[Table("recipe")]
	public class Recipe
	{
		public Recipe ()
		{
		}

		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }

		[MaxLength(255), NotNull, Unique, Column("name")]
		public string Name { get; set; }

		public override string ToString ()
		{
			return Name;
		}
	}
}

