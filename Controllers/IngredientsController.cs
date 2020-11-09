using CheckTheFridge.Data;
using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheckTheFridge.Controllers
{
    public class IngredientsController : Controller
    {
        // GET: Ingredients
        public ActionResult Index()
        {
            var ingredientDbInterface = new IngredientDAO();
            var allIngredients = ingredientDbInterface.FetchAllIngredients();
            return View("IngredientIndex", allIngredients);
        }

        public ActionResult ToCreateIngredientForm()
        {
            var ingredientToCreate = new Ingredient();
            return View("CreateIngredientForm", ingredientToCreate);
        }

        public ActionResult ProcessIngredientToCreate(Ingredient ingredientToAddToDb)
        {
            var ingredientDbInterface = new IngredientDAO();
            ingredientDbInterface.AddIngredientToDb(ingredientToAddToDb);
            var allIngredients = ingredientDbInterface.FetchAllIngredients();
            return View("IngredientIndex", allIngredients);
        }
    }
}