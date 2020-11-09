using CheckTheFridge.Data;
using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheckTheFridge.Controllers
{
    public class FridgeController : Controller
    {
        // GET: Fridge
        public ActionResult Index()
        {
            FridgeDAO fridgeDBInterface = new FridgeDAO();
            var myFridge = new Fridge();
            myFridge.Contents = fridgeDBInterface.FetchAll();
            return View("Index", myFridge);
        }

        public ActionResult EditIngredient(int id)
        {
            var ingredientDBInterface = new FridgeDAO();
            var newIngredient = new MealIngredientViewModel();
            newIngredient = ingredientDBInterface.FetchOne(id);
            var unitLabels = new List<Unit>();
            unitLabels = ingredientDBInterface.FetchUnits();
            newIngredient.AvailableUnitLabels = unitLabels;
            return View("MealIngredientForm", newIngredient);
        }

        public ActionResult Create()
        {
            var ingredientDBInterface = new FridgeDAO();
            var newIngredient = new MealIngredientViewModel();
            var unitLabels = new List<Unit>();
            unitLabels = ingredientDBInterface.FetchUnits();
            newIngredient.AvailableUnitLabels = unitLabels;
            return View("FridgeIngredientCreateForm", newIngredient);
        }

        public ActionResult Delete(int id)
        {
            var ingredientDBInterface = new FridgeDAO();
            var myFridge = new Fridge();
            ingredientDBInterface.DeleteIngredient(id);
            myFridge.Contents = ingredientDBInterface.FetchAll();
            return View("Index", myFridge);
        }

        public ActionResult EditFridgeIngredient(MealIngredientViewModel updatedIngredient)
        {
            var updateFridge = new FridgeDAO();
            var revisedIngredient = updatedIngredient;
            updateFridge.UpdateIngredient(revisedIngredient);
            var myFridge = new Fridge();
            myFridge.Contents = updateFridge.FetchAll();
            return View("Index", myFridge);
        }

        public ActionResult AddFridgeIngredient(MealIngredientViewModel newIngredient)
        {
            FridgeDAO dbInterface = new FridgeDAO();
            if (dbInterface.ingredientAlreadyInFridge(newIngredient))
            {
                ModelState.AddModelError("CustomError", "Ingredient already in Fridge");
                return View();
            }
            else
            {
                dbInterface.AddIngredientToFridge(newIngredient);
            }
            var myFridge = new Fridge();
            myFridge.Contents = dbInterface.FetchAll();
            return View("Index", myFridge);
        }
    }
}