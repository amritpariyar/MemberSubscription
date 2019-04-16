using MemberService.DAL;
using MemberService.Data.BLL;
using MemberService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemberService.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private DbManager _db;
        private Services _serviceRepo;
        public ServicesController()
        {
            _db = Db;
            _serviceRepo = ServiceRepo;
        }

        private DbManager Db
        {
            get
            {
                return _db = _db ?? new DbManager("");
            }
            set
            {
                _db = value;
            }
        }

        public Services ServiceRepo
        {
            get
            {
                return _serviceRepo = _serviceRepo ?? new Services(_db);
            }
            private set
            {
                _serviceRepo = value;
            }
        }


        // GET: Services
        public ActionResult Index()
        {

            //Services services = new Services(db);
            //var list = services.GetServiceList();
            var list = this._serviceRepo.GetServiceList();
            return View(list);
        }

        public ActionResult Create()
        {
            Service service = new Service();
            return View(service);
        }

        public ActionResult Edit(int Id)
        {
            Service service = this._serviceRepo.GetServiceById(Id);
            return View("Create", service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Service model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id == 0)
                    {
                        this._serviceRepo.AddService(model);
                    }
                    else
                    {
                        this._serviceRepo.UpdateService(model);
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Exception", ex);
                return View(model);
            }

        }

        public ActionResult ChangeStatus(int id)
        {
            try
            {
                this._serviceRepo.ChangeServiceStatus(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}