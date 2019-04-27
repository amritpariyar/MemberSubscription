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
    public class PaymentController : Controller
    {
        private DbManager _db;
        private MySubscription _mySubscriptionRepo;
        private UserTable<ApplicationUser> _userTable;
        private Services _serviceRepo;
        public PaymentController()
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

        // GET: Payment
        public ActionResult Index(int myserviceid)
        {
            ViewBag.MemberServiceId = myserviceid;
            ViewBag.MemberId = this._userTable.GetmemberId(User.Identity.Name);
            MemberServices memberServices = this._mySubscriptionRepo.GetSubscriptionDetail(myserviceid);
            return View(memberServices);
        }

        public ActionResult Charge(string stripeEmail, string stripeToken, MemberServices memberServices)
        {
            int userid = this._userTable.GetmemberId(User.Identity.Name);
            var memberServiceId = Request.Form["ServiceId"];
            var customers = new CustomerService();
            var charges = new ChargeService();

            IEnumerable<MemberServices> allServices = this._mySubscriptionRepo.GetMySubscriptions(userid);
            string stripe_customerId = allServices.Where(a => !string.IsNullOrEmpty(a.StripeCustomerId)).Select(a => a.StripeCustomerId).FirstOrDefault();

            if (string.IsNullOrEmpty(stripe_customerId))
            {
                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    SourceToken = stripeToken
                });
                stripe_customerId = customer.Id;
            }

            
            charges.ExpandBalanceTransaction = true;
            charges.ExpandCustomer = true;
            charges.ExpandInvoice = true;
            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(memberServices.ServiceRate) * 100),
                Description = memberServices.ServiceType + " Charge",
                Currency = "usd",
                CustomerId = stripe_customerId,
                ReceiptEmail=stripeEmail,
                Metadata = new Dictionary<String, String>()
                {
                    { "MemberId", memberServices.MemberId.ToString()},
                    { "ServiceId", memberServices.ServiceId.ToString()},
                    { "MemberName", memberServices.MemberName.ToString()},
                    { "ServiceType", memberServices.ServiceType.ToString()},
                    { "ServiceName", memberServices.ServiceName.ToString()},
                }
            });

            memberServices.StripeEmail = stripeEmail;
            memberServices.StripeToken = stripeToken;
            memberServices.StripeCustomerId = stripe_customerId;
            bool paid = this._mySubscriptionRepo.MakePaymentFor(memberServices);
            
            return View(memberServices);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}