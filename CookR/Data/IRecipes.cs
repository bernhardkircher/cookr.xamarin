using System;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CookR
{

	/// <summary>
	/// Defines all Database access methods.
	/// Since this is a small project, we do not careate a "classic" Repository/unit of work pattern structure.
	/// </summary>
	public interface IRecipes {

		void Init();

		/// <summary>
		/// Saves the recipe, by either inserting or updateing it.
		/// </summary>
		/// <param name="recipe">The Recipe to save.</param>
		void SaveRecipe(Recipe recipe);

		/// <summary>
		/// Deletes the recipe.
		/// </summary>
		/// <param name="recipe">The Recipe to delete.</param>
		void DeleteRecipe(Recipe recipe);

		/// <summary>
		/// Implementors should return all recipes.
		/// </summary>
		/// <returns>The all recipes.</returns>
		IEnumerable<Recipe> GetAllRecipes();

		/// <summary>
		/// Implementors should return an existing recipe with the given name or null if it cannot be found.
		/// </summary>
		/// <returns>The recipe by name.</returns>
		/// <param name="name">Name.</param>
		Recipe GetRecipeByName(string name);

		Recipe GetRecipe(int id);

	}


	public class Recipes : IRecipes {

		private readonly string _dbPath;

		public Recipes(){
			_dbPath = Path.Combine (
				System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
				"cookr.db3");
		}

		private SQLiteConnection GetConnection(){
			var db = new SQLiteConnection(_dbPath);
			return db;
		}

		public void Init(){
			using (var db = GetConnection()) {
				db.CreateTable<Recipe>();
			}
		}

		public void SaveRecipe(Recipe recipe){			
			using (var db = GetConnection()) {
				if(recipe.Id <= 0) {
					int id = db.Insert(recipe);
					recipe.Id = id;
				} else {
					db.Update(recipe);
				}
			}
		} 

		public void DeleteRecipe(Recipe recipe)
		{
			using (var db = GetConnection()) {
				db.Delete(recipe);
			}
		}

		public Recipe GetRecipeByName(string name){
			string preparedNameForSearch = name.ToLower();
			using (var db = GetConnection()) {
				return db.Table<Recipe>().Where(x => x.Name.ToLower() == preparedNameForSearch).FirstOrDefault();
			}
		}

		public Recipe GetRecipe(int id){			
			using (var db = GetConnection()) {
				return db.Get<Recipe>(id);
			}
		}

		public IEnumerable<Recipe> GetAllRecipes()
		{
			using (var db = GetConnection()) {
				return db.Table<Recipe>().OrderBy(x=> x.Name).ToArray();
			}
		}
	}
	
}
