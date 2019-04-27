using Dapper;
using MemberService.Data.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberService.DAL
{
    public class Services
    {
        private DbManager db;
        public Services(DbManager database)
        {
            db = database;
        }

        public List<Service> GetServiceList()
        {
            return db.Connection.Query<Service>("Select Id, Name, Rate,Status, ServiceType, AppliedDate, StripePlanName from Services").ToList();
        }
        public List<Service> GetAppliedServiceList(int member)
        {
            var query = @"select * from services
                        where Id = case 
                        when 
	                        (select COUNT(id) from MyServices where MemberId=@MemberId and ServiceId = (select top(1) id from services where servicetype='OneTime' order by AppliedDate desc))=0
                        then (select top(1) id from services where servicetype='OneTime' order by AppliedDate desc)
                        when 
	                        (select COUNT(id) from MyServices where MemberId=@MemberId and ServiceId = (select top(1) id from services where servicetype='OneTime' order by AppliedDate desc))=1
                        then (select top(1) id from services where servicetype='Monthly' order by AppliedDate desc)
                        end";
            return db.Connection.Query<Service>(query,new { MemberId=member}).ToList();
        }

        public void AddService(Service model)
        {
            try
            {
                string query = $@"INSERT INTO Services(Name,Rate,Status,ServiceType,AppliedDate) values('{model.Name}','{model.Rate}','A','{model.ServiceType}','{model.AppliedDate}')";
                this.db.Connection.Execute(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Service GetServiceById(int id)
        {
            string query = $@"Select Id, Name, Rate,Status, ServiceType, AppliedDate, StripePlanName from Services Where Id='{id}'";
            return this.db.Connection.Query<Service>(query).FirstOrDefault();
        }

        public void UpdateService(Service model)
        {
            string query = $@"Update Services Set Name='{model.Name}', Rate='{model.Rate}', ServiceType='{model.ServiceType}',AppliedDate='{model.AppliedDate}' Where Id='{model.Id}'";
            this.db.Connection.Execute(query);
        }

        public void ChangeServiceStatus(int id)
        {
            string query = $@"update Services Set Status=case when Status='A' then 'I' When Status='I' then 'A' end where id={id}";
            this.db.Connection.Execute(query);
        }

        public void CreateStripPlanForService(int id, string planName)
        {
            string query = $@"update Services Set StripePlanName=@PlanName where id=@Id";
            this.db.Connection.Execute(query,new { PlanName=planName, Id=id});
        }
    }
}
