using MemberService.DAL;
using MemberService.Data.BLL;
using MemberService.Models;
using Stripe;
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

        public ActionResult Charges()
        {
            return View();
        }

        public JsonResult GetAllCharges()
        {
            var service = new ChargeService();
            var collection = service.List();
            List<dynamic> resultData = new List<dynamic>();
            collection.Data.ForEach(a =>
            {
                try
                {
                    resultData.Add(new
                    {
                        Name = a.Metadata.Where(p => p.Key == "MemberName").First().Value,
                        Service = a.Metadata.Where(p => p.Key == "ServiceName").First().Value,
                        Amount = a.Amount > 0 ? (a.Amount / 100) : 0,
                        Email = a.ReceiptEmail,
                        Token = a.Id,
                        Paid = a.Paid,
                        Currency = a.Currency,
                        Description = a.Description
                    });
                }
                catch (Exception ex) {

                }
            });
            return new JsonResult()
            {
                Data = new { data = resultData },
                ContentType = "application/json",
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult CreatePlan(int Id)
        {
            var service = this._serviceRepo.GetServiceById(Id);
            var options = new PlanCreateOptions
            {
                Product = new PlanProductCreateOptions
                {
                    Name = "Monthly"
                },
                Amount = (long)(service.Rate*100),
                Currency = "usd",
                Interval = "month",
            };

            var planservice = new PlanService();
            
            if (string.IsNullOrEmpty(service.StripePlanName))
            {
                Plan plan = planservice.Create(options);
                this._serviceRepo.CreateStripPlanForService(Id, plan.Id);
            }
            else
            {
                Plan existingPlan =  planservice.Get(service.StripePlanName);
                if (existingPlan == null)
                {
                    Plan plan = planservice.Create(options);
                    this._serviceRepo.CreateStripPlanForService(Id, plan.Id);
                }                
            }  
            
            return RedirectToAction("Index");
        }
    }
}