using CheckTheFridge.Data;
using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheckTheFridge.Controllers
{
    public class ListsController : Controller
    {
        // GET: Lists
        public ActionResult Index()
        {
            //Populate Fridge
            var fridgeDbInterface = new FridgeDAO();
            var fridgeIngredients = fridgeDbInterface.FetchAll();
            var myFridge = new Fridge();
            myFridge.Contents = fridgeIngredients;

            //Populate Meals
            var mealDbInterface = new MealDAO();
            var allMeals = mealDbInterface.FetchAll();
            var populatedMealList = mealDbInterface.PopulateMealIngredientsIntoMeals(allMeals);

            //Calculate Lists
            var mealCalculator = new ListsDAO();
            var availableMeals = new List<MealViewModel>();
            availableMeals = mealCalculator.CalculateAvailableMeals(populatedMealList, myFridge);
            return View("Index", availableMeals);
        }
    }
}