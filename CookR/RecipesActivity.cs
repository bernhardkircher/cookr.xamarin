
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System.IO;

namespace CookR
{
	[Activity (Label = "RecipesActivity")]			
	public class RecipesActivity : ListActivity
	{
		private Recipe[] allRecipes;

		private readonly IRecipes recipes;

		public RecipesActivity() : this(new Recipes()){			
		}

		public RecipesActivity(IRecipes recipes){
			this.recipes = recipes;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Recipes);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.addRecipeButton);

			button.Click += delegate {
				Intent intent = new Intent(ApplicationContext, typeof(RecipeActivity));
				StartActivity(intent);
			};

			allRecipes = recipes.GetAllRecipes().ToArray();
			ListAdapter = new RecipesAdapter(this, allRecipes);
		}

		protected override void OnListItemClick(ListView l, View v, int position, long id)
		{
			var recipe = allRecipes[position];
			Intent intent = new Intent(ApplicationContext, typeof(RecipeActivity));
			ActivityParameters.SetRecipeId(intent, recipe);
			StartActivity(intent);
		}
	}

	public class RecipesAdapter : BaseAdapter<Recipe>, ISectionIndexer {
		
		Recipe[] items;
		Activity context;

		private RecipesSectionIndexer sectionIndexer;

	
		public RecipesAdapter(Activity context, Recipe[] items) : base() {
			this.context = context;
			this.items = items;

			// prepare sectionsIndexer data.
			sectionIndexer = new RecipesSectionIndexer();
			sectionIndexer.Init(items);				
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Recipe this[int position] {  
			get { return items[position]; }
		}

		public override int Count {
			get { return items.Length; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if(view == null) { // otherwise create a new one
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
			}

			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position].Name;
			return view;
		}

		#region ISectionIndexer

		public int GetPositionForSection(int sectionIndex)
		{
			return sectionIndexer.GetPositionForSection(sectionIndex);
		}
		public int GetSectionForPosition(int position)
		{
			return sectionIndexer.GetSectionForPosition(position);
		}
		public Java.Lang.Object[] GetSections()
		{
			return sectionIndexer.GetSections();
		}

		#endregion
	}

	public class RecipesSectionIndexer : ISectionIndexer {

		Dictionary<string, int> alphaIndex;
		string[] sections;
		Java.Lang.Object[] sectionsObjects;

		public void Init(Recipe[] items){

			alphaIndex = new Dictionary<string, int>();
			for (int i = 0; i < items.Length; i++) { // loop through items
				var key = items[i].Name[0].ToString();
				// linq would be nicer....
				if (!alphaIndex.ContainsKey(key)) {
					alphaIndex.Add(key, i); // add each 'new' letter to the index
				}
			}

			sections = new string[alphaIndex.Keys.Count];
			alphaIndex.Keys.CopyTo(sections, 0); // convert letters list to string[]
			// Interface requires a Java.Lang.Object[], so we create one here
			sectionsObjects = new Java.Lang.Object[sections.Length];
			for (int i = 0; i < sections.Length; i++) {
				sectionsObjects[i] = new Java.Lang.String(sections[i]);
			}
		}
		
		#region ISectionIndexer implementation

		public int GetPositionForSection(int sectionIndex)
		{
			return alphaIndex[sections[sectionIndex]];
		}

		public int GetSectionForPosition(int position)
		{			
			int prevSection = 0;
			for (int i = 0; i < sections.Length; i++)
			{
				if (GetPositionForSection(i) > position)
				{
					break;
				}
				prevSection = i;
			}
			return prevSection;
		}

		public Java.Lang.Object[] GetSections()
		{
			return sectionsObjects;
		}

		#endregion

		public void Dispose()
		{
			
		}

		public IntPtr Handle {
			get {
				return IntPtr.Zero;
			}
		}





	}
}

