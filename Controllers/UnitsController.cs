using CheckTheFridge.Data;
using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheckTheFridge.Controllers
{
    public class UnitsController : Controller
    {

        // GET: Units
        public ActionResult Index()
        {
            var unitDbInterface = new UnitDAO();
            var allUnits = unitDbInterface.FetchAllUnits();
            return View("UnitIndex", allUnits);
        }

        public ActionResult ToCreateUnitForm()
        {
            var newUnit = new Unit();
            return View("CreateUnitForm", newUnit);
        }

        public ActionResult ProcessUnitToCreate(Unit unitToAddToDb)
        {
            var unitDbInterface = new UnitDAO();
            unitDbInterface.AddUnitToDb(unitToAddToDb);
            var allUnits = unitDbInterface.FetchAllUnits();
            return View("UnitIndex", allUnits);
        }


    }
}