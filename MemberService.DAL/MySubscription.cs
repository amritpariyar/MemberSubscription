using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MemberService.Data.BLL;

namespace MemberService.DAL
{
    public class MySubscription
    {
        private DbManager db;
        public MySubscription(DbManager database)
        {
            db = database;
        }

        public IEnumerable<MemberServices> GetMySubscriptions(int userid)
        {
            var query =$@"SELECT ms.*,s.Name as ServiceName,s.ServiceType FROM MyServices ms, Services s Where ms.ServiceId=s.Id and ms.MemberId=@memberId";
            return db.Connection.Query<MemberServices>(query, new { memberId = userid }).ToList();
        }

        public MyServices FindServiceById(int? id)
        {
            var query = $@"select * from MyServices where Id=@Id";
            return db.Connection.Query<MyServices>(query, new { Id = id }).FirstOrDefault();
        }

        public int AddService(MyServices model)
        {
            string query = @"Insert into MyServices(MemberId,ServiceId,StartDate,ValidDate,Status,IsPaid,PaymentConfirmed) values(@MemberId,@ServiceId,@StartDate,@ValidDate,@Status,@IsPaid,@PaymentConfirmed);
                SELECT @@IDENTITY";
            int insertedId = db.Connection.Query<int>(query, new { MemberId = model.MemberId, ServiceId = model.ServiceId, StartDate = model.StartDate, ValidDate = model.ValidDate, Status = model.Status, IsPaid = model.IsPaid, PaymentConfirmed = model.PaymentConfirmed }).Single();
            return insertedId;
        }

        public MemberServices GetSubscriptionDetail(int myserviceid)
        {
            var query = $@"SELECT ms.*,m.UserName as MemberName, s.Name as ServiceName,s.ServiceType,s.Rate as ServiceRate FROM MyServices ms, Services s, Member m Where ms.ServiceId=s.Id and m.Id=ms.MemberId and ms.Id=@myserviceid";
            return db.Connection.Query<MemberServices>(query, new { myserviceid = myserviceid }).FirstOrDefault();
        }

        public bool MakePaymentFor(MemberServices formColl)
        {
            try
            {
                var query = @"update MyServices 
                set IsPaid='true'
                , PaymentConfirmed='true'
                , Status='Active'
                , Amount=@Amount 
                , StripeEmail=@StripeEmail
                , StripeToken=@StripeToken
                , StripeCustomerId=@StripeCustomerId
                where id=@serviceId";
                db.Connection.Execute(query, new { Amount = formColl.Amount, serviceId = formColl.Id, formColl.StripeEmail,formColl.StripeToken,formColl.StripeCustomerId });
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public void CancelService(int myserviceid)
        {
            try
            {
                var query = @"Update MyServices Set Status='Cancelled' Where Id=@memberServiceId";
                db.Connection.Execute(query, new { memberServiceId = myserviceid });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string FindMembersStripeCustomerId(int member)
        {
            return (string)db.Connection.ExecuteScalar("Select StripeCustomerId from myservices where MemberId=@member and StripeCustomerId is not null", new { member });
        }

        public bool CheckIfInstallationActive(int member)
        {
            string query = @"select count(*) from MyServices Where ServiceId in (select id from Services where Name='Installation') and IsPaid='true' and PaymentConfirmed='true' and MemberId=@member";
            int count = db.Connection.Query<int>(query, new { member = member }).SingleOrDefault();
            if (count == 1) return true;
            else return false;
        }

        public void UpdateSubscriptionStatus(int memberServiceId, string stripeCustomerId, string stripeSubscriptionId)
        {
            db.Connection.Execute("Update MyServices Set Status='Subscription', StripeCustomerId=@stripeCustomerId, StripeSubscriptionId=@stripeSubscriptionId Where Id=@memberServiceId", new { stripeCustomerId, stripeSubscriptionId, memberServiceId });
        }

        public void CancelSubscription(string subscriptinId)
        {
            try
            {
                var query = @"Update MyServices Set Status='SubscriptionCanceled' Where StripeSubscriptionId=@subscriptinId";
                db.Connection.Execute(query, new { subscriptinId});
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
