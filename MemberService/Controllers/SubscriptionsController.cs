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
    public class SubscriptionsController : Controller
    {
        private DbManager _db;
        private MySubscription _mySubscriptionRepo;
        private UserTable<ApplicationUser> _userTable;
        private Services _serviceRepo;

        public SubscriptionsController()
        {
            this._db = Db;
            this._mySubscriptionRepo = mySubscriptionRepo;
            this._userTable = userTable;
            this._serviceRepo = ServiceRepo;
        }

        private DbManager Db
        {
            get { return _db = _db ?? new DbManager(""); }
            set { _db = value; }
        }

        private MySubscription mySubscriptionRepo
        {
            get { return _mySubscriptionRepo = _mySubscriptionRepo ?? new MySubscription(_db); }
            set { _mySubscriptionRepo = value; }
        }

        private UserTable<ApplicationUser> userTable
        {
            get
            {
                return _userTable ?? new UserTable<ApplicationUser>(_db);
            }
            set
            {
                _userTable = value;
            }
        }

        public Services ServiceRepo
        {
            get { return _serviceRepo ?? new Services(_db); }
            private set { _serviceRepo = value; }
        }

        // GET: Subscriptions
        public ActionResult Index()
        {
            int userid = this._userTable.GetmemberId(User.Identity.Name);
            IEnumerable<MemberServices> myServices = this._mySubscriptionRepo.GetMySubscriptions(userid);
            return View(myServices);
        }

        public ActionResult Create(int? Id)
        {
            MyServices subscription = new MyServices();
            subscription.StartDate = DateTime.Today;
            subscription.ValidDate = DateTime.Today.AddMonths(1);
            if (Id.HasValue)
                subscription = this._mySubscriptionRepo.FindServiceById(Id);
            //var serviceList = this._serviceRepo.GetServiceList();
            int member = this._userTable.GetmemberId(User.Identity.Name);
            var serviceList = this._serviceRepo.GetAppliedServiceList(member); // filtering the service list according to service taken by member.
            ViewBag.ServiceList = new SelectList(serviceList, "Id", "Name", subscription.ServiceId);
            bool isInstallationActive = this._mySubscriptionRepo.CheckIfInstallationActive(member);
            ViewBag.isInstallationActive = isInstallationActive;
            ViewBag.IsMemberService = serviceList.Where(a => !a.Name.Contains("Installation")).Count() == 1 ? true : false;
            return View(subscription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MyServices model)
        {
            if (model.Id == 0) //
            {
                Service requestedService = this._serviceRepo.GetServiceById(model.ServiceId);
                if (requestedService.ServiceType.ToUpper() == "MONTHLY")
                {
                    model.ValidDate = model.StartDate.AddMonths(1);
                }
                else
                {
                    model.ValidDate = model.StartDate.AddYears(50); // no need but value cannot be null so.
                }
                model.MemberId = this._userTable.GetmemberId(User.Identity.Name);                
                model.Status = "InActive"; // InActive untill payment not done.
                model.IsPaid = false;
                model.PaymentConfirmed = false;

                int memberServiceId = this._mySubscriptionRepo.AddService(model);
                return RedirectToAction("MakePayment",new { myserviceid=memberServiceId});
            }
            else
            {

            }
            var serviceList = this._serviceRepo.GetServiceList();
            ViewBag.ServiceList = new SelectList(serviceList, "Id", "Name", model.ServiceId);
            return View(model);
        }

        public ActionResult MakePayment(int myserviceid)
        {
            int userid = this._userTable.GetmemberId(User.Identity.Name);
            MemberServices memberServices = this._mySubscriptionRepo.GetSubscriptionDetail(myserviceid);
            return View(memberServices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePayment(MemberServices formColl)
        {
            bool paid =  this._mySubscriptionRepo.MakePaymentFor(formColl);
            if (paid)
            {
                // send subscriptin activated email
                string message = $@"Dear {formColl.MemberName} , The service {formColl.ServiceName} you orderd is active now. Thank you.";
                SendEmail.SendSubscriptionDetail(message);
                return RedirectToAction("Index");
            }
            else
            {
                return View(formColl);
            }
            
        }

        public ActionResult CancelService(int myserviceid)
        {
            this._mySubscriptionRepo.CancelService(myserviceid);
            return RedirectToAction("Index");
        }
    }
}